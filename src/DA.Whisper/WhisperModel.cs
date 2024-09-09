// <copyright file="WhisperModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

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
    internal unsafe whisper_context* Context;
#pragma warning restore SA1401 // Fields should be private

    private bool isDisposed = false;

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
            this.Context = NativeMethods.InitFromFileWithParams(modelPath, contextParams);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperModel"/> class with the specified model and context parameters.
    /// </summary>
    /// <param name="model">The Whisper Model Stream.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    private WhisperModel(Stream model, ContextParams contextParams)
    {
        unsafe
        {
            this.ContextParams = contextParams;
            this.Context = NativeMethods.InitFromStreamWithParams(model, contextParams);
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
                return this.Context is not null && ((IntPtr)this.Context) != IntPtr.Zero;
            }
        }
    }

    /// <summary>
    /// Gets the context parameters.
    /// </summary>
    internal ContextParams ContextParams { get; }

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
    public static WhisperModel FromStream(Stream model)
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
    public static WhisperModel? TryFromStream(Stream model)
    {
        try
        {
            return new WhisperModel(model, ContextParams.FromDefault());
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
    public static WhisperModel FromStreamWithParameters(Stream model, ContextParams contextParams)
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
    public static bool TryFromStreamWithParameters(Stream modelStream, ContextParams contextParams, out WhisperModel? model)
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
                NativeMethods.whisper_free(this.Context);
            }

            // Free any unmanaged objects here
            this.isDisposed = true;
        }
    }
}