using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.State;
using API.Extensions;
using API.Graphics;
using API.Util;
using MonoGame.Extended.Collections;

namespace API.Battle;

public sealed class Unit {
    #region Fields

    public UnitType UnitType { get; }
    public int Lvl { get; }
    public int Hp { get; set; }
    public int Sp { get; set; } = 200;

    /// <summary>
    /// Position on the battlefield.
    /// 0 4 /
    /// 1 5 /
    /// 2 6 /
    /// 3 7
    /// </summary>
    public int Pos { get; set; }

    /// <summary>
    /// If there's more than 1 Unit with this UnitType in the current battle, disambiguates which one this is.
    /// 0 = There are no duplicates
    /// </summary>
    public int DupeIndex { get; set; } = 0;

    /// <summary>
    /// Shield is hit before HP, and Defend before Shield. Together they cannot exceed max HP
    /// </summary>
    public int Shield { get; set; } = 0;

    /// <summary>
    /// Shield is hit before HP, and Defend before Shield. Together they cannot exceed max HP
    /// </summary>
    public int Defend { get; set; } = 0;

    public List<SkillInstance> SkillInstances { get; }
    public List<BuffInstance> BuffInstances { get; } = [];
    public List<Passive> Passives { get; }

    // Stats
    private readonly Dictionary<Stat, int> _Stats;

    /// <summary>
    /// Treated as multipliers applied to _stats, in 10ths of a % (1,000 = 100%), min 10%
    /// </summary>
    private readonly Dictionary<Stat, int> _StatsMult = [];

    private readonly Dictionary<Element, int> _Affinities;
    private readonly Dictionary<StageType, int> _Stages = [];
    private readonly Dictionary<StageType, int> _StageTurns = [];
    private readonly Dictionary<Mult, int> _Mults = [];
    private readonly Dictionary<BoolStat, int> _BoolStats = [];
    private readonly Dictionary<StatMod, int> _StatMods = [];

    /// <summary>
    /// Equipped item (Accessory or Weapon)
    /// </summary>
    public IEquippable? Equipped {
        get;
        set {
            field?.Unequip(this);
            field = value;
            field?.Equip(this);
        }
    }

    public int ExtraActions { get; set; } = 0;

    #endregion

    public Unit(UnitType unitType, int lvl, IEquippable? equipped, params Skill[] skills) {
        this.UnitType = unitType;
        this.Lvl = lvl;

        this._Stats = unitType.Stats.ToDictionary(static kvp => kvp.Key,
            kvp => kvp.Value + ((kvp.Value / 2) * this.Lvl * BattleLib.StatMult));
        this.Hp = this._Stats[Stats.Hp];

        this.SkillInstances = [.. skills.Select(static skill => new SkillInstance(skill))];
        this.Passives = [.. unitType.Passives];
        this._Affinities = unitType._Affinities;

        this.Equipped = equipped;
        this.Equipped?.Equip(this);
    }

    public void AddSkills(params Skill[] skills) {
        foreach (Skill skill in skills) this.SkillInstances.Add(new SkillInstance(skill));
    }

    public void RemoveSkills(params Skill[] skills) {
        foreach (Skill skill in skills) this.SkillInstances.Remove(new SkillInstance(skill));
    }

    public void AddPassives(params Passive[] passives) {
        foreach (Passive passive in passives) {
            this.Passives.Add(passive);
            foreach (IBuffEffect buffEffect in passive.BuffEffects) buffEffect.OnGive(this, 1);
        }
    }

    public void RemovePassives(params Passive[] passives) {
        foreach (Passive passive in passives) {
            this.Passives.Remove(passive);
            foreach (IBuffEffect buffEffect in passive.BuffEffects) buffEffect.OnRemove(this, 1);
        }
    }

    #region Stats

