using System;

namespace API.Modding;

public sealed class ModLoadException(string msg) : Exception(msg);