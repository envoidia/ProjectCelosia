using System;

namespace API.Battle;

public enum Mult {
    DmgDealt,
    DmgTaken,
    IgnisDmgDealt,
    IgnisDmgTaken,
    GlaciesDmgDealt,
    GlaciesDmgTaken,
    FulgurDmgDealt,
    FulgurDmgTaken,
    VentusDmgDealt,
    VentusDmgTaken,
    TerraDmgDealt,
    TerraDmgTaken,
    LuxDmgDealt,
    LuxDmgTaken,
    MalumDmgDealt,
    MalumDmgTaken,
    WeakDmgDealt,
    WeakDmgTaken,
    FollowUpDmgDealt,
    FollowUpDmgTaken,
    DotDmgTaken,
    PercentageDmgTaken,
    HealingDealt,
    HealingTaken,
    SpGain,
    SpUse
}

public static class MultExtensions {
    private record MultData(string Name, bool Positive);

    private static readonly MultData[] Data = CreateData();

    private static MultData[] CreateData() {
        Mult[] values = Enum.GetValues<Mult>();
        MultData[] data = new MultData[values.Length];

        for (int i = 0; i < values.Length; i++) {
            data[i] = values[i] switch {
                Mult.DmgDealt => new MultData(Lang.MultDmgDealt, true),
                Mult.DmgTaken => new MultData(Lang.MultDmgTaken, false),
                Mult.IgnisDmgDealt => new MultData(Lang.MultIgnisDmgDealt, true),
                Mult.IgnisDmgTaken => new MultData(Lang.MultIgnisDmgTaken, false),
                Mult.GlaciesDmgDealt => new MultData(Lang.MultGlaciesDmgDealt, true),
                Mult.GlaciesDmgTaken => new MultData(Lang.MultGlaciesDmgTaken, false),
                Mult.FulgurDmgDealt => new MultData(Lang.MultFulgurDmgDealt, true),
                Mult.FulgurDmgTaken => new MultData(Lang.MultFulgurDmgTaken, false),
                Mult.VentusDmgDealt => new MultData(Lang.MultVentusDmgDealt, true),
                Mult.VentusDmgTaken => new MultData(Lang.MultVentusDmgTaken, false),
                Mult.TerraDmgDealt => new MultData(Lang.MultTerraDmgDealt, true),
                Mult.TerraDmgTaken => new MultData(Lang.MultTerraDmgTaken, false),
                Mult.LuxDmgDealt => new MultData(Lang.MultLuxDmgDealt, true),
                Mult.LuxDmgTaken => new MultData(Lang.MultLuxDmgTaken, false),
                Mult.MalumDmgDealt => new MultData(Lang.MultMalumDmgDealt, true),
                Mult.MalumDmgTaken => new MultData(Lang.MultMalumDmgTaken, false),
                Mult.WeakDmgDealt => new MultData(Lang.MultWeakDmgDealt, true),
                Mult.WeakDmgTaken => new MultData(Lang.MultWeakDmgTaken, false),
                Mult.FollowUpDmgDealt => new MultData(Lang.MultFollowUpDmgDealt, true),
                Mult.FollowUpDmgTaken => new MultData(Lang.MultFollowUpDmgTaken, false),
                Mult.DotDmgTaken => new MultData(Lang.MultDotDmgTaken, false),
                Mult.PercentageDmgTaken => new MultData(Lang.MultPercentageDmgTaken, false),
                Mult.HealingDealt => new MultData(Lang.MultHealingDealt, true),
                Mult.HealingTaken => new MultData(Lang.MultHealingTaken, true),
                Mult.SpGain => new MultData(Lang.MultSpGain, true),
                Mult.SpUse => new MultData(Lang.MultSpUse, false)
            };
        }

        return data;
    }

    extension(Mult mult) {
        public string GetName() => Data[(int) mult].Name;

        public bool IsPositive() => Data[(int) mult].Positive;
    }
}