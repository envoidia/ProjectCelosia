using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Xna.Framework;

namespace API.Modding;

// Ignore trim/NativeAOT warnings
#pragma warning disable IL2026
#pragma warning disable IL2075

// todo add mod dependency loading, mod unloading, mod disabling, mod config, and saving logs to a temporary file
public static class ModLoader {
#if !NATIVE_AOT
    /// <summary>
    /// List of all loaded mods. Do not externally modify
    /// </summary>
    internal static readonly List<GameMod> LoadedMods = [];

    /// <summary>
    /// Lang key that should be used for a mod's display name
    /// </summary>
    public const string NameKey = "ModName";

    private static readonly string ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

    /// <summary>
    /// Do not call outside of the <c>Game1</c> instance
    /// </summary>
    public static void InitializeAllMods() {
        LoadAllMods();
        foreach (GameMod mod in LoadedMods) mod.Initialize();
    }

    private static void LoadAllMods() {
        IEnumerable<string> dllFiles = Directory.EnumerateFiles(ModsFolder, "*.dll", SearchOption.AllDirectories);
        foreach (string dllPath in dllFiles) LoadSingleModAssembly(dllPath);
    }

    private static void LoadSingleModAssembly(string dllPath) {
        AssemblyLoadContext alc = new(Path.GetFileNameWithoutExtension(dllPath));//, true); todo should be collectible/unloadable?

        Assembly asm;
        using (FileStream fs = new(dllPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            asm = alc.LoadFromStream(fs);
        }

        // Find entry point
        Type? entryPoint = asm.GetTypes()
            .FirstOrDefault(type => type.GetCustomAttribute<ModEntryPointAttribute>() != null)
            ?? throw new ModLoadException(string.Format(Lang.ErrModCantFindEntryPoint, Path.GetFileName(dllPath)));

        // Find all GameMods in the entryPoint class and add them to LoadedMods
        LoadedMods.AddRange(entryPoint.GetProperties()
            .Where(prop => {
                object? val = prop.GetValue(null);
                if (val == null) return false;
                return val.GetType() == typeof(GameMod);
            })
            .Select(prop => {
                Console.WriteLine(Lang.ModLoaded, prop.Name, Path.GetFileName(dllPath));
                return prop.GetValue(null);
            })
            .Cast<GameMod>());
    }

    /// <inheritdoc cref="InitializeAllMods" />
    public static void UpdateAllMods(GameTime gameTime) {
        foreach (GameMod mod in LoadedMods) mod.Update(gameTime);
    }

    /// <param name="mod">The <c>IGameMod</c> to look for</param>
    /// <returns>
    /// Whether the given <c>IGameMod</c> is loaded
    /// </returns>
    public static bool IsModLoaded(GameMod mod) => LoadedMods.Any(m => m == mod);
#endif
}