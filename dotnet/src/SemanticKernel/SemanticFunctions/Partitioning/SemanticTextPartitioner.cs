// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.SemanticKernel.SemanticFunctions.Partitioning;

/// <summary>
/// Split text in chunks, attempting to leave meaning intact.
/// For plain text, split looking at new lines first, then periods, and so on.
/// For markdown, split looking at punctuation first, and so on.
/// </summary>
public static class SemanticTextPartitioner
{
    /// <summary>
    /// Split plain text into lines.
    /// </summary>
    /// <param name="text">Text to split</param>
    /// <param name="maxTokensPerLine">Maximum number of tokens per line.</param>
    /// <returns>List of lines.</returns>
    public static List<string> SplitPlainTextLines(string text, int maxTokensPerLine)
    {
        return InternalSplitPlaintextLines(text, maxTokensPerLine, true);
    }

    /// <summary>
    /// Split markdown text into lines.
    /// </summary>
    /// <param name="text">Text to split</param>
    /// <param name="maxTokensPerLine">Maximum number of tokens per line.</param>
    /// <returns>List of lines.</returns>
    public static List<string> SplitMarkDownLines(string text, int maxTokensPerLine)
    {
        return InternalSplitMarkdownLines(text, maxTokensPerLine, true);
    }

    /// <summary>
    /// Split plain text into paragraphs.
    /// </summary>
    /// <param name="lines">Lines of text.</param>
    /// <param name="maxTokensPerParagraph">Maximum number of tokens per paragraph.</param>
    /// <returns>List of paragraphs.</returns>
    public static List<string> SplitPlainTextParagraphs(List<string> lines, int maxTokensPerParagraph)
    {
        return InternalSplitTextParagraphs(lines, maxTokensPerParagraph, text => InternalSplitPlaintextLines(text, maxTokensPerParagraph, false));
    }

    /// <summary>
    /// Split markdown text into paragraphs.
    /// </summary>
    /// <param name="lines">Lines of text.</param>
    /// <param name="maxTokensPerParagraph">Maximum number of tokens per paragraph.</param>
    /// <returns>List of paragraphs.</returns>
    public static List<string> SplitMarkdownParagraphs(List<string> lines, int maxTokensPerParagraph)
    {
        return InternalSplitTextParagraphs(lines, maxTokensPerParagraph, text => InternalSplitMarkdownLines(text, maxTokensPerParagraph, false));
    }

    private static List<string> InternalSplitTextParagraphs(List<string> lines, int maxTokensPerParagraph, Func<string, List<string>> longLinesSplitter)
    {
        if (lines.Count == 0)
        {
            return new List<string>();
        }

        // Split long lines first
        var truncatedLines = new List<string>();
        foreach (var line in lines)
        {
            truncatedLines.AddRange(longLinesSplitter(line));
        }

        lines = truncatedLines;

        