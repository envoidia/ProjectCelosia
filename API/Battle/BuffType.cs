namespace API.Battle;

// todo name
public enum BuffType {
    Buff,
    Debuff
}

public static class BuffTypeExtensions {
    extension(BuffType buffType) {
        public string GetName() => buffType switch {
            BuffType.Buff => Lang.Buff,
            BuffType.Debuff => Lang.Debuff
        };
    }
}