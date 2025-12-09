using System.Collections;
using System.Diagnostics;

namespace API.Util;

/// <summary>
/// Helper wrappers for <c>Debug.Assert()</c>
/// </summary>
public static class Assert {
    /// <summary>
    /// Asserts that the given value is convertible to <c>T</c>
    /// </summary>
    [Conditional("DEBUG")]
    public static void Is<T>(object v) =>
        Debug.Assert(v is T, string.Format(Lang.AssInvalidType, typeof(T)));

    /// <summary>
    /// Asserts that the given collection is a specific size
    /// </summary>
    [Conditional("DEBUG")]
    public static void SizeIs(IList c, int s) =>
        Debug.Assert(c.Count == s, string.Format(Lang.AssSizeMismatch, s, c.Count));

    /// <summary>
    /// Asserts that the given collection is a non-zero size
    /// </summary>
    [Conditional("DEBUG")]
    public static void SizeNotZero(object?[] c) =>
        Debug.Assert(c.Length != 0, Lang.AssSize0);
}
