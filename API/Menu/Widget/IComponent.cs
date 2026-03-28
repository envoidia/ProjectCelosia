using System;

namespace API.Menu.Widget;

/// <summary>
/// todo should ListWidget and etc just take this instead of strings? should this even be an iface?
/// </summary>
public interface IComponent
{
    int Index { get; set; }

    Action? OnSelect { get; init; }
    Action? OnUpdate { get; init; }
    Action? OnConfirm { get; init; }
}
