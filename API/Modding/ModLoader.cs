#if !NATIVE_AOT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using API.Debug;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Modding;

// Ignore trim/NativeAOT warnings
#pragma warning disable IL2026
#pragma warning disable IL2075


// todo add mod dependency loading, mod unloading, mod disabling, mod config, and saving logs to a temporary file
public static class ModLoader
{
    #region API

    /// <summary>
    /// Lang key that should be used for a mod's display name
    /// </summary>
    public const string NameKey = "ModName";

    /// <param name="modId">The ID of the mod to look for</param>
    /// <returns>
    /// Whether the given <c>IGameMod</c> is loaded
    /// </returns>
    public static bool IsModLoaded(string modId)
    {
        return _LoadedMods.Any(m => m.Id == modId);
    }

    /// <param name="modId">The ID of the mod to look for</param>
    /// <returns>
    /// The first <c>GameMod</c> with the given ID, or null if none
    /// </returns>
    public static GameMod? Get(string modId)
    {
        return _LoadedMods.FirstOrDefault(m => m.Id == modId);
    }

    #endregion

    #region Internals

    private const string _ClassName = nameof(ModLoader);

    /// <summary>
    /// List of all loaded mods. Do not externally modify
    /// </summary>
    internal static readonly List<GameMod> _LoadedMods = [];

    private static readonly string _ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");

    internal static void _LoadAllMods()
    {
        IEnumerable<string> dllFiles = Directory.EnumerateFiles(_ModsFolder,
            "*.dll", SearchOption.AllDirectories);

        foreach (string dllPath in dllFiles)
        {
            _LoadSingleModAssembly(dllPath);
        }

        DebugConsole.Log("AllModsLoaded".IcuFormatLang(
            [_LoadedMods.Count, dllFiles.Count()]), _ClassName);
    }

    private static void _LoadSingleModAssembly(string dllPath)
    {
        AssemblyLoadContext alc = new(Path.GetFileNameWithoutExtension(dllPath));
        Assembly asm;

        using (FileStream fs = new(dllPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            asm = alc.LoadFromStream(fs);
        }

        // Find entry point
        Type entryPoint = asm.GetTypes()
            .FirstOrDefault(static t => _IsStatic(t)
                && t.GetCustomAttribute<ModEntryPointAttribute>() is not null)
            ?? throw new _ModLoadException(dllPath,
            $"Could not find a static class marked with ModEntryPointAttribute");

        // Find all mods in the entry point class
        ReadOnlySpan<GameMod> mods = [.. entryPoint
            .GetProperties(BindingFlags.Static | BindingFlags.Public)
            .Where(prop =>
            {
                // Make sure it's a GameMod
                if (prop.PropertyType != typeof(GameMod))
                {
                    return false;
                }

                // Make sure it's not null
                object? val = prop.GetValue(null);
                if (val is null)
                {
                    return false;
                }

                string id = ((GameMod) val).Id;

                // Make sure it doesn't use _ ID prefix (unless it's the base mod)
                if (id != Core.BaseModId && id.StartsWith('_'))
                {
                    throw new _ModLoadException(dllPath, $"Mod ID of {entryPoint.FullName}.{prop.Name} cannot be"
                        + $" {id} because the _ prefix is reserved for the base mod");
                }

                // Make sure its ID doesn't match base mod ID other than _
                foreach(string str in Core.ReservedIds)
                {
                    if (string.Equals(id, str.Replace("_", ""),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new _ModLoadException(dllPath, $"Mod ID of {entryPoint.FullName}.{prop.Name} cannot be"
                            + $" {id} because {id} is a reserved name");
                    }
                }

                return true;
            })
            .Select(prop =>
            {
                DebugConsole.Log("ModLoaded".FormatLang([$"{entryPoint.FullName}.{prop.Name}",
                    Path.GetFileName(dllPath)]), _ClassName);

                return prop.GetValue(null);
            })
            .Cast<GameMod>()];

        // No mods were found
        if (mods.Length == 0)
        {
            throw new _ModLoadException(dllPath,
                $"{entryPoint.FullName} does not contain any non-null public static properties of type GameMod."
                    + " Ensure that you placed ModEntryPointAttribute on only 1 class and that it is the correct one");
        }

        foreach (GameMod mod in mods)
        {
            mod.OnInit?.Invoke();
            _LoadedMods.Add(mod);
        }
    }

    internal static void _UpdateAllMods(GameTime gt)
    {
        foreach (GameMod mod in _LoadedMods)
        {
            mod.OnUpdate?.Invoke(gt);
        }
    }

    private static bool _IsStatic(Type t)
    {
        return t.IsAbstract && t.IsSealed;
    }

    private sealed class _ModLoadException(string dllPath, string msg)
        : Exception($"[ModLoader] Failed to load {Path.GetFileName(dllPath)}: {msg}");

    #endregion
}
#endif