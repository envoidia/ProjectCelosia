using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Modding;
using API.Util;
using Apos.Shapes;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using ResolutionBuddy;

namespace API;

/// <summary>
/// Core of the game
/// </summary>
// todo internalify
public class Core : Game {
    /// <summary>
    /// Global instance of Core
    /// </summary>
    public static Core Instance { get; private set; } = null!;

    /// <summary>
    /// "Mod ID" of Core
    /// </summary>
    public const string Id = "__API";

    /// <summary>
    /// Mod ID of the base mod
    /// </summary>
    public const string BaseModId = "__Celosia";

    /// <summary>
    /// Mod ID of Core and the base mod
    /// </summary>
    public static readonly string[] ReservedIds = [Id, BaseModId];

    #region Rendering

    public static GraphicsDeviceManager Graphics { get; private set; } = null!;
    public new static GraphicsDevice GraphicsDevice { get; private set; } = null!;
    public static SpriteBatch SpriteBatch { get; private set; } = null!;
    public static ShapeBatch ShapeBatch { get; private set; } = null!;
    public static readonly Dictionary<string, Texture2DRegion> TextureCache = [];

    public static Texture2DAtlas IconsAtlas { get; set; } = null!;

    // Fonts
    public static FontSystem KoruriSystem { get; set; } = null!;
    public static DynamicSpriteFont Koruri60 { get; private set; } = null!;
    public static DynamicSpriteFont Koruri40 { get; private set; } = null!;

    public static bool ExitOnEscape { get; set; } = false;

    #endregion

    // temp debug
    public static Battle.Battle battle = null!;

    static Core() {
        // Setup font
        // todo
        //FontSystemDefaults.TextureWidth = 4096;
        //FontSystemDefaults.TextureHeight = 4096;

        // todo try bold font? diff font entirely?
        KoruriSystem = new FontSystem();
        FontSystemDefaults.FontResolutionFactor = 2f;
        FontSystemDefaults.KernelWidth = 2;
        FontSystemDefaults.KernelHeight = 2;

        KoruriSystem = new FontSystem();
        KoruriSystem.AddFont(File.ReadAllBytes("Font/koruri.ttf"));
        Koruri60 = KoruriSystem.GetFont(60);
        Koruri40 = KoruriSystem.GetFont(40);

        // Images in text
        RichTextDefaults.ImageResolver = static str => {
            if (TextureCache.TryGetValue(str, out Texture2DRegion? region)) {
                return new TextureFragmentColored(region.Texture, region.Bounds);
            }

            region = IconsAtlas.GetRegion(str);

            // Cache the region for future use
            TextureCache[str] = region;

            return new TextureFragmentColored(region.Texture, region.Bounds);
        };

#if !NATIVE_AOT
        ModLoader._LoadAllMods();
#else
        // Prevent crash caused by reflection in the atlas reader
        // Make sure to change this after updating MGE
        // todo can i write it without ()
        // todo can this be in static ctor
        // todo can the lambda be static
        ContentTypeReaderManager.AddTypeCreator(
            "MonoGame.Extended.Content.ContentReaders.Texture2DAtlasReader, MonoGame.Extended, Version=5.2.0.0, Culture=neutral, PublicKeyToken=null",
            () => new Texture2DAtlasReader());

        ContentTypeReaderManager.AddTypeCreator(
            "Microsoft.Xna.Framework.Content.EffectReader, MonoGame.Framework, Version=3.8.4.0, Culture=neutral, PublicKeyToken=null",
            () => new Texture2DAtlasReader());

        // todo fix Apos.Shapes AOT crash
        // todo: Force Celosia.Main to be loaded
        // The easy way is to call a dummy method from it
#endif

#if DEBUG
        ExitOnEscape = true;
#endif
    }

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    public Core(string title) {
        // Ensure that multiple cores are not created
        Debug.Assert(Instance is null, "Only a single instance of Core should be created");

        // Store reference to engine for global member access
        Instance = this;

        // Create a new graphics device manager
        Graphics = new GraphicsDeviceManager(this) {
            SynchronizeWithVerticalRetrace = false, // Vsync
            GraphicsProfile = GraphicsProfile.HiDef
            //PreferMultiSampling = true
        };

        // todo settings
        this.IsFixedTimeStep = false;
        // todo TargetElapsedTime

        // Apply the graphic presentation changes.
        //Graphics.PreferMultiSampling = true;
        Graphics.ApplyChanges();

        // Set the window title.
        this.Window.Title = title;

        // Set the root directory for content.
        this.Content.RootDirectory = "Content";

        // Scaling
        Resolution.Init(new ResolutionComponent(this, Graphics, new(World.W, World.H),
            new(2560, 1440), true, false, false));

#if DEBUG
        this.IsMouseVisible = true;
#endif
    }

    protected override void Initialize() {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create sprite and shape batches
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        ShapeBatch = new ShapeBatch(GraphicsDevice, this.Content);

        StateMachine.Add(States.MainMenu);
    }

    // Update is called before Draw
    protected override void Update(GameTime gameTime) {
        InputLib.Update(gameTime);
        DebugUtil._Update(gameTime);

        // Update the current State
        StateMachine.GetState().Update(gameTime);

#if !NATIVE_AOT
        ModLoader._UpdateAllMods(gameTime);
#endif
        // todo: if AOT and Celosia gets an Update: Celosia.Main.Mod.OnUpdate(gameTime);

        if (ExitOnEscape && InputLib._KeyboardState.IsKeyDown(Keys.Escape)) {
            this.Exit();
        }

        base.Update(gameTime);
    }

    protected override void LoadContent() {
        IconsAtlas = this.Content.Load<Texture2DAtlas>("img/icons");

        base.LoadContent();
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        //Console.WriteLine(KoruriSystem.Atlases.Count); //todo test

        // Act Actors
        // todo: avoid logic in draw by moving routines out of draw and also moving widget input out of draw?
        Stage.Act(gameTime);

        base.Draw(gameTime);
    }
}