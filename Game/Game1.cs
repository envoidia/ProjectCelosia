using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using API;
using API.Debug;
using API.Graphics;
using API.Input;
using API.Menu;
using FontStashSharp.RichText;
using ResolutionBuddy;

namespace Game;

public class Game1 : Core {
    // Rendering
    private IResolution _resolution; // Ignore unused warning

    private Texture2D _bg;

    private AnimatedSprite _slime;
    private AnimatedSprite _bat;

    // Menu stuff
    private int _index;

    // Debug
    private bool _isDebugInfoEnabled = false;

    public Game1() : base("Project Celosia", 0, 0, false) {
        this._resolution = new ResolutionComponent(this, Graphics, new Point(1920, 1080),
            new Point(1920, 1080), false, false, false);
    }

    private RichTextLayout _richTextLayout;

    protected override void Initialize() {
        NavPath.Push(MenuType.Main);
        
        base.Initialize();
    }

    protected override void LoadContent() {
        this._bg = Content.Load<Texture2D>("img/bg");

        TextureAtlas atlas = TextureAtlas.FromFile(Content, "img/atlas-definition.xml");

        this._slime = atlas.CreateAnimatedSprite("slime-animation");
        this._slime.Scale = new Vector2(4.0f, 4.0f);
        this._bat = atlas.CreateAnimatedSprite("bat-animation");
        this._bat.Scale = new Vector2(4.0f, 4.0f);

        this._richTextLayout = new RichTextLayout {
            Font = Koruri30,
            Text =
                "A small tree: /i[eating.png] :3c\namong us susssy among us roblox forntite vbucks adkfhsajkasljdlskajdkahsfdjkashdlasjdlkjas\nfwejfweifuowefipwef/i[eating.png]\nsdhjfiousdhfowuefopiuwepofiew[opifopweiufouwrpofg",
            Width = 800
        };

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            this.Exit();
        }

        this.CheckInput(gameTime);


        this._bat.Update(gameTime);
        this._slime.Update(gameTime);

        base.Update(gameTime);
    }

    private void CheckInput(GameTime gameTime) {
        this._isDebugInfoEnabled ^= Input.CheckInput(Keybind.DebugInfo);
        DebugMenu.HandleDebugInfo(this._isDebugInfoEnabled, gameTime);

        switch (NavPath.Peek()) {
            case MenuType.Main:
                this._index = MenuLib.CheckMovement1D(this._index, 5);
                //Console.WriteLine(this._index);
                //Console.WriteLine(1.0f / gameTime.ElapsedGameTime.TotalSeconds);
                // update cursor

                if (Input.CheckInput(Keybind.Confirm)) {
                    // continue based on selected option
                } else if (Input.CheckInput(Keybind.Back)) {
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
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
            null, null, Resolution.TransformationMatrix());

        SpriteBatch.Draw(this._bg, Vector2.Zero, Color.White);
        this._slime.Draw(SpriteBatch, Vector2.One);
        this._bat.Draw(SpriteBatch, new Vector2(this._slime.Width + 10, 0));

        // todo wip
        /*SpriteBatch.DrawString(
            Koruri25,              // spriteFont
            TestStr, // text
            Vector2.Zero, // position
            Color.White        // color
        );*/

        this._richTextLayout.Draw(SpriteBatch, new Vector2(0, 0), Color.White);
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