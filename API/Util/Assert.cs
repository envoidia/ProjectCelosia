using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace API.Util;

/// <summary>
/// Helper wrappers for <c>Debug.Assert()</c>
/// </summary>
public static class Assert {
    /// <summary>
    /// Asserts to assert that asserts assert
    /// </summary>
    static Assert() {
        // Passes
        Zero(0);
        NotZero(10);
        Is<IEnumerable>(new List<int>());
        SizeIs(new List<int> { 1, 2, 3 }, 3);
        SizeNotZero([1, 2, 3]);

        Trace.Listeners.Clear();

        // So we can test assert fails by catching them
        Trace.Listeners.Add(new _AssertListener());

        // Fails
        tryHelper(() => Zero(0.001f));
        tryHelper(() => NotZero(0));
        tryHelper(() => Is<int>(1f));
        tryHelper(() => Is<int>(new int[] { 1, 2, 3 }));
        tryHelper(() => SizeIs(new List<int> { 1, 2, 3 }, 4));
        tryHelper(() => SizeNotZero([]));

        static void tryHelper(Action a) {
            bool threw = false;

            try {
                a();
            } catch (Exception) {
                threw = true;
            }

            Debug.Assert(threw);
        }
    }

    /// <summary>
    /// Asserts that a value is 0
    /// </summary>
    [Conditional("DEBUG")]
    public static void Zero(object v) =>
        Debug.Assert(v.Equals(0), $"{v} must be 0");

    /// <summary>
    /// Asserts that a value is not 0
    /// </summary>
    [Conditional("DEBUG")]
    public static void NotZero(object v) =>
        Debug.Assert(!v.Equals(0), $"{v} must not be 0");

    /// <summary>
    /// Asserts that the given value is convertible to <c>T</c>
    /// </summary>
    [Conditional("DEBUG")]
    public static void Is<T>(object v) =>
        Debug.Assert(v is T, $"{v} must be type {typeof(T)}");

    /// <summary>
    /// Asserts that the given collection is a specific size
    /// </summary>
    [Conditional("DEBUG")]
    public static void SizeIs(IList c, int s) =>
        Debug.Assert(c.Count == s, $"Size of {c} must be {s}, was {c.Count}");

    /// <summary>
    /// Asserts that the given collection is a non-zero size
    /// </summary>
    [Conditional("DEBUG")]
    public static void SizeNotZero(object?[] c) =>
        Debug.Assert(c.Length != 0, $"Size of {c} must not be 0");
}

file class _AssertListener : TraceListener {
    public override void Write(string? message) { }
    public override void WriteLine(string? message) { }

    public override void Fail(string? message, string? detailMessage) {
        throw new Exception($"Assert failed: {message} {detailMessage}");
    }
}
