namespace API.Battle.BuffEffects;

public interface IBuffEffect {
    void OnGive(Unit self, int stacks) { }

    void OnRemove(Unit self, int stacks) { }

    void OnUseSkill(Unit self, Unit target, int stacks, Skill skill) { }

    // target = skill user
    void OnTargetedBySkill(Unit self, Unit target, int stacks, Skill skill) { }

    string[] OnTurnEnd(Unit self, int stacks) => [];

    void OnDealDamage(Unit self, Unit target, int stacks, int damage, Element element) { }

    // target = attacker if there is one, otherwise target = self
    void OnTakeDamage(Unit self, Unit target, int stacks, int damage, Element? element) { }

    void OnDealHeal(Unit self, Unit target, int stacks, int heal, int overheal) { }

    // target = healer if there is one, otherwise target = self
    void OnTakeHeal(Unit self, Unit target, int stacks, int heal, int overheal) { }

    void OnDealShield(Unit self, Unit target, int stacks, int turns, int heal) { }

    // target = shielder if there is one, otherwise target = self
    void OnTakeShield(Unit self, Unit target, int stacks, int turns, int heal) { }

    void OnGiveBuff(Unit self, Unit target, int stacks, Buff buff, int turns, int stacksChange) { }

    void OnChangeStage(Unit self, Unit target, int stacks, StageType stageType, int turns, int stacksChange) { }
}