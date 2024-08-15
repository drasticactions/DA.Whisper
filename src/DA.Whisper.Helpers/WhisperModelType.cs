// <copyright file="WhisperModelType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the available types of Whisper models.
/// </summary>
public enum WhisperModelType
{
    /// <summary>
    /// Tiny model.
    /// </summary>
    Tiny,

    /// <summary>
    /// Tiny model with quantization 5.1.
    /// </summary>
    TinyQ51,

    /// <summary>
    /// Tiny model for English.
    /// </summary>
    TinyEn,

    /// <summary>
    /// Tiny model for English with quantization 5.1.
    /// </summary>
    TinyEnQ51,

    /// <summary>
    /// Tiny model for English with quantization 8.0.
    /// </summary>
    TinyEnQ80,

    /// <summary>
    /// Base model.
    /// </summary>
    Base,

    /// <summary>
    /// Base model with quantization 5.1.
    /// </summary>
    BaseQ51,

    /// <summary>
    /// Base model for English.
    /// </summary>
    BaseEn,

    /// <summary>
    /// Base model for English with quantization 5.1.
    /// </summary>
    BaseEnQ51,

    /// <summary>
    /// Small model for English with quantization 8.0.
    /// </summary>
    Small,

    /// <summary>
    /// Small model with quantization 5.1.
    /// </summary>
    SmallQ51,

    /// <summary>
    /// Small model for English.
    /// </summary>
    SmallEn,

    /// <summary>
    /// Small model for English with quantization 5.1.
    /// </summary>
    SmallEnQ51,

    /// <summary>
    /// Small model for English with local diarization.
    /// </summary>
    SmallEnTdrz,

    /// <summary>
    /// Medium model.
    /// </summary>
    Medium,

    /// <summary>
    /// Medium model with quantization 5.0.
    /// </summary>
    MediumQ50,

    /// <summary>
    /// Medium model for English.
    /// </summary>
    MediumEn,

    /// <summary>
    /// Medium model for English with quantization 5.0.
    /// </summary>
    MediumEnQ50,

    /// <summary>
    /// Large model, Version 1.
    /// </summary>
    LargeV1,

    /// <summary>
    /// Large model, Version 2.
    /// </summary>
    LargeV2,

    /// <summary>
    /// Large model, Version 2 with quantization 5.0.
    /// </summary>
    Large2Q50,

    /// <summary>
    /// Large model, Version 3.
    /// </summary>
    LargeV3,

    /// <summary>
    /// Large model, Version 3 with quantization 5.0.
    /// </summary>
    LargeV3Q50,
}