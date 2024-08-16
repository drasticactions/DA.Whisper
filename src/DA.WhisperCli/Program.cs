// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using ConsoleAppFramework;
using DA.Whisper;
using DA.WhisperCli;
using Microsoft.Extensions.Logging;

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
        string? contextFile = default,
        string? parameterFile = default,
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

        await foreach (var segment in result)
        {
            var text = printTimestamps ? $"[{segment.Start} - {segment.End}] {segment.Text}" : segment.Text;
            consoleLog.Log(text);
            segments?.Add(segment);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            consoleLog.Log("Processing was cancelled.");
            return;
        }

        if (transcoded)
        {
            consoleLog.LogDebug($"Deleting transcoded file: {processFile}");
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
            var srtSubtitle = SrtSubtitle.FromSegments(segments);

            var outputFile = Path.Combine(outputDir, outputFilename ?? Path.GetFileNameWithoutExtension(mediaFileName) + ".srt");
            consoleLog.Log($"Writing SRT file: {outputFile}");
            File.WriteAllText(outputFile, srtSubtitle.ToString());
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

        whisperModel.Dispose();
        processor.Dispose();
    }

    /// <summary>Get all models available for download.</summary>
    /// <param name="verbose">-v, Verbose logging.</param>
    [Command("all-models")]
    public void ShowAllModels(bool verbose = false)
    {
        var consoleLog = new ConsoleLog(verbose);
        var modelService = new WhisperModelService();
        consoleLog.LogDebug($"Cache Location: {WhisperStatic.DefaultPath}");
        foreach (var model in modelService.AllModels)
        {
            consoleLog.Log($"[{model.GgmlType} {model.QuantizationType}]");
        }
    }

    /// <summary>Get available models that have been downloaded and cached.</summary>
    /// <param name="verbose">-v, Verbose logging.</param>
    [Command("available-models")]
    public void ShowAvailableModels(bool verbose = false)
    {
        var consoleLog = new ConsoleLog(verbose);
        var modelService = new WhisperModelService();
        consoleLog.LogDebug($"Cache Location: {WhisperStatic.DefaultPath}");
        foreach (var model in modelService.AvailableModels)
        {
            consoleLog.Log($"[{model.GgmlType} {model.QuantizationType}]");
        }

        if (modelService.AvailableModels.Count == 0)
        {
            consoleLog.Log("No models available.");
        }
    }

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
        if (parameterFile != null)
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
}