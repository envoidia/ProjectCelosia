using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Xna.Framework;

namespace API.Modding;

// todo add mod dependency loading, mod unloading, mod disabling, mod config, and saving logs to a temporary file
public static class ModLoader {
#if !NATIVE_AOT

    #region Fields

    /// <summary>
    /// List of all loaded mods (todo private/internal?)
    /// </summary>
    public static readonly List<IGameMod> LoadedMods = [];

    /// <summary>
    /// Lang key that should be used for a mod's display name
    /// </summary>
    public const string NameKey = "ModName";

    private static readonly string ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

    #endregion

    #region Mod Loading/Update Methods

    /// <summary>
    /// Do not call outside of the <c>Game1</c> instance
    /// </summary>
    public static void InitializeAllMods() {
        LoadAllMods();
        foreach (IGameMod mod in LoadedMods) mod.Initialize();
    }

    private static void LoadAllMods() {
        IEnumerable<string> dllFiles = Directory.EnumerateFiles(ModsFolder, "*.dll", SearchOption.TopDirectoryOnly);
        foreach (string dllPath in dllFiles) LoadSingleMod(dllPath);
    }

    private static void LoadSingleMod(string dllPath) {
        AssemblyLoadContext alc = new(Path.GetFileNameWithoutExtension(dllPath));//, true); todo should be collectible/unloadable?

        Assembly asm;
        using (FileStream fs = new(dllPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            asm = alc.LoadFromStream(fs);
        }

        // Find Main class
        Type? modType = asm.GetTypes()
            .FirstOrDefault(type => type.Name == "Main" && typeof(IGameMod).IsAssignableFrom(type))
            ?? throw new ModLoadException(string.Format(Lang.ErrModCantFindMain, Path.GetFileName(dllPath)));

        // Instantiate Main
        LoadedMods.Add((IGameMod) Activator.CreateInstance(modType)!);

        Console.WriteLine(Lang.ModLoaded, Path.GetFileName(dllPath));
    }

    #endregion

    #region Util Methods

    /// <inheritdoc cref="InitializeAllMods" />
    public static void UpdateAllMods(GameTime gameTime) {
        foreach (IGameMod mod in LoadedMods) mod.Update(gameTime);
    }

    /// <param name="mod">The <c>IGameMod</c> to look for</param>
    /// <returns>
    /// Whether the given <c>IGameMod</c> is loaded
    /// </returns>
    public static bool IsModLoaded(IGameMod mod) => LoadedMods.Any(m => m == mod);

    #endregion
#endif
}