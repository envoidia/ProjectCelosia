namespace API.Battle.BuffEffects;

public interface IBuffEffect
{
    void OnGive(Unit self, int stacks) { }

    void OnRemove(Unit self, int stacks) { }

    string[] OnTurnEnd(Unit self, int stacks)
    {
        return [];
    }

    void OnUseSkill(Unit self, Unit target, int stacks, Skill skill) { }

    /// <param name="target">Skill user</param>
    void OnTargetedBySkill(Unit self, Unit target, int stacks, Skill skill) { }

    void OnDealDamage(Unit self, Unit target, int stacks, int damage, Element element) { }

    /// <param name="target">Attacker if there is one, otherwise self</param>
    void OnTakeDamage(Unit self, Unit target, int stacks, int damage, Element? element) { }

    void OnDealHeal(Unit self, Unit target, int stacks, int heal, int overheal) { }

    /// <param name="target">Healer if there is one, otherwise self</param>
    void OnTakeHeal(Unit self, Unit target, int stacks, int heal, int overheal) { }

    void OnDealShield(Unit self, Unit target, int stacks, int turns, int heal) { }

    /// <param name="target">Shielder if there is one, otherwise self</param>
    void OnTakeShield(Unit self, Unit target, int stacks, int turns, int heal) { }

    void OnGiveBuff(Unit self, Unit target, int stacks, Buff buff, int turns, int stacksChange) { }

    void OnChangeStage(Unit self, Unit target, int stacks, StageType stageType, int turns, int stacksChange) { }
}