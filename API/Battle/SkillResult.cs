using System.Collections.Generic;

namespace API.Battle;

public sealed record SkillResult(SkillResultType ResultType, params List<string> Messages);

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