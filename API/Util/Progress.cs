using System;

namespace API.Util;

#pragma warning disable CS0660
#pragma warning disable CS0661

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
}
