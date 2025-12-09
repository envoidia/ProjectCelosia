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
    /// Asserts that the given collection is a specific length
    /// </summary>
    [Conditional("DEBUG")]
    public static void LenIs(IList c, int s) =>
        Debug.Assert(c.Count == s, $"Size of {c} must be {s}, was {c.Count}");

    /// <summary>
    /// Asserts that the given collection is a non-zero length
    /// </summary>
    [Conditional("DEBUG")]
    public static void LenNotZero(object?[] c) =>
        Debug.Assert(c.Length != 0, $"Size of {c} must not be 0");

    /// <summary>
    /// Asserts to assert that asserts assert.
    /// Also known as <c>Assert.Debug()</c>
    /// </summary>
    static Assert() {
        // Passes
        Zero(0);
        NotZero(10);
        Is<IEnumerable>(new List<int>());
        LenIs(new List<int> { 1, 2, 3 }, 3);
        LenNotZero([1, 2, 3]);

        Trace.Listeners.Clear();

        // So we can test assert fails by catching them
        // Since I don't bother restoring the default behavior, this means asserts can be caught forever now
        // So don't do that
        Trace.Listeners.Add(new _AssertListener());

        // Fails
        tryHelper(() => Zero(0.001f));
        tryHelper(() => NotZero(0));
        tryHelper(() => Is<int>(1f));
        tryHelper(() => Is<int>(new int[] { 1, 2, 3 }));
        tryHelper(() => LenIs(new List<int> { 1, 2, 3 }, 4));
        tryHelper(() => LenNotZero([]));

        static void tryHelper(Action a) {
            bool threw = false;

            try {
                a();
            } catch (_AssertFailedException) {
                Console.WriteLine("Ignore this exception");
                threw = true;
            }

            Debug.Assert(threw);
        }
    }

    private class _AssertListener : TraceListener {
        public override void Write(string? message) { }
        public override void WriteLine(string? message) { }

        public override void Fail(string? message, string? detailMessage) {
            throw new _AssertFailedException(
                $"Assert failed: {message} {detailMessage}");
        }
    }

    private class _AssertFailedException(string msg) : Exception(msg);
}

