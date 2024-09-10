// <copyright file="FullParams.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DA.Whisper;

/// <summary>
/// Represents the full parameters for the Whisper class.
/// </summary>
public class FullParams
{
    private const byte TrueByte = 1;
    private const byte FalseByte = 0;

    /// <summary>
    /// The underlying whisper_full_params object.
    /// </summary>
    private whisper_full_params @params;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullParams"/> class with the specified sampling strategy.
    /// </summary>
    [JsonConstructor]
    public FullParams()
    {
        unsafe
        {
            this.@params = NativeMethods.whisper_full_default_params((int)SamplingStrategy.Greedy);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FullParams"/> class with the specified sampling strategy.
    /// </summary>
    /// <param name="strategy">The sampling strategy.</param>
    internal FullParams(SamplingStrategy strategy)
    {
        unsafe
        {
            this.@params = NativeMethods.whisper_full_default_params((whisper_sampling_strategy)strategy);
        }
    }

    /// <summary>
    /// Gets or sets the strategy.
    /// </summary>
    public whisper_sampling_strategy Strategy
    {
        get => this.@params.strategy;
        set => this.@params.strategy = value;
    }

    /// <summary>
    /// Gets or sets the number of threads.
    /// </summary>
    public int Threads
    {
        get => this.@params.n_threads;
        set => this.@params.n_threads = value;
    }

    /// <summary>
    /// Gets or sets the maximum text context.
    /// </summary>
    public int MaxTextContext
    {
        get => this.@params.n_max_text_ctx;
        set => this.@params.n_max_text_ctx = value;
    }

    /// <summary>
    /// Gets or sets the offset in milliseconds.
    /// </summary>
    public int OffsetMs
    {
        get => this.@params.offset_ms;
        set => this.@params.offset_ms = value;
    }

    /// <summary>
    /// Gets or sets the duration in milliseconds.
    /// </summary>
    public int DurationMs
    {
        get => this.@params.duration_ms;
        set => this.@params.duration_ms = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable translation.
    /// </summary>
    public bool Translate
    {
        get => this.@params.translate != FalseByte;
        set => this.@params.translate = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to disable context.
    /// </summary>
    public bool NoContext
    {
        get => this.@params.no_context != FalseByte;
        set => this.@params.no_context = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to disable timestamps.
    /// </summary>
    public bool NoTimestamps
    {
        get => this.@params.no_timestamps != FalseByte;
        set => this.@params.no_timestamps = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use single segment.
    /// </summary>
    public bool SingleSegment
    {
        get => this.@params.single_segment != FalseByte;
        set => this.@params.single_segment = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to print special characters.
    /// </summary>
    public bool PrintSpecial
    {
        get => this.@params.print_special != FalseByte;
        set => this.@params.print_special = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to print progress.
    /// </summary>
    public bool PrintProgress
    {
        get => this.@params.print_progress != FalseByte;
        set => this.@params.print_progress = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to print in real-time.
    /// </summary>
    public bool PrintRealtime
    {
        get => this.@params.print_realtime != FalseByte;
        set => this.@params.print_realtime = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to print timestamps.
    /// </summary>
    public bool PrintTimestamps
    {
        get => this.@params.print_timestamps != FalseByte;
        set => this.@params.print_timestamps = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to include token timestamps.
    /// </summary>
    public bool TokenTimestamps
    {
        get => this.@params.token_timestamps != FalseByte;
        set => this.@params.token_timestamps = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets the threshold for PT.
    /// </summary>
    public float ThresholdPt
    {
        get => this.@params.thold_pt;
        set => this.@params.thold_pt = value;
    }

    /// <summary>
    /// Gets or sets the threshold for PT sum.
    /// </summary>
    public float ThresholdPtsum
    {
        get => this.@params.thold_ptsum;
        set => this.@params.thold_ptsum = value;
    }

    /// <summary>
    /// Gets or sets the maximum length.
    /// </summary>
    public int MaxLength
    {
        get => this.@params.max_len;
        set => this.@params.max_len = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to split on word.
    /// </summary>
    public bool SplitOnWord
    {
        get => this.@params.split_on_word != FalseByte;
        set
        {
            var byteValue = value ? 1 : 0;
            this.@params.split_on_word = (byte)byteValue;
        }
    }

    /// <summary>
    /// Gets or sets the maximum number of tokens.
    /// </summary>
    public int MaxTokens
    {
        get => this.@params.max_tokens;
        set => this.@params.max_tokens = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable debug mode.
    /// </summary>
    public bool DebugMode
    {
        get => this.@params.debug_mode != FalseByte;
        set => this.@params.debug_mode = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets the audio context.
    /// </summary>
    public int AudioContext
    {
        get => this.@params.audio_ctx;
        set => this.@params.audio_ctx = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable TDRZ.
    /// </summary>
    public bool TdrzEnable
    {
        get => this.@params.tdrz_enable != FalseByte;
        set => this.@params.tdrz_enable = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    public string Language
    {
        get
        {
            unsafe
            {
                return Marshal.PtrToStringAnsi((IntPtr)this.@params.language) ?? string.Empty;
            }
        }

        set
        {
            unsafe
            {
                this.@params.language = (byte*)Marshal.StringToHGlobalAnsi(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the initial prompt.
    /// </summary>
    public string InitialPrompt
    {
        get
        {
            unsafe
            {
                return Marshal.PtrToStringAnsi((IntPtr)this.@params.initial_prompt) ?? string.Empty;
            }
        }

        set
        {
            unsafe
            {
                this.@params.initial_prompt = (byte*)Marshal.StringToHGlobalAnsi(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the suppression regex.
    /// </summary>
    public string SuppressRegex
    {
        get
        {
            unsafe
            {
                return Marshal.PtrToStringAnsi((IntPtr)this.@params.suppress_regex) ?? string.Empty;
            }
        }

        set
        {
            unsafe
            {
                this.@params.suppress_regex = (byte*)Marshal.StringToHGlobalAnsi(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to detect language.
    /// </summary>
    public bool DetectLanguage
    {
        get => this.@params.detect_language != FalseByte;
        set => this.@params.detect_language = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to suppress blank.
    /// </summary>
    public bool SuppressBlank
    {
        get => this.@params.suppress_blank != FalseByte;
        set => this.@params.suppress_blank = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to suppress non-speech tokens.
    /// </summary>
    public bool SuppressNonSpeechTokens
    {
        get => this.@params.suppress_non_speech_tokens != FalseByte;
        set => this.@params.suppress_non_speech_tokens = value ? TrueByte : FalseByte;
    }

    /// <summary>
    /// Gets or sets the temperature.
    /// </summary>
    public float Temperature
    {
        get => this.@params.temperature;
        set => this.@params.temperature = value;
    }

    /// <summary>
    /// Gets or sets the maximum initial timestamp.
    /// </summary>
    public float MaxInitialTimestamp
    {
        get => this.@params.max_initial_ts;
        set => this.@params.max_initial_ts = value;
    }

    /// <summary>
    /// Gets or sets the length penalty.
    /// </summary>
    public float LengthPenalty
    {
        get => this.@params.length_penalty;
        set => this.@params.length_penalty = value;
    }

    /// <summary>
    /// Gets or sets the temperature increment.
    /// </summary>
    public float TemperatureIncrement
    {
        get => this.@params.temperature_inc;
        set => this.@params.temperature_inc = value;
    }

    /// <summary>
    /// Gets or sets the entropy threshold.
    /// </summary>
    public float EntropyThreshold
    {
        get => this.@params.entropy_thold;
        set => this.@params.entropy_thold = value;
    }

    /// <summary>
    /// Gets or sets the log probability threshold.
    /// </summary>
    public float LogprobThreshold
    {
        get => this.@params.logprob_thold;
        set => this.@params.logprob_thold = value;
    }

    /// <summary>
    /// Gets or sets the no speech threshold.
    /// </summary>
    public float NoSpeechThreshold
    {
        get => this.@params.no_speech_thold;
        set => this.@params.no_speech_thold = value;
    }

    /// <summary>
    /// Gets or sets the best of value for greedy strategy.
    /// </summary>
    public int BestOf
    {
        get => this.@params.greedy.best_of;
        set => this.@params.greedy.best_of = value;
    }

    /// <summary>
    /// Gets or sets the beam size for beam search strategy.
    /// </summary>
    public int BeamSize
    {
        get => this.@params.beam_search.beam_size;
        set => this.@params.beam_search.beam_size = value;
    }

    /// <summary>
    /// Gets or sets the patience for beam search strategy.
    /// </summary>
    public float Patience
    {
        get => this.@params.beam_search.patience;
        set => this.@params.beam_search.patience = value;
    }

    /// <summary>
    /// Gets the underlying whisper_full_params object.
    /// </summary>
    internal ref whisper_full_params Params => ref this.@params;

    /// <summary>
    /// Creates a new instance of <see cref="FullParams"/> with the Greedy sampling strategy.
    /// </summary>
    /// <returns>A new instance of <see cref="FullParams"/> with the Greedy sampling strategy.</returns>
    public static FullParams FromGreedyStrategy()
    {
        return new FullParams(SamplingStrategy.Greedy);
    }

    /// <summary>
    /// Converts the <see cref="FullParams"/> object to a JSON string.
    /// </summary>
    /// <param name="json">JSON string.</param>
    /// <returns><see cref="FullParams"/>.</returns>
    public static FullParams FromJson(string json)
    {
        return JsonSerializer.Deserialize<FullParams>(json, SourceGenerationContext.Default.FullParams) ?? FullParams.FromGreedyStrategy();
    }

    /// <summary>
    /// Creates a new instance of <see cref="FullParams"/> with the BeamSearch sampling strategy.
    /// </summary>
    /// <returns>A new instance of <see cref="FullParams"/> with the BeamSearch sampling strategy.</returns>
    public static FullParams FromBeamSearchStrategy()
    {
        return new FullParams(SamplingStrategy.BeamSearch);
    }

    /// <summary>
    /// Converts the <see cref="FullParams"/> object to a JSON string.
    /// </summary>
    /// <returns>JSON string of full params.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, SourceGenerationContext.Default.FullParams);
    }

    /// <summary>
    /// Returns a string representation of the <see cref="FullParams"/> object.
    /// </summary>
    /// <returns>A string representation of the <see cref="FullParams"/> object.</returns>
    public override string ToString()
    {
        return $"strategy: {this.Strategy}\n" +
               $"n_threads: {this.Threads}\n" +
               $"n_max_text_ctx: {this.MaxTextContext}\n" +
               $"offset_ms: {this.OffsetMs}\n" +
               $"duration_ms: {this.DurationMs}\n" +
               $"translate: {this.Translate}\n" +
               $"no_context: {this.NoContext}\n" +
               $"no_timestamps: {this.NoTimestamps}\n" +
               $"single_segment: {this.SingleSegment}\n" +
               $"print_special: {this.PrintSpecial}\n" +
               $"print_progress: {this.PrintProgress}\n" +
               $"print_realtime: {this.PrintRealtime}\n" +
               $"print_timestamps: {this.PrintTimestamps}\n" +
               $"token_timestamps: {this.TokenTimestamps}\n" +
               $"thold_pt: {this.ThresholdPt}\n" +
               $"thold_ptsum: {this.ThresholdPtsum}\n" +
               $"max_len: {this.MaxLength}\n" +
               $"split_on_word: {this.SplitOnWord}\n" +
               $"max_tokens: {this.MaxTokens}\n" +
               $"debug_mode: {this.DebugMode}\n" +
               $"audio_ctx: {this.AudioContext}\n" +
               $"tdrz_enable: {this.TdrzEnable}\n" +
               $"suppress_regex: {this.SuppressRegex}\n" +
               $"initial_prompt: {this.InitialPrompt}\n" +
               $"detect_language: {this.DetectLanguage}\n" +
               $"language: {this.Language}\n" +
               $"suppress_blank: {this.SuppressBlank}\n" +
               $"suppress_non_speech_tokens: {this.SuppressNonSpeechTokens}\n" +
               $"temperature: {this.Temperature}\n" +
               $"max_initial_ts: {this.MaxInitialTimestamp}\n" +
               $"length_penalty: {this.LengthPenalty}\n" +
               $"temperature_inc: {this.TemperatureIncrement}\n" +
               $"entropy_thold: {this.EntropyThreshold}\n" +
               $"logprob_thold: {this.LogprobThreshold}\n" +
               $"no_speech_thold: {this.NoSpeechThreshold}\n" +
               $"greedy.best_of: {this.BestOf}\n" +
               $"beam_search.beam_size: {this.BeamSize}\n" +
               $"beam_search.patience: {this.Patience}";
    }
}