// <copyright file="CachedWhisperModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.Whisper;

/// <summary>
/// Cached Whisper Model.
/// </summary>
public class CachedWhisperModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachedWhisperModel"/> class.
    /// </summary>
    /// <param name="type">GGML Type.</param>
    /// <param name="quantizationType">Quantization Type.</param>
    public CachedWhisperModel(GgmlType type, QuantizationType quantizationType)
    {
        this.GgmlType = type;
        this.QuantizationType = quantizationType;
        this.Name = $"{type.ToString()} - {quantizationType}";
        this.CachedType = CachedType.Managed;

        // TODO: Add descriptions
        var modelDescription = type switch
        {
            GgmlType.Tiny => "Tiny model trained on 1.5M samples",
            GgmlType.TinyEn => "Tiny model trained on 1.5M samples (English)",
            GgmlType.Base => "Base model trained on 1.5M samples",
            GgmlType.BaseEn => "Base model trained on 1.5M samples (English)",
            GgmlType.Small => "Small model trained on 1.5M samples",
            GgmlType.SmallEn => "Small model trained on 1.5M samples (English)",
            GgmlType.SmallEnTdrz => "Small model trained on 1.5M samples (English)",
            GgmlType.Medium => "Medium model trained on 1.5M samples",
            GgmlType.MediumEn => "Medium model trained on 1.5M samples (English)",
            GgmlType.LargeV1 => "Large model trained on 1.5M samples (v1)",
            GgmlType.LargeV2 => "Large model trained on 1.5M samples (v2)",
            GgmlType.LargeV3 => "Large model trained on 1.5M samples (v3)",
            _ => throw new NotImplementedException(),
        };

        this.Description = $"{modelDescription} - {quantizationType}";
    }

    /// <summary>
    /// Gets the type of the cached model.
    /// </summary>
    public CachedType CachedType { get; } = CachedType.Unknown;

    /// <summary>
    /// Gets the type of the GGML model.
    /// </summary>
    public GgmlType GgmlType { get; internal set; } = GgmlType.Unknown;

    /// <summary>
    /// Gets the type of the quantization.
    /// </summary>
    public QuantizationType QuantizationType { get; internal set; } = QuantizationType.Unknown;

    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the location of the model.
    /// </summary>
    public string FileLocation { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the download url of the model.
    /// </summary>
    public string DownloadUrl { get; } = string.Empty;

    /// <summary>
    /// Gets the description of the model.
    /// </summary>
    public string Description { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the model exists.
    /// </summary>
    public bool Exists => !string.IsNullOrEmpty(this.FileLocation) && System.IO.Path.Exists(this.FileLocation);
}