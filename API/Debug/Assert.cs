using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace API.Debug;

/// <summary>
/// Helper wrappers for <c>Debug.Assert</c>
/// </summary>
public static class Assert
{
    /// <summary>
    /// Asserts that this call is unreachable
    /// </summary>
    [Conditional("DEBUG")]
    public static void Unreachable(object v)
    {
        System.Diagnostics.Debug.Assert(false, "Code thought to be unreachable was executed");
    }

    /// <summary>
    /// Asserts that this call is unreachable with a custom message
    /// </summary>
    [Conditional("DEBUG")]
    public static void Unreachable(object v, string msg)
    {
        System.Diagnostics.Debug.Assert(false, msg);
    }

    /// <summary>
    /// Asserts that values are equal
    /// </summary>
    [Conditional("DEBUG")]
    public static void Eq(object v1, object v2)
    {
        System.Diagnostics.Debug.Assert(v1.Equals(v2), $"{v1} and {v2} must be equal");
    }

    /// <inheritdoc cref="Eq(object, object)" />
    [Conditional("DEBUG")]
    public static void Eq(object v1, object v2, object v3)
    {
        System.Diagnostics.Debug.Assert(v1.Equals(v2) && v2.Equals(v3),
            $"{v1}, {v2}, and {v3} must be equal");
    }

    /// <summary>
    /// Asserts that a value is null
    /// </summary>
    [Conditional("DEBUG")]
    public static void Null(object v)
    {
        System.Diagnostics.Debug.Assert(v is null, $"{v} must be null");
    }

    /// <summary>
    /// Asserts that a value is not null
    /// </summary>
    [Conditional("DEBUG")]
    public static void NotNull(object v)
    {
        System.Diagnostics.Debug.Assert(v is not null, $"{v} cannot be null");
    }

    /// <summary>
    /// Asserts that a value is 0
    /// </summary>
    [Conditional("DEBUG")]
    public static void Zero(object v)
    {
        System.Diagnostics.Debug.Assert(v.Equals(0), $"{v} must be 0");
    }

    /// <summary>
    /// Asserts that a value is 1
    /// </summary>
    [Conditional("DEBUG")]
    public static void One(object v)
    {
        System.Diagnostics.Debug.Assert(v.Equals(1), $"{v} must be 1");
    }

    /// <summary>
    /// Asserts that a value is not 0
    /// </summary>
    [Conditional("DEBUG")]
    public static void NotZero(object v)
    {
        System.Diagnostics.Debug.Assert(!v.Equals(0), $"{v} must not be 0");
    }

    /// <summary>
    /// Asserts that an int is less than another
    /// </summary>
    [Conditional("DEBUG")]
    public static void LessThan(int v1, int v2)
    {
        System.Diagnostics.Debug.Assert(v1 < v2, $"{v1} must be less than {v2}");
    }

    /// <summary>
    /// Asserts that an int is within 2 others
    /// </summary>
    [Conditional("DEBUG")]
    public static void InRange(int v, int min, int max)
    {
        System.Diagnostics.Debug.Assert(v <= max && v >= min, $"{v} must between {min} and {max} (inclusive)");
    }

    /// <summary>
    /// Asserts that an int is within 2 others or is a special exception
    /// </summary>
    [Conditional("DEBUG")]
    public static void InRangeOr(int v, int min, int max, int exception)
    {
        System.Diagnostics.Debug.Assert((v <= max && v >= min) || v == exception,
            $"{v} must between {min} and {max} (inclusive), or must be {exception}");
    }

    /// <summary>
    /// Asserts that the given value is convertible to <c>T</c>
    /// </summary>
    [Conditional("DEBUG")]
    public static void Is<T>(object v)
    {
        System.Diagnostics.Debug.Assert(v is T, $"{v} must be type {typeof(T)}");
    }

    /// <summary>
    /// Asserts that the given collection is a specific length
    /// </summary>
    [Conditional("DEBUG")]
    public static void LenIs(IList c, int s)
    {
        System.Diagnostics.Debug.Assert(c.Count == s, $"Size of {c} must be {s}, was {c.Count}");
    }

    /// <summary>
    /// Asserts that the given <c>List</c> has a specific capacity
    /// </summary>
    [Conditional("DEBUG")]
    public static void CapIs<T>(List<T> c, int s)
    {
        System.Diagnostics.Debug.Assert(c.Capacity == s, $"Size of {c} must be {s}, was {c.Capacity}");
    }

    /// <summary>
    /// Asserts that the given <c>StringBuilder</c> has a specific capacity
    /// </summary>
    [Conditional("DEBUG")]
    public static void CapIs(StringBuilder c, int s)
    {
        System.Diagnostics.Debug.Assert(c.Capacity == s, $"Size of {c} must be {s}, was {c.Capacity}");
    }

    /// <summary>
    /// Asserts that the given collection is a non-zero length
    /// </summary>
    [Conditional("DEBUG")]
    public static void LenNotZero(object?[] c)
    {
        System.Diagnostics.Debug.Assert(c.Length != 0, $"Size of {c} must not be 0");
    }

    /// <summary>
    /// Asserts that the given collection doesn't contain the given object
    /// </summary>
    [Conditional("DEBUG")]
    public static void DoesntContain(IList c, object v)
    {
        System.Diagnostics.Debug.Assert(!c.Contains(v), $"{c} must not contain {v}");
    }
}

