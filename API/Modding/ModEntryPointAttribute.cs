using System;

namespace API.Modding;

/// <summary>
/// Indicates the class that serves as the entry point for this mod assembly.
/// Only put this attribute on 1 class per assembly.
/// All contained properties of type <c>GameMod</c> will be added to <c>ModLoader.LoadedMods</c>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ModEntryPointAttribute : Attribute { }