// <copyright file="WhisperDownload.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Downloader;

namespace DA.Whisper;

/// <summary>
/// Whisper Download.
/// </summary>
public class WhisperDownload : INotifyPropertyChanged, IDisposable
{
    private DownloadService download;
    private CancellationTokenSource source;
    private bool downloadStarted;
    private double precent;
    private bool disposedValue;
    private WhisperModelService service;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperDownload"/> class.
    /// </summary>
    /// <param name="model">The cached model.</param>
    /// <param name="service">The model service.</param>
    public WhisperDownload(CachedWhisperModel model, WhisperModelService service)
    {
        this.service = service;
        this.Model = model;

        this.download = new DownloadService(new DownloadConfiguration()
        {
            ChunkCount = 8,
            ParallelDownload = true,
        });

        this.download.DownloadStarted += this.Download_DownloadStarted;
        this.download.DownloadFileCompleted += this.Download_DownloadFileCompleted;
        this.download.DownloadProgressChanged += this.Download_DownloadProgressChanged;

        this.source = new CancellationTokenSource();
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the model.
    /// </summary>
    public CachedWhisperModel Model { get; private set; }

    /// <summary>
    /// Gets the download service.
    /// </summary>
    public DownloadService DownloadService => this.download;

    /// <summary>
    /// Gets the download precent.
    /// </summary>
    public double Precent
    {
        get { return this.precent; }
        private set { this.SetProperty(ref this.precent, value); }
    }

    /// <summary>
    /// Gets a value indicating whether the download has started.
    /// </summary>
    public bool DownloadStarted
    {
        get { return this.downloadStarted; }
        private set { this.SetProperty(ref this.downloadStarted, value); }
    }

    /// <summary>
    /// Raise events on the items.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

#pragma warning disable SA1600 // Elements should be documented
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
#pragma warning restore SA1600 // Elements should be documented
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        this.OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    /// <param name="disposing">Is disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.source?.Cancel();
                this.download.DownloadStarted -= this.Download_DownloadStarted;
                this.download.DownloadFileCompleted -= this.Download_DownloadFileCompleted;
                this.download.DownloadProgressChanged -= this.Download_DownloadProgressChanged;
                this.download.Dispose();
            }

            this.disposedValue = true;
        }
    }

    /// <summary>
    /// On Property Changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = this.PropertyChanged;
        if (changed == null)
        {
            return;
        }

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Download_DownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
    {
        this.Precent = e.ProgressPercentage / 100;
    }

    private void Download_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        this.DownloadStarted = false;
        if (e.Cancelled && e.UserState is Downloader.DownloadPackage package)
        {
            this.DeleteAsync();
        }

        this.RaiseCanExecuteChanged();
    }

    private void Download_DownloadStarted(object? sender, DownloadStartedEventArgs e)
    {
        this.DownloadStarted = true;
        this.RaiseCanExecuteChanged();
    }

    private async Task DownloadAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(this.Model.FileLocation)!);
        await this.download.DownloadFileTaskAsync(this.Model.DownloadUrl, this.Model.FileLocation, this.source.Token);
    }

    private Task CancelAsync()
    {
        this.download.CancelAsync();
        this.RaiseCanExecuteChanged();
        return Task.CompletedTask;
    }

    private Task DeleteAsync()
    {
        if (File.Exists(this.Model.FileLocation))
        {
            File.Delete(this.Model.FileLocation);
        }

        this.OnPropertyChanged(nameof(this.Model.Exists));
        this.RaiseCanExecuteChanged();
        return Task.CompletedTask;
    }
}