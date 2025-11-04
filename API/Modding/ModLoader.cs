using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Xna.Framework;

namespace API.Modding;

// todo add mod dependency loading, mod unloading, mod disabling, and saving logs to a temporary file
public static class ModLoader {
#if !NATIVE_AOT
    private static readonly List<AssemblyLoadContext> ALCs = [];
    public static readonly List<GameMod> LoadedMods = [];

    private static readonly string ModsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

    public static void LoadAllMods() {
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

        // Find Initialize()
        Type? modType = asm.GetTypes()
            .Where(t => typeof(GameMod).IsAssignableFrom(t) && !t.IsAbstract)
            .FirstOrDefault(t => {
                MethodInfo? initMethod = t.GetMethod(
                    "Initialize",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    CallingConventions.Any,
                    Type.EmptyTypes,
                    null);

                return (initMethod != null) && (initMethod.DeclaringType == t);
            });

        // Couldn't find Initialize()
        if (modType == null) {
            Console.WriteLine(Lang.ModFindInititalizeFail, Path.GetFileName(dllPath));
            alc.Unload();
            return;
        }

        // Invoke Initialize()
        if (Activator.CreateInstance(modType) is not GameMod instance) {
            alc.Unload();
            return;
        }

        instance.Initialize();

        ALCs.Add(alc);
        LoadedMods.Add(instance);

        Console.WriteLine(Lang.ModLoaded, Path.GetFileName(dllPath));
    }

    public static void UpdateAllMods(GameTime gameTime) {
        foreach (GameMod mod in LoadedMods) {
            mod.Update(gameTime);
        }
    }
#endif
}