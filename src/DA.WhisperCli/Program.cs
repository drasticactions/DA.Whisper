// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using System.Text.RegularExpressions;
using ConsoleAppFramework;
using DA.Whisper;
using DA.WhisperCli;
using Downloader;
using Microsoft.Extensions.Logging;
using OpenTK.Audio.OpenAL;
using YoutubeExplode.Videos.Streams;

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
    private bool whisperLoggingSet = false;
    private ConsoleLog? consoleLog;

    /// <summary>
    /// Gets the native Whisper system info from the bundled library.
    /// </summary>
    [Command("system-info")]
    public void GetSystemInfo()
    {
        Console.WriteLine(Whisper.GetSystemInfo());
    }

    /// <summary>Transcribe directory of media. Not recursive.</summary>
    /// <param name="directory">Directory to transcribe.</param>
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="contextFile">-c, Optional Context Parameter file. Generate the file with 'generate-context-file'.</param>
    /// <param name="parameterFile">-p, Optional Parameter file. Generate the file with 'generate-parameter-file'.</param>
    /// <param name="extensions">-e, File extensions to transcribe.</param>
    /// <param name="defaultSamplingStrategy">-s, Default Sampling Strategy, ignored if parameter file is used.</param>
    /// <param name="printTimestamps">-ts, Print timestamps with the text.</param>
    /// <param name="force">-fo, Force overwrite of existing files.</param>
    /// <param name="outputFormats">-f, Output the text to files.</param>
    /// <param name="outputDirectory">-o, Output directory for the subtitle file, defaults to the current working directory.</param>
    /// <param name="outputFilename">-of, Output file name for the subtitle file, defaults to the name of the media file if available.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("transcribe directory")]
    public async Task TranscribeDirectoryAsync(
    [Argument] string directory,
    string model,
    string? contextFile = "context_file.json",
    string? parameterFile = "full_params.json",
    string[]? extensions = default,
    SamplingStrategy defaultSamplingStrategy = SamplingStrategy.Greedy,
    bool printTimestamps = false,
    bool force = false,
    string[]? outputFormats = default,
    string? outputDirectory = default,
    string? outputFilename = default,
    bool verbose = false,
    CancellationToken cancellationToken = default)
    {
        extensions = extensions ?? new[] { ".mp4", ".mkv", ".avi", ".mov", ".webm", ".flv", ".wmv", ".mp3", ".wav", ".flac", ".ogg" };
        outputFormats = outputFormats ?? new[] { "json" };
        this.consoleLog = new ConsoleLog(verbose);
        var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly).Where(f => extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)).ToList();
        if (files.Count == 0)
        {
            this.consoleLog.LogError("No files found.");
            return;
        }

        foreach (var file in files)
        {
            this.consoleLog.Log($"Processing file: {file}");
            await this.TranscribeAsync(file, model, contextFile, parameterFile, defaultSamplingStrategy, true, printTimestamps, force, outputFormats, outputDirectory, outputFilename, verbose, cancellationToken);
        }
    }

    /// <summary>Transcribe YouTube video to text.</summary>
    /// <param name="youtubeId">Youtube Id to transcribe.</param>
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="contextFile">-c, Optional Context Parameter file. Generate the file with 'generate-context-file'.</param>
    /// <param name="parameterFile">-p, Optional Parameter file. Generate the file with 'generate-parameter-file'.</param>
    /// <param name="defaultSamplingStrategy">-s, Default Sampling Strategy, ignored if parameter file is used.</param>
    /// <param name="printTimestamps">-ts, Print timestamps with the text.</param>
    /// <param name="force">-fo, Force overwrite of existing files.</param>
    /// <param name="outputFormats">-f, Output the text to files.</param>
    /// <param name="outputDirectory">-o, Output directory for the subtitle file, defaults to the current working directory.</param>
    /// <param name="outputFilename">-of, Output file name for the subtitle file, defaults to the name of the media file if available.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("transcribe youtube")]
    public async Task TranscribeYoutubeAsync(
        [Argument] string youtubeId,
        string model,
        string? contextFile = "context_file.json",
        string? parameterFile = "full_params.json",
        SamplingStrategy defaultSamplingStrategy = SamplingStrategy.Greedy,
        bool printTimestamps = false,
        bool force = false,
        string[]? outputFormats = default,
        string? outputDirectory = default,
        string? outputFilename = default,
        bool verbose = false,
        CancellationToken cancellationToken = default)
    {
        this.consoleLog = new ConsoleLog(verbose);
        var ffmpeg = new FFMpegTranscodeService(logger: this.SetupLogger("ffmpeg", verbose));

        var ytId = YoutubeExplode.Videos.VideoId.TryParse(youtubeId);
        if (!ytId.HasValue)
        {
            this.consoleLog.LogError("Invalid YouTube Id.");
            return;
        }

        var youtube = new YoutubeExplode.YoutubeClient();
        var video = await youtube.Videos.GetAsync(ytId.Value);
        this.consoleLog.Log($"Processing video: {video.Title}");
        var filename = this.MakeValidFileName(video.Title);
        if (!this.CanWriteFileName(filename))
        {
            this.consoleLog.LogError("Unable to write filename.");
            return;
        }

        var videoStreamInfoSet = await youtube.Videos.Streams.GetManifestAsync(ytId.Value);
        var audioStreamInfo = videoStreamInfoSet.GetAudioOnlyStreams().GetWithHighestBitrate();
        if (audioStreamInfo is null)
        {
            this.consoleLog.LogError("No audio stream found.");
            return;
        }

        var audioStream = await youtube.Videos.Streams.GetAsync(audioStreamInfo);
        if (audioStream is null)
        {
            this.consoleLog.LogError("Unable to download audio stream.");
            return;
        }

        var processResult = await ffmpeg.ProcessUri(audioStreamInfo.Url);

        outputFilename = outputFilename ?? filename;
        await this.TranscribeAsync(processResult, model, contextFile, parameterFile, defaultSamplingStrategy, false, printTimestamps, force, outputFormats, outputDirectory, outputFilename, verbose, cancellationToken);
        File.Delete(processResult);
    }

    /// <summary>
    /// Convert a JSON file to a subtitle file.
    /// </summary>
    /// <param name="jsonFile">JSON output file.</param>
    /// <param name="outputFormats">-f, Output the text to files.</param>
    /// <param name="outputDirectory">-o, Output directory for the subtitle file, defaults to the current working directory.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("convert-json")]
    public async Task ConvertJsonToSubtitleFileAsync([Argument] string jsonFile, string[]? outputFormats = default, string? outputDirectory = default, bool verbose = false, CancellationToken cancellationToken = default)
    {
        outputFormats = outputFormats ?? new[] { "json" };
        var consoleLog = new ConsoleLog(verbose);
        if (outputFormats.Length == 0)
        {
            consoleLog.LogError("No output formats specified.");
            return;
        }

        if (!File.Exists(jsonFile))
        {
            consoleLog.LogError("JSON file does not exist.");
            return;
        }

        var json = File.ReadAllText(jsonFile);
        var segments = JsonSerializer.Deserialize<List<SegmentData>>(json, SourceGenerationContext.Default.ListSegmentData);
        if (segments == null)
        {
            consoleLog.LogError("Unable to deserialize JSON file.");
            return;
        }

        var srt = outputFormats.Contains("srt", StringComparer.OrdinalIgnoreCase);
        var vtt = outputFormats.Contains("vtt", StringComparer.OrdinalIgnoreCase);
        var txt = outputFormats.Contains("txt", StringComparer.OrdinalIgnoreCase);

        var outputDir = outputDirectory ?? Directory.GetCurrentDirectory();

        if (srt)
        {
            var subtitle = Subtitle.FromSegments(segments);
            var outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(jsonFile) + ".srt");
            consoleLog.Log($"Writing SRT file: {outputFile}");
            await File.WriteAllTextAsync(outputFile, subtitle.ToString(), cancellationToken);
        }

        if (vtt)
        {
            var vttSubtitle = Subtitle.FromSegments(segments, SubtitleType.VTT);
            var vttOutputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(jsonFile) + ".vtt");
            consoleLog.Log($"Writing VTT file: {vttOutputFile}");
            await File.WriteAllTextAsync(vttOutputFile, vttSubtitle.ToString(), cancellationToken);
        }

        if (txt)
        {
            var txtOutputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(jsonFile) + ".txt");
            consoleLog.Log($"Writing TXT file: {txtOutputFile}");
            await File.WriteAllTextAsync(txtOutputFile, string.Join("\n", segments.Select(s => s.Text)), cancellationToken);
        }
    }

    /// <summary>Transcribe media file to text.</summary>
    /// <param name="mediaFile">Media file to transcribe.</param>
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="contextFile">-c, Optional Context Parameter file. Generate the file with 'generate-context-file'.</param>
    /// <param name="parameterFile">-p, Optional Parameter file. Generate the file with 'generate-parameter-file'.</param>
    /// <param name="defaultSamplingStrategy">-s, Default Sampling Strategy, ignored if parameter file is used.</param>
    /// <param name="transcodeFile">-t, Transcode the file using ffmpeg to a format that Whisper can process. Requires ffmpeg to be installed.</param>
    /// <param name="printTimestamps">-ts, Print timestamps with the text.</param>
    /// <param name="force">-fo, Force overwrite of existing files.</param>
    /// <param name="outputFormats">-f, Output the text to files.</param>
    /// <param name="outputDirectory">-o, Output directory for the subtitle file, defaults to the current working directory.</param>
    /// <param name="outputFilename">-of, Output file name for the subtitle file, defaults to the name of the media file if available.</param>
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
        bool force = false,
        string[]? outputFormats = default,
        string? outputDirectory = default,
        string? outputFilename = default,
        bool verbose = false,
        CancellationToken cancellationToken = default)
    {
        outputFormats = outputFormats ?? new[] { "json" };
        this.consoleLog ??= new ConsoleLog(verbose);
        var supportOutputFormats = outputFormats?.Length > 0;
        var enumOutputFormats = outputFormats?.Select(o => Enum.Parse<OutputFormat>(o, true)).ToArray();
        var srt = enumOutputFormats?.Contains(OutputFormat.SRT) ?? false;
        var json = enumOutputFormats?.Contains(OutputFormat.Json) ?? false;
        var vtt = enumOutputFormats?.Contains(OutputFormat.VTT) ?? false;
        var txt = enumOutputFormats?.Contains(OutputFormat.TXT) ?? false;
        var outputDir = outputDirectory ?? Directory.GetCurrentDirectory();
        var mediaFileName = outputFilename ?? Path.GetFileNameWithoutExtension(mediaFile);
        this.consoleLog.LogDebug($"Media file name: {mediaFileName}");
        var jsonName = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".json");
        this.consoleLog.LogDebug($"JSON: {jsonName}");

        if (srt && File.Exists(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".srt")) && !force)
        {
            this.consoleLog.LogError("SRT file already exists.");
            return;
        }

        if (json && File.Exists(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".json")) && !force)
        {
            this.consoleLog.LogError("JSON file already exists.");
            return;
        }

        if (vtt && File.Exists(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".vtt")) && !force)
        {
            this.consoleLog.LogError("VTT file already exists.");
            return;
        }

        if (txt && File.Exists(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".txt")) && !force)
        {
            this.consoleLog.LogError("TXT file already exists.");
            return;
        }

        this.SetupWhisperLogger(verbose);
        var ffmpeg = new FFMpegTranscodeService(logger: this.SetupLogger("ffmpeg", verbose));
        var contextParams = this.GetContextParams(contextFile);
        if (contextParams == null)
        {
            this.consoleLog.LogError("Unable to load context parameters.");
            return;
        }

        var fullParams = this.GetFullParams(parameterFile, defaultSamplingStrategy);
        if (fullParams == null)
        {
            this.consoleLog.LogError("Unable to load parameters.");
            return;
        }

        if (!WhisperModel.TryFromFileWithParameters(model, contextParams, out WhisperModel? whisperModel) || whisperModel is null)
        {
            this.consoleLog.LogError("Unable to load model.");
            return;
        }

        if (!WhisperProcessor.TryCreateWithParams(whisperModel, fullParams, out WhisperProcessor? processor) || processor is null)
        {
            this.consoleLog.LogError("Unable to create processor.");
            return;
        }

        if (!File.Exists(mediaFile))
        {
            this.consoleLog.LogError("Media file does not exist.");
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
                this.consoleLog.LogError($"Error transcoding file: {ex.Message}");
                return;
            }
        }

        using var processFileStream = File.OpenRead(processFile);
        var result = processor.ProcessAsync(processFileStream, cancellationToken);
        List<SegmentData>? segments = outputFormats?.Length > 0 ? new() : null;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await foreach (var segment in result)
        {
            var text = printTimestamps ? $"[{segment.Start} - {segment.End}] {segment.Text}" : segment.Text;
            this.consoleLog.Log(text);
            segments?.Add(segment);
        }

        stopwatch.Stop();

        if (cancellationToken.IsCancellationRequested)
        {
            this.consoleLog.Log("Processing was cancelled.");
            return;
        }

        this.consoleLog.Log($"Processing took: {stopwatch.Elapsed}");

        if (transcoded)
        {
            this.consoleLog.LogDebug($"Deleting transcoded file: {processFile}");
            await processFileStream.DisposeAsync();
            File.Delete(processFile);
        }

        if (string.IsNullOrEmpty(mediaFileName))
        {
            this.consoleLog.LogDebug("Media file name is empty, using 'output' as the file name.");
            mediaFileName = "output";
        }

        if (srt && segments != null)
        {
            var subtitle = Subtitle.FromSegments(segments);

            var outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".srt");
            this.consoleLog.Log($"Writing SRT file: {outputFile}");
            File.WriteAllText(outputFile, subtitle.ToString());
        }
        else
        {
            this.consoleLog.LogDebug("Not writing SRT file.");
        }

        if (json && segments != null)
        {
            var jsonOutputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".json");
            this.consoleLog.Log($"Writing JSON file: {jsonOutputFile}");
            File.WriteAllText(jsonOutputFile, segments.ToJson());
        }
        else
        {
            this.consoleLog.LogDebug("Not writing JSON file.");
        }

        if (vtt && segments != null)
        {
            var vttSubtitle = Subtitle.FromSegments(segments, SubtitleType.VTT);
            var vttOutputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".vtt");
            this.consoleLog.Log($"Writing VTT file: {vttOutputFile}");
            File.WriteAllText(vttOutputFile, vttSubtitle.ToString());
        }
        else
        {
            this.consoleLog.LogDebug("Not writing VTT file.");
        }

        if (txt && segments != null)
        {
            var txtOutputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(mediaFileName) + ".txt");
            this.consoleLog.Log($"Writing TXT file: {txtOutputFile}");
            File.WriteAllText(txtOutputFile, string.Join("\n", segments.Select(s => s.Text)));
        }
        else
        {
            this.consoleLog.LogDebug("Not writing TXT file.");
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
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Task.</returns>
    [Command("realtime")]
    public async Task RealtimeAsync(string model, bool verbose = false, CancellationToken cancellationToken = default)
    {
        var consoleLog = new ConsoleLog(verbose);
        this.SetupWhisperLogger(verbose);
        if (!WhisperModel.TryFromFileWithParameters(model, ContextParams.FromDefault(), out WhisperModel? whisperModel) || whisperModel is null)
        {
            consoleLog.LogError("Unable to load model.");
            return;
        }

        if (!WhisperProcessor.TryCreateWithParams(whisperModel, FullParams.FromGreedyStrategy(), out WhisperProcessor? processor) || processor is null)
        {
            consoleLog.LogError("Unable to create processor.");
            return;
        }

        consoleLog.LogDebug("Default Mic:\n" + ALC.GetString(ALDevice.Null, AlcGetString.CaptureDefaultDeviceSpecifier));
        consoleLog.LogDebug("Mic List:\n" + string.Join("\n", ALC.GetString(ALDevice.Null, AlcGetStringList.CaptureDeviceSpecifier)));
        var totalLength = 1024 * 100;
        var isRecording = false;
        var length = 0;
        var byteSample = new byte[totalLength];
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await this.RecordAudioAsync(
        (audio) =>
        {
            var hasSpeech = DetectSpeech(audio);

            if (hasSpeech && !isRecording)
            {
                consoleLog.Log("Recording...");
                isRecording = true;
            }

            if (hasSpeech)
            {
                stopwatch.Reset();
                stopwatch.Restart();
            }

            var lengthMet = length >= totalLength;

            if (isRecording && !lengthMet)
            {
                Array.Copy(audio, 0, byteSample, length, audio.Length);
                length += audio.Length;
            }

            var snapshot = isRecording && (stopwatch.ElapsedMilliseconds > 1500 || lengthMet);

            if (isRecording && snapshot)
            {
                // Create a new byte array based on the current length
                lock (this)
                {
                    stopwatch.Stop();
                    stopwatch.Reset();
                    isRecording = false;
                    var newArray = new byte[totalLength];
                    Array.Copy(byteSample, 0, newArray, 0, length);
                    byteSample = new byte[totalLength];
                    Task.Run(async () => await ProcessSamples(newArray));
                    length = 0;
                }

                consoleLog.Log("Done...");
            }

            async Task ProcessSamples(byte[] byteSample)
            {
                var floatSample = new float[totalLength / 2];
                for (int i = 0; i < totalLength / 2; i++)
                {
                    floatSample[i] = BitConverter.ToInt16(byteSample, i * 2) / 32768f;
                }

                var result = processor.ProcessRawAsync(floatSample, cancellationToken);
                await foreach (var segment in result)
                {
                    consoleLog.Log(segment.Text);
                }
            }

            bool DetectSpeech(byte[] audioData)
            {
                var sum = 0;
                for (int i = 0; i < audioData.Length; i += 2)
                {
                    var sample = BitConverter.ToInt16(audioData, i);
                    if (sample < -32768f)
                    {
                        return true;
                    }

                    sum += Math.Abs(sample);
                }

                var avg = sum / (audioData.Length / 2);
                return avg > 1000;
            }
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
        if (this.whisperLoggingSet)
        {
            return;
        }

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
        this.whisperLoggingSet = true;
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

    private string MakeValidFileName(string input)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string validName = Regex.Replace(input, $"[{Regex.Escape(new string(invalidChars))}]", "_");
        validName = validName.Trim();
        if (validName.Length > 255)
        {
            validName = validName.Substring(0, 255);
        }

        return validName;
    }

    private bool CanWriteFileName(string fileName)
    {
        string tempPath = Path.GetTempPath();
        string tempFile = Path.Combine(tempPath, fileName);

        try
        {
            using (FileStream fs = File.Create(tempFile))
            {
                fs.Close();
            }

            File.Delete(tempFile);
            return true;
        }
        catch
        {
            return false;
        }
    }
}