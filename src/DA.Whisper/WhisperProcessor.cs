// <copyright file="WhisperProcessor.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DA.Whisper;

/// <summary>
/// Whisper Processor.
/// </summary>
public sealed class WhisperProcessor : IAsyncDisposable, IDisposable
{
    private static readonly ConcurrentDictionary<long, WhisperProcessor> ProcessorInstances = new();
    private static long currentProcessorId;

    private readonly WhisperModel model;

    private readonly FullParams fullParams;

    private readonly SemaphoreSlim processingSemaphore;

    // Id is used to identify the current instance when calling the callbacks from C++
    private readonly long myId;

    private CancellationToken? cancellationToken;

    private int segmentIndex;

    private bool isDisposed;

    private IProgress<int>? progress;

    private Action<SegmentData>? onSegmentEventHandler;

    private bool calculateProbability;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperProcessor"/> class.
    /// </summary>
    /// <param name="model">The Whisper model.</param>
    /// <param name="fullParams">The full parameters.</param>
    /// <param name="calculateProbability">Flag indicating whether to calculate probability.</param>
    /// <param name="progress">The progress reporter.</param>
    internal WhisperProcessor(WhisperModel model, FullParams fullParams, bool calculateProbability = false, IProgress<int>? progress = default)
    {
        this.calculateProbability = calculateProbability;
        this.progress = progress;
        this.fullParams = fullParams;
        this.myId = Interlocked.Increment(ref currentProcessorId);
        ProcessorInstances[this.myId] = this;
        this.model = model;
        this.processingSemaphore = new(1);
        this.SetupCallbacksForParams(fullParams);
    }

    /// <summary>
    /// Creates a new <see cref="WhisperProcessor"/> instance with the default greedy strategy.
    /// </summary>
    /// <param name="model">The Whisper model.</param>
    /// <returns>The created <see cref="WhisperProcessor"/> instance.</returns>
    public static WhisperProcessor CreateWithDefaultGreedyStrategy(WhisperModel model)
    {
        return new WhisperProcessor(model, FullParams.FromGreedyStrategy());
    }

    /// <summary>
    /// Creates a new <see cref="WhisperProcessor"/> instance with the default beam search strategy.
    /// </summary>
    /// <param name="model">The Whisper model.</param>
    /// <returns>The created <see cref="WhisperProcessor"/> instance.</returns>
    public static WhisperProcessor CreateWithDefaultBeamSearchStrategy(WhisperModel model)
    {
        return new WhisperProcessor(model, FullParams.FromBeamSearchStrategy());
    }

    /// <summary>
    /// Creates a new <see cref="WhisperProcessor"/> instance with the specified parameters.
    /// </summary>
    /// <param name="model">The Whisper model.</param>
    /// <param name="fullParams">The full parameters.</param>
    /// <returns>The created <see cref="WhisperProcessor"/> instance.</returns>
    public static WhisperProcessor CreateWithParams(WhisperModel model, FullParams fullParams)
    {
        return new WhisperProcessor(model, fullParams);
    }

    /// <summary>
    /// Creates a new <see cref="WhisperProcessor"/> instance with the specified parameters.
    /// </summary>
    /// <param name="model">The Whisper model.</param>
    /// <param name="fullParams">The full parameters.</param>
    /// <param name="processor">The created <see cref="WhisperProcessor"/> instance.</param>
    /// <returns>Bool if created.</returns>
    public static bool TryCreateWithParams(WhisperModel model, FullParams fullParams, out WhisperProcessor? processor)
    {
        try
        {
            processor = new WhisperProcessor(model, fullParams);
            return true;
        }
        catch (Exception)
        {
            processor = null;
            return false;
        }
    }

    /// <summary>
    /// Processes the audio stream asynchronously.
    /// </summary>
    /// <param name="waveStream">The audio stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous enumerable of segment data.</returns>
    public async IAsyncEnumerable<SegmentData> ProcessAsync(Stream waveStream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var waveParser = new WaveParser(waveStream);
        var samples = await waveParser.GetAvgSamplesAsync(cancellationToken);
        await foreach (var segmentData in this.ProcessAsync(samples, cancellationToken))
        {
            yield return segmentData;
        }
    }

