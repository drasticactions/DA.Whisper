// <copyright file="SamplingStrategy.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the sampling strategy used in the Whisper system.
/// </summary>
public enum SamplingStrategy
{
    /// <summary>
    /// The greedy sampling strategy.
    /// </summary>
    Greedy = 0,

    /// <summary>
    /// The beam search sampling strategy.
    /// </summary>
    BeamSearch = 1,
}
