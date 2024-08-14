// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using ConsoleAppFramework;
using DA.Whisper;

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
    [Command("transcribe")]
    public void Transcribe([Argument]string mediaFile, string model)
    {
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
