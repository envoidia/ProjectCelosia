using System;

namespace API.Menu;

/// <summary>
/// todo should ListWidget and etc just take this instead of strings
/// </summary>
public interface IComponent
{
    int Index { get; set; }

    Action? OnSelect { get; init; }
    Action? OnUpdate { get; init; }
    Action? OnConfirm { get; init; }
}
