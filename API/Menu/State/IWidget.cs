using System;
using API.Graphics;

namespace API.Menu.State;

public interface IWidget : IActor {
    int Index { get; set; }

    /// <inheritdoc cref="WidgetSelectionType" />
    WidgetSelectionType PrefDir { get; }

    /// <summary>
    /// The directions that this is currently using for input
    /// </summary>
    WidgetSelectionType CurDir { get; set; }

    Padding Padding { get; set; }

    Action? OnSelect { get; set; }

    void CalcLayout();
}