    public int GetStatWithStage(Stat stat, int stage) {
        if (stat == Stats.Hp) return this.Hp;

        return (int) (this._Stats.GetValueOrDefault(stat, 0) *
               (Math.Max(this._StatsMult.GetValueOrDefault(stat, 1000), 100) / 1000f) *
               (1 + (stage / 10 / (stage < 0 ? 2 : 1))));
    }

    public int GetStat(Stat stat) => this.GetStatWithStage(stat, this.GetStage(stat.StageType));
    public int GetBaseStat(Stat stat) => this._Stats.GetValueOrDefault(stat, 0);

    public int GetStatMult(Stat stat) => this._StatsMult.GetValueOrDefault(stat, 1000);
    public void SetStatMult(Stat stat, int set) => this._StatsMult[stat] = set;

    #endregion

    #region Affinities

    public int GetAffinity(Element element) => this._Affinities.GetValueOrDefault(element, 0);
    public void SetAffinity(Element element, int set) => this._Affinities[element] = set;
    public bool IsWeakTo(Element element) => this._Affinities.GetValueOrDefault(element, 0) < 0;
    public bool Resists(Element element) => this._Affinities.GetValueOrDefault(element, 0) > 0;
    public bool IsImmuneTo(Element element) => this._Affinities.GetValueOrDefault(element, 0) >= 5;
    public bool IsNeutralTo(Element element) => this._Affinities.GetValueOrDefault(element, 0) == 0;

    private static readonly int[] _CoreDmgDealt = [300, 500, 650, 800, 900, 1000, 1100, 1200, 1350, 1500, 1700];
    public int GetElementDmgDealt(Element element) =>
        _Extrapolate(this.GetAffinity(element), _CoreDmgDealt, 200, -200);

    private static readonly int[] _CoreDmgTaken = [2500, 2000, 1700, 1400, 1200, 1000, 900, 800, 650, 500, 0];
    public int GetElementDmgTaken(Element element) =>
        _Extrapolate(this.GetAffinity(element), _CoreDmgTaken, 0, 500);

    private static readonly int[] _CoreSpCost = [1700, 1500, 1300, 1200, 1100, 1000, 950, 900, 850, 800, 750];
    public int GetElementSpCost(Element element) =>
            _Extrapolate(this.GetAffinity(element), _CoreSpCost, -50, 200);

    private static int _Extrapolate(int i, int[] core, int stepUp, int stepDown) {
        // Real index
        int index = i + 5;

        int value;

        // In bounds
        if (index >= 0 && index < core.Length) {
            value = core[index];
        }
        // Above bounds
        else if (i >= core.Length) {
            value = core[^1] + ((stepUp * index) - (core.Length - 1));
        }
        // Below bounds
        else {
            value = core[0] + (stepDown * Math.Abs(index));
        }

        // Max to 0
        return Math.Max(value, 0);
    }


    public string GetAffinitiesString() {
        StringBuilder str = new();
        foreach (Element element in Core.Elements) {
            this._Affinities.TryGetValue(element, out int aff);
            str.Append(element.Icon).Append(aff.Format()).Append(ColorCode.White).Append("  ");
        }

        return str.ToString();
    }

    #endregion

    #region Stages

    public int GetStage(StageType stageType) => this._Stages.GetValueOrDefault(stageType, 0);
    public void SetStage(StageType stageType, int set) => this._Stages[stageType] = set;

    public int GetStageTurns(StageType stageType) => this._StageTurns.GetValueOrDefault(stageType, 0);
    public void SetStageTurns(StageType stageType, int set) => this._StageTurns[stageType] = set;

    public string GetTurnsStacksFormatted(StageType stageType) =>
        $"{this.GetStage(stageType).Format()}({this.GetStageTurns(stageType)})";

