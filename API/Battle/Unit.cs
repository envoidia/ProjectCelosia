using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Util;
using MonoGame.Extended.Collections;

namespace API.Battle;

public class Unit {
    public UnitType UnitType { get; }
    public uint Lvl { get; }
    public uint Hp { get; set; }
    public int Sp { get; set; } = 200;

    // Position on the battlefield
    // 0 4
    // 1 5
    // 2 6
    // 3 7
    public uint Pos { get; set; }

    // Shield is hit before HP, and Defend before Shield
    // Together they cannot exceed max HP
    public uint Shield { get; set; } = 0;
    public uint Defend { get; set; } = 0;

    public List<SkillInstance> SkillInstances { get; }
    public List<BuffInstance> BuffInstances { get; } = [];
    public List<Passive> Passives { get; }

    // Stats
    // _statsMult is treated as multipliers applied to _stats, in 10ths of a % (1000 = 100%), min 10%
    private readonly Dictionary<Stat, uint> _stats;
    private readonly Dictionary<Stat, uint> _statsMult = new();
    private readonly Dictionary<Element, int> _affinities;
    private readonly Dictionary<StageType, int> _stages = new();
    private readonly Dictionary<StageType, uint> _stageTurns = new();
    private readonly Dictionary<Mult, uint> _mults = new();
    private readonly Dictionary<BoolStat, uint> _boolStats = new();
    private readonly Dictionary<StatMod, int> _statMods = new();

    // Equipped item (Accessory or Weapon)
    public IEquippable Equipped {
        get;
        set {
            field.Unequip(this);
            field = value;
            field.Equip(this);
        }
    }

    public uint ExtraActions { get; set; } = 0;

    public Unit(UnitType unitType, uint lvl, IEquippable equipped, uint pos, params Skill[] skills) {
        this.UnitType = unitType;
        this.Lvl = lvl;
        this.Pos = pos;

        this._stats = unitType.Stats.ToDictionary(kvp => kvp.Key,
            kvp => kvp.Value + ((kvp.Value / 2) * this.Lvl * BattleLib.StatMult));
        this.Hp = this._stats[Stats.Hp];

        this.SkillInstances = skills.Select(skill => (SkillInstance) skill).ToList();
        this.Passives = new List<Passive>(unitType.Passives);
        this._affinities = unitType._affinities;

        this.Equipped = equipped;
        this.Equipped.Equip(this);
    }

    public void AddSkills(params Skill[] skills) {
        foreach (Skill skill in skills) this.SkillInstances.Add((SkillInstance) skill);
    }

