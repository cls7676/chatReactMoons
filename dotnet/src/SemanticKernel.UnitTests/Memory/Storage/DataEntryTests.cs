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
    public void ItWillSetNullValueTypeInpu