// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using ConsoleAppFramework;
using DA.Whisper;
using DA.WhisperCli;
using Downloader;
using Microsoft.Extensions.Logging;
using OpenTK.Audio.OpenAL;

_ = WhisperLogger.Instance;
var app = ConsoleApp.Create();
app.Add<WhisperCommands>();
app.Run(args);

/// <summary>
/// Whisper Commands.
/// </summary>
#pragma warning disable SA1649 // File name should match first type name
public class WhisperCommands
#pragma warning restore SA1649 // File name should match first type name
{
    /// <summary>
    /// Gets the native Whisper system info from the bundled library.
    /// </summary>
    [Command("system-info")]
    public void GetSystemInfo()
    {
        Console.WriteLine(Whisper.GetSystemInfo());
    }

    /// <summary>Transcribe media file to text.</summary>
    /// <param name="mediaFile">Media file to transcribe.</param>
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="contextFile">-c, Optional Context Parameter file. Generate the file with 'generate-context-file'.</param>
    /// <param name="parameterFile">-p, Optional Parameter file. Generate the file with 'generate-parameter-file'.</param>
    /// <param name="defaultSamplingStrategy">-s, Default Sampling Strategy, ignored if parameter file is used.</param>
    /// <param name="transcodeFile">-t, Transcode the file using ffmpeg to a format that Whisper can process. Requires ffmpeg to be installed.</param>
    /// <param name="printTimestamps">-ts, Print timestamps with the text.</param>
    /// <param name="outputFormats">-f, Output the text to files.</param>
    /// <param name="outputDirectory">-o, Output directory for the SRT file, defaults to the current working directory.</param>
    /// <param name="outputFilename">-of, Output file name for the SRT file, defaults to the name of the media file if available.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("transcribe")]
    public async Task TranscribeAsync(
        [Argument] string mediaFile,
        string model,
        string? contextFile = "context_file.json",
        string? parameterFile = "full_params.json",
        SamplingStrategy defaultSamplingStrategy = SamplingStrategy.Greedy,
        bool transcodeFile = true,
        bool printTimestamps = false,
        string[]? outputFormats = default,
        string? outputDirectory = default,
        string? outputFilename = default,
        bool verbose = false,
        CancellationToken cancellationToken = default)
    {
        outputFormats = outputFormats ?? Array.Empty<string>();
        var consoleLog = new ConsoleLog(verbose);
        this.SetupWhisperLogger(verbose);
        var ffmpeg = new FFMpegTranscodeService(logger: this.SetupLogger("ffmpeg", verbose));
        var contextParams = this.GetContextParams(contextFile);
        if (contextParams == null)
        {
            consoleLog.LogError("Unable to load context parameters.");
            return;
        }

        var fullParams = this.GetFullParams(parameterFile, defaultSamplingStrategy);
        if (fullParams == null)
        {
            consoleLog.LogError("Unable to load parameters.");
            return;
        }

        if (!WhisperModel.TryFromFileWithParameters(model, contextParams, out WhisperModel? whisperModel) || whisperModel is null)
        {
            consoleLog.LogError("Unable to load model.");
            return;
        }

        if (!WhisperProcessor.TryCreateWithParams(whisperModel, fullParams, out WhisperProcessor? processor) || processor is null)
        {
            consoleLog.LogError("Unable to create processor.");
            return;
        }

        if (!File.Exists(mediaFile))
        {
            consoleLog.LogError("Media file does not exist.");
            return;
        }

        var transcoded = false;
        var processFile = mediaFile;
        if (transcodeFile)
        {
            try
            {
                var transcodeResult = await ffmpeg.ProcessFile(mediaFile);
                if (transcodeResult.Transcoded)
                {
                    processFile = transcodeResult.FilePath;
                    transcoded = true;
                }
            }
            catch (Exception ex)
            {
                consoleLog.LogError($"Error transcoding file: {ex.Message}");
                return;
            }
        }

        using var processFileStream = File.OpenRead(processFile);
        var result = processor.ProcessAsync(processFileStream, cancellationToken);
        List<SegmentData>? segments = outputFormats?.Length > 0 ? new() : null;
        var supportOutputFormats = outputFormats?.Length > 0;
        var enumOutputFormats = outputFormats?.Select(o => Enum.Parse<OutputFormat>(o, true)).ToArray();
        var srt = enumOutputFormats?.Contains(OutputFormat.SRT) ?? false;
        var json = enumOutputFormats?.Contains(OutputFormat.Json) ?? false;
        var vtt = enumOutputFormats?.Contains(OutputFormat.VTT) ?? false;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await foreach (var segment in result)
        {
            var text = printTimestamps ? $"[{segment.Start} - {segment.End}] {segment.Text}" : segment.Text;
            consoleLog.Log(text);
            segments?.Add(segment);
        }

        stopwatch.Stop();

        if (cancellationToken.IsCancellationRequested)
        {
            consoleLog.Log("Processing was cancelled.");
            return;
        }

        consoleLog.Log($"Processing took: {stopwatch.Elapsed}");

        if (transcoded)
        {
            consoleLog.LogDebug($"Deleting transcoded file: {processFile}");
            await processFileStream.DisposeAsync();
            File.Delete(processFile);
        }

        var mediaFileName = outputFilename ?? Path.GetFileNameWithoutExtension(mediaFile);
        consoleLog.LogDebug($"Media file name: {mediaFileName}");
        if (string.IsNullOrEmpty(mediaFileName))
        {
            consoleLog.LogDebug("Media file name is empty, using 'output' as the file name.");
            mediaFileName = "output";
        }

        var outputDir = outputDirectory ?? Directory.GetCurrentDirectory();

        if (srt && segments != null)
        {
            var subtitle = Subtitle.FromSegments(segments);

            var outputFile = Path.Combine(outputDir, outputFilename ?? Path.GetFileNameWithoutExtension(mediaFileName) + ".srt");
            consoleLog.Log($"Writing SRT file: {outputFile}");
            File.WriteAllText(outputFile, subtitle.ToString());
        }
        else
        {
            consoleLog.LogDebug("Not writing SRT file.");
        }

        if (json && segments != null)
        {
            var jsonOutputFile = Path.Combine(outputDir, outputFilename ?? Path.GetFileNameWithoutExtension(mediaFileName) + ".json");
            consoleLog.Log($"Writing JSON file: {jsonOutputFile}");
            File.WriteAllText(jsonOutputFile, segments.ToJson());
        }
        else
        {
            consoleLog.LogDebug("Not writing JSON file.");
        }

        if (vtt && segments != null)
        {
            var vttSubtitle = Subtitle.FromSegments(segments, SubtitleType.VTT);
            var vttOutputFile = Path.Combine(outputDir, outputFilename ?? Path.GetFileNameWithoutExtension(mediaFileName) + ".vtt");
            consoleLog.Log($"Writing VTT file: {vttOutputFile}");
            File.WriteAllText(vttOutputFile, vttSubtitle.ToString());
        }
        else
        {
            consoleLog.LogDebug("Not writing VTT file.");
        }

        whisperModel.Dispose();
        processor.Dispose();
    }

    /// <summary>
    /// Download a whisper model from the whisper.cpp Hugging Face model hub.
    /// </summary>
    /// <param name="modelType">Model Type.</param>
    /// <param name="quantizationType">-q, Quantization Type.</param>
    /// <param name="outputPath">-o, Model Output Path.</param>
    /// <param name="force">-f, Force download.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("download-model")]
    public async Task DownloadModelAsync([Argument]GgmlType modelType, QuantizationType quantizationType = QuantizationType.NoQuantization, string? outputPath = default, bool force = false, bool verbose = false, CancellationToken cancellationToken = default)
    {
        var consoleLog = new ConsoleLog(verbose);
        consoleLog.Log($"Downloading model: {modelType} - {quantizationType}");
        var modelUrl = WhisperStatic.ToDownloadUrl(modelType, quantizationType);
        consoleLog.Log($"Model URL: {modelUrl}");
        var downloader = new DownloadService(new DownloadConfiguration()
        {
            ChunkCount = 8,
            ParallelDownload = true,
        });
        downloader.DownloadFileCompleted += (sender, e) =>
        {
            Console.WriteLine();
            if (e.Cancelled && e.UserState is Downloader.DownloadPackage package)
            {
                consoleLog.Log($"Download cancelled");
            }
            else
            {
                consoleLog.Log($"Download completed");
            }
        };
        downloader.DownloadProgressChanged += (sender, e) =>
        {
              Console.Write($"\r{e.ProgressPercentage.ToString("0.00").PadLeft(6)}%");
        };

        var modelName = modelType.ToFilename(quantizationType);

        var outputFilePath = !string.IsNullOrEmpty(outputPath) ? Path.Combine(outputPath, $"{modelName}".ToLowerInvariant()) : Path.Combine(Environment.CurrentDirectory, $"{modelName}".ToLowerInvariant());
        consoleLog.LogDebug($"Output Path: {outputFilePath}");

        if (File.Exists(outputFilePath))
        {
            if (force)
            {
                consoleLog.Log("Deleting existing file.");
                File.Delete(outputFilePath);
            }
            else
            {
                consoleLog.Log("File already exists. Use -f to force download.");
                return;
            }
        }

        await downloader.DownloadFileTaskAsync(modelUrl, outputFilePath, cancellationToken);
    }

#if DEBUG
    /// <summary>
    /// Transcribe live audio to text.
    /// </summary>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("realtime")]
    public async Task RealtimeAsync(bool verbose = false, CancellationToken cancellationToken = default)
    {
        var consoleLog = new ConsoleLog(verbose);

        consoleLog.LogDebug("Default Mic:\n" + ALC.GetString(ALDevice.Null, AlcGetString.CaptureDefaultDeviceSpecifier));
        consoleLog.LogDebug("Mic List:\n" + string.Join("\n", ALC.GetString(ALDevice.Null, AlcGetStringList.CaptureDeviceSpecifier)));

        await this.RecordAudioAsync(
        (audio) =>
        {
            consoleLog.LogDebug($"Audio: {audio.Length}");
        },
        null,
        cancellationToken: cancellationToken);
    }
#endif

    /// <summary>Generates the context parameters file.</summary>
    /// <param name="outputName">Output file name.</param>
    [Command("generate-context-file")]
    public void GenerateContextFile(string outputName = "context_params.json")
    {
        File.WriteAllText(outputName, ContextParams.FromDefault().ToJson());
    }

    /// <summary>Generates the parameter file.</summary>
    /// <param name="samplingStrategy">Default parameters to generate from.</param>
    /// <param name="outputName">Output file name.</param>
    [Command("generate-parameter-file")]
    public void GenerateParameterFile(SamplingStrategy samplingStrategy = SamplingStrategy.Greedy, string outputName = "full_params.json")
    {
        FullParams fullParams = samplingStrategy switch
        {
            SamplingStrategy.Greedy => FullParams.FromGreedyStrategy(),
            SamplingStrategy.BeamSearch => FullParams.FromBeamSearchStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(samplingStrategy)),
        };

        File.WriteAllText(outputName, fullParams.ToJson());
    }

    private ContextParams? GetContextParams(string? contextFile)
    {
        if (!File.Exists(contextFile))
        {
            return ContextParams.FromDefault();
        }

        if (contextFile != null)
        {
            try
            {
                return ContextParams.FromJson(File.ReadAllText(contextFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading context file: {ex.Message}");
                return ContextParams.FromDefault();
            }
        }

        return ContextParams.FromDefault();
    }

    private FullParams? GetFullParams(string? parameterFile, SamplingStrategy defaultSamplingStrategy)
    {
        if (File.Exists(parameterFile))
        {
            try
            {
                return FullParams.FromJson(File.ReadAllText(parameterFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading parameter file: {ex.Message}");
                return null;
            }
        }

        return defaultSamplingStrategy switch
        {
            SamplingStrategy.Greedy => FullParams.FromGreedyStrategy(),
            SamplingStrategy.BeamSearch => FullParams.FromBeamSearchStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(defaultSamplingStrategy)),
        };
    }

    private ILogger SetupLogger(string name, bool useConsoleLogger = false)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            if (useConsoleLogger)
            {
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                });
            }
        });
        return loggerFactory.CreateLogger(name);
    }

    private void SetupWhisperLogger(bool useConsoleLogger = false)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            if (useConsoleLogger)
            {
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                });
            }
        });
        var logger = loggerFactory.CreateLogger("Whisper");
        WhisperLogger.Instance.OnLog += (e) =>
        {
            switch (e.Level)
            {
                case DA.Whisper.LogLevel.Error:
                    logger.LogError(e.Message);
                    break;
                case DA.Whisper.LogLevel.Info:
                    logger.LogInformation(e.Message);
                    break;
                case DA.Whisper.LogLevel.Warning:
                    logger.LogWarning(e.Message);
                    break;
                default:
                    logger.LogDebug(e.Message);
                    break;
            }
        };
    }

    private Task RecordAudioAsync(Action<byte[]> transcribeAudio, string? deviceName = null, CancellationToken cancellationToken = default)
    {
        int sampleRate = 16_000;
        int bufferSize = 1024;
        var captureDevice = ALC.CaptureOpenDevice(null, sampleRate, ALFormat.Mono16, bufferSize);
        ALC.CaptureStart(captureDevice);

        var buffer = new byte[bufferSize];
        while (true)
        {
            var current = 0;
            while (current < buffer.Length && !cancellationToken.IsCancellationRequested)
            {
                var samplesAvailable = ALC.GetInteger(captureDevice, AlcGetInteger.CaptureSamples);
                if (samplesAvailable < 512)
                {
                    continue;
                }

                var samplesToRead = Math.Min(samplesAvailable, buffer.Length - current);
                ALC.CaptureSamples(captureDevice, ref buffer[current], samplesToRead);
                current += samplesToRead;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            transcribeAudio(buffer.ToArray());
        }

        ALC.CaptureStop(captureDevice);

        return Task.CompletedTask;
    }
}