    public string GetStageStatString(StageType stageType, int stageNew) {
        StringBuilder builder = new();
        builder.Append(ColorCode.White).Append(" (");
        int statCount = stageType.Stats.Length;

        for (int i = 0; i < statCount; i++) {
            Stat stat = stageType.Stats[i];
            int statDefault = this._Stats[stat];
            int statOld = this.GetStat(stat);
            int statNew = this.GetStatWithStage(stat, stageNew);
            int change = statNew - statOld;

            builder.Append(string.Format(Lang.LogStageStat, this.FormatName(), stat.GetName(),
                TextLib.FormatStat(statOld, statDefault), TextLib.FormatStat(statNew, statDefault),
                statDefault.Format(ColorCode.Num), change.Format()));

            builder.Append(i == (statCount - 1) ? ")" : ", ");
        }

        return builder.ToString();
    }

    #endregion

    #region Mults

    public float GetMult(Mult mult) => this._Mults.GetValueOrDefault(mult, 1000) / 1000f;
    public int GetRawMult(Mult mult) => this._Mults.GetValueOrDefault(mult, 1000);
    public void SetMult(Mult mult, int set) => this._Mults[mult] = set; // todo ensure these dont crash

    public string GetMultsString() {
        StringBuilder str = new();
        foreach (Mult mult in Core.Mults) {
            int curMult = this._Mults[mult];
            str.Append(mult.Format(curMult)).Append('\n');
        }

        return str.ToString();
    }

    #endregion

    #region BoolStats

    public int GetBoolStat(BoolStat boolStat) => this._BoolStats.GetValueOrDefault(boolStat, 1000);
    public void SetBoolStat(BoolStat boolStat, int set) => this._BoolStats[boolStat] = set;

    public bool IsBoolStat(BoolStat boolStat) {
        if (boolStat == BoolStats.EquipDisabled) {
            return (this._BoolStats.GetValueOrDefault(BoolStats.EquipDisabled, 0) > 0) &&
                   (this._BoolStats.GetValueOrDefault(BoolStats.EquipDisabledImmunity, 0) <= 0);
        }

        if (boolStat == BoolStats.UnableToAct) {
            return (this._BoolStats.GetValueOrDefault(BoolStats.UnableToAct, 0) > 0) &&
                   (this._BoolStats.GetValueOrDefault(BoolStats.UnableToActImmunity, 0) <= 0);
        }

        return this._BoolStats.GetValueOrDefault(boolStat, 0) > 0;
    }

    public bool IsImmuneToBoolStat(BoolStat boolStat) =>
        ((boolStat == BoolStats.UnableToAct) &&
        (this._BoolStats.GetValueOrDefault(BoolStats.UnableToActImmunity, 0) > 0)) ||
        ((boolStat == BoolStats.EquipDisabled) &&
        (this._BoolStats.GetValueOrDefault(BoolStats.EquipDisabledImmunity, 0) > 0));

    public string GetOtherStatsString() {
        StringBuilder str = new();
        foreach (BoolStat stat in Core.BoolStats) {
            if (stat.IsVisible) str.Append(this.GetBoolStatString(stat)).Append('\n');
        }

        return str.Append(this.ExtraActions.Format()).ToString();
    }

    public string GetBoolStatString(BoolStat stat) {
        if (this.IsImmuneToBoolStat(stat)) return ColorCode.Pos + Lang.Immune;

        return (stat.IsPositive ? ColorCode.Pos : ColorCode.Neg) + (this.IsBoolStat(stat) ? Lang.Yes : Lang.No);
    }

    #endregion

    #region StatMods

    public int GetStatMod(StatMod statMod) => this._StatMods.GetValueOrDefault(statMod, 0);
    public void SetStatMod(StatMod statMod, int set) => this._StatMods[statMod] = set;

