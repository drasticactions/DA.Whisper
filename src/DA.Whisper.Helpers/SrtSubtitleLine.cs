// <copyright file="SrtSubtitleLine.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#nullable disable

using System.Text;

namespace DA.Whisper;

/// <summary>
/// Represents a subtitle line in SRT format.
/// </summary>
public class SrtSubtitleLine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SrtSubtitleLine"/> class.
    /// </summary>
    public SrtSubtitleLine()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SrtSubtitleLine"/> class with the specified subtitle text.
    /// </summary>
    /// <param name="subtitleText">The subtitle text.</param>
    public SrtSubtitleLine(string subtitleText)
    {
        subtitleText = subtitleText.Trim();
        using (StringReader data = new StringReader(subtitleText))
        {
            this.LineNumber = int.Parse(data.ReadLine().Trim());

            string secondLine = data.ReadLine();
            this.Start = TimeSpan.ParseExact(secondLine.Substring(0, 12), @"hh\:mm\:ss\,fff", null);
            this.End = TimeSpan.ParseExact(secondLine.Substring(17, 12), @"hh\:mm\:ss\,fff", null);

            this.Text = data.ReadToEnd().Trim();
        }
    }

    /// <summary>
    /// Gets or sets the start time.
    /// </summary>
    public TimeSpan Start { get; set; }

    /// <summary>
    /// Gets or sets the end time.
    /// </summary>
    public TimeSpan End { get; set; }

    /// <summary>
    /// Gets the time range of the subtitle line.
    /// </summary>
    public string Time => $"{this.Start} -> {this.Start}";

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the line number of the subtitle line.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the image data of the subtitle line.
    /// </summary>
    public byte[] Image { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(this.LineNumber.ToString());

        sb.Append(this.Start.ToString(@"hh\:mm\:ss\,fff"));
        sb.Append(" --> ");
        sb.Append(this.End.ToString(@"hh\:mm\:ss\,fff"));
        sb.AppendLine();

        sb.Append(this.Text);

        return sb.ToString();
    }
}