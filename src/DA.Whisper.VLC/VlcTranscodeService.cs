// <copyright file="VlcTranscodeService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Logging;

namespace DA.Whisper;

/// <summary>
/// Represents a service for transcoding media files using VLC.
/// </summary>
public class VlcTranscodeService : ITranscodeService, IDisposable
{
    private string basePath;
    private string? generatedFilename;

    private LibVLC libVLC;
    private ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VlcTranscodeService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="basePath">The base path for storing transcoded files.</param>
    /// <param name="generatedFilename">The generated filename for the transcoded file.</param>
    public VlcTranscodeService(ILogger logger, string? basePath = default, string? generatedFilename = default)
    {
        this.logger = logger;
        this.basePath = basePath ?? Path.GetTempPath();
        this.libVLC = new LibVLC();
        this.generatedFilename = generatedFilename;
    }

    /// <inheritdoc/>
    public string BasePath => this.basePath;

    /// <inheritdoc/>
    public async Task<string> ProcessFile(string file)
    {
        using var mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(this.libVLC);
        var processingCancellationTokenSource = new CancellationTokenSource();
        var outputfile = Path.Combine(this.basePath, $"{this.generatedFilename ?? Path.GetRandomFileName()}.wav");
        mediaPlayer.Stopped += (s, e) => processingCancellationTokenSource.CancelAfter(1);

        Media media;

        if (IsUrl(file))
        {
            media = new Media(this.libVLC, file, FromType.FromLocation);
        }
        else
        {
            media = new Media(this.libVLC, file, FromType.FromPath);
        }

        media.AddOption(":no-video");
        media.AddOption(":sout=#transcode{acodec=s16l,channels=1,samplerate=16000}:file{dst='" + outputfile + "'");
        var testing = media.Parse();
        mediaPlayer.Play(media);

        while (!processingCancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), processingCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        return outputfile;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.libVLC.Dispose();
    }

    private static bool IsUrl(string str)
    {
        // URL pattern
        string urlPattern = @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";

        // Check if the string matches the URL pattern
        return Regex.IsMatch(str, urlPattern);
    }
}