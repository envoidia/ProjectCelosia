using System;
using System.Collections.Generic;
using System.Linq;
using API.Battle.SkillEffects;
using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public sealed class Skill : ComplexDescriptionEntity, IModItem {
    public Range Range { get; }
    public int Cost { get; }

    public int Cooldown { get; init; } = 0;
    public Prio Prio { get; init; } = Prio.Normal;
    public bool IsBloom { get; init; } = false;

    public SkillRole[] SkillRoles { get; init; } = [];
    public SkillEffect[] SkillEffects {
        get;
        init {
            field = value;
            this.Icon = this.GetElement().Icon;
        }
    } = [];

    public GameMod? Source { get; }

    public Skill(GameMod? source, string keyName, string keyDescription, Range range, int cost)
        : base(keyName, keyDescription, Elements.Vis.Icon) {
        // todo null icon
        this.Source = source;
        this.Range = range;
        this.Cost = cost;
        Core.Skills.Add(this);
    }

    public string GetCostFormatted() =>
        string.Format(this.IsBloom ? Lang.SkillCostBloom : Lang.SkillCostSP, this.Cost.Format());

    public bool IsRangeSelf() => (this.Range == Ranges.Self) || (this.Range == Ranges.SelfUpDown);

    // todo i guess this would be better written with a contains form that searches for all 3 of them at once but this isnt perf critical anyway
    public bool ShouldTargetOpponent() => (this.Range.Side == Side.Opponent) ||
                                          this.SkillRoles.Contains(SkillRole.Attack) ||
                                          this.SkillRoles.Contains(SkillRole.DebuffDefensive) ||
                                          this.SkillRoles.Contains(SkillRole.DebuffOffensive);

    // todo multiple elements
    public Element GetElement() {
        foreach (SkillEffect skillEffect in this.SkillEffects) {
            if (skillEffect.Element != Elements.Vis) {
                return skillEffect.Element;
            }
        }

        return Elements.Vis;
    }

    /// <returns>
    /// The index a skill should start at based off of its role
    /// </returns>
    // todo more complex logic
    public int GetStartingIndex() => this.ShouldTargetOpponent() ? PosLib.LowestOpp : 0;

    public override string GetName(GameMod? mod = null) => this.GetName(Colors.Skill);

    protected override HashSet<DescriptionEntity> GetDescriptionInclusions() {
        HashSet<DescriptionEntity> inclusions = [.. this.DescriptionInclusions];

        foreach (SkillEffect skillEffect in this.SkillEffects) {
            DescriptionEntity? inclusion = skillEffect.DescInclusion;
            if (inclusion is not null) inclusions.Add(inclusion);
        }

        return inclusions;
    }

    // todo stat skills
    public override string GetDescriptionWithInclusions(GameMod? mod = null) {
        int pow = 0;
        HashSet<string> skillTypes = [];
        foreach (SkillEffect skillEffect in this.SkillEffects) {
            // todo better pow logic
            // multihit should output eg 60+20*2
            int effectPow = skillEffect.Pow;
            if (effectPow > pow) {
                pow = effectPow;
            }

            SkillType? effectType = skillEffect.SkillType;

            if (effectType is null) continue;

            skillTypes.Add(effectType.GetName(mod) + Colors.White);
        }

        string skillTypesStr = skillTypes.Count != 0
            ? string.Join(", ", skillTypes)
            : SkillTypes.Stat.GetName(mod) + Colors.White;

        return string.Format(Lang.SkillDesc, skillTypesStr, this.GetElement().GetName(mod),
            this.Range.GetName(mod), pow == 0 ? "" : $", {Colors.Num}{pow} {Colors.White}{Lang.Pow}",
            this.Prio == 0
                ? ""
                : $", {((int) this.Prio).Format()} {Colors.White}{Lang.Prio}",
            this.GetFormattedDescriptionInclusions(mod));
    }
}

public static class Skills {
    public static readonly Skill Nothing = new(null, "SkillNothing", "Blank", Ranges.Other3ROrSelf, 0);
    public static readonly Skill Defend = new(null, "SkillDefend", "Todo", Ranges.Self, 0);
}