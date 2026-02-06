using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using API.Debug;

namespace API.Util;

public static partial class Clipboard
{
    private const string _Sdl2 = "SDL2";

    /// <summary>
    /// Gets/sets OS clipboard text
    /// </summary>
    public static string Text
    {
        get
        {
            nint ptr = SDL_GetClipboardText();
            string result = Marshal.PtrToStringUTF8(ptr)!;
            SDL_free(ptr);
            return result;
        }

        set
        {
            Assert.NotNull(value);
            nint utf8Text = Marshal.StringToHGlobalAnsi(value);
            SDL_SetClipboardText(utf8Text);
            Marshal.FreeHGlobal(utf8Text);
        }
    }

    [LibraryImport(_Sdl2)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.ApplicationDirectory)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial void SDL_SetClipboardText(nint text);

    [LibraryImport(_Sdl2)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.ApplicationDirectory)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial nint SDL_GetClipboardText();

    [LibraryImport(_Sdl2)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.ApplicationDirectory)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial void SDL_free(nint ptr);
}
