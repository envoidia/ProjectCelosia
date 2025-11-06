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
    public static readonly List<IGameMod> LoadedMods = [];

    private static readonly string ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

    public static void InitializeAllMods() {
        LoadAllMods();

        foreach (IGameMod mod in LoadedMods) {
            mod.Initialize();
        }
    }

    public static void InitializeCelosiaMod() {
        LoadSingleMod(Path.Combine(ModsFolder, "Celosia.dll"));
        LoadedMods[0].Initialize();
    }

    private static void LoadAllMods() {
        IEnumerable<string> dllFiles = Directory.EnumerateFiles(ModsFolder, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (string dllPath in dllFiles) {
            try {
                LoadSingleMod(dllPath);
            } catch (Exception ex) {
                Console.WriteLine(Lang.ModLoadFail, Path.GetFileName(dllPath), ex.Message);
            }
        }
    }

    private static void LoadSingleMod(string dllPath) {
        AssemblyLoadContext alc = new(Path.GetFileNameWithoutExtension(dllPath), true);
        FileStream fs = new(dllPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        Assembly asm = alc.LoadFromStream(fs);

        // Find Main class
        Type? modType = asm.GetTypes()
            .FirstOrDefault(t => (t.Name == "Main") && typeof(IGameMod).IsAssignableFrom(t));

        // Couldn't find Main
        if (modType is null) {
            Console.WriteLine(Lang.ModCantFindMain, Path.GetFileName(dllPath));
            return;
        }

        // Instantiate Main
        LoadedMods.Add((IGameMod) Activator.CreateInstance(modType)!);

        Console.WriteLine(Lang.ModLoaded, Path.GetFileName(dllPath));
    }

    public static void UpdateAllMods(GameTime gameTime) {
        foreach (IGameMod mod in LoadedMods) {
            mod.Update(gameTime);
        }
    }
#endif
}