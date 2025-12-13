using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using API.Battle;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using API.Modding;
using Apos.Shapes;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using ResolutionBuddy;

namespace API;

public class Core : Game {
    public static Core Instance { get; private set; } = null!;

    #region Rendering

    public static GraphicsDeviceManager Graphics { get; private set; } = null!;
    public new static GraphicsDevice GraphicsDevice { get; private set; } = null!;
    public static SpriteBatch SpriteBatch { get; private set; } = null!;
    public static ShapeBatch ShapeBatch { get; private set; } = null!;
    public static Texture2D WhitePixel { get; private set; } = null!;
    public static readonly Dictionary<string, Texture2DRegion> TextureCache = [];

    public static Texture2DAtlas IconsAtlas { get; set; } = null!;

    // Fonts
    public static FontSystem KoruriSystem { get; set; } = null!;
    public static DynamicSpriteFont Koruri60 { get; private set; } = null!;

    public static bool ExitOnEscape { get; set; } = false;

    #endregion

    #region _IModItem Lists
    // todo add custom IModItem categories?
    public static readonly List<Accessory> Accessories = [];
    public static readonly List<BoolStat> BoolStats = [];
    public static readonly List<Buff> Buffs = [];
    public static readonly List<Element> Elements = [];
    public static readonly List<Mult> Mults = [];
    public static readonly List<Passive> Passives = [];
    public static readonly List<Battle.Range> Ranges = [];
    public static readonly List<Skill> Skills = [];
    public static readonly List<SkillType> SkillTypes = [];
    public static readonly List<StageType> StageTypes = [];
    public static readonly List<StatMod> StatMods = [];
    public static readonly List<Stat> Stats = [];
    public static readonly List<UnitType> UnitTypes = [];
    public static readonly List<Weapon> Weapons = [];

    #endregion

    // temp debug
    public static Battle.Battle battle = null!;

    // Debug
    private static bool _isDebugInfoEnabled;

    static Core() {
        // Setup font
        // todo
        //FontSystemDefaults.TextureWidth = 4096;
        //FontSystemDefaults.TextureHeight = 4096;

        KoruriSystem = new FontSystem();
        FontSystemDefaults.FontResolutionFactor = 2f;
        FontSystemDefaults.KernelWidth = 2;
        FontSystemDefaults.KernelHeight = 2;

        KoruriSystem = new FontSystem();
        KoruriSystem.AddFont(File.ReadAllBytes("Font/koruri.ttf"));
        Koruri60 = KoruriSystem.GetFont(60);

        // Images in text
        RichTextDefaults.ImageResolver = str => {
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
        Resolution.Init(new ResolutionComponent(this, Graphics, new Point(World.W, World.H),
            new Point(2560, 1440), true, false, false));

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

        WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
        WhitePixel.SetData([Color.White]);

        StateMachine.Add(States.MainMenu);
    }

    // Update is called before Draw
    protected override void Update(GameTime gameTime) {
        // Update the input manager.
        InputLib.Update(gameTime);

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

        DebugMenu._CheckDebugHotkeys();

        // Switch input prompt between kb/controller
        if (InputLib.InputDeviceChanged) StateMachine.UpdateInputPrompt();

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
        Stage.Act(gameTime);

        base.Draw(gameTime);
    }
}