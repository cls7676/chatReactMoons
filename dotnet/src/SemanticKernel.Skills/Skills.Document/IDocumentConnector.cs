// Copyright (c) Microsoft. All rights reserved.

using System.IO;

namespace Microsoft.SemanticKernel.Skills.Document;

/// <summary>
/// Interface for document connections (e.g. Microsoft Word)
/// </summary>
public interface IDocumentConnector
{
    /// <summary>
    /// Read all text from the document.
    /// </summary>
    /// <param name="stream">Document stream</param>
    /// <returns>Str