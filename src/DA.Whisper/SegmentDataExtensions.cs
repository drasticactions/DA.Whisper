// <copyright file="SegmentDataExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json;

namespace DA.Whisper;

/// <summary>
/// Segment Data Extensions.
/// </summary>
public static class SegmentDataExtensions
{
    /// <summary>
    /// Convert a list of SegmentData to a JSON string.
    /// </summary>
    /// <param name="segments">List of SegmentData.</param>
    /// <returns>JSON string.</returns>
    public static string ToJson(this List<SegmentData> segments)
    {
        return JsonSerializer.Serialize(segments, SourceGenerationContext.Default.ListSegmentData);
    }
}