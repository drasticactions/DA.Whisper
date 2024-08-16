// <copyright file="QuantizationType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents the available types of quantization.
/// Not all models support all quantization types.
/// </summary>
public enum QuantizationType
{
    /// <summary>
    /// Unknown quantization.
    /// </summary>
    Unknown,

    /// <summary>
    /// No quantization.
    /// </summary>
    NoQuantization,

    /// <summary>
    /// Quantization 5_0.
    /// </summary>
    Q5_0,

    /// <summary>
    /// Quantization 5_1.
    /// </summary>
    Q5_1,

    /// <summary>
    /// Quantization 8_0.
    /// </summary>
    Q8_0,
}