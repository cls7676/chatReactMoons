// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.AI.Embeddings;

/// <summary>
/// Represents an searchable index of <see cref="Embedding{TEmbedding}"/> structs.
/// </summary>
/// <typeparam name="TEmbedding">The data type of the embedding.</typeparam>
public interface IEmbeddingIndex<TEmbedding>
    where TEmbedding : unmanaged
{
    /// <summary>
    /// Gets the nearest matches to the <see cref="Embedding{TEmbedding}"/>.
    /// </summary>
    /// <param name="collection">The storage collection to search.</param>
    /// <param name="embedding">The input <see cref="Embedding{TEmbedding}"/> to use as the search.</param>
    /// <param name="limit">The max number of results to return.</param>
    /// <param name="minRelevanceScore">The minimum score to consider in the distance calculation.</param>
    /// <returns>A tuple consisting of the <see