// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Memory.Collections;

/// <summary>
/// Implements the classic 'heap' data structure. By default, the item with the lowest value is at the top of the heap.
/// </summary>
/// <typeparam name="T">Data type.</typeparam>
internal class MinHeap<T> : IEnumerable<T> where T : IComparable<T>
{
    private const int DefaultCapacity = 7;
    private const int MinCapacity = 0;

    private static readonly T[] s_emptyBuffer = Array.Empty<T>();

    private T[] _items;
    private int _count;

    public MinHeap(T minValue)
        : this(minValue, DefaultCapacity)
    {
    }

    public MinHeap(T minValue, int capacity)
    {
        Verify.GreaterThan(capacity, MinCapacity, $"MinHeap capacity must be greater than {MinCapacity}.");

        this._items = new T[capacity + 1];
        //
        // The 0'th item is a sentinal entry that simplies the code
        //
        this._items[0] = minValue;
    }

    public MinHeap(T minValue, IList<T> items)
        : this(minValue, items.Count)
    {
        this.Add(items);
    }

    public int Count
    {
        get => this._count;
        internal set
        {
            Debug.Assert(value <= this.Capacity);
            this._count = value;
        }
    }

    public int Capacity => this._items.Length - 1; // 0'th item is always a sentinal to simplify code

    public T this[int index]
    {
        get => this._items[index + 1];
        internal set { this._items[index + 1] = value; }
    }

    public T Top => this._items[1];

    public bool IsEmpty => (this._count == 0);

    public void Clear()
    {
        this._count = 0;
    }

    public void Erase()
    {
        Array.Clear(this._items, 1, this._count);
        this._count = 0;
    }

    public T[] DetachBuffer()
    {
        T[] buf = this._items;
        this._items = s_emptyBuffer;
        this._count = 0;
        return buf;
    }

    public void Add(T item)
    {
        //
        // the 0'th item is always a sentinal and not included in this._count. 
        // The length of the buffer is always this._count + 1
        //
        this._count++;
        this.EnsureCapacity();
        this._items[this._count] = item;
        this.UpHeap(this._count);
    }

    public void Add(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            this.Add(item);
        }
    }

    public void Add(IList<T> items, int startAt = 0)
    {
        Verify.NotNull(items, nameof(items));

        int newItemCount = items.Count;

        Verify.LessThan(startAt, newItemCount, $"{nameof(startAt)} value must be less than {nameof(items)} count.");

        this.EnsureCapacity(this._count + (newItemCount - startAt));
        for (int i = startAt; i < newItemCount; ++i)
        {
            //
            // the 0'th item is always a sentinal and not included in this._count. 
            // The length of the buffer is always this._count + 1
            //
            this._count++;
            this._items[this._count] = items[i];
            this.UpHeap(this._count);
        }
    }

    public T RemoveTop()
    {
        if (this._count == 0)
        {
            throw new InvalidOperationException("MinHeap is empty.");
        }

        T item = this._items[1];
        this._items[1] = this._items[this._count--];
        this.DownHeap(1);
        return item;
    }

    public IEnumerable<T> RemoveAll()
    {
        while (this._count > 0)
        {
            yield return this.RemoveTop();
        }
    }

    public void EnsureCapacity(int capacity)
    {
        Verify.GreaterThan(capacity, MinCapacity, $"MinHeap capacity must be greater than {MinCapacity}.");

        // 0th item is always a sentinal
        capacity++;
        if (capacit