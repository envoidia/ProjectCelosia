using System;

namespace API.Battle;

public enum StatMod {
    DurationBuffDealt,
    DurationBuffTaken,
    DurationDebuffDealt,
    DurationDebuffTaken,
    StacksBuffDealt,
    StacksBuffTaken,
    StacksDebuffDealt,
    StacksDebuffTaken,
    Range
}

public static class StatModExtensions {
    private record StatModData(string Name, bool Positive);

    private static readonly StatModData[] Data = CreateData();

    private static StatModData[] CreateData() {
        StatMod[] values = Enum.GetValues<StatMod>();
        StatModData[] data = new StatModData[values.Length];

        for (int i = 0; i < values.Length; i++) {
            data[i] = values[i] switch {
                StatMod.DurationBuffDealt => new StatModData(Lang.ModDurationBuffDealt, true),
                StatMod.DurationBuffTaken => new StatModData(Lang.ModDurationBuffTaken, true),
                StatMod.DurationDebuffDealt => new StatModData(Lang.ModDurationDebuffDealt, true),
                StatMod.DurationDebuffTaken => new StatModData(Lang.ModDurationBuffTaken, false),
                StatMod.StacksBuffDealt => new StatModData(Lang.ModStacksBuffDealt, true),
                StatMod.StacksBuffTaken => new StatModData(Lang.ModStacksBuffTaken, true),
                StatMod.StacksDebuffDealt => new StatModData(Lang.ModStacksDebuffDealt, true),
                StatMod.StacksDebuffTaken => new StatModData(Lang.ModStacksBuffTaken, false),
                StatMod.Range => new StatModData(Lang.ModRange, true)
            };
        }

        return data;
    }

    extension(StatMod statMod) {
        public string GetName() => Data[(int) statMod].Name;

        public bool IsPositive() => Data[(int) statMod].Positive;
    }
}