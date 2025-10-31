using System.Security.Cryptography;
using API.Entity;

namespace API.Battle;

public class StageType : IconEntity {
    public StageTypeId Id { get; }
    public Stat[] Stats { get; }

    public StageType(string name, string desc, string icon, StageTypeId id, params Stat[] stats) : base(name, desc,
        icon) {
        this.Id = id;
        this.Stats = stats;
        Core.StageTypes.Add(this);
    }

    // todo unit public string GetTurnsStacksFormatted(Unit unit) => unit.GetStage(this).Format() + "(" + unit.GetStageTurns(this) + ")";

    public string GetNameWithIconAndSign(int stage) => this.GetNameWithIcon() + " " + (stage > 0 ? Lang.Up : Lang.Down);
}