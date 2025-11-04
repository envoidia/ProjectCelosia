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
    public static readonly List<GameMod> LoadedMods = [];

    private static readonly string ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

    public static void InitializeAllMods() {
        LoadAllMods();

        foreach (GameMod mod in LoadedMods) {
            mod.Initialize();
        }
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

        Assembly asm;
        using (FileStream fs = new(dllPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            asm = alc.LoadFromStream(fs);
        }
        
        Type? modType = asm.GetTypes()
            .FirstOrDefault(t => t.Name == "Main" && typeof(GameMod).IsAssignableFrom(t));

        // Couldn't find Main
        if (modType == null) {
            Console.WriteLine(Lang.ModCantFindMain, Path.GetFileName(dllPath));
            alc.Unload();
            return;
        }
        
        // todo ensure this doesn't crash
        LoadedMods.Add((GameMod) Activator.CreateInstance(modType)!);

        Console.WriteLine(Lang.ModLoaded, Path.GetFileName(dllPath));
    }

    public static void UpdateAllMods(GameTime gameTime) {
        foreach (GameMod mod in LoadedMods) {
            mod.Update(gameTime);
        }
    }
#endif
}