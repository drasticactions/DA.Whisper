// <copyright file="ContextParams.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DA.Whisper;

/// <summary>
/// Represents the parameters for the context of Whisper.
/// </summary>
public class ContextParams
{
    private const byte TrueByte = 1;
    private const byte FalseByte = 0;
    private whisper_context_params @params;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextParams"/> class.
    /// </summary>
    [JsonConstructor]
    public ContextParams()
    {
        this.@params = NativeMethods.whisper_context_default_params();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use GPU for processing.
    /// </summary>
    public bool UseGpu
    {
        get => this.@params.use_gpu != FalseByte;
        set => this.@params.use_gpu = (value ? TrueByte : FalseByte);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use flash attention.
    /// </summary>
    public bool FlashAttn
    {
        get => this.@params.flash_attn != FalseByte;
        set => this.@params.flash_attn = (value ? TrueByte : FalseByte);
    }

    /// <summary>
    /// Gets or sets the GPU device index.
    /// </summary>
    public int GpuDevice
    {
        get => this.@params.gpu_device;
        set => this.@params.gpu_device = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use token timestamps for dynamic time warping (DTW).
    /// </summary>
    public bool DtwTokenTimestamps
    {
        get => this.@params.dtw_token_timestamps != FalseByte;
        set => this.@params.dtw_token_timestamps = (value ? TrueByte : FalseByte);
    }

    /// <summary>
    /// Gets or sets the preset value for the number of DTW aheads.
    /// </summary>
    public whisper_alignment_heads_preset DtwAheadsPreset
    {
        get => this.@params.dtw_aheads_preset;
        set => this.@params.dtw_aheads_preset = value;
    }

    /// <summary>
    /// Gets or sets the number of top results to return for DTW.
    /// </summary>
    public int DtwNTop
    {
        get => this.@params.dtw_n_top;
        set => this.@params.dtw_n_top = value;
    }

    /// <summary>
    /// Gets or sets the memory size for DTW.
    /// </summary>
    public nuint DtwMemSize
    {
        get => this.@params.dtw_mem_size;
        set => this.@params.dtw_mem_size = value;
    }

    /// <summary>
    /// Gets or sets the number of DTW aheads.
    /// </summary>
    public nuint NumberOfAheads
    {
        get => this.@params.dtw_aheads.n_heads;
        set => this.@params.dtw_aheads.n_heads = value;
    }

    /// <summary>
    /// Gets the array of aheads for DTW.
    /// </summary>
    public Ahead[] Aheads
    {
        get
        {
            unsafe
            {
                if (this.@params.dtw_aheads.heads == null)
                {
                    return Array.Empty<Ahead>();
                }

                var results = new Ahead[(int)this.@params.dtw_aheads.n_heads];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = new Ahead(&this.@params.dtw_aheads.heads[i]);
                }

                return results;
            }
        }
    }

    /// <summary>
    /// Gets the context parameters.
    /// </summary>
    internal ref whisper_context_params Params => ref this.@params;

    /// <summary>
    /// Creates a new instance of <see cref="ContextParams"/> with default parameters.
    /// </summary>
    /// <returns>A new instance of <see cref="ContextParams"/> with default parameters.</returns>
    public static ContextParams FromDefault() => new ContextParams();

    /// <summary>
    /// Converts the <see cref="ContextParams"/> object to a JSON string.
    /// </summary>
    /// <param name="json">JSON string.</param>
    /// <returns><see cref="ContextParams"/>.</returns>
    public static ContextParams FromJson(string json)
    {
        return JsonSerializer.Deserialize<ContextParams>(json, SourceGenerationContext.Default.ContextParams) ?? new ContextParams();
    }

    /// <summary>
    /// Converts the <see cref="FullParams"/> object to a JSON string.
    /// </summary>
    /// <returns>JSON string of full params.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, SourceGenerationContext.Default.FullParams);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var heads = string.Join<Ahead>(", ", this.Aheads);
        return $"Heads: {heads}, UseGpu: {this.UseGpu}, FlashAttn: {this.FlashAttn}, GpuDevice: {this.GpuDevice}, DtwTokenTimestamps: {this.DtwTokenTimestamps}, DtwAheadsPreset: {this.DtwAheadsPreset}, DtwNTop: {this.DtwNTop}, DtwMemSize: {this.DtwMemSize}, NumberOfAheads: {this.NumberOfAheads}";
    }
}