using System;
using API.Graphics;

namespace API.Menu;

public interface IWidget : IActor {
    /// <summary>
    /// The <c>Menu</c> this is a part of
    /// </summary>
    Menu Menu { get; }

    /// <inheritdoc cref="WidgetSelectionType" />
    WidgetSelectionType PrefDir { get; }

    /// <summary>
    /// The directions that this is currently using for input
    /// </summary>
    WidgetSelectionType CurDir { get; set; }

    Action? OnSelect { get; set; }

    void CalcLayout();
}
