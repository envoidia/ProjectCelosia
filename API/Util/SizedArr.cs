using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace API.Util;

/// <summary>
/// Array that tracks its length and cannot be resized
/// </summary>
[CollectionBuilder(typeof(SizedArrBuilder), "Create")]
public sealed class SizedArr<T>(int capacity) : IEnumerable, IEnumerable<T>
{
    private readonly T[] _arr = new T[capacity];

    public int Capacity = capacity;
    public int Count = 0;

    public T this[int index]
    {
        get
        {
            if (index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return this._arr[index];
        }
    }

    public SizedArr(ReadOnlySpan<T> values) : this(values.Length)
    {
        this._arr = values.ToArray();
    }

    // AggressiveInlining because List.Add() has it
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        if (this.Count == this.Capacity)
        {
            throw new ArgumentOutOfRangeException(
                $"Cannot add {item} to SizedArr<{typeof(T)}> that has met capacity of {this.Capacity}");
        }

        this._arr[this.Count] = item;
        this.Count++;
    }

    public void AddRange(params ReadOnlySpan<T> source)
    {
        if (source.IsEmpty)
        {
            return;
        }

        if (this.Count + source.Length > this.Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(source),
                $"Cannot add collection to SizedArr<{typeof(T)}> because collection length " +
                $"{source.Length} + current length {this.Count} exceeds capacity {this.Capacity}");
        }

        source.CopyTo(this._arr.AsSpan(this.Count));
        this.Count += source.Length;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < this.Count; i++)
        {
            yield return this._arr[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public static implicit operator T[](SizedArr<T> arr)
    {
        return arr._arr;
    }
}

file static class SizedArrBuilder
{
    public static SizedArr<T> Create<T>(ReadOnlySpan<T> values)
    {
        return new(values);
    }
}