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
using MonoGame.Extended.Graphics;
using ResolutionBuddy;
using API.Battle;
using static API.Menu.MenuType;

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
            new Point(1920, 1080), false, false, false);

    protected override void Initialize() {
        base.Initialize();
        AddMenu(Main);
        
        // text testing
        Label a = new() {
            Text = "/c[blue]/i[shield]TopLeft",
            Position = Vector2.One * 300,
            HasBackground = true
        };
        
        Label b = new() {
            Text = "/c[green]/i[whirlwind]TopRight",
            Alignment = Alignment.TopRight,
            Position = Vector2.One * 400,
            HasBackground = true,
            BackgroundColor = new Color(0.3f, 0, 0, 0.6f)
        };
        
        Label c = new() {
            Text = "/c[cyan]/i[earth-spit]Bottom/i[bubbles]L/c[red]eft",
            Alignment = Alignment.BottomLeft,
            Position = Vector2.One * 500,
            HasBackground = true
        };
        
        Label d = new() {
            Text = "/c[yellow]/i[star-formation]BottomRight",
            Alignment = Alignment.BottomRight,
            Position = Vector2.One * 600,
            HasBackground = true
        };
        
        Label e = new() {
            Text = "/c[orange]/i[dread-skull]Center",
            Alignment = Alignment.Center,
            Position = Vector2.One * 700,
            HasBackground = true,
            BackgroundColor = new Color(0, 0.3f, 0, 0.6f)
        };

    }

    protected override void LoadContent() {
        this._bg = Content.Load<Texture2D>("img/bg");

        IconsAtlas = Content.Load<Texture2DAtlas>("img/icons");

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
            case Main:
                this._index = MenuLib.CheckMovement1D(this._index, Enum.GetValues<MainMenu>().Length);
                //Console.WriteLine(this._index);
                // update cursor

                if (Input.CheckInput(Keybind.Confirm)) {
                    switch ((MainMenu) this._index) {
                        case MainMenu.Start:
                            // Overworld/battle
                            AddMenu(Battle);
                            BattleHandler.Init();
                            break;
                        case MainMenu.Encyclopedia:
                            // todo
                            break;
                        case MainMenu.Options:
                            // todo
                            break;
                        case MainMenu.Mods:
                            // todo
                            break;
                        case MainMenu.Credits:
                            // todo
                            break;
                        case MainMenu.Quit:
                            // todo
                            break;
                    }
                } else if (Input.CheckInput(Keybind.Back)) {
                    if ((MainMenu) this._index == MainMenu.Quit) {
                        this.Exit();
                    } else {
                        this._index = (int) MainMenu.Quit;
                    }
                }

                break;
            case Popup:
                if (Input.CheckInput(Keybind.Confirm, Keybind.Back)) {
                    // close
                }

                break;
            case Battle or Targeting or Log or InspectTargeting or Inspect:
                BattleHandler.Input(gameTime);
                break;
            case Debug or None:
                break;
            default:
                throw new ArgumentOutOfRangeException(NavPath.Peek().ToString());
        }

        if (Input.InputDeviceChanged) UpdateInputPrompt();
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