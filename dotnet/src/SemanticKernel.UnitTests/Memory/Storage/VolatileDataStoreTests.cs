// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Memory.Storage;
using Xunit;

namespace SemanticKernel.UnitTests.Memory.Storage;

/// <summary>
/// Unit tests of <see cref="VolatileDataStore{TValue}"/>.
/// </summary>
public class VolatileDataStoreTests
{
    private readonly VolatileDataStore<string> _db;

    public VolatileDataStoreTests()
    {
        this._db = new();
    }

    [Fact]
    public void ItSucceedsInitialization()
    {
        // Assert
        Assert.NotNull(this._db);
    }

#pragma warning disable CA5394 // Random is an insecure random number generator
    [Fact]
    public async Task ItWillPutAndRetrieveNoTimestampAsync()
    {
        // Arrange
        int rand = Random.Shared.Next();
        string collection = "collection" + rand;
        string key = "key" + rand;
        string value = "value" + rand;

        // Act
        await this._db.PutValueAsync(collection, key, value);

        var actual = 