using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// An elemental type that can be used for <c>Skill</c>s
/// </summary>
public sealed class Element : INameable {
    /// <summary>
    /// Base/no element
    /// </summary>
    public static readonly Element Vis = new(null, "ElementVis",
        $"{ThemeColor.Vis.Str()}/i[rolling-energy]") { IsVisible = false };

    /// <summary>
    /// <c>Mult</c> to use as the dmg taken mult for this
    /// </summary>
    public Mult? MultDmgDealt { get; }

    /// <summary>
    /// <c>Mult</c> to use as the dmg deald mult for this
    /// </summary>
    public Mult? MultDmgTaken { get; }

    /// <summary>
    /// Whether this should be listed in affinities. Intended for elements that aren't meant to have affinities
    /// </summary>
    public bool IsVisible { get; init; } = true;

    public GameMod? Source { get; }
    public string KeyName { get; }
    public string Icon { get; }

    public Element(GameMod? source, string keyName, string icon,
        Mult? multDmgDealt = null, Mult? multDmgTaken = null) {
        this.Source = source;
        this.KeyName = keyName;
        this.Icon = icon;

        this.MultDmgDealt = multDmgDealt;
        this.MultDmgTaken = multDmgTaken;

        Core.Elements.Add(this);
    }

    public string GetName(ThemeColor color, GameMod? mod = null) =>
        $"{this.Icon} {color.Str()}{this.KeyName.GetLang(mod ?? this.Source)}";
    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.Element, mod ?? this.Source);
}