#if !NATIVE_AOT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Modding;

// Ignore trim/NativeAOT warnings
#pragma warning disable IL2026
#pragma warning disable IL2075

// todo add mod dependency loading, mod unloading, mod disabling, mod config, and saving logs to a temporary file
public static class ModLoader {
    /// <summary>
    /// List of all loaded mods. Do not externally modify
    /// </summary>
    internal static readonly List<GameMod> _LoadedMods = [];

    /// <summary>
    /// Lang key that should be used for a mod's display name
    /// </summary>
    public const string NameKey = "ModName";

    private static readonly string _ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

    internal static void _LoadAllMods() {
        IEnumerable<string> dllFiles = Directory.EnumerateFiles(_ModsFolder, "*.dll", SearchOption.AllDirectories);
        foreach (string dllPath in dllFiles) _LoadSingleModAssembly(dllPath);
    }

    private static void _LoadSingleModAssembly(string dllPath) {
        AssemblyLoadContext alc = new(Path.GetFileNameWithoutExtension(dllPath));//, true); todo should be collectible/unloadable?

        Assembly asm;
        using (FileStream fs = new(dllPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            asm = alc.LoadFromStream(fs);
        }

        // Find entry point
        Type? entryPoint = asm.GetTypes()
            .FirstOrDefault(static t => _IsStatic(t) && t.GetCustomAttribute<ModEntryPointAttribute>() is not null)
            ?? throw new _ModLoadException(string.Format(Lang.ErrModCantFindEntryPoint, Path.GetFileName(dllPath)));

        // Find all GameMods in the entryPoint class and add them to LoadedMods
        _LoadedMods.AddRange(entryPoint
            .GetProperties(BindingFlags.Static | BindingFlags.Public)
            .Where(static prop => {
                if (prop.PropertyType != typeof(GameMod)) return false;
                return prop.GetValue(null) is not null;
            })
            .Select(prop => {
                DebugUtil.Log(string.Format(Lang.ModLoaded, prop.Name, Path.GetFileName(dllPath)),
                "ModLoader");
                return prop.GetValue(null);
            })
            .Cast<GameMod>());
    }

    internal static void _UpdateAllMods(GameTime gameTime) {
        foreach (GameMod mod in _LoadedMods) mod.OnUpdate?.Invoke(gameTime);
    }

    /// <param name="mod">The <c>IGameMod</c> to look for</param>
    /// <returns>
    /// Whether the given <c>IGameMod</c> is loaded
    /// </returns>
    public static bool IsModLoaded(GameMod mod) => _LoadedMods.Any(m => m == mod);

    private static bool _IsStatic(Type t) => t.IsAbstract && t.IsSealed;

    private sealed class _ModLoadException(string msg) : Exception(msg);
}
#endif