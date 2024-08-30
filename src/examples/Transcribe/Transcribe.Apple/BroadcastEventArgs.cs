// <copyright file="BroadcastEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Transcribe.Apple;

/// <summary>
/// Broadcast Event Args.
/// </summary>
/// <param name="audioData">Audio Data.</param>
public class BroadcastEventArgs(byte[] audioData) : EventArgs
{
    /// <summary>
    /// Gets the audio data.
    /// </summary>
    public byte[] AudioData { get; } = audioData;
}