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
    Greedy = (int)whisper_sampling_strategy.WHISPER_SAMPLING_GREEDY,

    /// <summary>
    /// The beam search sampling strategy.
    /// </summary>
    BeamSearch = (int)whisper_sampling_strategy.WHISPER_SAMPLING_BEAM_SEARCH,
}
