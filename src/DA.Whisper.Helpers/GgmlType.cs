// <copyright file="GgmlType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the available types of Whisper models.
/// </summary>
public enum GgmlType
{
    /// <summary>
    /// Unknown model.
    /// </summary>
    Unknown,

    /// <summary>
    /// Tiny model.
    /// </summary>
    Tiny,

    /// <summary>
    /// Tiny model for English.
    /// </summary>
    TinyEn,

    /// <summary>
    /// Base model.
    /// </summary>
    Base,

    /// <summary>
    /// Base model for English.
    /// </summary>
    BaseEn,

    /// <summary>
    /// Small model.
    /// </summary>
    Small,

    /// <summary>
    /// Small model for English.
    /// </summary>
    SmallEn,

    /// <summary>
    /// Small model for English with local diarization.
    /// </summary>
    SmallEnTdrz,

    /// <summary>
    /// Medium model.
    /// </summary>
    Medium,

    /// <summary>
    /// Medium model for English.
    /// </summary>
    MediumEn,

    /// <summary>
    /// Large model, Version 1.
    /// </summary>
    LargeV1,

    /// <summary>
    /// Large model, Version 2.
    /// </summary>
    LargeV2,

    /// <summary>
    /// Large model, Version 3.
    /// </summary>
    LargeV3,
}