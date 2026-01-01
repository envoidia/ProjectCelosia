namespace API.Battle;

/// <summary>
/// A single relative target for a <c>Range</c>
/// </summary>
public enum Target
{
    Self,

    /// <summary>
    /// Ally above self
    /// </summary>
    SelfUp,

    /// <summary>
    /// Ally below self
    /// </summary>
    SelfDown,

    /// <summary>
    /// Opponent across from self
    /// </summary>
    SelfAcross,

    /// <summary>
    /// Opponent across from and above self
    /// </summary>
    SelfAcrossUp,

    /// <summary>
    /// Opponent across from and below self
    /// </summary>
    SelfAcrossDown,

    /// <summary>
    /// Self's team (not including self)
    /// </summary>
    SelfTeam,

    /// <summary>
    /// Target
    /// </summary>
    Target,

    /// <summary>
    /// Unit above target
    /// </summary>
    TargetUp,

    /// <summary>
    /// Unit below target
    /// </summary>
    TargetDown,

    /// <summary>
    /// Target's team (not including target)
    /// </summary>
    TargetTeam
}