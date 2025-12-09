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

#if NATIVE_AOT
    private static Celosia.Main _celosiaMain = null!;
#endif

    public Game1() : base("Project Celosia", 0, 0, false) {
#if NATIVE_AOT
        // Prevent crash caused by reflection in the atlas reader
        // Make sure to change this after updating MGE
        // todo can i write it without ()
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

        StateMachine.Add(States.MainMenu);

#if NATIVE_AOT
        celosiaMain = new Celosia.Main();
        celosiaMain.Initialize();
#else
        ModLoader.LoadAllMods();
#endif
    }

    protected override void LoadContent() {
        _bg = Content.Load<Texture2D>("img/bg");
        IconsAtlas = Content.Load<Texture2DAtlas>("img/icons");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        // Toggle debug info overlay
        if (InputLib.Check(Keybinds.DebugInfo)) {
            if (!_isDebugInfoEnabled) {
                _isDebugInfoEnabled = true;
                MenuDebug.Create();
            } else {
                _isDebugInfoEnabled = false;
                MenuDebug.Destroy();
            }
        }

        if (_isDebugInfoEnabled) MenuDebug.Update(gameTime);

        // Switch input prompt between kb/controller
        if (InputLib.InputDeviceChanged) StateMachine.UpdateInputPrompt();

        // Update the current State
        StateMachine.GetState().Update(gameTime);

        base.Update(gameTime);

#if NATIVE_AOT
        celosiaMain.Update(gameTime);
#else
        ModLoader.UpdateAllMods(gameTime);
#endif
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        //Console.WriteLine(KoruriSystem.Atlases.Count); //todo test

        // Act Actors
        Stage.Act(gameTime);

        base.Draw(gameTime);
    }
}