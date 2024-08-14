// <copyright file="SourceGenerationContext.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;
using DA.Whisper;

namespace DA.WhisperCli;

/// <summary>
/// Message Source Generation Context.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FullParams))]
[JsonSerializable(typeof(ContextParams))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}