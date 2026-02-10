using System;
using System.Collections.Generic;
using System.Linq;
using API.Debug;

namespace API.Modding;

/// <summary>
/// Contains all registered <c>IRegisterable</c>s. Keys are formatted as <c>ModId:ItemId</c>.
/// <c>IRegisterable</c>s that are also <c>INameable</c>s are set up to use their <c>KeyName</c> as their <c>ItemId</c> by default
/// </summary>
public static class Registry
{
    private static readonly Dictionary<string, IRegistrable> _Reg = new(StringComparer.Ordinal);

    /// <summary>
    /// Registers an item. Call in ctor
    /// </summary>
    public static void Register(IRegistrable val)
    {
        string key = val.GetId();

        // todo should this refuse to overwrite instead?
        if (_Reg.ContainsKey(key))
        {
            DebugConsole.Log($"Key {key} already contains {Get(key)}. Overwriting with {val}",
            nameof(Registry), LogLevel.Warning);
        }

        _Reg[key] = val;
    }

    /// <returns>
    /// The value associated with the given key, or <c>default(T)</c> if none (null for reference types)
    /// </returns>
    public static IRegistrable? Get(string key)
    {
        return _Reg.GetValueOrDefault(key);
    }

    /// <summary>
    /// Adds the value to the registry if it's not already present
    /// </summary>
    /// <returns>
    /// true if it was added; false if it was already present
    /// </returns>
    public static bool Set(string key, IRegistrable val)
    {
        return _Reg.TryAdd(key, val);
    }

    /// <returns>
    /// All registered items of the given type
    /// </returns>
    public static IEnumerable<T> Of<T>() where T : IRegistrable
    {
        return _Reg.Values.OfType<T>();
    }

    public new static string ToString()
    {
        return $"Registry: {string.Join('\n', _Reg.OrderBy(kvp => kvp.Key)
        .Select(kvp => $"{kvp.Key} = {kvp.Value}"))}";
    }
}