    /// <summary>
    /// Processes the audio samples asynchronously.
    /// </summary>
    /// <param name="samples">The audio samples.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous enumerable of segment data.</returns>
    public async IAsyncEnumerable<SegmentData> ProcessAsync(ReadOnlyMemory<float> samples, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var resetEvent = new AsyncAutoResetEvent();
        var buffer = new ConcurrentQueue<SegmentData>();

        void OnSegmentHandler(SegmentData segmentData)
        {
            buffer!.Enqueue(segmentData);
            resetEvent!.Set();
        }

        try
        {
            this.onSegmentEventHandler = OnSegmentHandler;
            this.cancellationToken = cancellationToken;
            var whisperTask = this.ProcessInternalAsync(samples, cancellationToken)
                .ContinueWith(_ => resetEvent.Set(), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);

            while (!whisperTask.IsCompleted || !buffer.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (buffer.IsEmpty)
                {
                    await Task.WhenAny(whisperTask, resetEvent.WaitAsync())
                        .ConfigureAwait(false);
                }

                while (!buffer.IsEmpty && buffer.TryDequeue(out var segmentData))
                {
                    yield return segmentData;
                }
            }

            await whisperTask.ConfigureAwait(false);

            while (buffer.TryDequeue(out var segmentData))
            {
                yield return segmentData;
            }
        }
        finally
        {
            this.onSegmentEventHandler = null;
        }
    }

    /// <summary>
    /// Disposes the <see cref="WhisperProcessor"/> instance.
    /// </summary>
    public void Dispose()
    {
        if (this.processingSemaphore.CurrentCount == 0)
        {
            throw new Exception("Cannot dispose while processing, please use DisposeAsync instead.");
        }

        ProcessorInstances.TryRemove(this.myId, out _);

        this.isDisposed = true;
    }

