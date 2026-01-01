using System.Collections.Generic;
using System.Linq;
using API.Menu;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;

namespace API.Graphics;

/// <summary>
/// List of active <c>IActor</c>s with helper methods
/// </summary>
public static class Stage
{
    // todo decide starting size (use max size and assert it?)
    private static readonly List<IActor> _Actors = new(250);

    internal static bool _needsSorting = false;

    /// <summary>
    /// Draws all visible <c>IActor</c>s and performs their <c>Routine</c>s
    /// </summary>
    public static void Act(GameTime gt)
    {
        int i = _Actors.Count - 1;

        // The only reason this gross stuff has to happen is bc SpriteBatch and ShapeBatch are separate
        begin();

        for (; i >= 0 && _Actors[i].Data.Priority < RenderPriority.B2Low; i--)
        {
            _Actors[i].Data.Act(gt);
        }

        end();

        begin();

        for (; i >= 0 && _Actors[i].Data.Priority < RenderPriority.B3Low; i--)
        {
            _Actors[i].Data.Act(gt);
        }

        end();

        begin();

        for (; i >= 0 && _Actors[i].Data.Priority < RenderPriority.Highest; i--)
        {
            _Actors[i].Data.Act(gt);
        }

        end();

        begin();

        for (; i >= 0; i--)
        {
            _Actors[i].Data.Act(gt);
        }

        if (DebugUtil.DrawPalette)
        {
            Settings.Theme._DrawPalette(); // todo how is it possible that this draws between f1 and its text
        }

        end();

        // Debug overlay (F3)
        if (DebugUtil.DrawActorOutlines)
        {
            begin();

            foreach (IActor a in _Actors)
            {
                a.Data.DrawDebug();
            }
            
            end();
        }

        static void begin()
        {
            Core.ShapeBatch.Begin(Resolution.TransformationMatrix());

            Core.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.PointClamp, null, null, null,
                Resolution.TransformationMatrix());
        }

        static void end()
        {
            Core.ShapeBatch.End();
            Core.SpriteBatch.End();
        }
    }

    /// <summary>
    /// Add <c>IActor</c>(s). Do NOT add an actor that is already on the Stage.
    /// After you're done adding, call <c>Cleanup()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public static void Add(IActor actor)
    {
        Assert.DoesntContain(_Actors, actor);

        _Actors.Add(actor);
        actor.Create();

        _needsSorting = true;
    }

    /// <inheritdoc cref="Add" />
    public static void AddRange(params IEnumerable<IActor> actors)
    {
        foreach (IActor actor in actors)
        {
            Assert.DoesntContain(_Actors, actor);

            _Actors.Add(actor);
            actor.Create();
        }

        _needsSorting = true;
    }

    /// <summary>
    /// Removes an <c>IActor</c>. Should be called from <c>ActorData.Destroy</c> -- Do not call directly
    /// </summary>
    public static void Remove(IActor actor)
    {
        _Actors.Remove(actor);
    }

    /// <summary>
    /// Applies sorting
    /// todo remove individual sort calls all over and just call 1 time after init
    /// </summary>
    public static void Sort()
    {
        if (!_needsSorting)
        {
            return;
        }

        _Actors.Sort(static (a, b) =>
           ((int) b.Data.Priority).CompareTo((int) a.Data.Priority));

        _needsSorting = false;
    }

    public static int ActorCount()
    {
        return _Actors.Count;
    }

    public new static string ToString()
    {
        return string.Join("\n", [.. _Actors.Select(static a => a.ToString() + " " + a.Priority.ToString())]);
    }

    internal static void _RecalcLayoutWidgets()
    {
        _Actors.OfType<ILayoutWidget>().ToList().ForEach(w => w.CalcLayout());
    }
}