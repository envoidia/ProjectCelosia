using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace API.Util;

public static class SdlNative
{
#if !NATIVE_AOT
    internal const string _SDL2 = "SDL2";

    internal static nint SdlResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != _SDL2)
        {
            return 0;
        }

        Console.WriteLine("Resolving SDL2");

        string sdlName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            sdlName = "SDL2";
            Console.WriteLine("Resolved to Windows");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            sdlName = "libSDL2-2.0.0.dylib";
            Console.WriteLine("Resolved to OSX");
        }
        else
        {
            sdlName = "libSDL2-2.0.so.0";
            Console.WriteLine("Resolved to Linux");
        }

        if (NativeLibrary.TryLoad(sdlName, assembly, searchPath, out nint handle))
        {
            Console.WriteLine("Successfully loaded SDL2");
            return handle;
        }

        Console.WriteLine("Failed to load SDL2");
        return 0;
    }
#endif
}