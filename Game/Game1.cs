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
using API.Extensions;
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
    private static readonly Label _testLabel = new() {
        Position = new Vector2(1000, 800),
        Text = "",
        Width = 2000
    };

    public Game1() : base("Project Celosia", 0, 0, false) =>
        this._resolution = new ResolutionComponent(this, Graphics, new Point(World.W, World.H),
            new Point(1920, 1080), false, false, false);

    protected override void Initialize() {
        base.Initialize();
        AddMenu(Main);

        _testLabel.Text = "RangeAllies21R".GetLang();
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
        this._isDebugInfoEnabled ^= Input.CheckInput(Keybinds.DebugInfo);

        DebugMenu.HandleDebugInfo(this._isDebugInfoEnabled, gameTime);

        switch (NavPath.Peek()) {
            case Main:
                this._index = MenuLib.CheckMovement1D(this._index, Enum.GetValues<MainMenu>().Length);
                //Console.WriteLine(this._index);
                // update cursor

                if (Input.CheckInput(Keybinds.Confirm)) {
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
                } else if (Input.CheckInput(Keybinds.Back)) {
                    if ((MainMenu) this._index == MainMenu.Quit) {
                        this.Exit();
                    } else {
                        this._index = (int) MainMenu.Quit;
                    }
                }

                break;
            case Popup:
                if (Input.CheckInput(Keybinds.Confirm, Keybinds.Back)) {
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

        //Console.WriteLine(KoruriSystem.Atlases.Count); //todo test with more diverse chars

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