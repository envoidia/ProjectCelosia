namespace API.Battle;

public enum Prio {
    Last,
    VeryLate,
    Late,
    Normal,
    Early,
    VeryEarly,
    BeforeAllAttacks,
    Immediate
}

public static class PrioExtensions {
    extension(Prio prio) {
        public string Format() => "todo";
    }
}