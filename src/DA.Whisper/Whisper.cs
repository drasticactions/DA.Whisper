// <copyright file="Whisper.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Whisper System Class.
/// </summary>
public static class Whisper
{
    /// <summary>
    /// Gets the native Whisper system info from the bundled library.
    /// Throws a DllNotFoundException if the library is not found.
    /// </summary>
    /// <returns>String of the system version.</returns>
    public static string GetSystemInfo()
    {
        return NativeMethods.GetWhisperSystemInfo();
    }
}