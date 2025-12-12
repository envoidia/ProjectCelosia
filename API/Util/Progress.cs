using System;

namespace API.Util;



/// <summary>
/// A float clamped from 0-1 representing progress of some action
/// </summary>
public readonly struct Progress(float p = 0) {
    private readonly float _p = Math.Clamp(p, 0, 1);

    public static Progress operator +(Progress l, float r) => new(l._p + r);

    public static Progress operator *(Progress l, float r) => new(l._p * r);

    public static bool operator ==(Progress l, float r) => l._p == r;

    public static bool operator !=(Progress l, float r) => l._p != r;

    public static explicit operator float(Progress p) => p._p;

    public override bool Equals(object? obj) {
        if (obj is null) return false;

        return obj switch {
            Progress p => this._p == p._p,
            int i => this._p == i,
            long l => this._p == l,
            float f => this._p == f,
            double d => this._p == d,
            _ => base.Equals(obj)
        };
    }

    public override int GetHashCode() => this._p.GetHashCode();
}
