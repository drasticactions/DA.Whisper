using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[assembly: DisableRuntimeMarshalling]

namespace DA.Whisper
{
    internal static unsafe partial class NativeMethods
    {
        internal static unsafe whisper_context* InitFromFileWithParams(string modelPath, ContextParams contextParams)
        {
            unsafe
            {
                byte* modelPathByte = (byte*)Marshal.StringToHGlobalAnsi(modelPath).ToPointer();
                return NativeMethods.whisper_init_from_file_with_params_no_state(modelPathByte, contextParams.Params);
            }
        }

        internal static string GetWhisperSystemInfo()
        {
            unsafe
            {
                byte* versionByte = NativeMethods.whisper_print_system_info();
                return Marshal.PtrToStringAnsi((IntPtr)versionByte) ?? string.Empty;
            }
        }
    }
}