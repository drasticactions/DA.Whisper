using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DA.Whisper;

public sealed class WhisperModel : IDisposable
{
    private bool _isDisposed = false;

    internal unsafe whisper_context* _context;

    internal ContextParams ContextParams { get; }

    internal WhisperModel(string modelPath, ContextParams contextParams)
    {
        unsafe {
            this.ContextParams = contextParams;
            this._context = NativeMethods.InitFromFileWithParams(modelPath, contextParams);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
            }

            unsafe {
                NativeMethods.whisper_free(this._context);
            }
            // Free any unmanaged objects here
            _isDisposed = true;
        }
    }

    ~WhisperModel()
    {
        Dispose(false);
    }

    public static WhisperModel FromFile(string modelPath)
    {
        return new WhisperModel(modelPath, ContextParams.FromDefault());
    }

    public static WhisperModel FromFileWithParameters(string modelPath, ContextParams contextParams)
    {
        return new WhisperModel(modelPath, contextParams);
    }
}