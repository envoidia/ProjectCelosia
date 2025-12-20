using System;
using System.Collections.Generic;
using System.Linq;
using API.Name;
using API.Util;

namespace API.Modding;

/// <summary>
/// Contains all registered items. Keys ignore case
/// </summary>
public static class Registry {
    private static readonly Dictionary<string, IRegistrable> _Reg = new(StringComparer.Ordinal);

    public static void Register(IRegistrable val) {
        string key = val.GetId();

        if (_Reg.ContainsKey(key)) {
            DebugUtil.Log($"Key {key} already contains {Get(key)}. Overwriting with {val}",
            nameof(Registry), DebugUtil.LogLevel.Warning);
        }

        UnsafeOverwrite(key, val);
    }

    /// <returns>
    /// The value associated with the given key, or <c>default(T)</c> if none (null for reference types)
    /// </returns>
    public static IRegistrable? Get(string key) => _Reg.GetValueOrDefault(key);

    /// <summary>
    /// Adds the value to the registry if it's not already present
    /// </summary>
    /// <returns>
    /// true if it was added; false if it was already present
    /// </returns>
    public static bool Set(string key, IRegistrable val) => _Reg.TryAdd(key, val);

    /// <returns>
    /// All registered items of the given type
    /// </returns>
    public static IEnumerable<T> OfType<T>() where T : IRegistrable => _Reg.Values.OfType<T>();

    /// <summary>
    /// Adds the value to the registry, overwriting existing values. Avoid.
    /// If you need to modify an existing registry entry, it's better to access it directly and modify its fields when able
    /// </summary>
    public static void UnsafeOverwrite(string key, IRegistrable val) => _Reg[key] = val;

    public new static string ToString() => $"Registry:\n{string.Join('\n', _Reg.OrderBy(kvp => kvp.Key)
        .Select(kvp => $"{kvp.Key} = {kvp.Value}"))}";
}
