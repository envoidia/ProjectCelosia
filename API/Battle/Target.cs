namespace API.Battle;

public enum Target {
    Self, // Self
    SelfUp, // Ally above self
    SelfDown, // Ally below self
    SelfAcross, // Opponent across from self
    SelfAcrossUp, // Opponent across from and above self
    SelfAcrossDown, // Opponent across from and below self
    SelfTeam, // Self's team (not including self)
    Target, // Target
    TargetUp, // Unit above target
    TargetDown, // Unit below target
    TargetTeam // Target's team (not including target)
}