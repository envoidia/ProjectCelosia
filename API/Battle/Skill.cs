using System.Collections.Generic;
using System.Linq;
using API.Battle.SkillEffects;
using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public class Skill : ComplexDescriptionEntity, IModItem {
    public Range Range { get; }
    public int Cost { get; }

    public int Cooldown { get; init; } = 0;
    public Prio Prio { get; init; } = Prio.Normal;
    public bool IsBloom { get; init; } = false;

    public SkillRole[] SkillRoles { get; init; } = [];
    public SkillEffect[] SkillEffects { get; init; } = [];

    public IGameMod? Source { get; }

    public Skill(IGameMod? source, string keyName, string keyDescription, Range range, int cost)
        : base(keyName, keyDescription, "") {
        // todo null icon
        this.Source = source;
        this.Range = range;
        this.Cost = cost;
        Core.Skills.Add(this);
    }

    public string GetCostFormatted() =>
        string.Format(this.IsBloom ? Lang.SkillCostBloom : Lang.SkillCostSP, this.Cost.Format());

    public bool HasRole(SkillRole skillRole) => this.SkillRoles.Contains(skillRole);

    public bool IsRangeSelf() => (this.Range == Ranges.Self) || (this.Range == Ranges.SelfUpDown);

    public bool ShouldTargetOpponent() => (this.Range.Side == Side.Opponent) || this.HasRole(SkillRole.Attack) ||
                                          this.HasRole(SkillRole.DebuffDefensive) ||
                                          this.HasRole(SkillRole.DebuffOffensive);

    // todo multiple elements
    public Element GetElement() {
        foreach (SkillEffect skillEffect in this.SkillEffects) {
            if (skillEffect.Element != Elements.Vis) return skillEffect.Element;
        }

        return Elements.Vis;
    }

    public static explicit operator SkillInstance(Skill skill) => new(skill);

    public override string GetName(IGameMod? mod = null) => this.GetName(Colors.Skill);

    protected override HashSet<DescriptionEntity> GetDescriptionInclusions() {
        HashSet<DescriptionEntity> inclusions = new(this.DescriptionInclusions);

        foreach (SkillEffect skillEffect in this.SkillEffects) {
            DescriptionEntity? inclusion = skillEffect.DescInclusion;
            if (inclusion is not null) inclusions.Add(inclusion);
        }

        return inclusions;
    }

    // todo stat skills
    public override string GetDescriptionWithInclusions(IGameMod? mod = null) {
        uint pow = 0;
        HashSet<string> skillTypes = [];
        foreach (SkillEffect skillEffect in this.SkillEffects) {
            // todo better pow logic
            // multihit should output eg 60+20*2
            uint effectPow = skillEffect.Pow;
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
            this.Range.GetName(mod), pow == 0 ? "" : ", " + Colors.Num + pow + " " + Colors.White + Lang.Pow,
            this.Prio == 0
                ? ""
                : ", " + ((int) this.Prio).Format() + " " + Colors.White + Lang.Prio,
            this.GetFormattedDescriptionInclusions(mod));
    }
}