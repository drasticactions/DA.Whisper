// <copyright file="Subtitle.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#nullable disable
using System.Text;
using System.Text.RegularExpressions;

namespace DA.Whisper;

/// <summary>
/// Represents an SRT subtitle.
/// </summary>
public class Subtitle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Subtitle"/> class.
    /// </summary>
    /// <param name="subtitleType">The subtitle type.</param>
    public Subtitle(SubtitleType subtitleType = SubtitleType.SRT)
    {
        this.SubtitleType = subtitleType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Subtitle"/> class with the specified subtitle.
    /// </summary>
    /// <param name="subtitle">The subtitle text.</param>
    /// <param name="subtitleType">The subtitle type.</param>
    public Subtitle(string subtitle, SubtitleType subtitleType = SubtitleType.SRT)
    {
        this.SubtitleType = subtitleType;
        string[] subtitleLines = Regex.Split(subtitle, @"^\s*$", RegexOptions.Multiline);

        for (var index = 0; index < subtitleLines.Length; index++)
        {
            var subtitleLine = subtitleLines[index];
            string subtitleLineTrimmed = subtitleLine.Trim();
            SubtitleLine block = new SubtitleLine(subtitleLineTrimmed);
            block.LineNumber = index + 1;
            this.Lines.Add(block);
        }
    }

    /// <summary>
    /// Gets or sets the subtitle type.
    /// </summary>
    public SubtitleType SubtitleType { get; set; }

    /// <summary>
    /// Gets or sets the lines.
    /// </summary>
    public List<SubtitleLine> Lines { get; set; } = new List<SubtitleLine>();

    /// <summary>
    /// Converts a list of segments to a subtitle.
    /// </summary>
    /// <param name="segments">Segments.</param>
    /// <param name="subtitleType">Subtitle Type.</param>
    /// <returns>From Subtitle.</returns>
    public static Subtitle FromSegments(List<SegmentData> segments, SubtitleType subtitleType = SubtitleType.SRT)
    {
        Subtitle subtitle = new Subtitle(subtitleType);
        foreach (var segment in segments)
        {
            subtitle.AddSegment(segment);
        }

        return subtitle;
    }

    /// <summary>
    /// Add a line to the subtitle.
    /// </summary>
    /// <param name="line">The line.</param>
    public void AddLine(SubtitleLine line)
    {
        line.LineNumber = this.Lines.Count + 1;
        this.Lines.Add(line);
    }

    /// <summary>
    /// Adds a segment to the subtitle.
    /// </summary>
    /// <param name="segment">The segment.</param>
    public void AddSegment(SegmentData segment)
    {
        SubtitleLine line = new SubtitleLine
        {
            LineNumber = this.Lines.Count + 1,
            Start = segment.Start,
            End = segment.End,
            Text = segment.Text,
            SubtitleType = this.SubtitleType,
        };
        this.AddLine(line);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        if (this.SubtitleType == SubtitleType.VTT)
        {
            sb.AppendLine("WEBVTT");
            sb.AppendLine();
        }

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