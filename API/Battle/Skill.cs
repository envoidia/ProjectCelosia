using System.Collections.Generic;
using System.Linq;
using API.Battle.SkillEffects;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// todo docs
/// </summary>
public sealed class Skill : ComplexDescribable, IRegistrable
{
    public Range Range { get; }
    public int Cost { get; }

    public int Cooldown { get; init; } = 0;
    public RenderPriority Prio { get; init; } = RenderPriority.B1Med;
    public bool IsBloom { get; init; } = false;

    // todo automatically assign
    public SkillRole[] SkillRoles { get; init; } = [];
    public SkillEffect[] SkillEffects { get; init; } = [];

    public string ItemId { get; init; }

    public Skill(string modId, string keyName, string keyDesc, Range range, int cost, string? itemId = null)
        : base(keyName, "", keyDesc)
    {
        this.Range = range;
        this.Cost = cost;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public string GetCostFormatted()
    {
        return (this.IsBloom ? "SkillCostBloom" : "SkillCostSP").FormatLang(args: this.Cost.FormatNoColor(false));
    }

    public bool IsRangeSelf()
    {
        return this.Range == Ranges.Self || this.Range == Ranges.SelfUpDown;
    }

    public bool ShouldTargetOpponent()
    {
        return this.Range.Side == Side.Opponent || this.SkillRoles.Any(static sr =>
            sr is SkillRole.Attack or SkillRole.DebuffDefensive or SkillRole.DebuffOffensive);
    }

    public bool IsDamaging()
    {
        // todo SkillRoles
        return this.SkillEffects.Any(se => se is Damage);
        // return this.SkillRoles.Contains(SkillRole.Attack);
    }

    // todo multiple elements
    public Element GetElement()
    {
        foreach (SkillEffect se in this.SkillEffects)
        {
            if (se.Element != Element.Vis)
            {
                return se.Element;
            }
        }

        return Element.Vis;
    }

    /// <returns>
    /// The index a skill should start at based off of its role
    /// </returns>
    // todo more complex logic
    public int GetStartingIndex(int selfIndex)
    {
        if (this.IsRangeSelf())
        {
            return selfIndex;
        }

        return this.ShouldTargetOpponent() ? PosLib.LowestOpp : 0;
    }

    public override string GetName(ThemeColor color)
    {
        return $"{this.GetElement().Icon} {color.Str}{this.GetLang()}";
    }

    public override string GetName()
    {
        return this.GetName(ThemeColor.Skill);
    }

    // todo stat skills
    public override string GetFullDesc()
    {
        int pow = 0;
        HashSet<string> skillTypes = [];
        foreach (SkillEffect skillEffect in this.SkillEffects)
        {
            // todo better pow logic
            // multihit should output eg 60+20*2
            int effectPow = skillEffect.Pow;
            if (effectPow > pow)
            {
                pow = effectPow;
            }

            SkillType? effectType = skillEffect.SkillType;

            if (effectType is null)
            {
                continue;
            }

            skillTypes.Add(effectType.GetName() + ThemeColor.Fg.Str);
        }

        string skillTypesStr = skillTypes.Count != 0
            ? string.Join(", ", skillTypes)
            : SkillTypes.Stat.GetName() + ThemeColor.Fg.Str;

        return "SkillDesc".FormatLang([skillTypesStr, this.GetElement().GetName(),
            this.Range.GetName(), pow == 0 ? "" : $", {ThemeColor.Emphasis.Str}{pow} {ThemeColor.Fg.Str}{"Pow".GetLang()}",
            this.Prio == 0
                ? ""
                : $", {((int) this.Prio).Format()} {ThemeColor.Fg.Str}{"Prio".GetLang()}",
            this._GetFormattedDescInclusions()]);
    }

    protected override HashSet<IDescribable> _GetDescInclusions()
    {
        HashSet<IDescribable> inclusions = [.. this.DescInclusions];

        foreach (SkillEffect skillEffect in this.SkillEffects)
        {
            IDescribable? inclusion = skillEffect.DescInclusion;
            if (inclusion is not null)
            {
                inclusions.Add(inclusion);
            }
        }

        return inclusions;
    }
}
