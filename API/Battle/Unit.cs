using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using API.Extensions;
using API.Graphics;

namespace API.Battle;

public class Unit {
    public UnitType UnitType { get; }
    public uint Lvl { get; }

    // Stats
    // statsMult is treated as multipliers applied to statsDefault, in 10ths of a % (1000 = 100%)
    // Is never treated as less than 10%
    // HP, Str, Mag, Amr, Res, and Agi
    // todo array?
    private readonly Stats _stats;
    private readonly Stats _statsMult = new(1000);
    public uint Hp { get; }

    // Equipped item (Accessory or Weapon)
    public IEquippable Equipped {
        get;
        set {
            field.Unequip(this);
            field = value;
            field.Equip(this);
        }
    }

    public List<SkillInstance> SkillInstances { get; }
    public List<BuffInstance> BuffInstances { get; } = [];
    public List<Passive> Passives { get; }

    // Affinities
    // Multiplies the damage dealt, damage taken, and SP cost for their corresponding element
    public Dictionary<Element, int> Affinities { get; }

    public int Sp { get; set; } = 200;

    // Position on the battlefield
    // 0 4
    // 1 5
    // 2 6
    // 3 7
    public uint Pos { get; set; }

    // Stat stages
    // Increases/decreases corresponding stats by +10/-5% each level, between -5 and
    // +5 levels
    public int StageAtk { get; set; }
    public uint StageAtkTurns { get; set; }
    public int StageDef { get; set; }
    public uint StageDefTurns { get; set; }
    public int StageFth { get; set; }
    public uint StageFthTurns { get; set; }
    public int StageAgi { get; set; }
    public uint StageAgiTurns { get; set; }

    // Multipliers
    // Multiplies the corresponding numbers
    // In 10ths of a % (1000 = *100%)
    public uint MultDmgDealt { get; set; } = 1000;
    public uint MultDmgTaken { get; set; } = 1000;
    public Dictionary<Element, uint> MultElementDmgDealt { get; } = new();
    public Dictionary<Element, uint> MultElementDmgTaken { get; } = new();
    public uint MultWeakDmgDealt { get; set; } = 1000;
    public uint MultWeakDmgTaken { get; set; } = 1000;
    public uint MultFollowUpDmgDealt { get; set; } = 1000;
    public uint MultFollowUpDmgTaken { get; set; } = 1000;
    public uint MultDoTDmgTaken { get; set; } = 1000;
    public uint MultPercentageDmgTaken { get; set; } = 1000;
    public uint MultHealingDealt { get; set; } = 1000;
    public uint MultHealingTaken { get; set; } = 1000;
    public uint MultSpGain { get; set; } = 1000;
    public uint MultSpUse { get; set; } = 1000;

    // Shield is hit before HP, and Defend before Shield
    // Together they cannot exceed max HP
    public uint Shield { get; set; } = 0;
    public uint Defend { get; set; } = 0;

    public uint ExtraActions { get; set; } = 0;

    /// Boolean stats
    // Secondary effect block; >= 1 blocks secondary effects the same as Shield
    public uint EffectBlock { get; set; } = 0;

    // >= 1 means SP is infinite
    public uint InfiniteSp { get; set; } = 0;

    // >= 1 removes ability to move
    public uint UnableToAct { get; set; } = 0;

    // >= 1 conveys immunity to unableToAct
    public uint UnableToActImmunity { get; set; } = 0;

    // >= 1 disables equipped
    public uint EquipDisabled { get; set; } = 0;

    // >= 1 conveys immunity to equipDisabled
    public uint EquipDisabledImmunity { get; set; } = 0;

    // Modifiers
    public int ModDurationBuffDealt { get; set; } = 0;
    public int ModDurationBuffTaken { get; set; } = 0;
    public int ModDurationDebuffDealt { get; set; } = 0;
    public int ModDurationDebuffTaken { get; set; } = 0;
    public int ModStacksBuffDealt { get; set; } = 0;
    public int ModStacksBuffTaken { get; set; } = 0;
    public int ModStacksDebuffDealt { get; set; } = 0;
    public int ModStacksDebuffTaken { get; set; } = 0;
    public int ModRange { get; set; } = 0;

