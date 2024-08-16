// <copyright file="LogEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the event arguments for a log event.
/// </summary>
public class LogEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogEventArgs"/> class.
    /// </summary>
    /// <param name="level">The log level of the event.</param>
    /// <param name="message">The log message of the event.</param>
    public LogEventArgs(LogLevel level, string message)
    {
        this.Level = level;
        this.Message = message.Trim();
    }

    /// <summary>
    /// Gets the log level of the event.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the log message of the event.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Returns a string representation of the log event.
    /// </summary>
    /// <returns>A string representation of the log event.</returns>
    public override string ToString()
    {
        return $"{this.Level}: {this.Message}";
    }
}