    public void RemoveSkills(params Skill[] skills) {
        foreach (Skill skill in skills) this.SkillInstances.Remove((SkillInstance) skill);
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

    // Stats
    public uint GetStatWithStage(Stat stat, int stage) {
        if (stat == Stats.Hp) return this.Hp;

        return this._stats.GetValueOrDefault(stat, 0u) *
               (Math.Max(this._statsMult.GetValueOrDefault(stat, 1000u), 100) / 1000) *
               (uint) (stage / 10 / (stage < 0 ? 2 : 1));
    }

    public uint GetStat(Stat stat) => this.GetStatWithStage(stat, this.GetStage(stat.StageType));

    public uint GetStatMult(Stat stat) => this._statsMult.GetValueOrDefault(stat, 1000u);
    public void SetStatMult(Stat stat, uint set) => this._statsMult[stat] = set;

    // Affinities
    public int GetAffinity(Element element) => this._affinities.GetValueOrDefault(element, 0);
    public void SetAffinity(Element element, int set) => this._affinities[element] = set;
    public bool IsWeakTo(Element element) => this._affinities.GetValueOrDefault(element, 0) < 0;
    public bool Resists(Element element) => this._affinities.GetValueOrDefault(element, 0) > 0;
    public bool IsImmuneTo(Element element) => this._affinities.GetValueOrDefault(element, 0) >= 5;
    public bool IsNeutralTo(Element element) => this._affinities.GetValueOrDefault(element, 0) == 0;

    public string GetAffinitiesString() {
        StringBuilder str = new();
        foreach (Element element in Core.Elements) {
            this._affinities.TryGetValue(element, out int aff);
            str.Append(element.Icon).Append(aff.Format()).Append("/c[white]  ");
        }

        return str.ToString();
    }

    // Stages
    public int GetStage(StageType stageType) => this._stages.GetValueOrDefault(stageType, 0);
    public void SetStage(StageType stageType, int set) => this._stages[stageType] = set;

    public uint GetStageTurns(StageType stageType) => this._stageTurns.GetValueOrDefault(stageType, 0u);
    public void SetStageTurns(StageType stageType, uint set) => this._stageTurns[stageType] = set;

    public string GetTurnsStacksFormatted(StageType stageType) =>
        this.GetStage(stageType).Format() + "(" + this.GetStageTurns(stageType) + ")";

    public string GetStageStatString(StageType stageType, int stageNew) {
        StringBuilder builder = new();
        builder.Append("[WHITE] (");
        int statCount = stageType.Stats.Length;
        for (int i = 0; i < statCount; i++) {
            Stat stat = stageType.Stats[i];
            uint statDefault = this._stats[stat];
            uint statOld = this.GetStat(stat);
            uint statNew = this.GetStatWithStage(stat, stageNew);
            int change = (int) (statNew - statOld);
            // todo choiceformat
            builder.Append(string.Format(Lang.LogStageStat, Colors.Stat + stat.KeyName,
                TextLib.GetStatColor(statOld, statDefault) + TextLib.FormatNum(statOld),
                TextLib.GetStatColor(statNew, statDefault) + TextLib.FormatNum(statNew), Colors.Num +
                TextLib.FormatNum(statDefault), change.Format()));
            builder.Append(i == (statCount - 1) ? ")" : ", ");
        }

        return builder.ToString();
    }

    // Mults
    public uint GetMult(Mult mult) => this._mults.GetValueOrDefault(mult, 1000u);
    public void SetMult(Mult mult, uint set) => this._mults[mult] = set;

    public string GetMultsString() {
        StringBuilder str = new();
        foreach (Mult mult in Core.Mults) {
            uint curMult = this.GetMult(mult);
            str.Append(mult.FormatVal(curMult)).Append('\n');
        }

        return str.ToString();
    }

    // BoolStats
    public uint GetBoolStat(BoolStat boolStat) => this._boolStats.GetValueOrDefault(boolStat, 1000u);
    public void SetBoolStat(BoolStat boolStat, uint set) => this._boolStats[boolStat] = set;

    public bool IsBoolStat(BoolStat boolStat) {
        if (boolStat == BoolStats.EquipDisabled) {
            return (this._boolStats.GetValueOrDefault(BoolStats.EquipDisabled, 0u) > 0) &&
                   (this._boolStats.GetValueOrDefault(BoolStats.EquipDisabledImmunity, 0u) <= 0);
        }

        if (boolStat == BoolStats.UnableToAct) {
            return (this._boolStats.GetValueOrDefault(BoolStats.UnableToAct, 0u) > 0) &&
                   (this._boolStats.GetValueOrDefault(BoolStats.UnableToActImmunity, 0u) <= 0);
        }

        return this._boolStats.GetValueOrDefault(boolStat, 0u) > 0;
    }

    public bool IsImmuneToBoolStat(BoolStat boolStat) =>
        ((boolStat == BoolStats.UnableToAct) &&
         (this._boolStats.GetValueOrDefault(BoolStats.UnableToActImmunity, 0u) > 0))
        || ((boolStat == BoolStats.EquipDisabled) &&
            (this._boolStats.GetValueOrDefault(BoolStats.EquipDisabledImmunity, 0u) > 0));

    public string GetOtherStatsString() {
        StringBuilder str = new();
        foreach (BoolStat stat in Core.BoolStats) {
            if (stat.IsVisible) str.Append(this.GetBoolStatString(stat)).Append('\n');
        }

        return str.Append(this.ExtraActions.Format()).ToString();
    }

    public string GetBoolStatString(BoolStat stat) => this.IsImmuneToBoolStat(stat)
        ? Colors.Pos + Lang.Immune
        : this.IsBoolStat(stat)
            ? (stat.IsPositive ? Colors.Pos : Colors.Neg) + Lang.Yes
            : (stat.IsPositive ? Colors.Neg : Colors.Pos) + Lang.No;

    // StatMods
    public int GetStatMod(StatMod statMod) => this._statMods.GetValueOrDefault(statMod, 0);
    public void SetStatMod(StatMod statMod, int set) => this._statMods[statMod] = set;

    public int GetDurationModBuffTypeDealt(BuffType buffType) =>
        this._statMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.DurationBuffDealt : StatMods.DurationDebuffDealt, 0);

