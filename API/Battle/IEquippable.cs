namespace API.Battle;

public interface IEquippable {
    void Apply(Unit unit, bool giving);

    void Equip(Unit unit) {
        this.Apply(unit, true);
    }

    void Unequip(Unit unit) {
        this.Apply(unit, false);
    }
}