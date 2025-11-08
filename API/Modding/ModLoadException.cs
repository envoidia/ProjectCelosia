using System;

namespace API.Modding;

public class ModLoadException(string msg) : Exception(msg) { }