    public Unit(UnitType unitType, uint lvl, IEquippable equipped, uint pos, params Skill[] skills) {
        this.UnitType = unitType;
        this.Lvl = lvl;
        this._stats = unitType.StatsBase.GetScaledStats(lvl);
        this.Hp = this._stats.Hp;
        this.Equipped = equipped;
        this.Equipped.Equip(this);
        this.SkillInstances = skills.Select(skill => (SkillInstance) skill).ToList();
        this.Passives = new List<Passive>(unitType.Passives);
        this.Affinities = unitType.AffinitiesBase;
        this.Pos = pos;
    }

    public uint GetStat(Stat stat) => this._stats.GetStat(stat) * (Math.Max(this._statsMult.GetStat(stat), 100) / 1000);

    public virtual void AddSkills(params Skill[] skills)
    {
        foreach (Skill skill in skills) SkillInstances.Add((SkillInstance) skill);
    }

    public virtual void RemoveSkills(params Skill[] skills)
    {
        foreach (Skill skill in skills) SkillInstances.Remove((SkillInstance) skill);
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

    public int GetStage(StageType stageType) => stageType.Id switch {
        StageTypeId.Atk => this.StageAtk,
        StageTypeId.Def => this.StageDef,
        StageTypeId.Fth => this.StageFth,
        StageTypeId.Agi => this.StageAgi
    };

    public void SetStage(StageType stageType, int set) {
        switch (stageType.Id) {
            case StageTypeId.Atk: this.StageAtk = set; break;
            case StageTypeId.Def: this.StageDef = set; break;
            case StageTypeId.Fth: this.StageFth = set; break;
            case StageTypeId.Agi: this.StageAgi = set; break;
        }
    }

    public uint GetStageTurns(StageType stageType) => stageType.Id switch {
        StageTypeId.Atk => this.StageAtkTurns,
        StageTypeId.Def => this.StageDefTurns,
        StageTypeId.Fth => this.StageFthTurns,
        StageTypeId.Agi => this.StageAgiTurns
    };

    public void SetStageTurns(StageType stageType, uint set) {
        switch (stageType.Id) {
            case StageTypeId.Atk: this.StageAtkTurns = set; break;
            case StageTypeId.Def: this.StageDefTurns = set; break;
            case StageTypeId.Fth: this.StageFthTurns = set; break;
            case StageTypeId.Agi: this.StageAgiTurns = set; break;
        }
    }

    public uint GetStatWithStage(Stat stat) => stat switch {
        Stat.Str => Math.Max((uint)
            (this.GetStat(Stat.Str) + ((this._stats.Str * (this.StageAtk / 10)) / (this.StageAtk < 0 ? 2 : 1))), 1),
        Stat.Mag => Math.Max((uint)
            (this.GetStat(Stat.Mag) + ((this._stats.Mag * (this.StageAtk / 10)) / (this.StageAtk < 0 ? 2 : 1))), 1),
        Stat.Fth => Math.Max((uint)
            (this.GetStat(Stat.Fth) + ((this._stats.Fth * (this.StageFth / 10)) / (this.StageFth < 0 ? 2 : 1))), 1),
        Stat.Amr => Math.Max((uint)
            (this.GetStat(Stat.Amr) + ((this._stats.Amr * (this.StageDef / 10)) / (this.StageDef < 0 ? 2 : 1))), 1),
        Stat.Res => Math.Max((uint)
            (this.GetStat(Stat.Res) + ((this._stats.Res * (this.StageDef / 10)) / (this.StageDef < 0 ? 2 : 1))), 1),
        Stat.Agi => Math.Max((uint)
            (this.GetStat(Stat.Agi) + ((this._stats.Agi * (this.StageAgi / 10)) / (this.StageAgi < 0 ? 2 : 1))), 1)
    };

    public uint GetStatWithStage(Stat stat, int stage) => stat switch {
        Stat.Str =>
            Math.Max((uint) (this.GetStat(Stat.Str) + ((this._stats.Str * (stage / 10)) / (stage < 0 ? 2 : 1))), 1),
        Stat.Mag =>
            Math.Max((uint) (this.GetStat(Stat.Mag) + ((this._stats.Mag * (stage / 10)) / (stage < 0 ? 2 : 1))), 1),
        Stat.Fth =>
            Math.Max((uint) (this.GetStat(Stat.Fth) + ((this._stats.Fth * (stage / 10)) / (stage < 0 ? 2 : 1))), 1),
        Stat.Amr =>
            Math.Max((uint) (this.GetStat(Stat.Amr) + ((this._stats.Amr * (stage / 10)) / (stage < 0 ? 2 : 1))), 1),
        Stat.Res =>
            Math.Max((uint) (this.GetStat(Stat.Res) + ((this._stats.Res * (stage / 10)) / (stage < 0 ? 2 : 1))), 1),
        Stat.Agi =>
            Math.Max((uint) (this.GetStat(Stat.Agi) + ((this._stats.Agi * (stage / 10)) / (stage < 0 ? 2 : 1))), 1)
    };

    public string GetAffinitiesString() {
        StringBuilder str = new();
        foreach (Element element in Core.Elements) {
            this.Affinities.TryGetValue(element, out int aff);
            str.Append(element.Icon).Append(aff.Format()).Append("/c[white]  ");
        }

        return str.ToString();
    }

    public void SetMult(Mult mult, uint set) {
        switch (mult) {
            case Mult.DmgDealt: this.MultDmgDealt = set; break;
            case Mult.DmgTaken: this.MultDmgTaken = set; break;
            case Mult.IgnisDmgDealt: this.MultElementDmgDealt.Add(Elements.Ignis, set); break;
            case Mult.IgnisDmgTaken: this.MultElementDmgTaken.Add(Elements.Ignis, set); break;
            case Mult.GlaciesDmgDealt: this.MultElementDmgDealt.Add(Elements.Glacies, set); break;
            case Mult.GlaciesDmgTaken: this.MultElementDmgTaken.Add(Elements.Glacies, set); break;
            case Mult.FulgurDmgDealt: this.MultElementDmgDealt.Add(Elements.Fulgur, set); break;
            case Mult.FulgurDmgTaken: this.MultElementDmgTaken.Add(Elements.Fulgur, set); break;
            case Mult.VentusDmgDealt: this.MultElementDmgDealt.Add(Elements.Ventus, set); break;
            case Mult.VentusDmgTaken: this.MultElementDmgTaken.Add(Elements.Ventus, set); break;
            case Mult.TerraDmgDealt: this.MultElementDmgDealt.Add(Elements.Terra, set); break;
            case Mult.TerraDmgTaken: this.MultElementDmgTaken.Add(Elements.Terra, set); break;
            case Mult.LuxDmgDealt: this.MultElementDmgDealt.Add(Elements.Lux, set); break;
            case Mult.LuxDmgTaken: this.MultElementDmgTaken.Add(Elements.Lux, set); break;
            case Mult.MalumDmgDealt: this.MultElementDmgDealt.Add(Elements.Malum, set); break;
            case Mult.MalumDmgTaken: this.MultElementDmgTaken.Add(Elements.Malum, set); break;
            case Mult.WeakDmgDealt: this.MultWeakDmgDealt = set; break;
            case Mult.WeakDmgTaken: this.MultWeakDmgTaken = set; break;
            case Mult.FollowUpDmgDealt: this.MultFollowUpDmgDealt = set; break;
            case Mult.FollowUpDmgTaken: this.MultFollowUpDmgTaken = set; break;
            case Mult.DotDmgTaken: this.MultDoTDmgTaken = set; break;
            case Mult.PercentageDmgTaken: this.MultPercentageDmgTaken = set; break;
            case Mult.HealingDealt: this.MultHealingDealt = set; break;
            case Mult.HealingTaken: this.MultHealingTaken = set; break;
            case Mult.SpGain: this.MultSpGain = set; break;
            case Mult.SpUse: this.MultSpUse = set; break;
        }
    }

    public uint GetMult(Mult mult) => mult switch {
        Mult.DmgDealt => this.MultDmgDealt,
        Mult.DmgTaken => this.MultDmgTaken,
        Mult.IgnisDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Ignis, 1000u),
        Mult.IgnisDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Ignis, 1000u),
        Mult.GlaciesDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Glacies, 1000u),
        Mult.GlaciesDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Glacies, 1000u),
        Mult.FulgurDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Fulgur, 1000u),
        Mult.FulgurDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Fulgur, 1000u),
        Mult.VentusDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Ventus, 1000u),
        Mult.VentusDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Ventus, 1000u),
        Mult.TerraDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Terra, 1000u),
        Mult.TerraDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Terra, 1000u),
        Mult.LuxDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Lux, 1000u),
        Mult.LuxDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Lux, 1000u),
        Mult.MalumDmgDealt => this.MultElementDmgDealt.GetValueOrDefault(Elements.Malum, 1000u),
        Mult.MalumDmgTaken => this.MultElementDmgTaken.GetValueOrDefault(Elements.Malum, 1000u),
        Mult.WeakDmgDealt => this.MultWeakDmgDealt,
        Mult.WeakDmgTaken => this.MultWeakDmgTaken,
        Mult.FollowUpDmgDealt => this.MultFollowUpDmgDealt,
        Mult.FollowUpDmgTaken => this.MultFollowUpDmgTaken,
        Mult.DotDmgTaken => this.MultDoTDmgTaken,
        Mult.PercentageDmgTaken => this.MultPercentageDmgTaken,
        Mult.HealingDealt => this.MultHealingDealt,
        Mult.HealingTaken => this.MultHealingTaken,
        Mult.SpGain => this.MultSpGain,
        Mult.SpUse => this.MultSpUse
    };

    public void SetBooleanStat(BooleanStat stat, uint set) {
        switch (stat) {
            case BooleanStat.EffectBlock:
                this.EffectBlock = set; break;
            case BooleanStat.InfiniteSp:
                this.InfiniteSp = set; break;
            case BooleanStat.UnableToAct:
                this.UnableToAct = set; break;
            case BooleanStat.UnableToActImmunity:
                this.UnableToActImmunity = set; break;
            case BooleanStat.EquipDisabled:
                this.EquipDisabled = set; break;
            case BooleanStat.EquipDisabledImmunity:
                this.EquipDisabledImmunity = set; break;
        }
    }

    public uint GetBooleanStat(BooleanStat stat) => stat switch {
        BooleanStat.EffectBlock => this.EffectBlock,
        BooleanStat.InfiniteSp => this.InfiniteSp,
        BooleanStat.UnableToAct => this.UnableToAct,
        BooleanStat.UnableToActImmunity => this.UnableToActImmunity,
        BooleanStat.EquipDisabled => this.EquipDisabled,
        BooleanStat.EquipDisabledImmunity => this.EquipDisabledImmunity
    };

    public bool IsBooleanStat(BooleanStat stat) {
        return stat switch {
            BooleanStat.EffectBlock => this.EffectBlock > 0,
            BooleanStat.InfiniteSp => this.InfiniteSp > 0,
            BooleanStat.UnableToAct => (this.UnableToAct > 0) && (this.UnableToActImmunity <= 0),
            BooleanStat.UnableToActImmunity => this.UnableToActImmunity > 0,
            BooleanStat.EquipDisabled => (this.EquipDisabled > 0) && (this.EquipDisabledImmunity <= 0),
            BooleanStat.EquipDisabledImmunity => this.EquipDisabledImmunity > 0
        };
    }

    public bool IsImmuneToBooleanStat(BooleanStat stat) => stat switch {
        BooleanStat.UnableToAct => this.UnableToActImmunity > 0,
        BooleanStat.EquipDisabled => this.EquipDisabledImmunity > 0,
        _ => false
    };

    public int GetDurationModBuffTypeDealt(BuffType buffType) =>
        buffType == BuffType.Buff ? this.ModDurationBuffDealt : this.ModDurationDebuffDealt;

    public int GetDurationModBuffTypeTaken(BuffType buffType) =>
        buffType == BuffType.Buff ? this.ModDurationBuffTaken : this.ModDurationDebuffTaken;

    public int GetStacksModBuffTypeDealt(BuffType buffType) =>
        buffType == BuffType.Buff ? this.ModStacksBuffDealt : this.ModStacksDebuffDealt;

    public int GetStacksModBuffTypeTaken(BuffType buffType) =>
        buffType == BuffType.Buff ? this.ModStacksBuffTaken : this.ModStacksDebuffTaken;

    public void SetStatMod(StatMod statMod, int set) {
        switch (statMod) {
            case StatMod.DurationBuffDealt: this.ModDurationBuffDealt = set; break;
            case StatMod.DurationBuffTaken: this.ModDurationBuffTaken = set; break;
            case StatMod.DurationDebuffDealt: this.ModDurationDebuffDealt = set; break;
            case StatMod.DurationDebuffTaken: this.ModDurationDebuffTaken = set; break;
            case StatMod.StacksBuffDealt: this.ModStacksBuffDealt = set; break;
            case StatMod.StacksBuffTaken: this.ModStacksBuffTaken = set; break;
            case StatMod.StacksDebuffDealt: this.ModStacksDebuffDealt = set; break;
            case StatMod.StacksDebuffTaken: this.ModStacksDebuffTaken = set; break;
            case StatMod.Range: this.ModRange = set; break;
        }
    }

    public int GetStatMod(StatMod statMod) => statMod switch {
        StatMod.DurationBuffDealt => this.ModDurationBuffDealt,
        StatMod.DurationBuffTaken => this.ModDurationBuffTaken,
        StatMod.DurationDebuffDealt => this.ModDurationDebuffDealt,
        StatMod.DurationDebuffTaken => this.ModDurationDebuffTaken,
        StatMod.StacksBuffDealt => this.ModStacksBuffDealt,
        StatMod.StacksBuffTaken => this.ModStacksBuffTaken,
        StatMod.StacksDebuffDealt => this.ModStacksDebuffDealt,
        StatMod.StacksDebuffTaken => this.ModStacksDebuffTaken,
        StatMod.Range => this.ModRange
    };

    public bool IsWeakTo(Element element) {
        this.Affinities.TryGetValue(element, out int value);
        return value < 0;
    }

    public bool Resists(Element element) {
        this.Affinities.TryGetValue(element, out int value);
        return value > 0;
    }

    public bool IsImmuneTo(Element element) {
        this.Affinities.TryGetValue(element, out int value);
        return value >= 5;
    }

    public bool IsNeutralTo(Element element) {
        this.Affinities.TryGetValue(element, out int value);
        return value == 0;
    }
    
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

    public void OnUseSkill(Unit target, Skill skill) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnUseSkill(s, t, stacks, skill));
    }

    public void OnTargetedBySkill(Unit target, Skill skill) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnTargetedBySkill(s, t, stacks, skill));
    }

    public void OnDealDamage(Unit target, uint damage, Element element) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnDealDamage(s, t, stacks, damage, element));
    }

    public void OnTakeDamage(Unit target, uint damage, Element element) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnDealDamage(s, t, stacks, damage, element));
    }

    public void OnDealHeal(Unit target, uint heal, uint overheal) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnDealHeal(s, t, stacks, heal, overheal));
    }

    public void OnTakeHeal(Unit target, uint heal, uint overHeal) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnTakeHeal(s, t, stacks, heal, overHeal));
    }

    public void OnDealShield(Unit target, uint turns, uint heal) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnDealShield(s, t, stacks, turns, heal));
    }

    public void OnTakeShield(Unit target, uint turns, uint heal) {
        NotifyBuffEffects(target, (effect, s, t, stacks) => effect.OnTakeShield(s, t, stacks, turns, heal));
    }

    public void OnGiveBuff(Unit target, Buff buff, uint turnsMod, int stacksChange) {
        NotifyBuffEffects(target,
            (effect, s, t, stacks) => effect.OnGiveBuff(s, t, stacks, buff, turnsMod, stacksChange));
    }

    public void OnChangeStage(Unit target, StageType stageType, uint turnsMod, int stacksChange) {
        NotifyBuffEffects(target,
            (effect, s, t, stacks) => effect.OnChangeStage(s, t, stacks, stageType, turnsMod, stacksChange));
    }

    public Side GetSide() => this.Pos < 4 ? Side.Ally : Side.Opponent;

    public void DecrementTurns() {
        // Stages
        if ((this.StageAtk != 0) && (--this.StageAtkTurns <= 0)) {
            // todo stringformat
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseStage, this.UnitType.FormatName(this.Pos, false),
                this.StageAtk, this.StageAtk.Format(),
                StageTypes.Atk.Icon + Colors.Buff + StageTypes.Atk.Name, this.GetStageStatString(StageTypes.Atk, 0)));
            this.StageAtk = 0;
        }

        if ((this.StageDef != 0) && (--this.StageDefTurns <= 0)) {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseStage, this.UnitType.FormatName(this.Pos, false),
                this.StageDef, this.StageDef.Format(), StageTypes.Def.Icon + Colors.Buff + StageTypes.Def.Name,
                this.GetStageStatString(StageTypes.Def, 0)));
            this.StageDef = 0;
        }

        if ((this.StageFth != 0) && (--this.StageFthTurns <= 0)) {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseStage, this.UnitType.FormatName(this.Pos, false),
                this.StageFth, this.StageFth.Format(), StageTypes.Fth.Icon + Colors.Buff + StageTypes.Fth.Name,
                this.GetStageStatString(StageTypes.Fth, 0)));
            this.StageFth = 0;
        }

        if ((this.StageAgi != 0) && (--this.StageAgiTurns <= 0)) {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseStage, this.UnitType.FormatName(this.Pos, false),
                this.StageAgi, this.StageAgi.Format(), StageTypes.Agi.Icon + Colors.Buff + StageTypes.Agi.Name,
                this.GetStageStatString(StageTypes.Agi, 0)));
            this.StageAgi = 0;
        }

        // Buffs
        // Iterate backwards so they can be removed
        for (int i = this.BuffInstances.Count - 1; i >= 0; i--) {
            BuffInstance buffInstance = this.BuffInstances[i];
            uint turns = buffInstance.Turns;

            // 1000+ turns == infinite
            if (turns is >= 2 and < 1000) {
                buffInstance.Turns = turns - 1;
            } else {
                BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseBuff, this.UnitType.FormatName(this.Pos, false), 
                    buffInstance.Buff.MaxStacks, Colors.Num + buffInstance.Stacks, buffInstance.Buff.Icon + Colors.Buff 
                    + buffInstance.Buff.Name, string.Format(Lang.LogStackS, buffInstance.Stacks)));

                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                    buffEffect.OnRemove(this, buffInstance.Stacks);
                }

                this.BuffInstances.RemoveAt(i);
            }
        }

        // Skill cooldowns
        foreach (SkillInstance skillInstance in this.SkillInstances) {
            skillInstance.Cooldown--;
        }
    }

    public string GetStageStatString(StageType stageType, int stageNew) => "todo";
    
    // todo: Damage, GetMult/Mods/Other/BooleanStatsString
}