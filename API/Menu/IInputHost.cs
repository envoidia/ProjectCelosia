using System;

namespace API.Menu;

/// <summary>
/// An object that can capture inputs
/// </summary>
public interface IInputHost {
    /// <summary>
    /// Object currently receiving input
    /// </summary>
    static IInputHost? Cur { get; set; } = null;
}
