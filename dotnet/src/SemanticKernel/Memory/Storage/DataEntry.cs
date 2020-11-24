// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Memory.Storage;

/// <summary>
/// A struct containing properties for storage and retrieval of data.
/// </summary>
/// <typeparam name="TValue">The data <see cref="Type"/>.</typeparam>
public struct DataEntry<TValue> : IEquatable<DataEntry<TValue>>
{
    /// <summary>
    /// Creates an instance of a <see cref="DataEntry{TValue}"/>.
    /// </summary>
    /// <param name="key">The data key.</param>
    /// <param name="value">The data value.</param>
    /// <param name="timestamp">The data timestamp.</param>
    [JsonConstructor]
    public DataEntry(string key, TValue? value, DateTimeOffset? timestamp = null)
    {
        this.Key = key;
        this.Value = value;
        this.Timestamp = timestamp;
    }

    /// <summary>
    /// Gets the key of the data.
    /// </summary>
    [JsonPropertyName("key")]
    public readonly string Key { get; }

    /// <summary>
    /// Gets the value of the data.
    /// </summary>
    [JsonPropertyName("value")]
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets the timestamp of the data.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset