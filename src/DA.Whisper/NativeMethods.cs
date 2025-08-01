// <copyright file="NativeMethods.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: DisableRuntimeMarshalling]

namespace DA.Whisper;

/// <summary>
/// Native methods.
/// </summary>
public static unsafe partial class NativeMethods
{
    /// <summary>
    /// Initializes a whisper context from a file with the specified parameters.
    /// </summary>
    /// <param name="modelPath">The path to the model file.</param>
    /// <param name="contextParams">The context parameters.</param>
    /// <returns>The initialized whisper context.</returns>
    internal static unsafe whisper_context* InitFromFileWithParams(string modelPath, ContextParams contextParams)
    {
        unsafe
        {
            byte* modelPathByte = (byte*)Marshal.StringToHGlobalAnsi(modelPath).ToPointer();
            var context = NativeMethods.whisper_init_from_file_with_params_no_state(modelPathByte, contextParams.Params);
            Marshal.FreeHGlobal((IntPtr)modelPathByte);
            return context;
        }
    }

    /// <summary>
    /// Initializes a whisper context from a buffer with the specified parameters.
    /// </summary>
    /// <param name="bufferArray">The buffer array.</param>
    /// <param name="bufferLength">The length of the buffer.</param>
    /// <param name="contextParams">The context parameters.</param>
    /// <returns>The initialized whisper context.</returns>
    internal static unsafe whisper_context* InitFromBufferWithParams(void* bufferArray, nuint bufferLength, ContextParams contextParams)
    {
        unsafe
        {
            var context = NativeMethods.whisper_init_from_buffer_with_params_no_state((void*)bufferArray, (nuint)bufferLength, contextParams.Params);
            return context;
        }
    }

    /// <summary>
    /// Initializes a whisper context from a buffer.
    /// </summary>
    /// <param name="bufferArray">The buffer array.</param>
    /// <param name="bufferLength">The length of the buffer.</param>
    /// <returns>The initialized whisper context.</returns>
    internal static unsafe whisper_context* InitFromBuffer(void* bufferArray, nuint bufferLength)
    {
        unsafe
        {
            var context = NativeMethods.whisper_init_from_buffer_no_state((void*)bufferArray, (nuint)bufferLength);
            return context;
        }
    }

