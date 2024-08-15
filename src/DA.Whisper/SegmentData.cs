// <copyright file="SegmentData.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the data for a segment.
/// </summary>
/// <param name="Text">The text of the segment.</param>
/// <param name="Start">The start time of the segment.</param>
/// <param name="End">The end time of the segment.</param>
/// <param name="MinProbability">The minimum probability of the segment.</param>
/// <param name="MaxProbability">The maximum probability of the segment.</param>
/// <param name="Probability">The probability of the segment.</param>
/// <param name="Language">The language of the segment.</param>
public record SegmentData(string Text, TimeSpan Start, TimeSpan End, float MinProbability, float MaxProbability, float Probability, string Language);