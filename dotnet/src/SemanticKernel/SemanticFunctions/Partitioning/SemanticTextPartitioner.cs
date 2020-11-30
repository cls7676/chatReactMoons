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

        // Group lines in paragraphs
        var paragraphs = new List<string>();
        var currentParagraph = new StringBuilder();
        foreach (var line in lines)
        {
            // "+1" to account for the "new line" added by AppendLine()
            if (TokenCount(currentParagraph.ToString()) + TokenCount(line) + 1 >= maxTokensPerParagraph &&
                currentParagraph.Length > 0)
            {
                paragraphs.Add(currentParagraph.ToString().Trim());
                currentParagraph.Clear();
            }

            currentParagraph.AppendLine(line);
        }

        if (currentParagraph.Length > 0)
        {
            paragraphs.Add(currentParagraph.ToString().Trim());
            currentParagraph.Clear();
        }

        // distribute text more evenly in the last paragraphs when the last paragraph is too short.
        if (paragraphs.Count > 1)
        {
            var lastParagraph = paragraphs[^1];
            var secondLastParagraph = paragraphs[^2];

            if (TokenCount(lastParagraph) < maxTokensPerParagraph / 4)
            {
                var lastParagraphTokens = lastParagraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var secondLastParagraphTokens = secondLastParagraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var lastParagraphTokensCount = lastParagraphTokens.Length;
                var secondLastParagraphTokensCount = secondLastParagraphTokens.Length;

                if (lastParagraphTokensCount + secondLastParagraphTokensCount <= maxTokensPerParagraph)
                {
                    var newSecondLastParagraph = new StringBuilder();
                    for (var i = 0; i < secondLastParagraphTokensCount; i++)
                    {
                        newSecondLastParagraph.Append(secondLastParagraphTokens[i])
                            .Append(' ');
                    }

                    for (var i = 0; i < lastParagraphTokensCount; i++)
                    {
                        newSecondLastParagraph.Append(lastParagraphTokens[i])
                            .Append(' ');
                    }

                    paragraphs[^2] = newSecondLastParagraph.ToString().Trim();
                    paragraphs.RemoveAt(paragraphs.Count - 1);
                }
            }
        }

        return paragraphs;
    }

    private static List<string> InternalSplitPlaintextLines(string text, int maxTokensPerLine, bool trim)
    {
        text = text.Replace("\r\n", "\n", StringComparison.OrdinalIgnoreCase);

        var splitOptions = new List<List<char>?>
        {
            new List<char> { '\n', '\r' },
            new List<char> { '.' },
            new List<char> { '?', '!' },
            new List<char> { ';' },
            new List<char> { ':' },
            new List<char> { ',' },
            new List<char> { ')', ']', '}' },
            new List<char> { ' ' },
            new List<char> { '-' },
            null
        };

        List<string>? result = null;
        bool inputWasSplit;
        foreach (var splitOption in splitOptions)
        {
            if (result is null)
            {
                result = Split(text, maxTokensPerLine, splitOption, trim, out inputWasSplit);
            }
            else
            {
                result = Split(result, maxTokensPerLine, splitOption, trim, out inputWasSplit);
            }

            if (!inputWasSplit)
            {
                break;
            }
        }

        return result ?? new List<string>();
    }

    private static List<string> InternalSplitMarkdownLines(string text, int maxTokensPerLine, bool trim)
    {
        text = text.Replace("\r\n", "\n", StringComparison.OrdinalIgnoreCase);

        var splitOptions = new List<List<char>?>
        {
            new List<char> { '.' },
            new List<char> { '?', '!' },
            new List<char> { ';', },
            new List<char> { ':' },
            new List<char> { ',', },
            new List<char> { ')', ']', '}' },
            new List<char> { ' ' },
            new List<char> { '-' },
            new List<char> { '\n', '\r' },
            null
        };

        List<string>? result = null;
        bool inputWasSplit;
        foreach (var splitOption in splitOptions)
        {
            if (result is null)
            {
                result = Split(text, maxTokensPerLine, splitOption, trim, out inputWasSplit);
            }
            else
            {
                result = Split(result, m