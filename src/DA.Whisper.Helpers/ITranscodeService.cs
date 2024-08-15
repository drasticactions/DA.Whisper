// <copyright file="ITranscodeService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Transcode Service.
/// </summary>
public interface ITranscodeService : IDisposable
{
    /// <summary>
    /// Gets the base path.
    /// </summary>
    string BasePath { get; }

    /// <summary>
    /// Process file.
    /// </summary>
    /// <param name="filePath">File Path.</param>
    /// <returns>Filepath and Transcoded.</returns>
    Task<(string FilePath, bool Transcoded)> ProcessFile(string filePath);
}