// <copyright file="Microphone.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;
using AudioToolbox;

namespace Transcribe.Apple;

/// <summary>
/// Microphone.
/// Stolen from https://github.com/takenet/Xamarin-Forms-Labs/blob/cc3d8f24f1c1a827d128c44043f03a6b1d12d06e/src/Platform/XLabs.Platform.iOS/Services/Media/Microphone.cs#L19.
/// </summary>
public class Microphone
{
    /// <summary>
    /// The _buffer size.
    /// </summary>
    private readonly int bufferSize;

    /// <summary>
    /// The _audio queue.
    /// </summary>
    private InputAudioQueue? audioQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Microphone"/> class.
    /// </summary>
    /// <param name="bufferSize">Size of the buffer.</param>
    public Microphone(int bufferSize = 4098)
    {
        this.bufferSize = bufferSize;
    }

    /// <summary>
    /// Occurs when new audio has been streamed.
    /// </summary>
    public event EventHandler<BroadcastEventArgs>? OnBroadcast;

    /// <summary>
    /// Gets the sample rate.
    /// </summary>
    /// <value>The sample rate in hertz.</value>
    public int SampleRate { get; private set; }

    /// <summary>
    /// Gets the channel count.
    /// </summary>
    /// <value>The channel count.</value>
    public int ChannelCount
    {
        get
        {
            return 1;
        }
    }

    /// <summary>
    /// Gets bits per sample.
    /// </summary>
    /// <value>The bits per sample.</value>
    public int BitsPerSample
    {
        get
        {
            return 16;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Microphone"/> is active.
    /// </summary>
    /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
    public bool Active
    {
        get
        {
            return this.audioQueue != null && this.audioQueue.IsRunning;
        }
    }

    /// <summary>
    /// Gets the average data transfer rate.
    /// </summary>
    /// <value>The average data transfer rate in bytes per second.</value>
    public IEnumerable<int> SupportedSampleRates
    {
        get
        {
            return new[] { 8000, 16000, 22050, 41000, 44100 };
        }
    }

    /// <summary>
    /// Starts the specified sample rate.
    /// </summary>
    /// <param name="sampleRate">The sample rate.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task<bool> Start(int sampleRate)
    {
        return Task.Run(
            () =>
                {
                    if (!this.SupportedSampleRates.Contains(sampleRate))
                    {
                        return false;
                    }

                    this.StartRecording(sampleRate);

                    return this.Active;
                });
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    /// <returns>Task.</returns>
    public Task Stop()
    {
        return Task.Run(() => this.Clear());
    }

    /// <summary>
    /// Starts the recording.
    /// </summary>
    /// <param name="rate">The rate.</param>
    private void StartRecording(int rate)
    {
        if (this.Active)
        {
            this.Clear();
        }

        this.SampleRate = rate;

        var audioFormat = new AudioStreamBasicDescription
        {
            SampleRate = this.SampleRate,
            Format = AudioFormatType.LinearPCM,
            FormatFlags =
                                      AudioFormatFlags.LinearPCMIsSignedInteger
                                      | AudioFormatFlags.LinearPCMIsPacked,
            FramesPerPacket = 1,
            ChannelsPerFrame = 1,
            BitsPerChannel = this.BitsPerSample,
            BytesPerPacket = 2,
            BytesPerFrame = 2,
            Reserved = 0,
        };

        this.audioQueue = new InputAudioQueue(audioFormat);
        this.audioQueue.InputCompleted += this.QueueInputCompleted;

        var bufferByteSize = this.bufferSize * audioFormat.BytesPerPacket;

        IntPtr bufferPtr;
        for (var index = 0; index < 3; index++)
        {
            this.audioQueue.AllocateBufferWithPacketDescriptors(bufferByteSize, this.bufferSize, out bufferPtr);
            this.audioQueue.EnqueueBuffer(bufferPtr, bufferByteSize, null!);
        }

        this.audioQueue.Start();
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    private void Clear()
    {
        if (this.audioQueue != null)
        {
            this.audioQueue.Stop(true);
            this.audioQueue.InputCompleted -= this.QueueInputCompleted;
            this.audioQueue.Dispose();
            this.audioQueue = null;
        }
    }

    /// <summary>
    /// Handles iOS audio buffer queue completed message.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Input completed parameters.</param>
    private void QueueInputCompleted(object? sender, InputCompletedEventArgs e)
    {
        // return if we aren't actively monitoring audio packets
        if (!this.Active)
        {
            return;
        }

        var buffer = Marshal.PtrToStructure<AudioQueueBuffer>(e.IntPtrBuffer);
        if (this.OnBroadcast != null)
        {
            var send = new byte[buffer.AudioDataByteSize];
            Marshal.Copy(buffer.AudioData, send, 0, (int)buffer.AudioDataByteSize);

            this.OnBroadcast(this, new BroadcastEventArgs(send));
        }

        var status = this.audioQueue!.EnqueueBuffer(e.IntPtrBuffer, this.bufferSize, e.PacketDescriptions!);

        if (status != AudioQueueStatus.Ok)
        {
            // todo:
        }
    }
}