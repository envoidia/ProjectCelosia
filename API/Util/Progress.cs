using System;

namespace API.Util;

/// <summary>
/// A float clamped from 0-1 representing progress of some action
/// </summary>
public readonly struct Progress(float p = 0) {
    public static readonly Progress One = new(1);
    public static readonly Progress Zero = new(0);

    private readonly float _p = Math.Clamp(p, 0, 1);

    public static Progress operator +(Progress l, float r) => new(l._p + r);
    public static Progress operator -(Progress l, float r) => new(l._p - r);
    public static Progress operator *(Progress l, float r) => new(l._p * r);
    public static Progress operator /(Progress l, float r) => new(l._p / r);

    public static bool operator ==(Progress l, float r) => l._p == r;
    public static bool operator !=(Progress l, float r) => l._p != r;

    public static bool operator >(Progress l, float r) => l._p > r;
    public static bool operator <(Progress l, float r) => l._p < r;
    public static bool operator >=(Progress l, float r) => l._p >= r;
    public static bool operator <=(Progress l, float r) => l._p <= r;

    public static explicit operator float(Progress p) => p._p;

    public override bool Equals(object? obj) => obj switch {
        int i => this._p == i,
        float f => this._p == f,
        Progress p => this._p == p._p,
        long l => this._p == l,
        double d => this._p == d,
        null => false,
        _ => base.Equals(obj)
    };

    public override string ToString() => $"{base.ToString()}: {this._p}";

    public override int GetHashCode() => this._p.GetHashCode();

    /// <returns>
    /// The lesser of 2 <c>Progress</c>es
    /// </returns>
    public static Progress Min(Progress a, Progress b) => a._p > b._p ? b : a;
}
