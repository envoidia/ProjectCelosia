using System.Collections.Generic;

namespace API.Battle;

public enum ResultType {
    Fail,
    HitShield, // Also counts Effect Block
    Success
}

public record Result(ResultType ResultType, params List<string> Messages);