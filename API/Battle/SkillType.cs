namespace API.Battle;

public enum SkillType {
    Str,
    Mag,
    Fth,
    Stat
}

public static class SkillTypeExtensions {
    extension(SkillType skillType) {
        public string GetName() => skillType switch {
            SkillType.Str => Lang.StatStr,
            SkillType.Mag => Lang.StatMag,
            SkillType.Fth => Lang.StatFth,
            SkillType.Stat => Lang.Stat
        };
    }
}