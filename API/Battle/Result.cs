using System.Collections.Generic;

namespace API.Battle;

public enum ResultType {
    Fail,
    HitEffectBlock,
    Success,

    /// <summary>
    /// Isn't necessarily a success, but counts as one for the purpose of continuing the skill
    /// </summary>
    PseudoSuccess
}

public sealed record Result(ResultType ResultType, params List<string> Messages);