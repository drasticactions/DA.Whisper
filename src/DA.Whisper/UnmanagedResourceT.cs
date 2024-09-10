// <copyright file="UnmanagedResourceT.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents a resource that is managed manually and requires explicit deallocation.
/// </summary>
/// <typeparam name="T">The type of the unmanaged resource.</typeparam>
internal class UnmanagedResource<T> : IDisposable
{
    /// <summary>
    /// The action to deallocate the resource.
    /// </summary>
    protected Action<T>? dealloc;

    /// <summary>
    /// The handle to the unmanaged resource.
    /// </summary>
    protected T? handle;

    /// <summary>
    /// Gets a value indicating whether the resource has been created.
    /// </summary>
    public bool Created => !EqualityComparer<T>.Default.Equals(this.handle, default);

    /// <summary>
    /// Gets the handle to the unmanaged resource.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if the handle is null.</exception>
    public T Handle => EqualityComparer<T>.Default.Equals(this.handle, default) || this.handle == null ? throw new NullReferenceException() : this.handle;

    /// <summary>
    /// Releases the unmanaged resource.
    /// </summary>
    public void Dispose()
    {
        if (EqualityComparer<T>.Default.Equals(this.handle, default) || this.handle == null)
        {
            return;
        }

        this.dealloc?.Invoke(this.handle);
        this.handle = default;
    }

    /// <summary>
    /// Creates the unmanaged resource.
    /// </summary>
    /// <param name="alloc">The function to allocate the resource.</param>
    /// <param name="dealloc">The action to deallocate the resource.</param>
    /// <returns>The handle to the created resource.</returns>
    public T Create(Func<T> alloc, Action<T> dealloc)
    {
        this.handle = alloc();
        this.dealloc = dealloc;

        return this.handle;
    }

    /// <summary>
    /// Gets the unmanaged resource.
    /// </summary>
    /// <param name="resource">The output parameter to receive the resource handle.</param>
    public void GetResource(out T? resource) => resource = this.handle;
}