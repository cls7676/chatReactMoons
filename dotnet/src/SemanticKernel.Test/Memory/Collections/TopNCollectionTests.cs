// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Memory.Collections;
using Xunit;

namespace SemanticKernelTests.Memory.Collections;

/// <summary>
/// Contains tests for the <see cref="TopNCollection{T}"/> class.
/// </summary>
public class TopNCollectionTests
{
    private const int MaxItemsCount = 5;

    [Fact]
    public void ItResetsCollectionCorrectly()
    {
        // Arrange
        const int expectedItemsCount = 0;

        var topNCollection = this.GetTestCollection(MaxItemsCount);

        // Act
        topNCollection.Reset();

        // Assert
        Assert.Equal(expectedItemsCount, topNCollection.Count);
    }

    [Fact]
    public void ItKeepsMaxItemsCountWhenMoreItemsWereAdded()
    {
        // Arrange
        const int expectedCollectionCount = 5;

        // Act
        var topNCollection = this.GetTestCollection(expectedCollectionCount);

        // Assert
        Assert.Equal(expectedCollectionCount, topNCollection.Count);
    }

    [Fact]
    public void ItSortsCollectionByScoreInDescendingOrder()
    {
        // Arrange
        var topNCollection =