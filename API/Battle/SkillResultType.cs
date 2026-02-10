namespace API.Battle;

public enum SkillResultType
{
    Fail,
    HitEffectBlock,
    Success,

    /// <summary>
    /// Isn't necessarily a success, but counts as one for the purpose of continuing the skill
    /// </summary>
    PseudoSuccess
}
