using System;
using System.Runtime.InteropServices;

namespace DA.Whisper;

public class Ahead
{
    private unsafe whisper_ahead* _ahead;

    internal unsafe Ahead(whisper_ahead* ahead)
    {
        _ahead = ahead;
    }

    public int TextLayer
    {
        get {
            unsafe {
                return _ahead->n_text_layer;
            }
        }
        set
        {
            unsafe
            {
                _ahead->n_text_layer = value;
            }
        }
    }

    public int Head
    {
        get
        {
            unsafe
            {
                return _ahead->n_head;
            }
        }
        set
        {
            unsafe
            {
                _ahead->n_head = value;
            }
        }
    }

    public override string ToString()
    {
        return $"[TextLayer: {TextLayer}, Head: {Head}]";
    }
}