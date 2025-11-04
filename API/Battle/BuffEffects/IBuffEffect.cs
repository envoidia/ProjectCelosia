namespace API.Battle.BuffEffects;

public interface IBuffEffect {
    void OnGive(Unit self, uint stacks) { }

    void OnRemove(Unit self, uint stacks) { }

    void OnUseSkill(Unit self, Unit target, uint stacks, Skill skill) { }

    // target = skill user
    void OnTargetedBySkill(Unit self, Unit target, uint stacks, Skill skill) { }

    string[] OnTurnEnd(Unit self, uint stacks) => [];

    void OnDealDamage(Unit self, Unit target, uint stacks, uint damage, Element element) { }

    // target = attacker if there is one, otherwise target = self
    void OnTakeDamage(Unit self, Unit target, uint stacks, uint damage, Element? element) { }

    void OnDealHeal(Unit self, Unit target, uint stacks, uint heal, uint overHeal) { }

    // target = healer if there is one, otherwise target = self
    void OnTakeHeal(Unit self, Unit target, uint stacks, uint heal, uint overHeal) { }

    void OnDealShield(Unit self, Unit target, uint stacks, uint turns, uint heal) { }

    // target = shielder if there is one, otherwise target = self
    void OnTakeShield(Unit self, Unit target, uint stacks, uint turns, uint heal) { }

    void OnGiveBuff(Unit self, Unit target, uint stacks, Buff buff, uint turns, uint stacksChange) { }

    void OnChangeStage(Unit self, Unit target, uint stacks, StageType stageType, uint turns, int stacksChange) { }
}