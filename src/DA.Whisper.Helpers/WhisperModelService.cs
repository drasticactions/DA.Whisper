// <copyright file="WhisperModelService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DA.Whisper;

/// <summary>
/// Whisper Model Service.
/// </summary>
public class WhisperModelService : INotifyPropertyChanged
{
    private CachedWhisperModel? selectedModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperModelService"/> class.
    /// </summary>
    public WhisperModelService()
    {
        foreach (var item in Enum.GetValues<GgmlType>())
        {
            switch (item)
            {
                case GgmlType.Unknown:
                    break;
                case GgmlType.Tiny:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_1));
                    break;
                case GgmlType.TinyEn:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_1));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q8_0));
                    break;
                case GgmlType.Base:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_1));
                    break;
                case GgmlType.BaseEn:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_1));
                    break;
                case GgmlType.Small:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_1));
                    break;
                case GgmlType.SmallEn:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_1));
                    break;
                case GgmlType.SmallEnTdrz:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    break;
                case GgmlType.Medium:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_0));
                    break;
                case GgmlType.MediumEn:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_0));
                    break;
                case GgmlType.LargeV1:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    break;
                case GgmlType.LargeV2:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_0));
                    break;
                case GgmlType.LargeV3:
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.NoQuantization));
                    this.AllModels.Add(new CachedWhisperModel(item, QuantizationType.Q5_0));
                    break;
                default:
                    break;
            }
        }

        this.UpdateAvailableModels();
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Event for when the selected model is updated.
    /// </summary>
    public event EventHandler? OnUpdatedSelectedModel;

    /// <summary>
    /// Event for when the available models are updated.
    /// </summary>
    public event EventHandler? OnAvailableModelsUpdate;

    /// <summary>
    /// Gets all models.
    /// </summary>
    public ObservableCollection<CachedWhisperModel> AllModels { get; } = new ObservableCollection<CachedWhisperModel>();

    /// <summary>
    /// Gets the available models.
    /// </summary>
    public ObservableCollection<CachedWhisperModel> AvailableModels { get; } = new ObservableCollection<CachedWhisperModel>();

    /// <summary>
    /// Gets or sets the selected model.
    /// </summary>
    public CachedWhisperModel? SelectedModel
    {
        get
        {
            return this.selectedModel;
        }

        set
        {
            this.SetProperty(ref this.selectedModel, value);
            this.OnUpdatedSelectedModel?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Update Available Models.
    /// </summary>
    public void UpdateAvailableModels()
    {
        lock (this)
        {
            this.AvailableModels.Clear();
            var models = this.AllModels.Where(n => n.Exists);
            foreach (var model in models)
            {
                this.AvailableModels.Add(model);
            }

            if (this.SelectedModel is not null && !this.AvailableModels.Contains(this.SelectedModel))
            {
                this.SelectedModel = null;
            }

            this.SelectedModel ??= this.AvailableModels.FirstOrDefault();
        }

        this.OnAvailableModelsUpdate?.Invoke(this, EventArgs.Empty);
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
}