using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using API;
using API.Debug;
using API.Graphics;
using API.Input;
using API.Menu;
using FontStashSharp.RichText;
using MonoGame.Extended.Graphics;
using ResolutionBuddy;

namespace Game;

public class Game1 : Core {
    // Rendering
    private IResolution _resolution; // Ignore unused warning

    private Texture2D _bg;

    // Menu stuff
    private int _index;

    // Debug
    private bool _isDebugInfoEnabled;

    public Game1() : base("Project Celosia", 0, 0, false) =>
        this._resolution = new ResolutionComponent(this, Graphics, new Point(World.W, World.H),
            new Point(2560, 1440), false, false, false);

    private static readonly Label InputPrompt = new() {
        Position = new Vector2(300, 300)
    };

    protected override void Initialize() {
        NavPath.Push(MenuType.Main);

        base.Initialize();
    }

    protected override void LoadContent() {
        this._bg = Content.Load<Texture2D>("img/bg");

        iconsAtlas = Content.Load<Texture2DAtlas>("img/icons");

        RichTextDefaults.ImageResolver = p => {
            if (TextureCache.TryGetValue(p, out Texture2DRegion region)) {
                return new TextureFragment(region.Texture, region.Bounds);
            }

            region = iconsAtlas.GetRegion(p);

            // Cache the region for future use
            TextureCache[p] = region;

            return new TextureFragment(region.Texture, region.Bounds);
        };

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            this.Exit();
        }

        this.CheckInput(gameTime);

        base.Update(gameTime);
    }

    private void CheckInput(GameTime gameTime) {
        this._isDebugInfoEnabled ^= Input.CheckInput(Keybind.DebugInfo);
        DebugMenu.HandleDebugInfo(this._isDebugInfoEnabled, gameTime);

        switch (NavPath.Peek()) {
            case MenuType.Main:
                this._index = MenuLib.CheckMovement1D(this._index, 5);
                //Console.WriteLine(this._index);
                // update cursor

                if (Input.CheckInput(Keybind.Confirm)) {
                    Console.WriteLine("confirm");
                    // continue based on selected option
                } else if (Input.CheckInput(Keybind.Back)) {
                    Console.WriteLine("back");
                    // if (index == last) quit, else index = last
                }

                break;
            case MenuType.Popup:
                if (Input.CheckInput(Keybind.Back)) {
                    // cancel
                }

                break;
            case MenuType.Battle:
            case MenuType.Targeting:
            case MenuType.Log:
            case MenuType.InspectTargeting:
            case MenuType.Inspect:
                // Pass to BattleHandler
                break;
            case MenuType.Debug:
            case MenuType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(NavPath.Peek().ToString());
        }

        if (Input.InputDeviceChanged) {
            InputPrompt.Text = "Input: " + InputPrompts.Confirm.GetText() + " " + InputPrompts.Back.GetText() + "\n" +
                               InputPrompts.Close.GetText() + "\n" + InputPrompts.MoveLeftRight.GetText();
        }
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
            null, null, Resolution.TransformationMatrix());

        SpriteBatch.Draw(this._bg, Vector2.Zero, Color.White);

        //Console.WriteLine(KoruriSystem.Atlases.Count); todo test with more diverse chars

        DrawRenderPriority(LabelsLow);
        DrawRenderPriority(LabelsMed);
        DrawRenderPriority(LabelsHigh);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

    private static void DrawRenderPriority(List<Label> labels) {
        foreach (Label label in labels) label.Draw(SpriteBatch);
    }
}