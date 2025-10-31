using System;

namespace API.Battle;

public enum BooleanStat {
    EffectBlock,
    InfiniteSp,
    UnableToAct,
    UnableToActImmunity,
    EquipDisabled,
    EquipDisabledImmunity
}

public static class BooleanStatExtensions {
    private record BooleanStatData(
        string Name,
        string LogMsgLangId,
        bool Positive,
        bool PossessiveNameInLogMsg,
        bool Visible);

    private static readonly BooleanStatData[] Data = CreateData();

    private static BooleanStatData[] CreateData() {
        BooleanStat[] values = Enum.GetValues<BooleanStat>();
        BooleanStatData[] data = new BooleanStatData[values.Length];

        for (int i = 0; i < values.Length; i++) {
            data[i] = values[i] switch {
                BooleanStat.EffectBlock => new BooleanStatData(Lang.BoolEffectBlock,
                    Lang.LogChangeBooleanStatEffectBlock, true, false, true),
                BooleanStat.InfiniteSp => new BooleanStatData(Lang.BoolInfiniteSp, Lang.LogChangeBooleanStatInfiniteSp,
                    true, true, true),
                BooleanStat.UnableToAct => new BooleanStatData(Lang.BoolUnableToAct,
                    Lang.LogChangeBooleanStatUnableToAct, false, false, true),
                BooleanStat.UnableToActImmunity => new BooleanStatData(Lang.BoolUnableToActImmunity,
                    Lang.LogChangeBooleanStatUnableToActImmune, true, false, false),
                BooleanStat.EquipDisabled => new BooleanStatData(Lang.BoolEquipDisabled,
                    Lang.LogChangeBooleanStatEquipDisabled, false, true, true),
                BooleanStat.EquipDisabledImmunity => new BooleanStatData(Lang.BoolEquipDisabledImmunity,
                    Lang.LogChangeBooleanStatEquipDisabledImmune, true, false, false)
            };
        }

        return data;
    }

    extension(BooleanStat booleanStat) {
        public string GetName() => Data[(int) booleanStat].Name;

        public string GetLogMsgLangId() => Data[(int) booleanStat].LogMsgLangId;

        public bool IsPositive() => Data[(int) booleanStat].Positive;

        public bool IsPossessiveNameInLogMsg() => Data[(int) booleanStat].PossessiveNameInLogMsg;

        public bool IsVisible() => Data[(int) booleanStat].Visible;
    }
}