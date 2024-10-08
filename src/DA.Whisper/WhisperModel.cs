// <copyright file="WhisperModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;

namespace DA.Whisper;

/// <summary>
/// Represents a Whisper model.
/// </summary>
/// <remarks>
/// This class is responsible for managing the Whisper model and its associated resources.
/// </remarks>
public sealed class WhisperModel : IDisposable
{
    /// <summary>
    /// Gets the context.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    internal UnmanagedResource<nint> Context = new();
#pragma warning restore SA1401 // Fields should be private

    private bool isDisposed = false;
    private GCHandle? pinnedBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperModel"/> class with the specified model path and context parameters.
    /// </summary>
    /// <param name="modelPath">The path to the Whisper model file.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    private WhisperModel(string modelPath, ContextParams contextParams)
    {
        unsafe
        {
            this.ContextParams = contextParams;
            this.Context.Create(() => (nint)NativeMethods.InitFromFileWithParams(modelPath, contextParams), (x) => NativeMethods.whisper_free((whisper_context*)x));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperModel"/> class with the specified model and context parameters.
    /// </summary>
    /// <param name="model">The Whisper Model Stream.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    private WhisperModel(byte[] model, ContextParams contextParams)
    {
        unsafe
        {
            this.ContextParams = contextParams;
            this.pinnedBuffer = GCHandle.Alloc(model, GCHandleType.Pinned);
            var bufferLength = new UIntPtr((uint)model.Length);
            this.Context.Create(() => (nint)NativeMethods.InitFromBufferWithParams((void*)this.pinnedBuffer!.Value.AddrOfPinnedObject(), bufferLength, contextParams), (x) => NativeMethods.whisper_free((whisper_context*)x));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperModel"/> class with the specified model and context parameters.
    /// </summary>
    /// <param name="model">The Whisper Model Stream.</param>
    private WhisperModel(byte[] model)
    {
        unsafe
        {
            this.pinnedBuffer = GCHandle.Alloc(model, GCHandleType.Pinned);
            var bufferLength = new UIntPtr((uint)model.Length);
            this.Context.Create(() => (nint)NativeMethods.InitFromBuffer((void*)this.pinnedBuffer!.Value.AddrOfPinnedObject(), bufferLength), (x) => NativeMethods.whisper_free((whisper_context*)x));
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="WhisperModel"/> class.
    /// </summary>
    ~WhisperModel()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Gets a value indicating whether the Whisper model is initialized.
    /// </summary>
    public bool IsInitialized
    {
        get
        {
            unsafe
            {
                return this.Context.Handle != IntPtr.Zero;
            }
        }
    }

    /// <summary>
    /// Gets the context parameters.
    /// </summary>
    internal ContextParams? ContextParams { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model path.
    /// </summary>
    /// <param name="modelPath">The path to the Whisper model file.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel FromFile(string modelPath)
    {
        return new WhisperModel(modelPath, ContextParams.FromDefault());
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model path.
    /// </summary>
    /// <param name="model">The Whisper model file.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel FromBuffer(byte[] model)
    {
        return new WhisperModel(model, ContextParams.FromDefault());
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model path.
    /// </summary>
    /// <param name="modelPath">The path to the Whisper model file.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel? TryFromFile(string modelPath)
    {
        try
        {
            return new WhisperModel(modelPath, ContextParams.FromDefault());
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model.
    /// </summary>
    /// <param name="model">The Whisper model file.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel? TryFromBuffer(byte[] model)
    {
        try
        {
            return new WhisperModel(model);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model path and context parameters.
    /// </summary>
    /// <param name="modelPath">The path to the Whisper model file.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel FromFileWithParameters(string modelPath, ContextParams contextParams)
    {
        return new WhisperModel(modelPath, contextParams);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model and context parameters.
    /// </summary>
    /// <param name="model">The Whisper model file.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel FromBufferWithParameters(byte[] model, ContextParams contextParams)
    {
        return new WhisperModel(model, contextParams);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model path and context parameters.
    /// </summary>
    /// <param name="modelPath">The path to the Whisper model file.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    /// <param name="model">The WhisperModel.</param>
    /// <returns>Bool if model was initalized.</returns>
    public static bool TryFromFileWithParameters(string modelPath, ContextParams contextParams, out WhisperModel? model)
    {
        try
        {
            if (!File.Exists(modelPath))
            {
                model = null;
                return false;
            }

            model = new WhisperModel(modelPath, contextParams);
            return model?.IsInitialized ?? false;
        }
        catch (Exception)
        {
            model = null;
            return false;
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model and context parameters.
    /// </summary>
    /// <param name="modelStream">The Whisper model file.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    /// <param name="model">The WhisperModel.</param>
    /// <returns>Bool if model was initalized.</returns>
    public static bool TryFromBufferWithParameters(byte[] modelStream, ContextParams contextParams, out WhisperModel? model)
    {
        try
        {
            model = new WhisperModel(modelStream, contextParams);
            return model?.IsInitialized ?? false;
        }
        catch (Exception)
        {
            model = null;
            return false;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="WhisperModel"/> object.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="WhisperModel"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
            }

            unsafe
            {
                this.Context.Dispose();
                this.pinnedBuffer?.Free();
            }

            // Free any unmanaged objects here
            this.isDisposed = true;
        }
    }
}