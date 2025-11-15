namespace API.Battle;

// todo name
public enum BuffType {
    Buff,
    Debuff
}

public static class BuffTypeExtensions {
    extension(BuffType @this) {
        public string GetName() => @this switch {
            BuffType.Buff => Lang.Buff,
            BuffType.Debuff => Lang.Debuff
        };
    }
}