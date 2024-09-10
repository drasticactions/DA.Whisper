// <copyright file="UnmanagedResource.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// Represents a specialized unmanaged resource that uses a boolean handle.
/// </summary>
internal class UnmanagedResource : UnmanagedResource<bool>
{
    /// <summary>
    /// Creates the unmanaged resource.
    /// </summary>
    /// <param name="alloc">The action to allocate the resource.</param>
    /// <param name="dealloc">The action to deallocate the resource.</param>
    public void Create(Action alloc, Action dealloc)
    {
        try
        {
            alloc();
            this.handle = true;
        }
        catch
        {
            this.handle = false;
        }

        this.dealloc = _ => dealloc();
    }
}