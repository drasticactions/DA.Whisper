// <copyright file="FFMpegTranscodeService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;

namespace DA.Whisper;

/// <summary>
/// FFMpeg Transcode Service.
/// </summary>
public class FFMpegTranscodeService : ITranscodeService, IDisposable
{
    private string basePath;
    private string? generatedFilename;
    private ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FFMpegTranscodeService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="basePath">The base path for the transcode service. If not provided, the default temporary path will be used.</param>
    /// <param name="generatedFilename">The generated filename for the transcode service. If not provided, a random filename will be generated.</param>
    public FFMpegTranscodeService(ILogger logger, string? basePath = default, string? generatedFilename = default)
    {
        this.logger = logger;
        this.basePath = basePath ?? Path.GetTempPath();
        this.generatedFilename = generatedFilename;
    }

    /// <inheritdoc/>
    public string BasePath => this.basePath;

    /// <inheritdoc/>
    public async Task<string> ProcessFile(string file)
    {
        var mediaInfo = await FFmpeg.GetMediaInfo(file);
        this.logger?.LogInformation($"Processing file: {file}");
        var audioStream = mediaInfo.AudioStreams.FirstOrDefault();
        if (audioStream is null)
        {
            this.logger?.LogInformation("No audio stream found.");
            return string.Empty;
        }

        if (audioStream.SampleRate != 16000)
        {
            var outputfile = Path.Combine(this.basePath, $"{this.generatedFilename ?? Path.GetRandomFileName()}.wav");
            var result = await FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddParameter("-c pcm_s16le -ar 16000")
                .SetOutput(outputfile)
                .SetOverwriteOutput(true)
                .Start();

            if (result is null)
            {
                this.logger?.LogError("FFMpeg failed to transcode file.");
                return string.Empty;
            }

            this.logger?.LogInformation($"Transcoded file to: {outputfile}");
            return outputfile;
        }

        return file;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}