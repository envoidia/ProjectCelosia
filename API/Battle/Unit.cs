using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.State;
using API.Debug;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using API.Util;
using MonoGame.Extended.Collections;

namespace API.Battle;

public sealed class Unit
{
    #region Fields

    public UnitType UnitType { get; }
    public int Lvl { get; }
    public int Hp;

    public const int StartingSp = 200;
    public const int MaxSp = 1000;
    public int Sp = StartingSp;

    /// <summary>
    /// Position on the battlefield.
    /// 0 4 /
    /// 1 5 /
    /// 2 6 /
    /// 3 7
    /// </summary>
    public int Pos;

    /// <summary>
    /// If there's more than 1 Unit with this UnitType in the current battle, disambiguates which one this is.
    /// 0 = There are no duplicates
    /// </summary>
    public int DupeIndex = 0;

    /// <summary>
    /// Shield is hit before HP, and Defend before Shield. Together they cannot exceed max HP
    /// </summary>
    public int Shield = 0;

    /// <summary>
    /// Shield is hit before HP, and Defend before Shield. Together they cannot exceed max HP
    /// </summary>
    public int Defend = 0;

    public List<SkillInstance> SkillInstances { get; }
    public List<BuffInstance> BuffInstances { get; } = [];
    public List<Passive> Passives { get; }

    // Stats
    private readonly Dictionary<Stat, int> _Stats;

    public const int StartingStatMult = 1000;
    public const int MinStatMult = 100;

    /// <summary>
    /// Treated as multipliers applied to _stats, in 10ths of a % (<c>StartingStatMult</c> = 100%), min 10%
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
    public IEquippable? Equipped
    {
        get;
        set
        {
            field?.Unequip(this);
            field = value;
            field?.Equip(this);
        }
    }

    public int ExtraActions = 0;

    #endregion

    public Unit(UnitType unitType, int lvl, IEquippable? equipped, params Skill[] skills)
    {
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

    #region Add/Remove

    // todo add non params versions
    public void AddSkills(params ReadOnlySpan<Skill> skills)
    {
        foreach (Skill skill in skills)
        {
            this.SkillInstances.Add(new SkillInstance(skill));
        }
    }

    /// <returns>
    /// Whether any of the supplied <c>Skills</c> were removed successfully
    /// </returns>
    // todo fix cooldown error
    public bool RemoveSkills(params ReadOnlySpan<Skill> skills)
    {
        bool removedAny = false;

        foreach (Skill skill in skills)
        {
            if (this.SkillInstances.Remove(new SkillInstance(skill)))
            {
                removedAny = true;
            }
        }

        return removedAny;
    }

    public void AddPassives(params ReadOnlySpan<Passive> passives)
    {
        foreach (Passive passive in passives)
        {
            this.Passives.Add(passive);

            foreach (IBuffEffect buffEffect in passive.BuffEffects)
            {
                buffEffect.OnGive(this, 1);
            }
        }
    }

    /// <returns>
    /// Whether any of the supplied <c>Passives</c> were removed successfully
    /// </returns>
    public bool RemovePassives(params ReadOnlySpan<Passive> passives)
    {
        bool removedAny = false;

        foreach (Passive passive in passives)
        {
            if (!this.Passives.Remove(passive))
            {
                continue;
            }

            foreach (IBuffEffect buffEffect in passive.BuffEffects)
            {
                buffEffect.OnRemove(this, 1);
            }

            removedAny = true;
        }

        return removedAny;
    }

    public void GiveBuffInstances(params ReadOnlySpan<BuffInstance> buffInstances)
    {
        foreach (BuffInstance buffInstance in buffInstances)
        {
            this.BuffInstances.Add(buffInstance);

            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects)
            {
                buffEffect.OnGive(this, buffInstance.Stacks);
            }
        }
    }

    /// <returns>
    /// Whether any of the supplied <c>Buffs</c> were removed successfully
    /// </returns>
    public bool RemoveBuffs(params ReadOnlySpan<Buff> buffs)
    {
        bool removedAny = false;

        foreach (Buff buff in buffs)
        {
            BuffInstance? buffInstance = this.BuffInstances.FirstOrDefault(bi => bi.Buff == buff);
            if (buffInstance is null)
            {
                continue;
            }

            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects)
            {
                buffEffect.OnRemove(this, 1);
            }

            this.BuffInstances.Remove(buffInstance);

            removedAny = true;
        }

