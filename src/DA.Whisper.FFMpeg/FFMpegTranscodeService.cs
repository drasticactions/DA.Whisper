// <copyright file="FFMpegTranscodeService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Logging;

namespace DA.Whisper;

/// <summary>
/// FFMpeg Transcode Service.
/// </summary>
public class FFMpegTranscodeService : ITranscodeService, IDisposable
{
    private string basePath;
    private string? generatedFilename;
    private ILogger? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FFMpegTranscodeService"/> class.
    /// </summary>
    /// <param name="basePath">The base path for the transcode service. If not provided, the default temporary path will be used.</param>
    /// <param name="generatedFilename">The generated filename for the transcode service. If not provided, a random filename will be generated.</param>
    /// <param name="logger">The logger.</param>
    public FFMpegTranscodeService(string? basePath = default, string? generatedFilename = default, ILogger? logger = default)
    {
        this.logger = logger;
        this.basePath = basePath ?? Path.GetTempPath();
        this.generatedFilename = generatedFilename;
    }

    /// <inheritdoc/>
    public string BasePath => this.basePath;

    /// <inheritdoc/>
    public async Task<(string FilePath, bool Transcoded)> ProcessFile(string file)
    {
        var mediaInfo = await FFProbe.AnalyseAsync(file);
        this.logger?.LogInformation($"Processing file: {file}");
        var audioStream = mediaInfo.AudioStreams.FirstOrDefault();
        if (audioStream is null)
        {
            this.logger?.LogError("No audio stream found.");
            throw new InvalidOperationException("No audio stream found.");
        }

        var shouldTranscode = audioStream.CodecName != "pcm_s16le" || audioStream.SampleRateHz != 16000;
        if (shouldTranscode)
        {
            var outputfile = Path.Combine(this.basePath, $"{this.generatedFilename ?? Path.GetRandomFileName()}.wav");
            var arguments = FFMpegArguments
                .FromFileInput(file)
                .OutputToFile(outputfile, true,
                    options => options.WithAudioCodec("pcm_s16le").WithAudioSamplingRate(16000)
                );
            
            this.logger?.LogInformation($"Transcoding file to: {outputfile}");

            var result = await arguments.ProcessAsynchronously();

            if (!result)
            {
                this.logger?.LogError("FFMpeg failed to transcode file.");
                throw new InvalidOperationException("FFMpeg failed to transcode file.");
            }

            this.logger?.LogInformation($"Transcoded file to: {outputfile}");
            return (outputfile, true);
        }

        return (file, false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}