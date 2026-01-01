using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Passive : ComplexDescribable, IRegistrable
{
    public IBuffEffect[] BuffEffects { get; init; }

    public string ItemId { get; init; }

    public Passive(string modId, string keyName, string keyDesc, string icon, IBuffEffect[] buffEffects,
        string? itemId = null) : base($"{modId}:{keyName}", icon, $"{modId}:{keyDesc}")
    {
        this.BuffEffects = buffEffects;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string GetName()
    {
        return this.GetName(ThemeColor.Passive);
    }

    public override string GetFullDesc()
    {
        return "todo";
    }
}