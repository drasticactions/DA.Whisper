using System;
using System.Runtime.InteropServices;

namespace DA.Whisper;

public class FullParams
{
    private whisper_full_params _params;

    internal FullParams(SamplingStrategy strategy)
    {
        _params = NativeMethods.whisper_full_default_params((int)strategy);
    }

    internal ref whisper_full_params Params => ref _params;

    public int Strategy
    {
        get => _params.strategy;
        set => _params.strategy = value;
    }

    public int Threads
    {
        get => _params.n_threads;
        set => _params.n_threads = value;
    }

    public int MaxTextContext
    {
        get => _params.n_max_text_ctx;
        set => _params.n_max_text_ctx = value;
    }

    public int OffsetMs
    {
        get => _params.offset_ms;
        set => _params.offset_ms = value;
    }

    public int DurationMs
    {
        get => _params.duration_ms;
        set => _params.duration_ms = value;
    }

    public bool Translate
    {
        get => _params.translate;
        set => _params.translate = value;
    }

    public bool NoContext
    {
        get => _params.no_context;
        set => _params.no_context = value;
    }

    public bool NoTimestamps
    {
        get => _params.no_timestamps;
        set => _params.no_timestamps = value;
    }

    public bool SingleSegment
    {
        get => _params.single_segment;
        set => _params.single_segment = value;
    }

    public bool PrintSpecial
    {
        get => _params.print_special;
        set => _params.print_special = value;
    }

    public bool PrintProgress
    {
        get => _params.print_progress;
        set => _params.print_progress = value;
    }

    public bool PrintRealtime
    {
        get => _params.print_realtime;
        set => _params.print_realtime = value;
    }

    public bool PrintTimestamps
    {
        get => _params.print_timestamps;
        set => _params.print_timestamps = value;
    }

    public bool TokenTimestamps
    {
        get => _params.token_timestamps;
        set => _params.token_timestamps = value;
    }

    public float ThresholdPt
    {
        get => _params.thold_pt;
        set => _params.thold_pt = value;
    }

    public float ThresholdPtsum
    {
        get => _params.thold_ptsum;
        set => _params.thold_ptsum = value;
    }

    public int MaxLength
    {
        get => _params.max_len;
        set => _params.max_len = value;
    }

    public bool SplitOnWord
    {
        get => _params.split_on_word;
        set => _params.split_on_word = value;
    }

    public int MaxTokens
    {
        get => _params.max_tokens;
        set => _params.max_tokens = value;
    }

    public bool DebugMode
    {
        get => _params.debug_mode;
        set => _params.debug_mode = value;
    }

    public int AudioContext
    {
        get => _params.audio_ctx;
        set => _params.audio_ctx = value;
    }

    public bool TdrzEnable
    {
        get => _params.tdrz_enable;
        set => _params.tdrz_enable = value;
    }

    public string Language
    {
        get
        {
            unsafe
            {
                return Marshal.PtrToStringAnsi((IntPtr)_params.language) ?? string.Empty;
            }
        }
        set
        {
            unsafe
            {
                _params.language = (byte*)Marshal.StringToHGlobalAnsi(value);
            }
        }
    }

    public string InitialPrompt
    {
        get
        {
            unsafe
            {
                return Marshal.PtrToStringAnsi((IntPtr)_params.initial_prompt) ?? string.Empty;
            }
        }
        set
        {
            unsafe
            {
                _params.initial_prompt = (byte*)Marshal.StringToHGlobalAnsi(value);
            }
        }
    }

    public string SuppressRegex
    {
        get 
        {
            unsafe
            {
                return Marshal.PtrToStringAnsi((IntPtr)_params.suppress_regex) ?? string.Empty;
            }
        }
        set
        {
            unsafe
            {
                _params.suppress_regex = (byte*)Marshal.StringToHGlobalAnsi(value);
            }
        }
    }

    public bool DetectLanguage
    {
        get => _params.detect_language;
        set => _params.detect_language = value;
    }

