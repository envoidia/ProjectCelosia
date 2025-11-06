using System;

namespace API.Modding;

public class ModLoadException(string message) : Exception(message) { }