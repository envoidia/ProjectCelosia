namespace API.Modding;

/// <summary>
/// An item that should know what mod it's from what whatever reason (most commonly to fetch lang entries)
/// </summary>
public interface IModItem {
    GameMod? Source { get; }
}