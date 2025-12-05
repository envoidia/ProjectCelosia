#if !NATIVE_AOT
using System;

namespace API.Modding;

public sealed class ModLoadException(string msg) : Exception(msg);
#endif