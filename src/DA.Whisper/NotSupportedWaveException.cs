// <copyright file="NotSupportedWaveException.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents an exception that is thrown when a wave file is not supported.
/// </summary>
public class NotSupportedWaveException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotSupportedWaveException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotSupportedWaveException(string? message)
        : base(message)
    {
    }
}