    /// <summary>
    /// Asynchronously disposes the <see cref="WhisperProcessor"/> instance.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        // If a processing is still running, wait for it to finish
        await this.processingSemaphore.WaitAsync();
        this.processingSemaphore.Release();
        this.Dispose();
    }

    /// <summary>
    /// Callback for encoder begin.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="state">The whisper state.</param>
    /// <param name="userData">The user data.</param>
    /// <returns><c>true</c> if the encoder should begin, otherwise <c>false</c>.</returns>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe bool OnEncoderBeginStatic(whisper_context* ctx, whisper_state* state, void* userData)
    {
        if (!ProcessorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        return processor.OnEncoderBegin();
    }

    /// <summary>
    /// Callback for progress update.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="state">The whisper state.</param>
    /// <param name="progress">The progress value.</param>
    /// <param name="userData">The user data.</param>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void OnProgressStatic(whisper_context* ctx, whisper_state* state, int progress, void* userData)
    {
        if (!ProcessorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        processor.OnProgress(progress);
    }

    /// <summary>
    /// Callback for new segment.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="state">The whisper state.</param>
    /// <param name="nNew">The number of new segments.</param>
    /// <param name="userData">The user data.</param>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void OnNewSegmentStatic(whisper_context* ctx, whisper_state* state, int nNew, void* userData)
    {
        if (!ProcessorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        processor.OnNewSegment(state, nNew);
    }

    /// <summary>
    /// Callback for whisper abort.
    /// </summary>
    /// <param name="userData">The user data.</param>
    /// <returns><c>true</c> if the whisper should abort, otherwise <c>false</c>.</returns>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe bool OnWhisperAbortStatic(void* userData)
    {
        if (!ProcessorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        return processor.OnWhisperAbort();
    }

    /// <summary>
    /// Processes the audio samples internally.
    /// </summary>
    /// <param name="samples">The audio samples.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task ProcessInternalAsync(ReadOnlyMemory<float> samples, CancellationToken cancellationToken)
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("This processor has already been disposed.");
        }

        return Task.Factory.StartNew(
            () =>
        {
            unsafe
            {
                fixed (float* pData = samples.Span)
                {
                    this.processingSemaphore.Wait();
                    this.segmentIndex = 0;

                    var state = NativeMethods.whisper_init_state((whisper_context*)this.model.Context.Handle);

                    try
                    {
                        var paramTest = this.fullParams.Params;
                        NativeMethods.whisper_full_with_state((whisper_context*)this.model.Context.Handle, state, this.fullParams.Params, pData, samples.Length);
                    }
                    finally
                    {
                        NativeMethods.whisper_free_state(state);
                        this.processingSemaphore.Release();
                    }
                }
            }
        },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Event handler for progress updates.
    /// </summary>
    /// <param name="progress">The progress value.</param>
    private void OnProgress(int progress)
        => this.progress?.Report(progress);

    /// <summary>
    /// Event handler for encoder begin.
    /// </summary>
    /// <returns><c>true</c> if the encoder should begin, otherwise <c>false</c>.</returns>
    private bool OnEncoderBegin()
    {
        if (this.cancellationToken?.IsCancellationRequested ?? false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Event handler for new segment.
    /// </summary>
    /// <param name="state">The whisper state.</param>
    /// <param name="nNew">The number of new segments.</param>
    private unsafe void OnNewSegment(whisper_state* state, int nNew)
    {
        if (this.cancellationToken?.IsCancellationRequested ?? false)
        {
            return;
        }

        var segments = NativeMethods.whisper_full_n_segments_from_state(state);

        while (this.segmentIndex < segments)
        {
            var t1 = TimeSpan.FromMilliseconds(NativeMethods.whisper_full_get_segment_t1_from_state(state, this.segmentIndex) * 10);
            var t0 = TimeSpan.FromMilliseconds(NativeMethods.whisper_full_get_segment_t0_from_state(state, this.segmentIndex) * 10);
            var textAnsi = Marshal.PtrToStringUTF8((IntPtr)NativeMethods.whisper_full_get_segment_text_from_state(state, this.segmentIndex));

            float minimumProbability = 0;
            float maximumProbability = 0;
            double sumProbability = 0;

            var numberOfTokens = NativeMethods.whisper_full_n_tokens_from_state(state, this.segmentIndex);
            var languageId = NativeMethods.whisper_full_lang_id_from_state(state);
            var language = Marshal.PtrToStringAnsi((IntPtr)NativeMethods.whisper_lang_str(languageId));
            bool speakerTurn = false;

            if (this.fullParams.TdrzEnable)
            {
                speakerTurn = NativeMethods.whisper_full_get_segment_speaker_turn_next_from_state(state, this.segmentIndex);
            }

            if (this.calculateProbability)
            {
                for (var tokenIndex = 0; tokenIndex < numberOfTokens; tokenIndex++)
                {
                    var tokenProbability = NativeMethods.whisper_full_get_token_p_from_state(state, this.segmentIndex, tokenIndex);
                    sumProbability += tokenProbability;
                    if (tokenIndex == 0)
                    {
                        minimumProbability = tokenProbability;
                        maximumProbability = tokenProbability;
                        continue;
                    }

                    if (tokenProbability < minimumProbability)
                    {
                        minimumProbability = tokenProbability;
                    }

                    if (tokenProbability > maximumProbability)
                    {
                        maximumProbability = tokenProbability;
                    }
                }
            }

            if (!string.IsNullOrEmpty(textAnsi))
            {
                var eventHandlerArgs = new SegmentData(textAnsi.Trim(), t0, t1, minimumProbability, maximumProbability, (float)(sumProbability / numberOfTokens), language!, speakerTurn);

                this.onSegmentEventHandler?.Invoke(eventHandlerArgs);
                if (this.cancellationToken?.IsCancellationRequested ?? false)
                {
                    return;
                }
            }

            this.segmentIndex++;
        }
    }

    /// <summary>
    /// Event handler for whisper abort.
    /// </summary>
    /// <returns><c>true</c> if the whisper should abort, otherwise <c>false</c>.</returns>
    private bool OnWhisperAbort()
    {
        if (this.cancellationToken?.IsCancellationRequested ?? false)
        {
            return false;
        }

        return false;
    }

    /// <summary>
    /// Sets up the callbacks for the parameters.
    /// </summary>
    /// <param name="fullParams">The full parameters.</param>
    private void SetupCallbacksForParams(FullParams fullParams)
    {
        unsafe
        {
            var myIntPtrId = new IntPtr(this.myId);
            fullParams.Params.new_segment_callback_user_data = (void*)myIntPtrId;
            fullParams.Params.progress_callback_user_data = (void*)myIntPtrId;
            fullParams.Params.encoder_begin_callback_user_data = (void*)myIntPtrId;
            fullParams.Params.abort_callback_user_data = (void*)myIntPtrId;

            delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, void*, bool> onEncoderBegin = &OnEncoderBeginStatic;
            delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, int, void*, void> onProgress = &OnProgressStatic;
            delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, int, void*, void> onNewSegment = &OnNewSegmentStatic;
            delegate* unmanaged[Cdecl]<void*, bool> onWhisperAbort = &OnWhisperAbortStatic;

            fullParams.Params.new_segment_callback = onNewSegment;
            fullParams.Params.progress_callback = onProgress;
            fullParams.Params.encoder_begin_callback = onEncoderBegin;
            fullParams.Params.abort_callback = onWhisperAbort;
        }
    }
}