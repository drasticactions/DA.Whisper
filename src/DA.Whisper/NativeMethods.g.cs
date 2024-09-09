// <auto-generated>
// This code is generated by csbindgen.
// DON'T CHANGE THIS DIRECTLY.
// </auto-generated>
#pragma warning disable CS8500
#pragma warning disable CS8981
using System;
using System.Runtime.InteropServices;


namespace DA.Whisper
{
    public static unsafe partial class NativeMethods
    {
#if IOS || MACOS || TVOS || MACCATALYST
        const string __DllName = "__Internal";
#else
        const string __DllName = "whisper";
#endif
        



        [DllImport(__DllName, EntryPoint = "whisper_init_from_file_with_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_file_with_params(byte* path_model, whisper_context_params @params);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_buffer_with_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_buffer_with_params(void* buffer, nuint buffer_size, whisper_context_params @params);

        [DllImport(__DllName, EntryPoint = "whisper_init_with_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_with_params(whisper_model_loader* loader, whisper_context_params @params);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_file_with_params_no_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_file_with_params_no_state(byte* path_model, whisper_context_params @params);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_buffer_with_params_no_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_buffer_with_params_no_state(void* buffer, nuint buffer_size, whisper_context_params @params);

        [DllImport(__DllName, EntryPoint = "whisper_init_with_params_no_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_with_params_no_state(whisper_model_loader* loader, whisper_context_params @params);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_file", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_file(byte* path_model);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_buffer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_buffer(void* buffer, nuint buffer_size);

        [DllImport(__DllName, EntryPoint = "whisper_init", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init(whisper_model_loader* loader);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_file_no_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_file_no_state(byte* path_model);

        [DllImport(__DllName, EntryPoint = "whisper_init_from_buffer_no_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_from_buffer_no_state(void* buffer, nuint buffer_size);

        [DllImport(__DllName, EntryPoint = "whisper_init_no_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context* whisper_init_no_state(whisper_model_loader* loader);

        [DllImport(__DllName, EntryPoint = "whisper_init_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_state* whisper_init_state(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_ctx_init_openvino_encoder", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_ctx_init_openvino_encoder(whisper_context* ctx, byte* model_path, byte* device, byte* cache_dir);

        [DllImport(__DllName, EntryPoint = "whisper_free", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_free(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_free_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_free_state(whisper_state* state);

        [DllImport(__DllName, EntryPoint = "whisper_free_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_free_params(whisper_full_params* @params);

        [DllImport(__DllName, EntryPoint = "whisper_free_context_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_free_context_params(whisper_context_params* @params);

        [DllImport(__DllName, EntryPoint = "whisper_pcm_to_mel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_pcm_to_mel(whisper_context* ctx, float* samples, int n_samples, int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_pcm_to_mel_with_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_pcm_to_mel_with_state(whisper_context* ctx, whisper_state* state, float* samples, int n_samples, int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_set_mel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_set_mel(whisper_context* ctx, float* data, int n_len, int n_mel);

        [DllImport(__DllName, EntryPoint = "whisper_set_mel_with_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_set_mel_with_state(whisper_context* ctx, whisper_state* state, float* data, int n_len, int n_mel);

        [DllImport(__DllName, EntryPoint = "whisper_encode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_encode(whisper_context* ctx, int offset, int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_encode_with_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_encode_with_state(whisper_context* ctx, whisper_state* state, int offset, int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_decode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_decode(whisper_context* ctx, int* tokens, int n_tokens, int n_past, int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_decode_with_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_decode_with_state(whisper_context* ctx, whisper_state* state, int* tokens, int n_tokens, int n_past, int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_tokenize", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_tokenize(whisper_context* ctx, byte* text, int* tokens, int n_max_tokens);

        [DllImport(__DllName, EntryPoint = "whisper_token_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_count(whisper_context* ctx, byte* text);

        [DllImport(__DllName, EntryPoint = "whisper_lang_max_id", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_lang_max_id();

        [DllImport(__DllName, EntryPoint = "whisper_lang_id", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_lang_id(byte* lang);

        [DllImport(__DllName, EntryPoint = "whisper_lang_str", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_lang_str(int id);

        [DllImport(__DllName, EntryPoint = "whisper_lang_str_full", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_lang_str_full(int id);

        [DllImport(__DllName, EntryPoint = "whisper_lang_auto_detect", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_lang_auto_detect(whisper_context* ctx, int offset_ms, int n_threads, float* lang_probs);

        [DllImport(__DllName, EntryPoint = "whisper_lang_auto_detect_with_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_lang_auto_detect_with_state(whisper_context* ctx, whisper_state* state, int offset_ms, int n_threads, float* lang_probs);

        [DllImport(__DllName, EntryPoint = "whisper_n_len", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_n_len(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_n_len_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_n_len_from_state(whisper_state* state);

        [DllImport(__DllName, EntryPoint = "whisper_n_vocab", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_n_vocab(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_n_text_ctx", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_n_text_ctx(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_n_audio_ctx", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_n_audio_ctx(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_is_multilingual", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_is_multilingual(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_vocab", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_vocab(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_audio_ctx", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_audio_ctx(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_audio_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_audio_state(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_audio_head", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_audio_head(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_audio_layer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_audio_layer(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_text_ctx", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_text_ctx(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_text_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_text_state(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_text_head", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_text_head(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_text_layer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_text_layer(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_n_mels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_n_mels(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_ftype", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_ftype(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_model_type", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_model_type(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_get_logits", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float* whisper_get_logits(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_get_logits_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float* whisper_get_logits_from_state(whisper_state* state);

        [DllImport(__DllName, EntryPoint = "whisper_token_to_str", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_token_to_str(whisper_context* ctx, int token);

        [DllImport(__DllName, EntryPoint = "whisper_model_type_readable", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_model_type_readable(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_eot", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_eot(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_sot", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_sot(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_solm", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_solm(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_prev", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_prev(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_nosp", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_nosp(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_not", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_not(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_beg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_beg(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_lang", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_lang(whisper_context* ctx, int lang_id);

        [DllImport(__DllName, EntryPoint = "whisper_token_translate", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_translate(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_token_transcribe", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_token_transcribe(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_print_timings", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_print_timings(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_reset_timings", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_reset_timings(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_print_system_info", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_print_system_info();

        [DllImport(__DllName, EntryPoint = "whisper_context_default_params_by_ref", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context_params* whisper_context_default_params_by_ref();

        [DllImport(__DllName, EntryPoint = "whisper_context_default_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_context_params whisper_context_default_params();

        [DllImport(__DllName, EntryPoint = "whisper_full_default_params_by_ref", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_full_params* whisper_full_default_params_by_ref(whisper_sampling_strategy strategy);

        [DllImport(__DllName, EntryPoint = "whisper_full_default_params", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_full_params whisper_full_default_params(whisper_sampling_strategy strategy);

        [DllImport(__DllName, EntryPoint = "whisper_full", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full(whisper_context* ctx, whisper_full_params @params, float* samples, int n_samples);

        [DllImport(__DllName, EntryPoint = "whisper_full_with_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_with_state(whisper_context* ctx, whisper_state* state, whisper_full_params @params, float* samples, int n_samples);

        [DllImport(__DllName, EntryPoint = "whisper_full_parallel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_parallel(whisper_context* ctx, whisper_full_params @params, float* samples, int n_samples, int n_processors);

        [DllImport(__DllName, EntryPoint = "whisper_full_n_segments", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_n_segments(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_full_n_segments_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_n_segments_from_state(whisper_state* state);

        [DllImport(__DllName, EntryPoint = "whisper_full_lang_id", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_lang_id(whisper_context* ctx);

        [DllImport(__DllName, EntryPoint = "whisper_full_lang_id_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_lang_id_from_state(whisper_state* state);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_t0", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long whisper_full_get_segment_t0(whisper_context* ctx, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_t0_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long whisper_full_get_segment_t0_from_state(whisper_state* state, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_t1", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long whisper_full_get_segment_t1(whisper_context* ctx, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_t1_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long whisper_full_get_segment_t1_from_state(whisper_state* state, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_speaker_turn_next", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool whisper_full_get_segment_speaker_turn_next(whisper_context* ctx, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_speaker_turn_next_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool whisper_full_get_segment_speaker_turn_next_from_state(whisper_state* state, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_text", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_full_get_segment_text(whisper_context* ctx, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_segment_text_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_full_get_segment_text_from_state(whisper_state* state, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_n_tokens", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_n_tokens(whisper_context* ctx, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_n_tokens_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_n_tokens_from_state(whisper_state* state, int i_segment);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_text", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_full_get_token_text(whisper_context* ctx, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_text_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_full_get_token_text_from_state(whisper_context* ctx, whisper_state* state, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_id", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_get_token_id(whisper_context* ctx, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_id_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_full_get_token_id_from_state(whisper_state* state, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_data", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_token_data whisper_full_get_token_data(whisper_context* ctx, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_data_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern whisper_token_data whisper_full_get_token_data_from_state(whisper_state* state, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_p", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float whisper_full_get_token_p(whisper_context* ctx, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_full_get_token_p_from_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float whisper_full_get_token_p_from_state(whisper_state* state, int i_segment, int i_token);

        [DllImport(__DllName, EntryPoint = "whisper_bench_memcpy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_bench_memcpy(int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_bench_memcpy_str", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_bench_memcpy_str(int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_bench_ggml_mul_mat", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int whisper_bench_ggml_mul_mat(int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_bench_ggml_mul_mat_str", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* whisper_bench_ggml_mul_mat_str(int n_threads);

        [DllImport(__DllName, EntryPoint = "whisper_log_set", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void whisper_log_set(delegate* unmanaged[Cdecl]<ggml_log_level, byte*, void*, void> log_callback, void* user_data);


    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_context
    {
        public fixed byte _unused[1];
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_state
    {
        public fixed byte _unused[1];
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_ahead
    {
        public int n_text_layer;
        public int n_head;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_aheads
    {
        public nuint n_heads;
        public whisper_ahead* heads;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_context_params
    {
        [MarshalAs(UnmanagedType.U1)] public bool use_gpu;
        [MarshalAs(UnmanagedType.U1)] public bool flash_attn;
        public int gpu_device;
        [MarshalAs(UnmanagedType.U1)] public bool dtw_token_timestamps;
        public whisper_alignment_heads_preset dtw_aheads_preset;
        public int dtw_n_top;
        public whisper_aheads dtw_aheads;
        public nuint dtw_mem_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_token_data
    {
        public int id;
        public int tid;
        public float p;
        public float plog;
        public float pt;
        public float ptsum;
        public long t0;
        public long t1;
        public long t_dtw;
        public float vlen;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_model_loader
    {
        public void* context;
        public delegate* unmanaged[Cdecl]<void*, void*, nuint, nuint> read;
        public delegate* unmanaged[Cdecl]<void*, bool> eof;
        public delegate* unmanaged[Cdecl]<void*, void> close;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_grammar_element
    {
        public whisper_gretype type_;
        public uint value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_full_params
    {
        public whisper_sampling_strategy strategy;
        public int n_threads;
        public int n_max_text_ctx;
        public int offset_ms;
        public int duration_ms;
        [MarshalAs(UnmanagedType.U1)] public bool translate;
        [MarshalAs(UnmanagedType.U1)] public bool no_context;
        [MarshalAs(UnmanagedType.U1)] public bool no_timestamps;
        [MarshalAs(UnmanagedType.U1)] public bool single_segment;
        [MarshalAs(UnmanagedType.U1)] public bool print_special;
        [MarshalAs(UnmanagedType.U1)] public bool print_progress;
        [MarshalAs(UnmanagedType.U1)] public bool print_realtime;
        [MarshalAs(UnmanagedType.U1)] public bool print_timestamps;
        [MarshalAs(UnmanagedType.U1)] public bool token_timestamps;
        public float thold_pt;
        public float thold_ptsum;
        public int max_len;
        [MarshalAs(UnmanagedType.U1)] public bool split_on_word;
        public int max_tokens;
        [MarshalAs(UnmanagedType.U1)] public bool debug_mode;
        public int audio_ctx;
        [MarshalAs(UnmanagedType.U1)] public bool tdrz_enable;
        public byte* suppress_regex;
        public byte* initial_prompt;
        public int* prompt_tokens;
        public int prompt_n_tokens;
        public byte* language;
        [MarshalAs(UnmanagedType.U1)] public bool detect_language;
        [MarshalAs(UnmanagedType.U1)] public bool suppress_blank;
        [MarshalAs(UnmanagedType.U1)] public bool suppress_non_speech_tokens;
        public float temperature;
        public float max_initial_ts;
        public float length_penalty;
        public float temperature_inc;
        public float entropy_thold;
        public float logprob_thold;
        public float no_speech_thold;
        public whisper_full_params__bindgen_ty_1 greedy;
        public whisper_full_params__bindgen_ty_2 beam_search;
        public delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, int, void*, void> new_segment_callback;
        public void* new_segment_callback_user_data;
        public delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, int, void*, void> progress_callback;
        public void* progress_callback_user_data;
        public delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, void*, bool> encoder_begin_callback;
        public void* encoder_begin_callback_user_data;
        public delegate* unmanaged[Cdecl]<void*, bool> abort_callback;
        public void* abort_callback_user_data;
        public delegate* unmanaged[Cdecl]<whisper_context*, whisper_state*, whisper_token_data*, int, float*, void*, void> logits_filter_callback;
        public void* logits_filter_callback_user_data;
        public whisper_grammar_element** grammar_rules;
        public nuint n_grammar_rules;
        public nuint i_start_rule;
        public float grammar_penalty;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_full_params__bindgen_ty_1
    {
        public int best_of;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct whisper_full_params__bindgen_ty_2
    {
        public int beam_size;
        public float patience;
    }


    public enum ggml_log_level : uint
    {
        GGML_LOG_LEVEL_ERROR = 2,
        GGML_LOG_LEVEL_WARN = 3,
        GGML_LOG_LEVEL_INFO = 4,
        GGML_LOG_LEVEL_DEBUG = 5,
    }

    public enum whisper_alignment_heads_preset : uint
    {
        WHISPER_AHEADS_NONE = 0,
        WHISPER_AHEADS_N_TOP_MOST = 1,
        WHISPER_AHEADS_CUSTOM = 2,
        WHISPER_AHEADS_TINY_EN = 3,
        WHISPER_AHEADS_TINY = 4,
        WHISPER_AHEADS_BASE_EN = 5,
        WHISPER_AHEADS_BASE = 6,
        WHISPER_AHEADS_SMALL_EN = 7,
        WHISPER_AHEADS_SMALL = 8,
        WHISPER_AHEADS_MEDIUM_EN = 9,
        WHISPER_AHEADS_MEDIUM = 10,
        WHISPER_AHEADS_LARGE_V1 = 11,
        WHISPER_AHEADS_LARGE_V2 = 12,
        WHISPER_AHEADS_LARGE_V3 = 13,
    }

    public enum whisper_gretype : uint
    {
        WHISPER_GRETYPE_END = 0,
        WHISPER_GRETYPE_ALT = 1,
        WHISPER_GRETYPE_RULE_REF = 2,
        WHISPER_GRETYPE_CHAR = 3,
        WHISPER_GRETYPE_CHAR_NOT = 4,
        WHISPER_GRETYPE_CHAR_RNG_UPPER = 5,
        WHISPER_GRETYPE_CHAR_ALT = 6,
    }

    public enum whisper_sampling_strategy : uint
    {
        WHISPER_SAMPLING_GREEDY = 0,
        WHISPER_SAMPLING_BEAM_SEARCH = 1,
    }


}
