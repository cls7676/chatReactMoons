// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.SemanticKernel.Skills.Document.OpenXml.Extensions;

namespace Microsoft.SemanticKernel.Skills.Document.OpenXml;

/// <summary>
/// Connector for Microsoft Word (.docx) files
/// </summary>
public class WordDocumentConnector : IDocumentConnector
{
    /// <summary>
    /// Read all text from the document.
    /// </summary>
    /// <param name="stream">Document stream</param>
    /// <returns>String containing all text from the document.</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="System.InvalidOperationException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="OpenXmlPackageException"></exception>
    public string ReadText(Stream stream)
    {
        using WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(stream, false);
        return wordprocessingDocument.ReadText();
    }

    /// <summary>
    /// Initialize a document from the given stream.
    /// </summary>
    /// <param name="stream">IO stream</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="OpenXmlPackageException"></exception>
    public void Initialize(Stream stream)
    {
        using (WordprocessingDocument wo