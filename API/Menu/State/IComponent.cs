using System;

namespace API.Menu.State;

public interface IComponent {
    int Index { get; set; }

    Action? OnSelect { get; init; }
    Action? OnUpdate { get; init; }
    Action? OnConfirm { get; init; }
}
