// <copyright file="LogLevel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the log levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Indicates a debug log level.
    /// </summary>
    Debug = (int)ggml_log_level.GGML_LOG_LEVEL_DEBUG,

    /// <summary>
    /// Indicates an error log level.
    /// </summary>
    Error = (int)ggml_log_level.GGML_LOG_LEVEL_ERROR,

    /// <summary>
    /// Indicates a warning log level.
    /// </summary>
    Warning = (int)ggml_log_level.GGML_LOG_LEVEL_WARN,

    /// <summary>
    /// Indicates an information log level.
    /// </summary>
    Info = (int)ggml_log_level.GGML_LOG_LEVEL_INFO,
}
