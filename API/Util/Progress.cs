using System;
using API.Extensions;

namespace API.Util;

/// <summary>
/// A float clamped from 0-1 representing progress of some action
/// </summary>
public readonly struct Progress(float p = 0) : IEquatable<Progress>
{
    public static readonly Progress One = new(1);
    public static readonly Progress Zero = new(0);

    private readonly float _p = Math.Clamp(p, 0, 1);

    public static Progress operator +(Progress l, Progress r)
    {
        return new(l._p + r._p);
    }

    public static Progress operator +(Progress l, float r)
    {
        return new(l._p + r);
    }

    public static Progress operator +(float l, Progress r)
    {
        return new(l + r._p);
    }

    public static Progress operator -(Progress l, Progress r)
    {
        return new(l._p - r._p);
    }

    public static Progress operator -(Progress l, float r)
    {
        return new(l._p - r);
    }

    public static Progress operator -(float l, Progress r)
    {
        return new(l - r._p);
    }

    public static Progress operator *(Progress l, Progress r)
    {
        return new(l._p * r._p);
    }

    public static Progress operator *(Progress l, float r)
    {
        return new(l._p * r);
    }

    public static Progress operator *(float l, Progress r)
    {
        return new(l * r._p);
    }

    public static Progress operator /(Progress l, Progress r)
    {
        return new(l._p / r._p);
    }

    public static Progress operator /(Progress l, float r)
    {
        return new(l._p / r);
    }

    public static Progress operator /(float l, Progress r)
    {
        return new(l / r._p);
    }

    public static bool operator ==(Progress l, Progress r)
    {
        return l.Equals(r);
    }

    public static bool operator ==(Progress l, float r)
    {
        return l._p == r;
    }

    public static bool operator ==(float l, Progress r)
    {
        return l == r._p;
    }

    public static bool operator !=(Progress l, Progress r)
    {
        return !l.Equals(r);
    }

    public static bool operator !=(Progress l, float r)
    {
        return l._p != r;
    }

    public static bool operator !=(float l, Progress r)
    {
        return l != r._p;
    }

    public static bool operator >(Progress l, Progress r)
    {
        return l._p > r._p;
    }

    public static bool operator >(Progress l, float r)
    {
        return l._p > r;
    }

    public static bool operator >(float l, Progress r)
    {
        return l > r._p;
    }

    public static bool operator <(Progress l, Progress r)
    {
        return l._p < r._p;
    }

    public static bool operator <(Progress l, float r)
    {
        return l._p < r;
    }

    public static bool operator <(float l, Progress r)
    {
        return l < r._p;
    }

    public static bool operator >=(Progress l, Progress r)
    {
        return l._p >= r._p;
    }

    public static bool operator >=(Progress l, float r)
    {
        return l._p >= r;
    }

    public static bool operator >=(float l, Progress r)
    {
        return l >= r._p;
    }

    public static bool operator <=(Progress l, Progress r)
    {
        return l._p <= r._p;
    }

    public static bool operator <=(Progress l, float r)
    {
        return l._p <= r;
    }

    public static bool operator <=(float l, Progress r)
    {
        return l <= r._p;
    }

    public static explicit operator float(Progress p)
    {
        return p._p;
    }

    public bool Equals(Progress other)
    {
        return this._p == other._p;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Progress p => this.Equals(p),
            float f => this._p == f,
            int i => this._p == i,
            long l => this._p == l,
            double d => this._p == d,
            _ => false
        };
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this._p:P0}";
    }

    public override int GetHashCode()
    {
        return this._p.GetHashCode();
    }

    public static Progress Min(Progress a, Progress b)
    {
        return a._p > b._p ? b : a;
    }

    public static Progress Max(Progress a, Progress b)
    {
        return a._p > b._p ? a : b;
    }

    public static Progress ParseOrDefault(string? str, Progress defaultValue = default)
    {
        if (str is null)
        {
            return defaultValue;
        }

        return new(float.ParseOrDefault(str, defaultValue._p));
    }
}
