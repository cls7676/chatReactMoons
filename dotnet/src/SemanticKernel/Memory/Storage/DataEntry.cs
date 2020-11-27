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
    public DateTimeOffset? Timestamp { get; set; } = null;

    /// <summary>
    /// Gets the data value type.
    /// </summary>
    [JsonIgnore]
    public Type ValueType => typeof(TValue);

    /// <summary>
    /// <c>true</c> if the data has a value.
    /// </summary>
    [JsonIgnore]
    public bool HasValue => (this.Value != null);

    /// <summary>
    /// <c>true</c> if the data has a timestamp.
    /// </summary>
    [JsonIgnore]
    public bool HasTimestamp => this.Timestamp.HasValue;

    /// <summary>
    /// The <see cref="Value"/> as a <see cref="string"/>.
    /// </summary>
    [JsonIgnore]
    public string? ValueString
    {
        get
        {
            if (this.ValueType == typeof(string))
            {
                return this.Value?.ToString();
            }

            if (this.Value != null)
            {
                return JsonSerializer.Serialize(this.Value);
            }

            return null;
        }
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="other">The <see cref="DataEntry{TValue}"/> to compare.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public bool Equals(DataEntry<TValue> other)
    {
        return (other != null)
               && (this.Key == other.Key)
               && (this.Value?.Equals(other.Value) == true)
               && (this.Timestamp == other.Timestamp);
    }

    /// <summary>
    /// Determines whether two object instances are equal.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return (obj is DataEntry<TValue> other) && this.Equals(other);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Key, this.Value, this.Timestamp);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>Returns a string that represents the current object.</returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// Parses a <see cref="DataEntry{TValue}"/> object from a serialized JSON string.
    /// </summary>
    /// <param name="json">The source JSON string.</param>
    /// <param name="entry">The resulting <see cref="DataEntry{TValue}"/>, or null if empty.</param>
    /// <returns>true if parsing is successful, false otherwise.</returns>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Parse type from string.")]
    [SuppressMessage("Design", "CA1031:Modify to catch a more specific allowed exception type, or rethrow exception",
        Justification = "Does not throw an exception by design.")]
    public static bool TryParse(string json, [NotNullWhen(true)] out DataEntry<TValue>? entry)
    {
        try
        {
            entry = JsonSerializer.Deserialize<DataEntry<TValue>>(json);
            return true;
        }
        catch
        {
            entry = default;
            return false;
        }
    }

    /// <summary>
    /// Compares two embeddings for equality.
    /// </summary>
    /// <param name="left">The left <see cref="DataEntry{TValue}"/>.</param>
    /// <param name="right">The right <see cref="DataEntry{TValue}"/>.</param>
    /// <returns><c>true</c> if the embeddings contain identical data.</returns>
    public static bool operator ==(DataEntry<TValue> left, DataEntry<TValue> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two embeddings for inequality.
    /// </summary>
    /// <param name="left">The left <see cref="DataEntry{TValue}"/>.</param>
    /// <param name="right">The right <see cref="DataEntry{TValue}"/>.</param>
    /// <returns><c>true</c> if the embeddings do not contain identical data.</returns>
    public static bool operator !=(DataEntry<TValue> left, DataEntry<TValue> right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Provides a collection of static methods for creating, manipulating, and otherwise ope