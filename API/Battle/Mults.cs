namespace API.Battle;

public static class Mults
{
    public static readonly Mult DmgDealt = new(Core.Id, "MultDmgDealt", true);
    public static readonly Mult DmgTaken = new(Core.Id, "MultDmgTaken", false);
    public static readonly Mult WeakDmgDealt = new(Core.Id, "MultWeakDmgDealt", true);
    public static readonly Mult WeakDmgTaken = new(Core.Id, "MultWeakDmgTaken", false);
    public static readonly Mult FollowUpDmgDealt = new(Core.Id, "MultFollowUpDmgDealt", true);
    public static readonly Mult FollowUpDmgTaken = new(Core.Id, "MultFollowUpDmgTaken", false);
    public static readonly Mult DoTDmgTaken = new(Core.Id, "MultDoTDmgTaken", false);
    public static readonly Mult HealingDealt = new(Core.Id, "MultHealingDealt", true);
    public static readonly Mult HealingTaken = new(Core.Id, "MultHealingTaken", true);
    public static readonly Mult SpGain = new(Core.Id, "MultSpGain", true);
    public static readonly Mult SpUse = new(Core.Id, "MultSpUse", false);

    public static readonly Mult PercentageDmgTaken = new(Core.Id, "MultPercentageDmgTaken", false)
    {
        MinValue = 1
    };
}
