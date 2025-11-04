using System.Collections.Generic;

namespace API.Battle;

public enum ResultType {
    Fail,
    HitEffectBlock,
    Success,

    // For when it's not necessarily a success, but you want it to count as one for the purpose of continuing the skill
    PseudoSuccess
}

public record Result(ResultType ResultType, params List<string> Messages);