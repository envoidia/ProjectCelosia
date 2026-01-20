using System.Collections;
using System.Diagnostics;

namespace API.Debug;

/// <summary>
/// Helper wrappers for <c>Debug.Assert</c>
/// </summary>
public static class Assert
{
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
    public static void LessThan(int i1, int i2)
    {
        System.Diagnostics.Debug.Assert(i1 < i2, $"{i1} must be less than {i2}");
    }

    /// <summary>
    /// Asserts that an int is within 2 others
    /// </summary>
    [Conditional("DEBUG")]
    public static void InRange(int i, int min, int max)
    {
        System.Diagnostics.Debug.Assert(i <= max && i >= min, $"{i} must between {min} and {max} (inclusive)");
    }

    /// <summary>
    /// Asserts that an int is within 2 others or is a special exception
    /// </summary>
    [Conditional("DEBUG")]
    public static void InRangeOr(int i, int min, int max, int exception)
    {
        System.Diagnostics.Debug.Assert((i <= max && i >= min) || i == exception,
            $"{i} must between {min} and {max} (inclusive), or must be {exception}");
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

