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

    public Game1() : base("Project Celosia") {
#if NATIVE_AOT
        // Prevent crash caused by reflection in the atlas reader
        // Make sure to change this after updating MGE
        // todo can i write it without ()
        ContentTypeReaderManager.AddTypeCreator(
            "MonoGame.Extended.Content.ContentReaders.Texture2DAtlasReader, MonoGame.Extended, Version=5.2.0.0, Culture=neutral, PublicKeyToken=null",
            () => new Texture2DAtlasReader());

        ContentTypeReaderManager.AddTypeCreator(
            "Microsoft.Xna.Framework.Content.EffectReader, MonoGame.Framework, Version=3.8.4.0, Culture=neutral, PublicKeyToken=null",
            () => new Texture2DAtlasReader());

        // todo fix Apos.Shapes AOT crash
#endif

        Resolution.Init(new ResolutionComponent(this, Graphics, new Point(World.W, World.H),
            new Point(1920, 1080), false, false, false));
    }

    protected override void Initialize() {
        base.Initialize();

        StateMachine.Add(States.MainMenu);

#if !NATIVE_AOT
        ModLoader.LoadAllMods();
#else
        // todo: Force Celosia.Main to be loaded
        // The easy way is to call a dummy method from it
#endif
    }

    protected override void LoadContent() {
        _bg = this.Content.Load<Texture2D>("img/bg");
        IconsAtlas = this.Content.Load<Texture2DAtlas>("img/icons");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        // Toggle debug info overlay
        if (InputLib.Check(Keybinds.DebugInfo)) {
            if (!_isDebugInfoEnabled) {
                _isDebugInfoEnabled = true;
                DebugMenu.Create();
            } else {
                _isDebugInfoEnabled = false;
                DebugMenu.Destroy();
            }
        }

        if (_isDebugInfoEnabled) DebugMenu.Update(gameTime);

        // Switch input prompt between kb/controller
        if (InputLib.InputDeviceChanged) StateMachine.UpdateInputPrompt();

        // Update the current State
        StateMachine.GetState().OnUpdate?.Invoke(gameTime);

        base.Update(gameTime);

#if !NATIVE_AOT
        ModLoader.UpdateAllMods(gameTime);
#endif
        // todo: if AOT and Celosia gets an Update: Celosia.Main.Mod.OnUpdate(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        //Console.WriteLine(KoruriSystem.Atlases.Count); //todo test

        // Act Actors
        Stage.Act(gameTime);

        base.Draw(gameTime);
    }
}