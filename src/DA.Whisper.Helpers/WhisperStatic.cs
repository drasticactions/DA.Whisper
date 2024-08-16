// <copyright file="WhisperStatic.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.Whisper;

/// <summary>
/// Whisper Static Helper.
/// </summary>
public static class WhisperStatic
{
    /// <summary>
    /// Gets the default path for Whisper models.
    /// </summary>
    public static string DefaultPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "DA.Whisper");

    /// <summary>
    /// Get the download URL for the GGML model.
    /// </summary>
    /// <param name="type">Type of model.</param>
    /// <param name="quantizationType">Quantization type.</param>
    /// <returns>String of download url.</returns>
    public static string ToDownloadUrl(this GgmlType type, QuantizationType quantizationType)
    {
        if (type == GgmlType.Unknown)
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (type == GgmlType.SmallEnTdrz)
        {
            return $"https://huggingface.co/akashmjn/tinydiarize-whisper.cpp/resolve/main/ggml-small.en-tdrz.bin";
        }

        var modelName = type.ToFilename();
        if (quantizationType != QuantizationType.NoQuantization)
        {
            modelName += $"-{quantizationType.ToFilename()}";
        }

        return $"https://huggingface.co/ggerganov/whisper.cpp/resolve/main/{modelName}.bin";
    }

    /// <summary>
    /// Get the model path.
    /// </summary>
    /// <param name="type">Type of model.</param>
    /// <param name="quantizationType">Quantization type.</param>
    /// <returns>String of model path.</returns>
    public static string GetModelPath(GgmlType type, QuantizationType quantizationType)
        => Path.Combine(DefaultPath, type.ToFilename(quantizationType));

    /// <summary>
    /// Gets the filenmae from the quantization type.
    /// </summary>
    /// <param name="type">GGML Type.</param>
    /// <returns>Filename.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If not set.</exception>
    public static string ToFilename(this QuantizationType type) => type switch
    {
        QuantizationType.NoQuantization => string.Empty,
        QuantizationType.Q5_0 => "q5_0",
        QuantizationType.Q5_1 => "q5_1",
        QuantizationType.Q8_0 => "q8_0",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    /// <summary>
    /// Gets the filenmae from the ggml type.
    /// </summary>
    /// <param name="type">GGML Type.</param>
    /// <returns>Filename.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If not set.</exception>
    public static string ToFilename(this GgmlType type) => type switch
    {
        GgmlType.Tiny => "ggml-tiny",
        GgmlType.TinyEn => "ggml-tiny.en",
        GgmlType.Base => "ggml-base",
        GgmlType.BaseEn => "ggml-base.en",
        GgmlType.Small => "ggml-small",
        GgmlType.SmallEn => "ggml-small.en",
        GgmlType.SmallEnTdrz => "ggml-small.en-tdrz",
        GgmlType.Medium => "ggml-medium",
        GgmlType.MediumEn => "ggml-medium.en",
        GgmlType.LargeV1 => "ggml-large-v1",
        GgmlType.LargeV2 => "ggml-large-v2",
        GgmlType.LargeV3 => "ggml-large-v3",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    /// <summary>
    /// Gets the filename from the ggml type and quantization type.
    /// </summary>
    /// <param name="type">GGML type.</param>
    /// <param name="quantizationType">Quantization type.</param>
    /// <returns>Filename string.</returns>
    public static string ToFilename(this GgmlType type, QuantizationType quantizationType)
    {
        var modelName = type.ToFilename();
        if (quantizationType != QuantizationType.NoQuantization)
        {
            modelName += $"-{quantizationType.ToFilename()}";
        }

        return $"{modelName}.bin";
    }

    /// <summary>
    /// Get the GGML model sha.
    /// </summary>
    /// <param name="type">GGML Type.</param>
    /// <returns>SHA as string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If not set.</exception>
    public static string ToSha(this GgmlType type) => type switch
    {
        GgmlType.Tiny => "bd577a113a864445d4c299885e0cb97d4ba92b5f",
        GgmlType.TinyEn => "c78c86eb1a8faa21b369bcd33207cc90d64ae9df",
        GgmlType.Base => "465707469ff3a37a2b9b8d8f89f2f99de7299dac",
        GgmlType.BaseEn => "137c40403d78fd54d454da0f9bd998f78703390c",
        GgmlType.Small => "55356645c2b361a969dfd0ef2c5a50d530afd8d5",
        GgmlType.SmallEn => "db8a495a91d927739e50b3fc1cc4c6b8f6c2d022",
        GgmlType.SmallEnTdrz => "ceac3ec06d1d98ef71aec665283564631055fd6129b79d8e1be4f9cc33cc54b4",
        GgmlType.Medium => "fd9727b6e1217c2f614f9b698455c4ffd82463b4",
        GgmlType.MediumEn => "8c30f0e44ce9560643ebd10bbe50cd20eafd3723",
        GgmlType.LargeV1 => "b1caaf735c4cc1429223d5a74f0f4d0b9b59a299",
        GgmlType.LargeV2 => "0f4c8e34f21cf1a914c59d8b3ce882345ad349d6",
        GgmlType.LargeV3 => "ad82bf6a9043ceed055076d0fd39f5f186ff8062",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}