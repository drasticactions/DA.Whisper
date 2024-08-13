// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.Whisper;

// Console.WriteLine(DA.Whisper.WhisperModel.WhisperSystemInfo);
WhisperLogger.Instance.OnLog += (args) => Console.WriteLine(args);

var jfk = File.OpenRead("Z:\\Code\\Personal\\DA.Whisper\\external\\whisper.cpp\\samples\\jfk.wav");

var model = WhisperModel.FromFile("Z:\\Models\\Whisper\\ggml-medium.en.bin");
var processor = WhisperProcessor.CreateWithDefaultGreedyStrategy(model);

var result = processor.ProcessAsync(jfk);
await foreach (var segment in result)
{
    Console.WriteLine(segment);
}
