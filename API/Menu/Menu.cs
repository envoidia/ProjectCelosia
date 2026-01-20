using System;
using System.Collections.Generic;
using System.Linq;
using API.Graphics;
using API.Menu.Widget;
using Microsoft.Xna.Framework;

namespace API.Menu;

// todo: how can widgets communicate with eachother? move input prompt logic here (partially)
/// <summary>
/// A set of actors and <c>IInputWidgets</c>. Handles when they should be added to / removed from the stage
/// and assigns controls
/// </summary>
public sealed class Menu
{
    /// <summary>
    /// Display name for this. Only used in debug features
    /// </summary>
    public string DbgName { get; }

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
    public List<IInputWidget> InputWidgets { get; init; } = [];

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
    public Menu(string name)
    {
        this.DbgName = name;
    }

    public Menu(string name, params IActor[] actors)
    {
        this.DbgName = name;
        this.Setup(actors);
    }

    public void Setup(params IActor[] actors)
    {
        this.Actors = actors;
        this.SetupWidgets();
    }

    /// <summary>
    /// Called by <c>StateMachine.AddMenu</c>. Do not call directly
    /// </summary>
    public void Create()
    {
        Stage.AddRange(this.Actors);
        this.OnCreate?.Invoke();

        Stage.Sort();
    }

    /// <summary>
    /// Called by <c>StateMachine.RemoveMenu</c>. Do not call directly
    /// </summary>
    public void Destroy()
    {
        foreach (IActor a in this.Actors)
        {
            a.Destroy();
        }

        this.OnDestroy?.Invoke();

        Stage.Sort();
    }

    /// <summary>
    /// Called by <c>State.Update</c>. Do not call directly
    /// </summary>
    // todo do i need this? will there be extra behavior?
    public void Update(GameTime gt)
    {
        this.OnUpdate?.Invoke(gt);
    }

    /// <inheritdoc cref="Update" />
    public void Input(GameTime gt)
    {
        foreach (IInputWidget iw in this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets))
        {
            iw.Input(gt);
        }
    }

    /// <returns>
    /// The <c>IInputWidget</c> currently assigned to a given <c>SelectionType</c>, if any
    /// </returns>
    public IInputWidget? GetInputWidget(SelectionType st)
    {
        if (st is SelectionType.Horiz or SelectionType.Vert)
        {
            return this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets)
                .FirstOrDefault(w => w.CurDir == st || w.CurDir == SelectionType.HorizVert);
        }

        return this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets)
            .FirstOrDefault(w => w.CurDir == st);
    }

    public void SetupWidgets()
    {
        // Assign inputs to each Widget based off of what they prefer and what's available
        bool usedHoriz = false;
        bool usedVert = false;
        bool usedPage = false;

        foreach (IInputWidget iw in this.Actors.OfType<IInputWidget>().Concat(this.InputWidgets))
        {
            // todo cleanup
            switch (iw.PrefDir)
            {
                case SelectionType.Horiz:
                    if (!usedHoriz)
                    {
                        iw.CurDir = SelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedPage)
                    {
                        iw.CurDir = SelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    if (!usedVert)
                    {
                        iw.CurDir = SelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case SelectionType.Vert:
                    if (!usedVert)
                    {
                        iw.CurDir = SelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    if (!usedHoriz)
                    {
                        iw.CurDir = SelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedPage)
                    {
                        iw.CurDir = SelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case SelectionType.Page:
                    if (!usedPage)
                    {
                        iw.CurDir = SelectionType.Page;
                        usedPage = true;
                        break;
                    }

                    if (!usedHoriz)
                    {
                        iw.CurDir = SelectionType.Horiz;
                        usedHoriz = true;
                        break;
                    }

                    if (!usedVert)
                    {
                        iw.CurDir = SelectionType.Vert;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
                case SelectionType.HorizVert:
                    if (!usedHoriz && !usedVert)
                    {
                        iw.CurDir = SelectionType.HorizVert;
                        usedHoriz = true;
                        usedVert = true;
                        break;
                    }

                    throw new _MenuAssignException();
            }
        }

        if ((usedHoriz && usedVert) || (!usedHoriz && !usedVert)) return;

        // Assign secondary inputs, if there are leftovers
        foreach (IInputWidget iw in this.Actors.OfType<IInputWidget>())
        {
            if (!usedHoriz && iw.CurDir == SelectionType.Vert)
            {
                iw.CurDir = SelectionType.HorizVert;
                return;
            }

            if (!usedVert && iw.CurDir == SelectionType.Horiz)
            {
                iw.CurDir = SelectionType.HorizVert;
                return;
            }
        }
    }

    private sealed class _MenuAssignException() :
        Exception("Could not assign controls to Menu widgets because all directions were used."
            + $"If you got this from requesting a {nameof(GridWidget)}, try requesting it first in the list");
}
