using System;
using System.Linq;
using API.Graphics;

namespace API.Menu;

// todo: how can widgets communicate with eachother? move input prompt logic here (partially)
public sealed class Menu {
    public IActor[] Actors { get; init; }

    public Action? OnCreate { get; init; }
    public Action? OnDestroy { get; init; }
    public Action? OnUpdate { get; init; }

    public Menu(params IActor[] actors) {
        this.Actors = actors;

        this._SetupWidgets();
    }

    public void Create() {
        Stage.AddRange(this.Actors);
        this.OnCreate?.Invoke();

        Stage.Cleanup();
    }

    public void Destroy() {
        foreach (IActor a in this.Actors) a.Destroy();
        this.OnDestroy?.Invoke();

        Stage.Cleanup();
    }

    public void Update() {
        this.OnUpdate?.Invoke();
    }

    #region Internals

    private void _SetupWidgets() {
        // Assign inputs to each Widget based off of what they prefer and what's available
        bool usedHoriz = false;
        bool usedVert = false;
        bool usedPage = false;

        foreach (IWidget w in this.Actors.OfType<IWidget>()) {
            // todo cleanup
            switch (w.PrefDir) {
                case WidgetSelectionType.Horiz:
                    if (!usedHoriz) {
                        w.CurDir = WidgetSelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedPage) {
                        w.CurDir = WidgetSelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    if (!usedVert) {
                        w.CurDir = WidgetSelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case WidgetSelectionType.Vert:
                    if (!usedVert) {
                        w.CurDir = WidgetSelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    if (!usedHoriz) {
                        w.CurDir = WidgetSelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedPage) {
                        w.CurDir = WidgetSelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case WidgetSelectionType.Page:
                    if (!usedPage) {
                        w.CurDir = WidgetSelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    if (!usedHoriz) {
                        w.CurDir = WidgetSelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedVert) {
                        w.CurDir = WidgetSelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case WidgetSelectionType.HorizVert:
                    if (!usedHoriz && !usedVert) {
                        w.CurDir = WidgetSelectionType.HorizVert;
                        usedHoriz = true;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
            }
        }

        if ((usedHoriz && usedVert) || (!usedHoriz && !usedVert)) return;

        // Assign secondary inputs, if there are leftovers
        foreach (IWidget w in this.Actors.OfType<IWidget>()) {
            if (!usedHoriz && w.CurDir == WidgetSelectionType.Vert) {
                w.CurDir = WidgetSelectionType.HorizVert;
                return;
            }

            if (!usedVert && w.CurDir == WidgetSelectionType.Horiz) {
                w.CurDir = WidgetSelectionType.HorizVert;
                return;
            }
        }
    }

    private class _MenuAssignException()
        : Exception("Could not assign controls to Menu IWidgets because all directions were used. If you got this from requesting a GridWidget, try requesting it first");

    #endregion
}
