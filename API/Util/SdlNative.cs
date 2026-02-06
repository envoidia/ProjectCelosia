using System.Reflection;
using System.Runtime.InteropServices;

namespace API.Util;

public static class SdlNative
{
#if !NATIVE_AOT
    private const string _SDL2 = "SDL2";

    internal static nint SdlResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != _SDL2)
        {
            return 0;
        }

        string sdlName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            sdlName = "SDL2.dll";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            sdlName = "libSDL2-2.0.0.dylib";
        }
        else
        {
            sdlName = "libSDL2-2.0.so.0";
        }

        if (NativeLibrary.TryLoad(sdlName, assembly, searchPath, out nint handle))
        {
            return handle;
        }

        return 0;
    }
#endif
}