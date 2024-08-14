// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;
using ConsoleAppFramework;
using DA.Whisper;
using DA.WhisperCli;

// WhisperLogger.Instance.OnLog += (args) =>
// {
//     Console.WriteLine(args);
// };

var app = ConsoleApp.Create();
app.Add<WhisperCommands>();
app.Run(args);

public class WhisperCommands
{
    /// <summary>Transcribe media file to text.</summary>
    /// <param name="mediaFile">Media file to transcribe.</param>
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="contextFile">-c, Optional Context Parameter file. Generate the file with 'generate-context-file'.</param>
    /// <param name="parameterFile">-p, Optional Parameter file. Generate the file with 'generate-parameter-file'.</param>
    /// <param name="defaultSamplingStrategy">-s, Default Sampling Strategy, ignored if parameter file is used.</param>
    /// <param name="transcodeFile">-t, Transcode the file using ffmpeg to a format that Whisper can process. Requires ffmpeg to be installed.</param>
    [Command("transcribe")]
    public async Task TranscribeAsync([Argument]string mediaFile, string model, string? contextFile = default, string? parameterFile = default, SamplingStrategy defaultSamplingStrategy = SamplingStrategy.Greedy, bool transcodeFile = true)
    {
        var logger = WhisperLogger.Instance;
        var ffmpeg = new FFMpegTranscodeService();
        var contextParams = this.GetContextParams(contextFile);
        if (contextParams == null)
        {
            Console.WriteLine("Unable to load context parameters.");
            return;
        }

        var fullParams = this.GetFullParams(parameterFile, defaultSamplingStrategy);
        if (fullParams == null)
        {
            Console.WriteLine("Unable to load parameters.");
            return;
        }

        using var whisperModel = WhisperModel.TryFromFileWithParameters(model, contextParams);
        if (whisperModel == null)
        {
            Console.WriteLine("Unable to load model.");
            return;
        }

        using var processor = WhisperProcessor.TryCreateWithParams(whisperModel, fullParams);
        if (processor == null)
        {
            Console.WriteLine("Unable to create processor.");
            return;
        }

        var processFile = transcodeFile ? await ffmpeg.ProcessFile(mediaFile) : mediaFile;
        using var processFileStream = File.OpenRead(processFile);
        var result = processor.ProcessAsync(processFileStream);
        await foreach (var segment in result)
        {
            Console.WriteLine(segment.Text);
        }
    }

    /// <summary>Generates the context parameters file.</summary>
    /// <param name="outputName">Output file name.</param>
    [Command("generate-context-file")]
    public void GenerateContextFile(string outputName = "context_params.json")
    {
        File.WriteAllText(outputName, JsonSerializer.Serialize(ContextParams.FromDefault(), SourceGenerationContext.Default.FullParams));
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

        File.WriteAllText(outputName, JsonSerializer.Serialize(fullParams, SourceGenerationContext.Default.FullParams));
    }

    private ContextParams? GetContextParams(string? contextFile)
    {
        if (contextFile != null)
        {
            try
            {
                return JsonSerializer.Deserialize<ContextParams>(File.ReadAllText(contextFile), SourceGenerationContext.Default.ContextParams);
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
                return JsonSerializer.Deserialize<FullParams>(File.ReadAllText(parameterFile), SourceGenerationContext.Default.FullParams);
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
}

// var jfk = File.OpenRead("/Users/drasticactions/Developer/Apps/DA.Whisper/external/whisper.cpp/samples/jfk.wav");

// var model = WhisperModel.FromFile("/Users/drasticactions/Developer/Models/Whisper/ggml-tiny.en.bin");
// var processor = WhisperProcessor.CreateWithDefaultGreedyStrategy(model);

// var result = processor.ProcessAsync(jfk);
// await foreach (var segment in result)
// {
//     Console.WriteLine(segment);
// }
