using System;
using API.Extensions;
using API.Graphics;
using API.Name;
using Microsoft.Xna.Framework;

namespace API.Modding;

/// <summary>
/// A dynamically loaded mod that can freely add and modify game content
/// </summary>
/// <param name="id">Unique string ID for this mod</param>
/// <param name="version">Mod version</param>
public sealed class GameMod(string id, Version version) : IDescribable
{
    /// <summary>
    /// Unique string ID for this mod
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// todo nyi IDs of all mods that this mod depends on
    /// </summary>
    public string[] DependencyIds { get; init; } = [];

    public Version Version { get; } = version;

    /// <summary>
    /// Called when the mod is loaded
    /// </summary>
    public Action? OnInit { get; init; } = null;

    /// <summary>
    /// Called every frame
    /// </summary>
    public Action<GameTime>? OnUpdate { get; init; } = null;

    /// <summary>
    /// todo nyi Called when this' ingame settings menu is opened.
    /// If null, it'll instead display a popup saying that this mod has no settings
    /// </summary>
    public Action? OnOpenSettings { get; init; } = null;

    public string KeyName
    {
        get
        {
            return $"{this.Id}:{ModLoader.NameKey}";
        }
    }

    public string KeyDesc
    {
        get
        {
            return $"{this.KeyName}Desc";
        }
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.White);
    }

    public string GetName(ThemeColor color)
    {
        return color.Str + this.KeyName.GetLang();
    }

    public string GetDesc()
    {
        return this.KeyDesc.GetLang();
    }
}