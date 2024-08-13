using System;
using System.Runtime.InteropServices;

namespace DA.Whisper;

public class ContextParams
{
    private whisper_context_params _params;

    internal ContextParams()
    {
        _params = NativeMethods.whisper_context_default_params();
    }

    internal ref whisper_context_params Params => ref _params;

    public bool UseGpu
    {
        get => _params.use_gpu;
        set => _params.use_gpu = value;
    }

    public bool FlashAttn
    {
        get => _params.flash_attn;
        set => _params.flash_attn = value;
    }

    public int GpuDevice
    {
        get => _params.gpu_device;
        set => _params.gpu_device = value;
    }

    public bool DtwTokenTimestamps
    {
        get => _params.dtw_token_timestamps;
        set => _params.dtw_token_timestamps = value;
    }

    public int DtwAheadsPreset
    {
        get => _params.dtw_aheads_preset;
        set => _params.dtw_aheads_preset = value;
    }

    public int DtwNTop
    {
        get => _params.dtw_n_top;
        set => _params.dtw_n_top = value;
    }

    public nuint DtwMemSize
    {
        get => _params.dtw_mem_size;
        set => _params.dtw_mem_size = value;
    }

    public nuint NumberOfAheads
    {
        get => _params.dtw_aheads.n_heads;
        set => _params.dtw_aheads.n_heads = value;
    }

    public Ahead[] Aheads
    {
        get
        {
            unsafe
            {
                if (_params.dtw_aheads.heads == null)
                return Array.Empty<Ahead>();

                var results = new Ahead[(int)_params.dtw_aheads.n_heads];
                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = new Ahead(&_params.dtw_aheads.heads[i]);
                }
                return results;
            }
        }
    }

    public static ContextParams FromDefault() => new ContextParams();

    public override string ToString()
    {
        var heads = string.Join<Ahead>(", ", Aheads);
        return $"Heads: {heads}, UseGpu: {UseGpu}, FlashAttn: {FlashAttn}, GpuDevice: {GpuDevice}, DtwTokenTimestamps: {DtwTokenTimestamps}, DtwAheadsPreset: {DtwAheadsPreset}, DtwNTop: {DtwNTop}, DtwMemSize: {DtwMemSize}, NumberOfAheads: {NumberOfAheads}";
    }
}