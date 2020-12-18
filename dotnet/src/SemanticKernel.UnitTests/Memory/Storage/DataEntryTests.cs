// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Memory.Storage;
using Xunit;

namespace SemanticKernel.UnitTests.Memory.Storage;

/// <summary>
/// Unit tests of <see cref="DataEntry"/>.
/// </summary>
public class DataEntryTests
{
    [Fact]
    public void ItCannotHaveNullKey()
    {
        Assert.Throws<ValidationException>(() => DataEntry.Create<string>(null!, "test_value"));
    }

    [Fact]
    public void ItCannotHaveEmptyKeyName()
    {
        Assert.Throws<ValidationException>(() => DataEntry.Create<string>(string.Empty, "test_value"));
    }

    [Fact]
    public void ItWillSetNullValueTypeInputToNonNullValueInt()
    {
        // Arrange
        var target = DataEntry.Create<int>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.Equal(0, target.Value);
        Assert.True(target.HasValue);
    }

    [Fact]
    public void ItWillSetNullValueTypeInputToNonNullValueFloat()
    {
        // Arrange
        var target = DataEntry.Create<float>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.Equal(0.0F, target.Value);
        Assert.True(target.HasValue);
    }

    [Fact]
    public void ItWillSetNullValueTypeInputToNonNullValueDouble()
    {
        // Arrange
        var target = DataEntry.Create<double>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.Equal(0.0, target.Value);
        Assert.True(target.HasValue);
    }

    [Fact]
    public void ItWillSetNullValueTypeInputToNonNullValueBool()
    {
        // Arrange
        var target = DataEntry.Create<bool>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.False(target.Value);
        Assert.True(target.HasValue);
    }

    [Fact]
    public void ItWillSetNullReferenceTypeInputToNullString()
    {
        // Arrange
        var target = DataEntry.Create<string>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.Null(target.Value);
        Assert.False(target.HasValue);
    }

    [Fact]
    public void ItWillSetNullReferenceTypeInputToNullObject()
    {
        // Arrange
        var target = DataEntry.Create<object>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.Null(target.Value);
        Assert.False(target.HasValue);
    }

    [Fact]
    public void ItWillSetNullReferenceTypeInputToNullDynamic()
    {
        // Arrange
        var target = DataEntry.Create<dynamic>("test_key", null!);

        // Assert
        Assert.Equal("test_key", target.Key);
        Assert.Null(target.Value);
        Assert.False(target.HasValue);
    }

    [Fact]
    public void ItCanCreateMemoryEntryWithNoTimestamp()
    {
        // Arrange
