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
    internal WhisperModel(string modelPath, ContextParams contextParams)
    {
        unsafe
        {
            this.ContextParams = contextParams;
            this.Context = NativeMethods.InitFromFileWithParams(modelPath, contextParams);
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
    /// Creates a new instance of the <see cref="WhisperModel"/> class from the specified model path and context parameters.
    /// </summary>
    /// <param name="modelPath">The path to the Whisper model file.</param>
    /// <param name="contextParams">The context parameters for the Whisper model.</param>
    /// <returns>A new instance of the <see cref="WhisperModel"/> class.</returns>
    public static WhisperModel? TryFromFileWithParameters(string modelPath, ContextParams contextParams)
    {
        try
        {
            return new WhisperModel(modelPath, contextParams);
        }
        catch (Exception)
        {
            return null;
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