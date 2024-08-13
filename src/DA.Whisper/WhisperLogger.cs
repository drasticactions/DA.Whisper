using System.Runtime.InteropServices;

namespace DA.Whisper;

public class WhisperLogger
{
    /// <summary>
    /// Returns the singleton instance of the <see cref="WhisperLogger"/> class used to log messages from the Whisper library.
    /// </summary>
    public static WhisperLogger Instance { get; } = new();

    private WhisperLogger()
    {
        unsafe
        {
            delegate* unmanaged[Cdecl]<int, byte*, void*, void> onLogging = &LogUnmanaged;
            NativeMethods.whisper_log_set(onLogging, null);
        }
    }

    public event Action<LogEventArgs>? OnLog;

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static unsafe void LogUnmanaged(int level, byte* message, void* user_data)
    {
        Instance.OnLog?.Invoke(new LogEventArgs((LogLevel)level, Marshal.PtrToStringAnsi((IntPtr)message) ?? string.Empty));
    }
}