﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Memory.Storage;

/// <summary>
/// A simple volatile memory implementation of an <see cref="IDataStore{TValue}"/> with a backing <see cref="IDictionary{TKey, TValue}"/>.
/// </summary>
/// <remarks>This is a transient data structure, the lifetime of which is controlled by the caller.
/// The data does not persist, and is not shared, between instances.</remarks>
/// <typeparam name="TValue">The type of data to be stored in this data store.</typeparam>
public class VolatileDataStore<TValue> : IDataStore<TValue>
{
    /// <inheritdoc/>
    public virtual IAsyncEnumerable<string> GetCollectionsAsync(CancellationToken cancel = default)
    {
        return this._store.Keys.ToAsyncEnumerable();
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<DataEntry<TValue>> GetAllAsync(string collection, CancellationToken cancel = default)
    {
        if (this.TryGetCollection(collection, out var collectionDict))
        {
            return collectionDict.Values.ToAsyncEnumerable();
        }

        return AsyncEnumerable.Empty<DataEntry<TValue>>();
    }

    /// <inheritdoc/>
    public virtual Task<DataEntry<TValue>?> GetAsync(string collection, string key, CancellationToken cancel = default)
 