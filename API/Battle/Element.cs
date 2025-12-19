using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// An elemental type that can be used for <c>Skill</c>s
/// </summary>
public sealed class Element : INameable, IRegistrable {
    /// <summary>
    /// Base/no element
    /// </summary>
    public static readonly Element Vis = new(Core.Id, "ElementVis",
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

    public string KeyName { get; }
    public string Icon { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public Element(string modId, string keyName, string icon,
        Mult? multDmgDealt = null, Mult? multDmgTaken = null) {
        this.MultDmgDealt = multDmgDealt;
        this.MultDmgTaken = multDmgTaken;

        this.KeyName = keyName;
        this.Icon = icon;

        this.ModId = modId;
        this.ItemId = keyName;

        Registry.Register(this);
    }

    public string GetName(ThemeColor color) => $"{this.Icon} {color.Str()}{this.GetLang()}";
    public string GetName() => this.GetName(ThemeColor.Element);
}