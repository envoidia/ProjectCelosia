using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using API;
using API.Graphics;
using API.Input;
using API.Menu;
using MonoGame.Extended.Graphics;
using ResolutionBuddy;
using API.Menu.State;


#if NATIVE_AOT
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content.ContentReaders;
#else
using API.Modding;
#endif

namespace Game;

public sealed class Game1 : Core {
    // Rendering
    private static Texture2D _bg = null!;

    // Debug
    private static bool _isDebugInfoEnabled;

    // temp
    //private static float[][] barProgs = [[1, 0.5f, 0.25f], [0.5f, 0.35f, 1], [0.75f, 1, 0.15f]];
    //private static int barIndex = -1;

#if NATIVE_AOT
    private static Celosia.Main _celosiaMain = null!;
#endif

    public Game1() : base("Project Celosia", 0, 0, false) {
#if NATIVE_AOT
        // Prevent crash caused by reflection in the atlas reader
        // Make sure to change this after updating MGE
        // todo whats the difference between () => ... and _ => ...
        ContentTypeReaderManager.AddTypeCreator(
            "MonoGame.Extended.Content.ContentReaders.Texture2DAtlasReader, MonoGame.Extended, Version=5.2.0.0, Culture=neutral, PublicKeyToken=null",
            () => new Texture2DAtlasReader()
        );
#endif

        Resolution.Init(new ResolutionComponent(this, Graphics, new Point(World.W, World.H),
            new Point(2560, 1440), false, false, false));
    }

    protected override void Initialize() {
        // temp
        //GuiBoxesHigh.Add(new GuiBox(World.W2 - 880, World.W2 + 880, World.H2 - 400, World.H2 + 400));
        //GuiBoxChainsHigh.Add(new GuiBoxChain(400, 1600, 500, 600, 120, 140, 180, 200, 80, 60, 130));
        //GuiBoxBarsHigh.Add(new GuiBoxBar(400, 1600, 800, 900, Color.Red, Color.Green, Color.Blue));

        base.Initialize();

        NavPath.Add(States.MainMenu);

#if NATIVE_AOT
        celosiaMain = new Celosia.Main();
        celosiaMain.Initialize();
#else
        ModLoader.InitializeAllMods();
#endif
    }

    protected override void LoadContent() {
        _bg = Content.Load<Texture2D>("img/bg");
        IconsAtlas = Content.Load<Texture2DAtlas>("img/icons");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        // Toggle debug info overlay
        _isDebugInfoEnabled ^= Input.CheckInput(Keybinds.DebugInfo);

        MenuDebug.HandleDebugInfo(_isDebugInfoEnabled, gameTime);

        // Switch input prompt between kb/controller
        if (Input.InputDeviceChanged) NavPath.UpdateInputPrompt();

        // Update the current State
        NavPath.GetState().Update(gameTime);

        base.Update(gameTime);

#if NATIVE_AOT
        celosiaMain.Update(gameTime);
#else
        ModLoader.UpdateAllMods(gameTime);
#endif
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);


        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            null, null, null, Resolution.TransformationMatrix());

        ShapeBatch.Begin(Resolution.TransformationMatrix());

        //SpriteBatch.Draw(bg, Vector2.Zero, Color.White);

        //Console.WriteLine(KoruriSystem.Atlases.Count); //todo test

        StageBase.Draw(gameTime);

        // Draw the current State
        NavPath.GetState().Draw(gameTime);

        StageSuper.Draw(gameTime);

        // temp
        //foreach (GuiBox label in GuiBoxesHigh) label.Draw(gameTime);
        //foreach (GuiBoxChain label in GuiBoxChainsHigh) label.Draw(gameTime);
        //foreach (GuiBoxBar label in GuiBoxBarsHigh) label.Draw(gameTime);

        SpriteBatch.End();
        ShapeBatch.End();

        base.Draw(gameTime);
    }
}