        return removedAny;
    }

    #endregion

    #region Stats

    public int GetStatWithStage(Stat stat, int stage)
    {
        if (stat == Stats.Hp)
        {
            return this.Hp;
        }

        return (int) (this._Stats.GetValueOrDefault(stat, 0) *
               (Math.Max(this._StatsMult.GetValueOrDefault(stat, 1000), 100) / 1000f) *
               (1 + (stage / 10 / (stage < 0 ? 2 : 1))));
    }

    public int GetStat(Stat stat)
    {
        return this.GetStatWithStage(stat, this.GetStage(stat.StageType));
    }

    public int GetBaseStat(Stat stat)
    {
        return this._Stats.GetValueOrDefault(stat, 0);
    }

    public int GetStatMult(Stat stat)
    {
        return this._StatsMult.GetValueOrDefault(stat, 1000);
    }

    public void SetStatMult(Stat stat, int set)
    {
        this._StatsMult[stat] = set;
    }

    public string GetDbgStatsString()
    {
        const int Len = 128;
        StringBuilder str = new(Len);

        foreach (Stat s in Registry.Of<Stat>())
        {
            str.Append($"{s.GetName()}: {this.GetStat(s)}/{this.GetBaseStat(s)} (mult {this.GetStatMult(s)})\n");
        }

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetDbgStatsString));
        }

        return str.ToString();
    }

    #endregion

    #region Affinities

    public int GetAffinity(Element element)
    {
        return this._Affinities.GetValueOrDefault(element, 0);
    }

    public void SetAffinity(Element element, int set)
    {
        this._Affinities[element] = set;
    }

    public bool IsWeakTo(Element element)
    {
        return this._Affinities.GetValueOrDefault(element, 0) < 0;
    }

    public bool Resists(Element element)
    {
        return this._Affinities.GetValueOrDefault(element, 0) > 0;
    }

    public bool IsImmuneTo(Element element)
    {
        return this._Affinities.GetValueOrDefault(element, 0) >= 5;
    }

    public bool IsNeutralTo(Element element)
    {
        return this._Affinities.GetValueOrDefault(element, 0) == 0;
    }

    private static readonly int[] _CoreDmgDealt = [300, 500, 650, 800, 900, 1000, 1100, 1200, 1350, 1500, 1700];
    public int GetElementDmgDealt(Element element)
    {
        return _Extrapolate(this.GetAffinity(element), _CoreDmgDealt, 200, -200);
    }

    private static readonly int[] _CoreDmgTaken = [2500, 2000, 1700, 1400, 1200, 1000, 900, 800, 650, 500, 0];
    public int GetElementDmgTaken(Element element)
    {
        return _Extrapolate(this.GetAffinity(element), _CoreDmgTaken, 0, 500);
    }

    private static readonly int[] _CoreSpCost = [1700, 1500, 1300, 1200, 1100, 1000, 950, 900, 850, 800, 750];
    public int GetElementSpCost(Element element)
    {
        return _Extrapolate(this.GetAffinity(element), _CoreSpCost, -50, 200);
    }

    private static int _Extrapolate(int i, int[] core, int stepUp, int stepDown)
    {
        // Real index
        int index = i + 5;

        int value;

        // In bounds
        if (index >= 0 && index < core.Length)
        {
            value = core[index];
        }

        // Above bounds
        else if (i >= core.Length)
        {
            value = core[^1] + ((stepUp * index) - (core.Length - 1));
        }

        // Below bounds
        else
        {
            value = core[0] + (stepDown * Math.Abs(index));
        }

        // Max to 0
        return Math.Max(value, 0);
    }

    /// <returns>
    /// Adjusted SP/Bloom cost of the given <c>Skill</c>
    /// </returns>
    public int GetCost(Skill s)
    {
        int cost = this.IsBoolStat(BoolStats.InfiniteSp) && !s.IsBloom ? 0 : s.Cost;

        // Make sure cost doesn't go below 1 unless the skill has a base 0 SP cost
        return cost > 0 ? (int) Math.Max(cost * (this.GetElementSpCost(s.GetElement()) / 1000d), 1) : 0;
    }

    /// <param name="isCurrent">Whether to compare to base</param>
    /// <returns>Affinities of this formatted readably</returns>
    public string GetAffinitiesString(bool isCurrent)
    {
        Dictionary<Element, int> affs = isCurrent ? this._Affinities : this.UnitType._Affinities;

        const int Len = 532;
        StringBuilder sb = new($"{ThemeColor.Stat.Str}{"Affinities".GetLang()}:{ThemeColor.White.Str} ", Len);
        foreach (Element element in Registry.Of<Element>().Where(e => e.IsVisible))
        {
            sb.Append($"{element.Icon} {affs.GetValueOrDefault(element, 0).Format()}");

            if (isCurrent)
            {
                sb.Append($"{ThemeColor.White.Str}//{this.UnitType._Affinities.GetValueOrDefault(element,
                    0).Format()}");
            }

            sb.Append("   ");
        }

        Assert.CapIs(sb, Len); // todo remove before final release
        return sb.ToString();
    }

    #endregion

    #region Stages

    public int GetStage(StageType? stageType)
    {
        if (stageType is null)
        {
            return 0;
        }

        return this._Stages.GetValueOrDefault(stageType, 0);
    }
    public void SetStage(StageType stageType, int set)
    {
        this._Stages[stageType] = set;
    }

    public int GetStageTurns(StageType stageType)
    {
        return this._StageTurns.GetValueOrDefault(stageType, 0);
    }

    public void SetStageTurns(StageType stageType, int set)
    {
        this._StageTurns[stageType] = set;
    }

    public string GetTurnsStacksFormatted(StageType stageType)
    {
        return $"{this.GetStage(stageType).Format()}({this.GetStageTurns(stageType)})";
    }

    public string GetStageStatString(StageType stageType, int stageNew)
    {
        StringBuilder str = new();
        str.Append(ThemeColor.White.Str).Append(" (");
        int statCount = stageType.Stats.Length;

        for (int i = 0; i < statCount; i++)
        {
            Stat stat = stageType.Stats[i];
            int statDefault = this._Stats[stat];
            int statOld = this.GetStat(stat);
            int statNew = this.GetStatWithStage(stat, stageNew);
            int change = statNew - statOld;

            str.Append("LogStageStat".FormatLang([this.FormatName(), stat.GetName(),
                TextLib.FormatStat(statOld, statDefault), TextLib.FormatStat(statNew, statDefault),
                statDefault.Format(ThemeColor.Imp), change.Format()]));

            str.Append(i == (statCount - 1) ? ")" : ", ");
        }

        Assert.Unreachable("init the sb size");
        return str.ToString();
    }

    public string GetDbgStagesString()
    {
        const int Len = 128;
        StringBuilder str = new(Len);

        foreach (StageType st in Registry.Of<StageType>())
        {
            str.Append($"{st.GetName()}: {this.GetStage(st)}, {this.GetStageTurns(st)} turns");
        }

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetDbgStagesString));
        }

        return str.ToString();
    }

    #endregion

    #region Mults

    public float GetMult(Mult mult)
    {
        return this._Mults.GetValueOrDefault(mult, 1000) / 1000f;
    }

    public int GetRawMult(Mult mult)
    {
        return this._Mults.GetValueOrDefault(mult, 1000);
    }

    public void SetMult(Mult mult, int set)
    {
        this._Mults[mult] = set; // todo ensure these dont crash
    }

    public string GetDbgMultsString()
    {
        const int Len = 512;
        StringBuilder str = new(Len);
        foreach (Mult mult in Registry.Of<Mult>())
        {
            int curMult = this.GetRawMult(mult);
            str.Append($"{mult.GetName()}: {mult.Format(curMult)}\n");
        }

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetDbgMultsString));
        }

        return str.ToString();
    }

    #endregion

    #region BoolStats

    public int GetBoolStat(BoolStat boolStat)
    {
        return this._BoolStats.GetValueOrDefault(boolStat, 1000);
    }

    public void SetBoolStat(BoolStat boolStat, int set)
    {
        this._BoolStats[boolStat] = set;
    }

    public bool IsBoolStat(BoolStat boolStat)
    {
        if (boolStat == BoolStats.EquipDisabled)
        {
            return (this._BoolStats.GetValueOrDefault(BoolStats.EquipDisabled, 0) > 0) &&
                   (this._BoolStats.GetValueOrDefault(BoolStats.EquipDisabledImmunity, 0) <= 0);
        }

        if (boolStat == BoolStats.UnableToAct)
        {
            return (this._BoolStats.GetValueOrDefault(BoolStats.UnableToAct, 0) > 0) &&
                   (this._BoolStats.GetValueOrDefault(BoolStats.UnableToActImmunity, 0) <= 0);
        }

        return this._BoolStats.GetValueOrDefault(boolStat, 0) > 0;
    }

    public bool IsImmuneToBoolStat(BoolStat boolStat)
    {
        return ((boolStat == BoolStats.UnableToAct) &&
        (this._BoolStats.GetValueOrDefault(BoolStats.UnableToActImmunity, 0) > 0)) ||
        ((boolStat == BoolStats.EquipDisabled) &&
        (this._BoolStats.GetValueOrDefault(BoolStats.EquipDisabledImmunity, 0) > 0));
    }

    public string GetOtherStatsString()
    {
        const int Len = 128;
        StringBuilder str = new(Len);
        foreach (BoolStat stat in Registry.Of<BoolStat>())
        {
            if (stat.IsVisible)
            {
                str.Append($"{this.GetBoolStatString(stat)}\n");
            }
        }

        str.Append(this.ExtraActions.Format());

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetOtherStatsString));
        }

        return str.ToString();
    }

    public string GetBoolStatString(BoolStat stat)
    {
        if (this.IsImmuneToBoolStat(stat))
        {
            return ThemeColor.Pos + "Immune".GetLang();
        }

        return (stat.IsPositive ? ThemeColor.Pos : ThemeColor.Neg) +
            (this.IsBoolStat(stat) ? "Yes".GetLang() : "No".GetLang());
    }

    public string GetDbgBoolStatsString()
    {
        const int Len = 128;
        StringBuilder str = new(Len);
        foreach (BoolStat stat in Registry.Of<BoolStat>())
        {
            str.Append($"{stat.GetName()}: {this.GetBoolStat(stat)}\n");
        }

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetDbgBoolStatsString));
        }

        return str.ToString();
    }

    #endregion

    #region StatMods

    public int GetStatMod(StatMod statMod)
    {
        return this._StatMods.GetValueOrDefault(statMod, 0);
    }

    public void SetStatMod(StatMod statMod, int set)
    {
        this._StatMods[statMod] = set;
    }

    public int GetDurationModBuffTypeDealt(BuffType buffType)
    {
        return this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.DurationBuffDealt : StatMods.DurationDebuffDealt,
            0);
    }

    public int GetDurationModBuffTypeTaken(BuffType buffType)
    {
        return this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.DurationBuffTaken : StatMods.DurationDebuffTaken,
            0);
    }

    public int GetStacksModBuffTypeDealt(BuffType buffType)
    {
        return this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.StacksBuffDealt : StatMods.StacksDebuffDealt, 0);
    }

    public int GetStacksModBuffTypeTaken(BuffType buffType)
    {
        return this._StatMods.GetValueOrDefault(
            buffType == BuffType.Buff ? StatMods.StacksBuffTaken : StatMods.StacksDebuffTaken, 0);
    }

    public string GetStatModsString()
    {
        const int Len = 128;
        StringBuilder str = new(Len);
        foreach (StatMod mod in Registry.Of<StatMod>())
        {
            str.Append($"{mod.Format(this.GetStatMod(mod))}\n");
        }

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetStatModsString));
        }

        return str.ToString();
    }

    public string GetDbgStatModsString()
    {
        const int Len = 128;
        StringBuilder str = new(Len);
        foreach (StatMod mod in Registry.Of<StatMod>())
        {
            str.Append($"{mod.GetName()}: {this.GetStatMod(mod)}\n");
        }

        // todo remove
        if (str.Length > Len)
        {
            DebugConsole.Log(str.Length.ToString(), nameof(GetDbgStatModsString));
        }

        return str.ToString();
    }

    #endregion

    #region BuffEffects

    private void _NotifyBuffEffects(Unit target, Action<IBuffEffect, Unit, Unit, int> notifier)
    {
        // Handle Passives
        foreach (Passive passive in this.Passives)
        {
            foreach (IBuffEffect buffEffect in passive.BuffEffects)
            {
                notifier(buffEffect, this, target, 1);
            }
        }

        // Handle Buffs
        foreach (BuffInstance buffInstance in this.BuffInstances)
        {
            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects)
            {
                notifier(buffEffect, this, target, buffInstance.Stacks);
            }
        }
    }

    public void OnUseSkill(Unit target, Skill skill)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnUseSkill(s, t, stacks, skill));
    }

    public void OnTargetedBySkill(Unit target, Skill skill)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTargetedBySkill(s, t, stacks, skill));
    }

    public void OnDealDamage(Unit target, int damage, Element element)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnDealDamage(s, t, stacks, damage, element));
    }

    public void OnTakeDamage(Unit target, int damage, Element? element = null)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTakeDamage(s, t, stacks, damage, element));
    }

    public void OnDealHeal(Unit target, int heal, int overheal)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnDealHeal(s, t, stacks, heal, overheal));
    }

    public void OnTakeHeal(Unit target, int heal, int overheal)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTakeHeal(s, t, stacks, heal, overheal));
    }

    public void OnDealShield(Unit target, int turns, int heal)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnDealShield(s, t, stacks, turns, heal));
    }

    public void OnTakeShield(Unit target, int turns, int heal)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnTakeShield(s, t, stacks, turns, heal));
    }

    public void OnGiveBuff(Unit target, Buff buff, int turns, int stacksChange)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnGiveBuff(s, t, stacks, buff, turns, stacksChange));
    }

    public void OnChangeStage(Unit target, StageType stageType, int turns, int stacksChange)
    {
        this._NotifyBuffEffects(target, (effect, s, t, stacks) =>
            effect.OnChangeStage(s, t, stacks, stageType, turns, stacksChange));
    }

    public Side GetSide()
    {
        return this.Pos < PosLib.LowestOpp ? Side.Ally : Side.Opponent;
    }

    #endregion

    #region Misc

    // todo weapon
    public string GetEquipString()
    {
        return $"{ThemeColor.Stat.Str}{"Accessory".GetLang()}:{ThemeColor.White.Str} {(this.Equipped as INameable)?
            .GetName() ?? "None".GetLang()}";
    }

    public void DecrementTurns()
    {
        // Stages
        foreach (StageType stageType in Registry.Of<StageType>())
        {
            int stage = this.GetStage(stageType);

            if (stage != 0 && --this._StageTurns[stageType] == 0)
            {
                LogLib.Add("LogLoseStage".IcuFormatLang([this.FormatName(false),
                    stage, stage.Format(), StageTypes.Atk.GetName(),
                    this.GetStageStatString(StageTypes.Atk, 0)]));

                this.SetStage(stageType, 0);
            }
        }

        // Buffs; iterate backwards so they can be removed
        for (int i = this.BuffInstances.Count - 1; i >= 0; i--)
        {
            BuffInstance buffInstance = this.BuffInstances[i];
            int turns = buffInstance.Turns;

            if (turns is >= 2 and < BuffInstance.InfiniteTurns)
            {
                buffInstance.Turns = turns - 1;
            }
            else
            {
                LogLib.Add("LogLoseBuff".IcuFormatLang([this.FormatName(false),
                    buffInstance.Buff.MaxStacks, ThemeColor.Imp + buffInstance.Stacks,
                    buffInstance.Buff.GetName(), buffInstance.Stacks]));

                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects)
                {
                    buffEffect.OnRemove(this, buffInstance.Stacks);
                }

                this.BuffInstances.RemoveAt(i);
            }
        }

        // Skill cooldowns
        foreach (SkillInstance skillInstance in this.SkillInstances)
        {
            if (skillInstance.Cooldown > 0)
            {
                skillInstance.Cooldown--;
            }
        }
    }

    public SkillResult Damage(int dmg, bool pierce = false, bool useName = true)
    {
        int dmgFull = dmg;
        int defendOld = this.Defend;
        List<string> msg = [];

        string name = "";
        string nameS = "";

        if (useName)
        {
            name = $"{this.FormatName(false)} ";
            nameS = $"{this.FormatName()} ";
        }

        // Pierce skips Defend and Shield
        if (!pierce)
        {
            if ((this.Defend > 0) && (dmg > 0))
            {
                // Only hit Defend
                if (this.Defend > dmg)
                {
                    this.Defend -= dmg;
                    return new SkillResult(SkillResultType.HitEffectBlock,
                        "LogChangeShield".FormatLang([nameS,
                        (defendOld + this.Shield).Format(ThemeColor.Shield),
                        (this.Defend + this.Shield).Format(ThemeColor.Shield),
                        this.GetStat(Stats.Hp).Format(ThemeColor.Hp), dmgFull.Format()]));
                }

                // Destroy Defend and proceed to Shield
                dmg -= this.Defend;
                this.Defend = 0;

                // todo this should come after the dmg message; is this needed now that shield is a buff
                if ((this.Shield == 0) && (this.GetBoolStat(BoolStats.EffectBlock) <= 0))
                {
                    msg.Add("LogChangeBooleanStatEffectBlock".FormatLang([name, 0]));
                }
            }

            if ((this.Shield > 0) && (dmg > 0))
            {
                // Only hit Shield
                if (this.Shield > dmg)
                {
                    int shieldOld = this.Shield;
                    this.Shield -= dmg;
                    return new SkillResult(SkillResultType.HitEffectBlock, "LogChangeShield"
                        .FormatLang([nameS, (defendOld + shieldOld)
                        .Format(ThemeColor.Shield), this.Shield.Format(ThemeColor.Shield),
                        this.GetBaseStat(Stats.Hp).Format(ThemeColor.Hp), (-dmgFull).Format()]));
                }

                // Destroy Shield and proceed to HP
                msg.Add("LogChangeShield".FormatLang([nameS, (defendOld + this.Shield)
                    .Format(ThemeColor.Shield), ThemeColor.Shield + 0, this.GetBaseStat(Stats.Hp)
                    .Format(ThemeColor.Hp), (-(defendOld + this.Shield)).Format()]));

                dmg -= this.Shield;
                this.Shield = 0;

                if (this.GetBoolStat(BoolStats.EffectBlock) <= 0)
                {
                    // todo is this needed
                    msg.Add("LogChangeBooleanStatEffectBlock".FormatLang([name, 0]));
                }
            }
        }

        int hpOld = this.Hp;
        this.Hp = Math.Clamp(this.Hp - dmg, 0, this._Stats[Stats.Hp]);
        int hpNew = this.Hp;

        msg.Add("LogChangeHp".FormatLang([nameS,
            hpOld.Format(ThemeColor.Hp, false), hpNew.Format(ThemeColor.Hp, false),
            this.GetBaseStat(Stats.Hp).Format(ThemeColor.Hp, false), (-dmg).Format()]));

        // todo should this be a separate result from hitting shield
        if (this.GetBoolStat(BoolStats.EffectBlock) > 0)
        {
            return new SkillResult(SkillResultType.HitEffectBlock, msg);
        }

        return dmg > 0
            ? new SkillResult(SkillResultType.Success, msg)
            : new SkillResult(SkillResultType.Fail, "LogNoEffect".FormatLang(name));
    }

    // todo support other langs (+ nicknames?)
    public string FormatName(bool possessive = true)
    {
        string name = this.UnitType.GetName(
            // Color
            this.GetSide() == Side.Ally ? ThemeColor.Ally : ThemeColor.Opp) +
            // Dupe disambiguation
            (this.DupeIndex == 0 ? "" : $" {this.DupeIndex}");

        string suffix = "";

        if (possessive)
        {
            suffix = name.ToUpperInvariant().EndsWith('S') ? "'" : "'s";
        }

        return name + suffix + ThemeColor.White.Str;
    }

    public string GetDbgInfo()
    {
        return $"{nameof(Unit)}: {this.FormatName(false)}\nHP: {this.Hp}/{this.GetBaseStat(Stats.Hp)}\nShield: {this.Shield}\nDefend: {this.Defend}\nSP: {this.Sp}\nStats: {this.GetDbgStatsString()}\nPos: {this.Pos}\nDupeIndex: {this.DupeIndex}\nSkillInstances: {string.Join('\n', this.SkillInstances.Select(si => si.ToString()))}\nBuffInstances: {string.Join('\n', this.BuffInstances.Select(bi => bi.ToString()))}\nAffinities: {this.GetAffinitiesString(true)}\nStages: {this.GetDbgStagesString()}\nMults: {this.GetDbgMultsString()}\nBoolStats: {this.GetDbgBoolStatsString()}\nStatMods: {this.GetDbgStatModsString()}\nEquipped: {this.GetEquipString()}\nExtraActions: {this.ExtraActions}";
    }

    #endregion
}