// <copyright file="AsyncAutoResetEvent.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace DA.Whisper;

/// <summary>
/// An asynchronous auto-reset event.
/// </summary>
internal class AsyncAutoResetEvent
{
    /// <summary>
    /// Represents an asynchronous auto-reset event.
    /// </summary>
    private static readonly Task Completed = Task.CompletedTask;

    private TaskCompletionSource<bool>? waitTcs;
    private int isSignaled; // 0 for false, 1 for true

    /// <summary>
    /// Asynchronously waits for the event to be signaled.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task WaitAsync()
    {
        if (Interlocked.CompareExchange(ref this.isSignaled, 0, 1) == 1)
        {
            return Completed;
        }
        else
        {
            var tcs = new TaskCompletionSource<bool>();
            var oldTcs = Interlocked.Exchange(ref this.waitTcs, tcs);
            oldTcs?.TrySetCanceled();
            return tcs.Task;
        }
    }

    /// <summary>
    /// Signals the event, releasing a waiting task.
    /// </summary>
    public void Set()
    {
        var toRelease = Interlocked.Exchange(ref this.waitTcs, null);
        if (toRelease != null)
        {
            toRelease.SetResult(true);
        }
        else
        {
            Interlocked.Exchange(ref this.isSignaled, 1);
        }
    }
}
