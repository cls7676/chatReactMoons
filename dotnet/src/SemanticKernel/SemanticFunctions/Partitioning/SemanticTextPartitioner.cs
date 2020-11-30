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
    /// <param name="maxTokensPerLine">Maximum number of tokens p