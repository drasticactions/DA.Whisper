using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DA.Whisper;

public sealed class WhisperProcessor : IAsyncDisposable, IDisposable
{
    private static readonly ConcurrentDictionary<long, WhisperProcessor> processorInstances = new();
    private static long currentProcessorId;

    private readonly WhisperModel model;

    private readonly FullParams fullParams;

    // Id is used to identify the current instance when calling the callbacks from C++
    private readonly long myId;

    private CancellationToken? cancellationToken;

    private int segmentIndex;

    private readonly SemaphoreSlim processingSemaphore;

    private bool isDisposed;

    private IProgress<int>? progress;

    private bool calculateProbability;

    internal WhisperProcessor(WhisperModel model, FullParams fullParams, bool calculateProbability = false, IProgress<int>? progress = default)
    {
        this.calculateProbability = calculateProbability;
        this.progress = progress;
        this.fullParams = fullParams;
        this.myId = Interlocked.Increment(ref currentProcessorId);
        processorInstances[myId] = this;
        this.model = model;
        this.processingSemaphore = new(1);
        SetupCallbacksForParams(fullParams);
    }

    private void OnProgress(int progress)
        => this.progress?.Report(progress);

    private bool OnEncoderBegin()
    {
        if (cancellationToken?.IsCancellationRequested ?? false)
        {
            return false;
        }

        return true;
    }

    public async IAsyncEnumerable<SegmentData> ProcessAsync(Stream waveStream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var waveParser = new WaveParser(waveStream);
        var samples = await waveParser.GetAvgSamplesAsync(cancellationToken);
        await foreach (var segmentData in ProcessAsync(samples, cancellationToken))
        {
            yield return segmentData;
        }
    }

