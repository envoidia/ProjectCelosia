using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using API;
using API.Graphics;
using API.Input;
using API.Menu;
using MonoGame.Extended.Graphics;
using ResolutionBuddy;
using API.Battle;
using API.Debug;
using API.Save;
#if NATIVE_AOT
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content.ContentReaders;
#else
using API.Modding;
#endif
using static API.Menu.MenuType;

namespace Game;

public class Game1 : Core {
    // Rendering
    private static Texture2D bg = null!;

    // Menu stuff
    private static uint index;
    private const uint OptCountMain = (uint) MainMenu.LastValue - 1;

    // Debug
    private static bool isDebugInfoEnabled;

#if NATIVE_AOT
    private static Celosia.Main celosiaMain = null!;
#endif

    private static readonly Label TestLabel = new() {
        Position = new Vector2(1000, 800),
        Text = "",
        Width = 2000
    };

    public Game1() : base("Project Celosia", 0, 0, false) {
#if NATIVE_AOT
        // Make sure to change this after updating MGE
        ContentTypeReaderManager.AddTypeCreator(
            "MonoGame.Extended.Content.ContentReaders.Texture2DAtlasReader, MonoGame.Extended, Version=5.2.0.0, Culture=neutral, PublicKeyToken=null",
            () => new Texture2DAtlasReader()
        );
#endif

        Resolution.Init(new ResolutionComponent(this, Graphics, new Point(World.W, World.H),
            new Point(1920, 1080), false, false, false));
    }

    protected override void Initialize() {
        base.Initialize();
        AddMenu(Main);
#if NATIVE_AOT
        // Load main game
        celosiaMain = new Celosia.Main();
        celosiaMain.Initialize();
#else
        if (Settings.EnableModLoader) {
            // Load arbitrary mods
            ModLoader.InitializeAllMods();
        } else {
            // Just load main game
            ModLoader.InitializeCelosiaMod();
        }
#endif
        TestLabel.Text = "";
    }

    protected override void LoadContent() {
        bg = Content.Load<Texture2D>("img/bg");
        IconsAtlas = Content.Load<Texture2DAtlas>("img/icons");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        this.CheckInput(gameTime);
        base.Update(gameTime);

        // Update mods
#if NATIVE_AOT
        celosiaMain.Update(gameTime);
#else
        ModLoader.UpdateAllMods(gameTime);
#endif
    }

    private void CheckInput(GameTime gameTime) {
        isDebugInfoEnabled ^= Input.CheckInput(Keybinds.DebugInfo);

        DebugMenu.HandleDebugInfo(isDebugInfoEnabled, gameTime);

        switch (NavPath.Peek()) {
            case Main:
                index = MenuLib.CheckMovement1D(index, OptCountMain);
                //Console.WriteLine(_index);
                // update cursor

                if (Input.CheckInput(Keybinds.Confirm)) {
                    switch ((MainMenu) index) {
                        case MainMenu.Start:
                            // Overworld/battle
                            AddMenu(MenuType.Battle);
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
                    if ((MainMenu) index == MainMenu.Quit) {
                        this.Exit();
                    } else {
                        index = (uint) MainMenu.Quit;
                    }
                }

                break;
            case Popup:
                if (Input.CheckInput(Keybinds.Confirm, Keybinds.Back)) {
                    // close
                }

                break;
            case MenuType.Battle or Targeting or Log or InspectTargeting or Inspect:
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

        SpriteBatch.Draw(bg, Vector2.Zero, Color.White);

        //Console.WriteLine(KoruriSystem.Atlases.Count); //todo test

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