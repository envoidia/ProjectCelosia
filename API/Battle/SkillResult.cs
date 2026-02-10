using System.Collections.Generic;

namespace API.Battle;

public sealed record SkillResult(SkillResultType ResultType, params List<string> Messages);