    public int GetDurationModBuffTypeTaken(BuffType buffType) =>
        this._statMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.DurationBuffTaken : StatMods.DurationDebuffTaken, 0);

    public int GetStacksModBuffTypeDealt(BuffType buffType) =>
        this._statMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.StacksBuffDealt : StatMods.StacksDebuffDealt, 0);

    public int GetStacksModBuffTypeTaken(BuffType buffType) =>
        this._statMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.StacksBuffTaken : StatMods.StacksDebuffTaken, 0);

    public string GetStatModsString() {
        StringBuilder str = new();
        foreach (StatMod mod in Core.StatMods) str.Append(mod.FormatVal(this.GetStatMod(mod))).Append("\n");
        return str.ToString();
    }

    // BuffEffects
    private delegate void BuffEffectNotifier(IBuffEffect effect, Unit self, Unit target, uint stacks);

    private void NotifyBuffEffects(Unit target, BuffEffectNotifier notifier) {
        // Handle Passives
        foreach (Passive passive in this.Passives) {
            foreach (IBuffEffect buffEffect in passive.BuffEffects) {
                notifier.Invoke(buffEffect, this, target, 1);
            }
        }

        // Handle Buffs
        foreach (BuffInstance buffInstance in this.BuffInstances) {
            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                notifier.Invoke(buffEffect, this, target, buffInstance.Stacks);
            }
        }
    }

    public void OnUseSkill(Unit target, Skill skill) =>
        this.NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnUseSkill(s, t, stacks, skill));

    public void OnTargetedBySkill(Unit target, Skill skill) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnTargetedBySkill(s, t, stacks, skill));

    public void OnDealDamage(Unit target, uint damage, Element element) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnDealDamage(s, t, stacks, damage, element));

    public void OnTakeDamage(Unit target, uint damage, Element element) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnDealDamage(s, t, stacks, damage, element));

    public void OnDealHeal(Unit target, uint heal, uint overheal) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnDealHeal(s, t, stacks, heal, overheal));

    public void OnTakeHeal(Unit target, uint heal, uint overheal) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnTakeHeal(s, t, stacks, heal, overheal));

    public void OnDealShield(Unit target, uint turns, uint heal) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnDealShield(s, t, stacks, turns, heal));

    public void OnTakeShield(Unit target, uint turns, uint heal) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnTakeShield(s, t, stacks, turns, heal));

    public void OnGiveBuff(Unit target, Buff buff, uint turnsMod, int stacksChange) => this.NotifyBuffEffects(target,
        (effect, s, t, stacks) => effect.OnGiveBuff(s, t, stacks, buff, turnsMod, stacksChange));

    public void OnChangeStage(Unit target, StageType stageType, uint turnsMod, int stacksChange) =>
        this.NotifyBuffEffects(target,
            (effect, s, t, stacks) => effect.OnChangeStage(s, t, stacks, stageType, turnsMod, stacksChange));

    public Side GetSide() => this.Pos < 4 ? Side.Ally : Side.Opponent;

    public void DecrementTurns() {
        // Stages
        foreach (StageType stageType in Core.StageTypes) {
            int stage = this.GetStage(stageType);
            if ((stage != 0) && (--this._stageTurns[stageType] == 0)) {
                // todo choiceformat
                BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseStage, this.UnitType.FormatName(this.Pos, false),
                    stage, stage.Format(), StageTypes.Atk.GetName(Colors.Buff),
                    this.GetStageStatString(StageTypes.Atk, 0)));
                this.SetStage(stageType, 0);
            }
        }

        // Buffs; iterate backwards so they can be removed
        for (int i = this.BuffInstances.Count - 1; i >= 0; i--) {
            BuffInstance buffInstance = this.BuffInstances[i];
            uint turns = buffInstance.Turns;

            // 1000+ turns == infinite
            if (turns is >= 2 and < 1000) {
                buffInstance.Turns = turns - 1;
            } else {
                BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseBuff, this.UnitType.FormatName(this.Pos, false),
                    buffInstance.Buff.MaxStacks, Colors.Num + buffInstance.Stacks,
                    buffInstance.Buff.GetName(Colors.Buff), string.Format(Lang.LogStacksPlural, buffInstance.Stacks)));

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

    public Result Damage(uint dmg, bool pierce, bool useName) {
        uint dmgFull = dmg;
        uint defendOld = this.Defend;
        List<string> msg = [];
        string name = useName ? this.UnitType.FormatName(this.Pos, false) + " " : "";
        string nameS = useName ? this.UnitType.FormatName(this.Pos) + " " : "";

        // Pierce skips Defend and Shield
        if (!pierce) {
            if ((this.Defend > 0) && (dmg > 0)) {
                // Only hit Defend
                if (this.Defend > dmg) {
                    this.Defend -= dmg;
                    // todo choiceformat
                    return new Result(ResultType.HitShield, string.Format(Lang.LogChangeShield, nameS,
                        Colors.Shield + (defendOld + this.Shield).Format(),
                        Colors.Shield + (this.Defend + this.Shield).Format(),
                        this.GetStat(Stats.Hp).Format(Colors.Hp), Colors.Neg + dmgFull.Format()));
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
                    long shieldOld = this.Shield;
                    this.Shield -= dmg;
                    return new Result(ResultType.HitShield, string.Format(Lang.LogChangeShield,
                        nameS /*, C_SHIELD + FormatNum((defendOld + shieldOld) / STAT_MULT_HIDDEN), C_SHIELD +
                        FormatNum(shield / STAT_MULT_HIDDEN), C_HP + FormatNum(statsDefault.GetDisplayHp()),
                        C_NEG + "-" + FormatNum(dmgFull / STAT_MULT_HIDDEN) todo*/));
                }

                // Destroy Shield and proceed to HP
                msg.Add(string.Format(Lang.LogChangeShield, nameS
                    /*,  todo C_SHIELD + FormatNum((defendOld + shield) / STAT_MULT_HIDDEN), C_SHIELD + 0, C_HP +
                         FormatNum(statsDefault.GetDisplayHp()), C_NEG + "-" + FormatNum((defendOld + shield) /
                         STAT_MULT_HIDDEN)*/));
                dmg -= this.Shield;
                this.Shield = 0;
                if (this.GetBoolStat(BoolStats.EffectBlock) <= 0) {
                    // todo is this needed
                    msg.Add(string.Format(Lang.LogChangeBooleanStatEffectBlock, name, 0));
                }
            }
        }

        uint hpOldDisp = this.Hp;
        this.Hp = Math.Clamp(this.Hp - dmg, 0, this._stats[Stats.Hp]);
        uint hpNewDisp = this.Hp;
        msg.Add(string.Format(Lang.LogChangeHp, nameS
            /*todo, C_HP + FormatNum(hpOldDisp), C_HP + FormatNum(hpNewDisp), C_HP +
             FormatNum(statsDefault.GetDisplayHp()), C_NEG + "-" + FormatNum(dmg / STAT_MULT_HIDDEN)*/));

        // todo should this be a separate result from hitting shield
        if (this.GetBoolStat(BoolStats.EffectBlock) > 0) {
            return new Result(ResultType.HitShield, msg);
        }

        if (dmg > 0) {
            return new Result(ResultType.Success, msg);
        }

        return new Result(ResultType.Fail, string.Format(Lang.LogNoEffect, name));
    }

    public Result Damage(uint dmg, bool pierce) => this.Damage(dmg, pierce, true);
    public Result Damage(uint dmg) => this.Damage(dmg, false, true);
}