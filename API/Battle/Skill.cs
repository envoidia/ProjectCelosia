using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Battle.SkillEffects;
using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public class Skill : ComplexDescriptionEntity, IModItem {
    public Element Element { get; }
    public Range Range { get; }
    public int Cost { get; }

    public int Cooldown { get; init; } = 0;
    public Prio Prio { get; init; } = Prio.Normal;
    public bool IsBloom { get; init; } = false;

    public SkillRole[] SkillRoles { get; init; } = [];
    public SkillEffect[] SkillEffects { get; init; } = [];

    public GameMod? Source { get; }

    public Skill(GameMod? source, string keyName, string keyDescription, Element element, Range range, int cost)
        : base(keyName, keyDescription, element.KeyName) {
        this.Source = source;
        this.Element = element;
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

    public static explicit operator SkillInstance(Skill skill) => new(skill);

    // todo stat skills
    public override string GetDescription() {
        uint pow = 0;
        List<string> skillTypes = new(3);
        foreach (SkillEffect skillEffect in this.SkillEffects) {
            // todo better pow logic
            // multihit should output eg 60+20*2
            uint effectPow = skillEffect.Pow;
            if (effectPow > pow) {
                pow = effectPow;
            }

            SkillType? effectType = skillEffect.SkillType;

            if (effectType is null) continue;

            string str = Colors.Stat + effectType.GetName() + "/c[white]";
            if (!skillTypes.Contains(str)) {
                skillTypes.Add(str);
            }
        }

        string skillTypesStr = skillTypes.Count != 0
            ? string.Join(", ", skillTypes)
            : Colors.Stat + SkillTypes.Stat.GetName() + "/c[white]";

        return string.Format(Lang.SkillDesc, skillTypesStr, this.Element.GetName(Colors.Element),
            this.Range.GetName(), pow == 0 ? "" : ", " + Colors.Num + pow + " [WHITE]" + Lang.Pow, this.Prio == 0
                ? ""
                : ", " + ((int) this.Prio).Format() + " /c[white]" + Lang.Prio, this.GetPartialDescription());
    }

    public override string GetPartialDescription() {
        StringBuilder partialDesc = new(base.GetPartialDescription());
        if (this.DescriptionInclusions.Length == 0) {
            partialDesc.Append('\n');
        }

        HashSet<IconEntity> inclusions = new(8);
        foreach (SkillEffect skillEffect in this.SkillEffects) {
            IconEntity? inclusion = skillEffect.DescInclusion;
            if (inclusion != null) inclusions.Add(inclusion);
        }

        foreach (IconEntity inclusion in inclusions) {
            partialDesc.Append("\n/c[white](").Append(inclusion.GetName(Colors.Buff)).Append("/c[white]: ")
                .Append(inclusion.GetDescription().Replace("\n", ". ")).Append("/c[white])");
        }

        return partialDesc.ToString();
    }
}