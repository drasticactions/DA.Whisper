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
    /// <summary>Transcribe audio file to text.</summary>
    /// <param name="model">-m, Whisper Model.</param>
    /// <param name="audioFile">-a, Audio file to transcribe.</param>
    [Command("transcribe")]
    public void Transcribe(string model, string audioFile)
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