    public int GetDurationModBuffTypeDealt(BuffType buffType) =>
        this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.DurationBuffDealt : StatMods.DurationDebuffDealt,
            0);

    public int GetDurationModBuffTypeTaken(BuffType buffType) =>
        this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.DurationBuffTaken : StatMods.DurationDebuffTaken,
            0);

    public int GetStacksModBuffTypeDealt(BuffType buffType) =>
        this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.StacksBuffDealt : StatMods.StacksDebuffDealt, 0);

    public int GetStacksModBuffTypeTaken(BuffType buffType) =>
        this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.StacksBuffTaken : StatMods.StacksDebuffTaken, 0);

    public string GetStatModsString() {
        StringBuilder str = new();
        foreach (StatMod mod in Core.StatMods) str.Append(mod.Format(this.GetStatMod(mod))).Append('\n');
        return str.ToString();
    }

    #endregion

    #region BuffEffects

    private void _NotifyBuffEffects(Unit target, Action<IBuffEffect, Unit, Unit, int> notifier) {
        // Handle Passives
        foreach (Passive passive in this.Passives) {
            foreach (IBuffEffect buffEffect in passive.BuffEffects) {
                notifier(buffEffect, this, target, 1);
            }
        }

        // Handle Buffs
        foreach (BuffInstance buffInstance in this.BuffInstances) {
            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                notifier(buffEffect, this, target, buffInstance.Stacks);
            }
        }
    }

    public void OnUseSkill(Unit target, Skill skill) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnUseSkill(s, t, stacks, skill));

    public void OnTargetedBySkill(Unit target, Skill skill) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTargetedBySkill(s, t, stacks, skill));

    public void OnDealDamage(Unit target, int damage, Element element) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnDealDamage(s, t, stacks, damage, element));

    public void OnTakeDamage(Unit target, int damage, Element? element = null) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTakeDamage(s, t, stacks, damage, element));

    public void OnDealHeal(Unit target, int heal, int overheal) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnDealHeal(s, t, stacks, heal, overheal));

    public void OnTakeHeal(Unit target, int heal, int overheal) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTakeHeal(s, t, stacks, heal, overheal));

    public void OnDealShield(Unit target, int turns, int heal) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnDealShield(s, t, stacks, turns, heal));

    public void OnTakeShield(Unit target, int turns, int heal) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTakeShield(s, t, stacks, turns, heal));

    public void OnGiveBuff(Unit target, Buff buff, int turns, int stacksChange) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnGiveBuff(s, t, stacks, buff, turns, stacksChange));

    public void OnChangeStage(Unit target, StageType stageType, int turns, int stacksChange) =>
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnChangeStage(s, t, stacks, stageType, turns, stacksChange));

    public Side GetSide() => this.Pos < PosLib.LowestOpp ? Side.Ally : Side.Opponent;

    #endregion

    public void DecrementTurns() {
        // Stages
        foreach (StageType stageType in Core.StageTypes) {
            int stage = this.GetStage(stageType);
            if (stage != 0 && --this._StageTurns[stageType] == 0) {
                LogLib.Add(Lang.LogLoseStage.FormatIcu(this.FormatName(false),
                    stage, stage.Format(), StageTypes.Atk.GetName(),
                    this.GetStageStatString(StageTypes.Atk, 0)));
                this.SetStage(stageType, 0);
            }
        }

        // Buffs; iterate backwards so they can be removed
        for (int i = this.BuffInstances.Count - 1; i >= 0; i--) {
            BuffInstance buffInstance = this.BuffInstances[i];
            int turns = buffInstance.Turns;

            if (turns is >= 2 and < BuffInstance.InfiniteTurns) {
                buffInstance.Turns = turns - 1;
            } else {
                LogLib.Add(Lang.LogLoseBuff.FormatIcu(this.FormatName(false),
                    buffInstance.Buff.MaxStacks, ColorCode.Num + buffInstance.Stacks,
                    buffInstance.Buff.GetName(), buffInstance.Stacks));

                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                    buffEffect.OnRemove(this, buffInstance.Stacks);
                }

                this.BuffInstances.RemoveAt(i);
            }
        }

        // Skill cooldowns
        foreach (SkillInstance skillInstance in this.SkillInstances) {
            if (skillInstance.Cooldown > 0) --skillInstance.Cooldown;
        }
    }

    public Result Damage(int dmg, bool pierce = false, bool useName = true) {
        int dmgFull = dmg;
        int defendOld = this.Defend;
        List<string> msg = [];

        string name = "";
        string nameS = "";

        if (useName) {
            name = $"{this.FormatName(false)} ";
            nameS = $"{this.FormatName()} ";
        }

        // Pierce skips Defend and Shield
        if (!pierce) {
            if ((this.Defend > 0) && (dmg > 0)) {
                // Only hit Defend
                if (this.Defend > dmg) {
                    this.Defend -= dmg;
                    return new Result(ResultType.HitEffectBlock, string.Format(Lang.LogChangeShield, nameS,
                        (defendOld + this.Shield).Format(ColorCode.Shield),
                        (this.Defend + this.Shield).Format(ColorCode.Shield),
                        this.GetStat(Stats.Hp).Format(ColorCode.Hp), dmgFull.Format()));
                }

                // Destroy Defend and proceed to Shield
                dmg -= this.Defend;
                this.Defend = 0;

                // todo this should come after the dmg message; is this needed now that shield is a buff
                if ((this.Shield == 0) && (this.GetBoolStat(BoolStats.EffectBlock) <= 0)) {
                    msg.Add(string.Format(Lang.LogChangeBooleanStatEffectBlock, name, 0));
                }
            }

            if ((this.Shield > 0) && (dmg > 0)) {
                // Only hit Shield
                if (this.Shield > dmg) {
                    int shieldOld = this.Shield;
                    this.Shield -= dmg;
                    return new Result(ResultType.HitEffectBlock, string.Format(Lang.LogChangeShield,
                        nameS, (defendOld + shieldOld).Format(ColorCode.Shield), this.Shield.Format(ColorCode.Shield),
                        this.GetBaseStat(Stats.Hp).Format(ColorCode.Hp), (-dmgFull).Format()));
                }

                // Destroy Shield and proceed to HP
                msg.Add(string.Format(Lang.LogChangeShield, nameS, (defendOld + this.Shield).Format(ColorCode.Shield),
                    ColorCode.Shield + 0, this.GetBaseStat(Stats.Hp).Format(ColorCode.Hp),
                    (-(defendOld + this.Shield)).Format()));
                dmg -= this.Shield;
                this.Shield = 0;
                if (this.GetBoolStat(BoolStats.EffectBlock) <= 0) {
                    // todo is this needed
                    msg.Add(string.Format(Lang.LogChangeBooleanStatEffectBlock, name, 0));
                }
            }
        }

        int hpOld = this.Hp;
        this.Hp = Math.Clamp(this.Hp - dmg, 0, this._Stats[Stats.Hp]);
        int hpNew = this.Hp;
        msg.Add(string.Format(Lang.LogChangeHp, nameS, hpOld.Format(ColorCode.Hp, false), hpNew.Format(ColorCode.Hp, false),
            this.GetBaseStat(Stats.Hp).Format(ColorCode.Hp, false), (-dmg).Format()));

        // todo should this be a separate result from hitting shield
        if (this.GetBoolStat(BoolStats.EffectBlock) > 0) {
            return new Result(ResultType.HitEffectBlock, msg);
        }

        return dmg > 0
            ? new Result(ResultType.Success, msg)
            : new Result(ResultType.Fail, string.Format(Lang.LogNoEffect, name));
    }

    // todo support other langs (+ nicknames?)
    public string FormatName(bool possessive = true) {
        string name = this.UnitType.GetName(
            // Color
            this.GetSide() == Side.Ally ? ColorCode.Ally : ColorCode.Opp) +
            // Dupe disambiguation
            (this.DupeIndex == 0 ? "" : $" {this.DupeIndex}");

        string suffix = "";

        if (possessive) {
            if (name.ToUpperInvariant().EndsWith('S')) suffix = "'";
            else suffix = "'s";
        }

        return name + suffix + ColorCode.White;
    }
}