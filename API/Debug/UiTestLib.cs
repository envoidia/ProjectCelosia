using System;
using System.Collections.Generic;
using API.Input;
using API.Menu.Widget;
using Microsoft.Xna.Framework.Input;

namespace API.Debug;

public static class UiTestLib
{
    private static readonly List<string> _Text = ["Foo", "Bar", "Baz", "Lorem", "Ipsum", "Dolor",
        "Si", "Amet"];

    private static readonly ListWidget _List = new(new(3000, 1000),
        false, Graphics.RenderPriority.Highest, [.. _Text])
    {
        HeightLimit = 6,
        Slant = ListWidget.NormalSlant,
        HasBackground = true
    };

    private static readonly TabBarWidget _Tab = new(new(3000, 500),
        "lorem", "ipsum", "dolor", "si", "amet");

    public static readonly Menu.Menu Menu = new("Test", _List, _Tab)
    {
        OnCreate = static () =>
        {
            _List.CalcLayout();
        },

        OnUpdate = static gt =>
        {
            if (InputLib.IsKeyJustPressed(Keys.Q))
            {
                _Text.Add("abc");
                _List.SetTextL([.. _Text]);
                _List.CalcLayout();
            }

            if (InputLib.IsKeyJustPressed(Keys.W))
            {
                _Text.Clear();
                _List.SetTextL();
                _List.CalcLayout();
            }
        }
    };
}
