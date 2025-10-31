using API.Entity;

namespace API.Battle;

public interface ISkillEffect {
    bool IsInstant { get; }
    SkillType SkillType { get; }
    int Pow { get; }

#nullable enable
    IconEntity? DescInclusion { get; init; }
#nullable disable

    ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType);
}