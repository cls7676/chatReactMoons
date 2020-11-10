﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.AI.Embeddings.VectorOperations;

namespace Microsoft.SemanticKernel.AI.Embeddings;

/// <summary>
/// A view of a vector that allows for low-level, optimized, read-write mathematical operations.
/// </summary>
/// <typeparam name="TEmbedding">The unmanaged data type (<see cref="float"/>, <see cref="double"/> currently supported).</typeparam>
public ref struct EmbeddingSpan<TEmbedding>
    where TEmbedding : unmanaged
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="vector">A a vector of contiguous, unmanaged data.</param>
    public EmbeddingSpan(Span<TEmbedding> vector)
    {
        SupportedTypes.VerifyTypeSupported(typeof(TEmbedding));

        this.Span = vector;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="vector">A vector of contiguous, unmanaged data.</param>
    public EmbeddingSpan(TEmbedding[] vector)
        : this(vector.AsSpan())
    {
    }

    /// <summary>
    /// Gets the underlying <see cref="Span{T}"/> of unmanaged data.
    /// </summary>
    public Span<TEmbedding> Span { get; internal set; }

    /// <summary>
    /// Normalizes the underlying vector in-place, such that the Euclidean length is 1.
    /// </summary>
    /// <returns>A <see cref="EmbeddingReadOnlySpan{TEmbedding}"/> with 'IsNormalized' set to true.</returns>
    public EmbeddingReadOnlySpan<TEmbedding> Normalize()
    {
        this.Span.NormalizeInPlace();
        return new EmbeddingReadOnlySpan<TEmbedding>(this.Span, true);
    }

    /// <summary>
    /// Calculates the dot product of this vector with another.
    /// </summary>
    /// <param name="other">The second vector.</param>
    /// <returns>The dot product as a <see c