    /// <summary>
    /// Gets the system information of the whisper library.
    /// </summary>
    /// <returns>The system information.</returns>
    internal static string GetWhisperSystemInfo()
    {
        unsafe
        {
            byte* versionByte = NativeMethods.whisper_print_system_info();
            return Marshal.PtrToStringAnsi((IntPtr)versionByte) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the whisper library version.
    /// </summary>
    /// <returns>The version string.</returns>
    internal static string GetWhisperVersion()
    {
        unsafe
        {
            byte* versionByte = NativeMethods.whisper_version();
            return Marshal.PtrToStringAnsi((IntPtr)versionByte) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the language string for the specified language ID.
    /// </summary>
    /// <param name="langId">The language ID.</param>
    /// <returns>The language string (e.g., "en", "de").</returns>
    internal static string GetLanguageString(int langId)
    {
        unsafe
        {
            byte* langStr = NativeMethods.whisper_lang_str(langId);
            return Marshal.PtrToStringAnsi((IntPtr)langStr) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the full language name for the specified language ID.
    /// </summary>
    /// <param name="langId">The language ID.</param>
    /// <returns>The full language name (e.g., "english", "german").</returns>
    internal static string GetLanguageFullName(int langId)
    {
        unsafe
        {
            byte* langStr = NativeMethods.whisper_lang_str_full(langId);
            return Marshal.PtrToStringAnsi((IntPtr)langStr) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the text representation of a token.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="token">The token ID.</param>
    /// <returns>The token text.</returns>
    internal static string GetTokenText(whisper_context* ctx, int token)
    {
        unsafe
        {
            byte* tokenStr = NativeMethods.whisper_token_to_str(ctx, token);
            return Marshal.PtrToStringAnsi((IntPtr)tokenStr) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the text of a segment from the transcription results.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="segmentIndex">The segment index.</param>
    /// <returns>The segment text.</returns>
    internal static string GetSegmentText(whisper_context* ctx, int segmentIndex)
    {
        unsafe
        {
            byte* text = NativeMethods.whisper_full_get_segment_text(ctx, segmentIndex);
            return Marshal.PtrToStringAnsi((IntPtr)text) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the text of a segment from the transcription results using state.
    /// </summary>
    /// <param name="state">The whisper state.</param>
    /// <param name="segmentIndex">The segment index.</param>
    /// <returns>The segment text.</returns>
    internal static string GetSegmentTextFromState(whisper_state* state, int segmentIndex)
    {
        unsafe
        {
            byte* text = NativeMethods.whisper_full_get_segment_text_from_state(state, segmentIndex);
            return Marshal.PtrToStringAnsi((IntPtr)text) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the text of a token within a segment.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="segmentIndex">The segment index.</param>
    /// <param name="tokenIndex">The token index within the segment.</param>
    /// <returns>The token text.</returns>
    internal static string GetTokenTextFromSegment(whisper_context* ctx, int segmentIndex, int tokenIndex)
    {
        unsafe
        {
            byte* text = NativeMethods.whisper_full_get_token_text(ctx, segmentIndex, tokenIndex);
            return Marshal.PtrToStringAnsi((IntPtr)text) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the text of a token within a segment using state.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="state">The whisper state.</param>
    /// <param name="segmentIndex">The segment index.</param>
    /// <param name="tokenIndex">The token index within the segment.</param>
    /// <returns>The token text.</returns>
    internal static string GetTokenTextFromSegmentWithState(whisper_context* ctx, whisper_state* state, int segmentIndex, int tokenIndex)
    {
        unsafe
        {
            byte* text = NativeMethods.whisper_full_get_token_text_from_state(ctx, state, segmentIndex, tokenIndex);
            return Marshal.PtrToStringAnsi((IntPtr)text) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the model type as a readable string.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <returns>The model type string.</returns>
    internal static string GetModelTypeReadable(whisper_context* ctx)
    {
        unsafe
        {
            byte* modelType = NativeMethods.whisper_model_type_readable(ctx);
            return Marshal.PtrToStringAnsi((IntPtr)modelType) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the language ID from a language string.
    /// </summary>
    /// <param name="language">The language string (e.g., "en", "english").</param>
    /// <returns>The language ID, or -1 if not found.</returns>
    internal static int GetLanguageId(string language)
    {
        unsafe
        {
            byte* langByte = (byte*)Marshal.StringToHGlobalAnsi(language).ToPointer();
            var langId = NativeMethods.whisper_lang_id(langByte);
            Marshal.FreeHGlobal((IntPtr)langByte);
            return langId;
        }
    }

    /// <summary>
    /// Gets the number of tokens in the provided text.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="text">The text to count tokens for.</param>
    /// <returns>The number of tokens.</returns>
    internal static int GetTokenCount(whisper_context* ctx, string text)
    {
        unsafe
        {
            byte* textByte = (byte*)Marshal.StringToHGlobalAnsi(text).ToPointer();
            var count = NativeMethods.whisper_token_count(ctx, textByte);
            Marshal.FreeHGlobal((IntPtr)textByte);
            return count;
        }
    }

    /// <summary>
    /// Tokenizes the provided text.
    /// </summary>
    /// <param name="ctx">The whisper context.</param>
    /// <param name="text">The text to tokenize.</param>
    /// <param name="maxTokens">The maximum number of tokens to return.</param>
    /// <returns>An array of token IDs.</returns>
    internal static int[] TokenizeText(whisper_context* ctx, string text, int maxTokens = 1024)
    {
        unsafe
        {
            byte* textByte = (byte*)Marshal.StringToHGlobalAnsi(text).ToPointer();
            int* tokens = stackalloc int[maxTokens];
            var tokenCount = NativeMethods.whisper_tokenize(ctx, textByte, tokens, maxTokens);
            Marshal.FreeHGlobal((IntPtr)textByte);

            if (tokenCount < 0)
            {
                return Array.Empty<int>();
            }

            var result = new int[tokenCount];
            for (int i = 0; i < tokenCount; i++)
            {
                result[i] = tokens[i];
            }
            return result;
        }
    }
}