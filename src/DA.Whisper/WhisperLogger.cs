// <copyright file="WhisperLogger.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;

namespace DA.Whisper;

/// <summary>
/// Represents a logger for the Whisper library.
/// </summary>
public class WhisperLogger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperLogger"/> class.
    /// </summary>
    private WhisperLogger()
    {
        unsafe
        {
            delegate* unmanaged[Cdecl]<int, byte*, void*, void> onLogging = &LogUnmanaged;
            NativeMethods.whisper_log_set(onLogging, null);
        }
    }

    /// <summary>
    /// Event that is raised when a log message is received.
    /// </summary>
    public event Action<LogEventArgs>? OnLog;

    /// <summary>
    /// Gets the singleton instance of the <see cref="WhisperLogger"/> class used to log messages from the Whisper library.
    /// </summary>
    public static WhisperLogger Instance { get; } = new();

    /// <summary>
    /// Callback method for logging unmanaged messages.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The log message.</param>
    /// <param name="user_data">User data.</param>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static unsafe void LogUnmanaged(int level, byte* message, void* user_data)
    {
        Instance.OnLog?.Invoke(new LogEventArgs((LogLevel)level, Marshal.PtrToStringAnsi((IntPtr)message) ?? string.Empty));
    }
}
