using System;
using System.Diagnostics;
using System.Resources;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Modding;

/// <summary>
/// A dynamically loaded mod that can freely add and modify game content
/// </summary>
/// <param name="id">Unique string ID for this mod</param>
/// <param name="version">Mod version</param>
/// <param name="resourceManager">Mod's <c>Lang.ResourceManager</c></param>
public sealed class GameMod(string id, Version version, ResourceManager resourceManager) {
    /// <summary>
    /// Unique string ID for this mod
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// todo nyi IDs of all mods that this mod depends on
    /// </summary>
    public string[] DependencyIds { get; init; } = [];

    /// <summary>
    /// Mod version
    /// </summary>
    public Version Version { get; } = version;

    /// <summary>
    /// Mod's <c>Lang.ResourceManager</c>
    /// </summary>
    public ResourceManager ResourceManager { get; } = resourceManager;

    /// <summary>
    /// Called every frame
    /// </summary>
    public Action<GameTime>? OnUpdate { get; init; } = null;
}