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

    private int _c = 0;
    public int Count
    {
        get
        {
            return this._c;
        }
    }

    public T this[int index]
    {
        get
        {
            if (index >= this._c)
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
        if (this._c == this.Capacity)
        {
            throw new ArgumentOutOfRangeException(
                $"Cannot add {item} to SizedArr<{typeof(T)}> that has met capacity of {this.Capacity}");
        }

        this._arr[this._c++] = item;
    }

    public void AddRange(params ReadOnlySpan<T> source)
    {
        if (source.IsEmpty)
        {
            return;
        }

        if (this._c + source.Length > this.Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(source),
                $"Cannot add collection to SizedArr<{typeof(T)}> because collection length " +
                $"{source.Length} + current length {this._c} exceeds capacity {this.Capacity}");
        }

        source.CopyTo(this._arr.AsSpan(this._c));
        this._c += source.Length;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < this._c; i++)
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