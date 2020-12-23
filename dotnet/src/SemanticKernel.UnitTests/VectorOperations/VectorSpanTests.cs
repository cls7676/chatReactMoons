// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.AI.Embeddings;
using Xunit;

namespace SemanticKernel.UnitTests.VectorOperations;

public class VectorSpanTests
{
    private readonly float[] _floatV1 = new float[] { 1.0F, 2.0F, -4.0F, 10.0F };
    private readonly float[] _floatV2 = new float[] { 3.0F, -7.0F, 1.0F, 6.0F };

    private readonly double[] _doubleV1 = new double[] { 1.0, 2.0, -4.0, 10.0 };
    private readonly double[] _doubleV2 = new double[] { 3.0, -7.0, 1.0, 6.0 };

    [Fact]
    public void ItOnlySupportsFPDataTypes()
    {
        // Assert
        Assert.True(EmbeddingSpan<double>.IsSupported);
        Assert.True(EmbeddingSpan<float>.IsSupported);

        Assert.False(EmbeddingSpan<bool>.IsSupported);
        Assert.False(EmbeddingSpan<byte>.IsSupported);
        Assert.False(EmbeddingSpan<sbyte>.IsSupported);
        Assert.False(EmbeddingSpan<char>.IsSupported);
        Assert.False(EmbeddingSpan<decimal>.IsSupported);
        Assert.False(EmbeddingSpan<int>.IsSupported);
        Assert.False(EmbeddingSpan<uint>.IsSupported);
        Assert.False(EmbeddingSpan<nint>.IsSupported);
        Assert.False(EmbeddingSpan<nuint>.IsSupported);
        Assert.False(EmbeddingSpan<long>.IsSupported);
        Assert.False(EmbeddingSpan<ulong>.IsSupported);
   