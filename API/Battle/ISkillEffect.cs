using API.Entity;

namespace API.Battle;

public interface ISkillEffect {
    bool IsInstant { get; }
    SkillType SkillType { get; }
    int Pow { get; }

    IconEntity? DescInclusion { get; init; }

    ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType);
}