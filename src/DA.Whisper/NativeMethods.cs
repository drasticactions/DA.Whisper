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

    internal static unsafe whisper_context* InitFromBufferWithParams(void* bufferArray, nuint bufferLength, ContextParams contextParams)
    {
        unsafe
        {
            var context = NativeMethods.whisper_init_from_buffer_with_params_no_state((void*)bufferArray, (nuint)bufferLength, contextParams.Params);
            return context;
        }
    }

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
}
