// <copyright file="Ahead.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace DA.Whisper;

/// <summary>
/// Represents the Ahead class.
/// </summary>
public class Ahead
{
    private unsafe whisper_ahead* ahead;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ahead"/> class.
    /// </summary>
    /// <param name="ahead">Ahead.</param>
    internal unsafe Ahead(whisper_ahead* ahead)
    {
        this.ahead = ahead;
    }

    /// <summary>
    /// Gets or sets the text layer.
    /// </summary>
    public int TextLayer
    {
        get
        {
            unsafe
            {
                return ahead->n_text_layer;
            }
        }

        set
        {
            unsafe
            {
                ahead->n_text_layer = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the head.
    /// </summary>
    public int Head
    {
        get
        {
            unsafe
            {
                return ahead->n_head;
            }
        }

        set
        {
            unsafe
            {
                ahead->n_head = value;
            }
        }
    }

    /// <summary>
    /// Returns a string that represents the Ahead object.
    /// </summary>
    /// <returns>A string that represents the Ahead object.</returns>
    public override string ToString()
    {
        return $"[TextLayer: {this.TextLayer}, Head: {this.Head}]";
    }
}