    public bool SuppressBlank
    {
        get => _params.suppress_blank;
        set => _params.suppress_blank = value;
    }

    public bool SuppressNonSpeechTokens
    {
        get => _params.suppress_non_speech_tokens;
        set => _params.suppress_non_speech_tokens = value;
    }

    public float Temperature
    {
        get => _params.temperature;
        set => _params.temperature = value;
    }

    public float MaxInitialTimestamp
    {
        get => _params.max_initial_ts;
        set => _params.max_initial_ts = value;
    }

    public float LengthPenalty
    {
        get => _params.length_penalty;
        set => _params.length_penalty = value;
    }

    public float TemperatureIncrement
    {
        get => _params.temperature_inc;
        set => _params.temperature_inc = value;
    }

    public float EntropyThreshold
    {
        get => _params.entropy_thold;
        set => _params.entropy_thold = value;
    }

    public float LogprobThreshold
    {
        get => _params.logprob_thold;
        set => _params.logprob_thold = value;
    }

    public float NoSpeechThreshold
    {
        get => _params.no_speech_thold;
        set => _params.no_speech_thold = value;
    }

    public int BestOf
    {
        get => _params.greedy.best_of;
        set => _params.greedy.best_of = value;
    }

    public int BeamSize
    {
        get => _params.beam_search.beam_size;
        set => _params.beam_search.beam_size = value;
    }

    public float Patience
    {
        get => _params.beam_search.patience;
        set => _params.beam_search.patience = value;
    }

    public static FullParams FromGreedyStrategy()
    {
        return new FullParams(SamplingStrategy.Greedy);
    }

    public static FullParams FromBeamSearchStrategy()
    {
        return new FullParams(SamplingStrategy.BeamSearch);
    }

    public override string ToString()
    {
        return $"strategy: {Strategy}\n" +
               $"n_threads: {Threads}\n" +
               $"n_max_text_ctx: {MaxTextContext}\n" +
               $"offset_ms: {OffsetMs}\n" +
               $"duration_ms: {DurationMs}\n" +
               $"translate: {Translate}\n" +
               $"no_context: {NoContext}\n" +
               $"no_timestamps: {NoTimestamps}\n" +
               $"single_segment: {SingleSegment}\n" +
               $"print_special: {PrintSpecial}\n" +
               $"print_progress: {PrintProgress}\n" +
               $"print_realtime: {PrintRealtime}\n" +
               $"print_timestamps: {PrintTimestamps}\n" +
               $"token_timestamps: {TokenTimestamps}\n" +
               $"thold_pt: {ThresholdPt}\n" +
               $"thold_ptsum: {ThresholdPtsum}\n" +
               $"max_len: {MaxLength}\n" +
               $"split_on_word: {SplitOnWord}\n" +
               $"max_tokens: {MaxTokens}\n" +
               $"debug_mode: {DebugMode}\n" +
               $"audio_ctx: {AudioContext}\n" +
               $"tdrz_enable: {TdrzEnable}\n" +
               $"suppress_regex: {SuppressRegex}\n" +
               $"initial_prompt: {InitialPrompt}\n" +
               $"detect_language: {DetectLanguage}\n" +
               $"language: {Language}\n" +
               $"suppress_blank: {SuppressBlank}\n" +
               $"suppress_non_speech_tokens: {SuppressNonSpeechTokens}\n" +
               $"temperature: {Temperature}\n" +
               $"max_initial_ts: {MaxInitialTimestamp}\n" +
               $"length_penalty: {LengthPenalty}\n" +
               $"temperature_inc: {TemperatureIncrement}\n" +
               $"entropy_thold: {EntropyThreshold}\n" +
               $"logprob_thold: {LogprobThreshold}\n" +
               $"no_speech_thold: {NoSpeechThreshold}\n" +
               $"greedy.best_of: {BestOf}\n" +
               $"beam_search.beam_size: {BeamSize}\n" +
               $"beam_search.patience: {Patience}";
    }
}