    private Action<SegmentData>? OnSegmentEventHandler;

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
            this.OnSegmentEventHandler = OnSegmentHandler;
            this.cancellationToken = cancellationToken;
            var whisperTask = ProcessInternalAsync(samples, cancellationToken)
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
            this.OnSegmentEventHandler = null;
        }
    }

    private Task ProcessInternalAsync(ReadOnlyMemory<float> samples, CancellationToken cancellationToken)
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("This processor has already been disposed.");
        }
        return Task.Factory.StartNew(() =>
        {
            unsafe {
                fixed (float* pData = samples.Span)
                {
                    processingSemaphore.Wait();
                    segmentIndex = 0;

                    var state = NativeMethods.whisper_init_state(this.model._context);

                    try
                    {
                        var paramTest =this.fullParams.Params;
                        NativeMethods.whisper_full_with_state(this.model._context, state, this.fullParams.Params, pData, samples.Length);
                    }
                    finally
                    {
                        NativeMethods.whisper_free_state(state);
                        processingSemaphore.Release();
                    }
                }
            }
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public IAsyncEnumerable<SegmentData> ProcessAsync(float[] samples, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(samples.AsMemory(), cancellationToken);
    }

    private unsafe void OnNewSegment(whisper_state* state, int nNew)
    {
        if (cancellationToken?.IsCancellationRequested ?? false)
        {
            return;
        }

        var segments = NativeMethods.whisper_full_n_segments_from_state(state);

        while (segmentIndex < segments)
        {
            var t1 = TimeSpan.FromMilliseconds(NativeMethods.whisper_full_get_segment_t1_from_state(state, segmentIndex) * 10);
            var t0 = TimeSpan.FromMilliseconds(NativeMethods.whisper_full_get_segment_t0_from_state(state, segmentIndex) * 10);
            var textAnsi = Marshal.PtrToStringUTF8(((IntPtr)NativeMethods.whisper_full_get_segment_text_from_state(state, segmentIndex)));

            float minimumProbability = 0;
            float maximumProbability = 0;
            double sumProbability = 0;
            var numberOfTokens = NativeMethods.whisper_full_n_tokens_from_state(state, segmentIndex);
            var languageId = NativeMethods.whisper_full_lang_id_from_state(state);
            var language = Marshal.PtrToStringAnsi((IntPtr)NativeMethods.whisper_lang_str(languageId));

            if (this.calculateProbability)
            {
                for (var tokenIndex = 0; tokenIndex < numberOfTokens; tokenIndex++)
                {
                    var tokenProbability = NativeMethods.whisper_full_get_token_p_from_state(state, segmentIndex, tokenIndex);
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
                var eventHandlerArgs = new SegmentData(textAnsi, t0, t1, minimumProbability, maximumProbability, (float)(sumProbability / numberOfTokens), language!);

                this.OnSegmentEventHandler?.Invoke(eventHandlerArgs);
                if (cancellationToken?.IsCancellationRequested ?? false)
                {
                    return;
                }
            }

            segmentIndex++;
        }
    }

    private bool OnWhisperAbort()
    {
        if (cancellationToken?.IsCancellationRequested ?? false)
        {
            return false;
        }

        return false;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe bool OnEncoderBeginStatic(whisper_context* ctx, whisper_state* state, void* userData)
    {
        if (!processorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        return processor.OnEncoderBegin();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void OnProgressStatic(whisper_context* ctx, whisper_state* state, int progress, void* userData)
    {
        if (!processorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        processor.OnProgress(progress);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void OnNewSegmentStatic(whisper_context* ctx, whisper_state* state, int nNew, void* userData)
    {
        if (!processorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        processor.OnNewSegment(state, nNew);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe bool OnWhisperAbortStatic(void* userData)
    {
        if (!processorInstances.TryGetValue(((IntPtr)userData).ToInt64(), out var processor))
        {
            throw new Exception("Couldn't find processor instance for user data");
        }

        return processor.OnWhisperAbort();
    }

    private void SetupCallbacksForParams(FullParams fullParams)
    {
        unsafe
        {
            var myIntPtrId = new IntPtr(myId);
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

    public static WhisperProcessor CreateWithDefaultGreedyStrategy(WhisperModel model)
    {
        return new WhisperProcessor(model, FullParams.FromGreedyStrategy());
    }

    public static WhisperProcessor CreateWithDefaultBeamSearchStrategy(WhisperModel model)
    {
        return new WhisperProcessor(model, FullParams.FromBeamSearchStrategy());
    }

    public static WhisperProcessor CreateWithParams(WhisperModel model, FullParams fullParams)
    {
        return new WhisperProcessor(model, fullParams);
    }

    public void Dispose()
    {
        if (processingSemaphore.CurrentCount == 0)
        {
            throw new Exception("Cannot dispose while processing, please use DisposeAsync instead.");
        }

        processorInstances.TryRemove(myId, out _);
        // if (language.HasValue)
        // {
        //     Marshal.FreeHGlobal(language.Value);
        //     language = null;
        // }

        // if (initialPromptText.HasValue)
        // {
        //     Marshal.FreeHGlobal(initialPromptText.Value);
        //     initialPromptText = null;
        // }

        isDisposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        // If a processing is still running, wait for it to finish
        await processingSemaphore.WaitAsync();
        processingSemaphore.Release();
        Dispose();
    }
}

internal class AsyncAutoResetEvent
{
    private static readonly Task Completed = Task.CompletedTask;
    private TaskCompletionSource<bool>? waitTcs;
    private int isSignaled; // 0 for false, 1 for true

    public Task WaitAsync()
    {
        if (Interlocked.CompareExchange(ref isSignaled, 0, 1) == 1)
        {
            return Completed;
        }
        else
        {
            var tcs = new TaskCompletionSource<bool>();
            var oldTcs = Interlocked.Exchange(ref waitTcs, tcs);
            oldTcs?.TrySetCanceled();
            return tcs.Task;
        }
    }

    public void Set()
    {
        var toRelease = Interlocked.Exchange(ref waitTcs, null);
        if (toRelease != null)
        {
            toRelease.SetResult(true);
        }
        else
        {
            Interlocked.Exchange(ref isSignaled, 1);
        }
    }
}