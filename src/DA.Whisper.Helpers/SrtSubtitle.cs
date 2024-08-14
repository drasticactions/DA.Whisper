// <copyright file="SrtSubtitle.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#nullable disable
using System.Text;
using System.Text.RegularExpressions;

namespace DA.Whisper;

/// <summary>
/// Represents an SRT subtitle.
/// </summary>
public class SrtSubtitle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SrtSubtitle"/> class.
    /// </summary>
    public SrtSubtitle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SrtSubtitle"/> class with the specified subtitle.
    /// </summary>
    /// <param name="subtitle">The subtitle text.</param>
    public SrtSubtitle(string subtitle)
    {
        string[] subtitleLines = Regex.Split(subtitle, @"^\s*$", RegexOptions.Multiline);

        for (var index = 0; index < subtitleLines.Length; index++)
        {
            var subtitleLine = subtitleLines[index];
            string subtitleLineTrimmed = subtitleLine.Trim();
            SrtSubtitleLine block = new SrtSubtitleLine(subtitleLineTrimmed);
            block.LineNumber = index + 1;
            this.Lines.Add(block);
        }
    }

    /// <summary>
    /// Gets or sets the lines.
    /// </summary>
    public List<SrtSubtitleLine> Lines { get; set; } = new List<SrtSubtitleLine>();

    /// <summary>
    /// Add a line to the subtitle.
    /// </summary>
    /// <param name="line">The line.</param>
    public void AddLine(SrtSubtitleLine line)
    {
        line.LineNumber = this.Lines.Count + 1;
        this.Lines.Add(line);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < this.Lines.Count; i++)
        {
            sb.Append(this.Lines[i].ToString());
            if (i + 1 < this.Lines.Count)
            {
                sb.AppendLine();
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}