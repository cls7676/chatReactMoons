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

        var actual = await this._db.GetValueAsync(collection, key);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(value, actual);
    }

    [Fact]
    public async Task ItWillPutAndRetrieveWithTimestampAsync()
    {
        // Arrange
        int rand = Random.Shared.Next();
        string collection = "collection" + rand;
        string key = "key" + rand;
        string value = "value" + rand;
        DateTimeOffset timestamp = DateTimeOffset.UtcNow;

        // Act
        await this._db.PutValueAsync(collection, key, value, timestamp);
        var actual = await this._db.GetAsync(collection, key);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(value, actual!.Value.Value);
        Assert.True(timestamp.Date.Equals(actual!.Value.Timestamp?.Date));
        Assert.True((int)timestamp.TimeOfDay.TotalSeconds == (int?)actual!.Value.Timestamp?.TimeOfDay.TotalSeconds);
    }

    [Fact]
    public async Task ItWillPutAndRetrieveDataEntryWithTimestampAsync()
    {
        // Arrange
        int rand = Random.Shared.Next();
        string collection = "collection" + rand;
        string key = "key" + rand;
        string value = "value" + rand;
        DateTimeOffset timestamp = DateTimeOffset.UtcNow;
        var data = DataEntry.Create(key, value, timestamp);

        // Act
        var placed = await this._db.PutAsync(collection, data);
        DataEntry<string>? actual = await this._db.GetAsync(collection, key);

        // Assert
        Assert.NotNull(actual);
        Assert.True(placed.Equals(data));
        Assert.Equal(value, actual!.Value.Value);
        Assert.True(timestamp.Date.Equals(actual!.Value.Timestamp?.Date));
        Assert.True((int)timestamp.TimeOfDay.TotalSeconds == (int?)actual!.Value.Timestamp?.TimeOfDay.TotalSeconds);
    }

    [Fact]
    public async Task ItWillPutAndDeleteDataEntryAsync()
    {
        // Arrange
        int rand = Random.Shared.Next();
        string collection = "collection" + rand;
        string key = "key" + rand;
        string value = "value" + rand;
        var data = DataEntry.Create(key, value, DateTimeOffset.UtcNow);

        // Act
        await this._db.PutA