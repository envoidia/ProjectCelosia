using System;
using System.Linq;
using API.Graphics;
using Microsoft.Xna.Framework;

namespace API.Menu;

// todo: how can widgets communicate with eachother? move input prompt logic here (partially)
/// <summary>
/// A set of actors and <c>IInputWidgets</c>. Handles when they should be added to / removed from the stage
/// and assigns controls
/// </summary>
public sealed class Menu {
    /// <summary>
    /// Display name for this (todo i18n)
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Called when this is first reached to update the input prompt <c>Label</c> in the bottom-right corner
    /// </summary>
    public Func<string>? GetInputPrompt { get; init; }

    /// <summary>
    /// <c>IActors</c> that this will add to the stage. Also handles controls for any that are <c>IInputWidgets</c>
    /// </summary>
    public IActor[] Actors { get; private set; } = [];

    /// <summary>
    /// <c>IInputWidgets</c> that this will handle controls for in addition to its actors
    /// </summary>
    public IInputWidget[] InputWidgets { get; init; } = [];

    /// <summary>
    /// Called by <c>this.Create</c>. Do not call directly
    /// </summary>
    public Action? OnCreate { get; init; }

    /// <summary>
    /// Called by <c>this.Destroy</c>. Do not call directly
    /// </summary>
    public Action? OnDestroy { get; init; }

    /// <summary>
    /// Called by <c>this.Update</c>. Do not call directly
    /// </summary>
    public Action<GameTime>? OnUpdate { get; init; }

    /// <summary>
    /// Initializes this with no behavior or actors
    /// </summary>
    public Menu(string name) {
        this.Name = name;
    }

    public Menu(string name, params IActor[] actors) {
        this.Name = name;
        this.Setup(actors);
    }

    public void Setup(params IActor[] actors) {
        this.Actors = actors;

        this.SetupWidgets();
    }

    /// <summary>
    /// Called by <c>StateMachine.AddMenu</c>. Do not call directly
    /// </summary>
    public void Create() {
        Stage.AddRange(this.Actors);
        this.OnCreate?.Invoke();

        Stage.Cleanup();
    }

    /// <summary>
    /// Called by <c>StateMachine.RemoveMenu</c>. Do not call directly
    /// </summary>
    public void Destroy() {
        foreach (IActor a in this.Actors) a.Destroy();
        this.OnDestroy?.Invoke();

        Stage.Cleanup();
    }

    /// <summary>
    /// Called by <c>State.Update</c>. Do not call directly
    /// </summary>
    // todo do i need this? will there be extra behavior?
    public void Update(GameTime gameTime) => this.OnUpdate?.Invoke(gameTime);

    /// <returns>
    /// The <c>IInputWidget</c> currently assigned to a given <c>SelectionType</c>, if any
    /// </returns>
    public IInputWidget? GetInputWidget(SelectionType st) {
        if (st is SelectionType.Horiz or SelectionType.Vert) {
            return this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets)
                .FirstOrDefault(w => w.CurDir == st || w.CurDir == SelectionType.HorizVert);
        }

        return this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets)
            .FirstOrDefault(w => w.CurDir == st);
    }

    public void SetupWidgets() {
        // Assign inputs to each Widget based off of what they prefer and what's available
        bool usedHoriz = false;
        bool usedVert = false;
        bool usedPage = false;

        foreach (IInputWidget ia in this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets)) {
            // todo cleanup
            switch (ia.PrefDir) {
                case SelectionType.Horiz:
                    if (!usedHoriz) {
                        ia.CurDir = SelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedPage) {
                        ia.CurDir = SelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    if (!usedVert) {
                        ia.CurDir = SelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case SelectionType.Vert:
                    if (!usedVert) {
                        ia.CurDir = SelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    if (!usedHoriz) {
                        ia.CurDir = SelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedPage) {
                        ia.CurDir = SelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case SelectionType.Page:
                    if (!usedPage) {
                        ia.CurDir = SelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    if (!usedHoriz) {
                        ia.CurDir = SelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedVert) {
                        ia.CurDir = SelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case SelectionType.HorizVert:
                    if (!usedHoriz && !usedVert) {
                        ia.CurDir = SelectionType.HorizVert;
                        usedHoriz = true;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
            }
        }

        if ((usedHoriz && usedVert) || (!usedHoriz && !usedVert)) return;

        // Assign secondary inputs, if there are leftovers
        foreach (IInputWidget w in this.Actors.OfType<IInputWidget>()) {
            if (!usedHoriz && w.CurDir == SelectionType.Vert) {
                w.CurDir = SelectionType.HorizVert;
                return;
            }

            if (!usedVert && w.CurDir == SelectionType.Horiz) {
                w.CurDir = SelectionType.HorizVert;
                return;
            }
        }
    }

    private class _MenuAssignException() :
        Exception("Could not assign controls to Menu widgets because all directions were used. If you got this from requesting a GridWidget, try requesting it first");
}
