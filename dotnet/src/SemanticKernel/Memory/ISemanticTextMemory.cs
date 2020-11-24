// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Memory;

/// <summary>
/// An interface for semantic memory that creates and recalls memories associated with text.
/// </summary>
public interface ISemanticTextMemory
{
    /// <summary>
    /// Save some information into the semantic memory, keeping a copy of the source information.
    /// </summary>
    /// <param name="collection">Collection where to save the information</param>
    /// <param name="id">Unique identifier</param>
    /// <param name="text">Information to save</param>
    /// <param name="description">Optional description</param>
    /// <param name="cancel">Cancellation token</param>
    public Task SaveInformationAsync(
        string collection,
        string text,
        string id,
        string? description = null,
        CancellationToken cancel = default);

    /// <summary>
    /// Save some information into the semantic memory, keeping only a reference to the source information.
    /// </summary>
    /// <param name="collection">Collection where to save the information</param>
    /// <param name="text">Information to save</param>
    /// <param name="externalId">Unique identifier, e.g. URL or GUID to the original source</param>
    /// <param name="externalSourceName">Name of the external service, e.g. "MSTeams", "GitHub", "WebSite", "Outlook IMAP", etc.</param>
    /// <param name="description">Optional description</param>
    /// <param name="cancel">Cance