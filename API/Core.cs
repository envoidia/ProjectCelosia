using System;
using System.Collections.Generic;
using System.IO;
using API.Battle;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Save;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;

namespace API;

public class Core : Game {
    internal static Core sInstance;

    public static Core Instance => sInstance;

    // Rendering
    public static GraphicsDeviceManager Graphics { get; private set; }
    public new static GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static Texture2D WhitePixel { get; private set; }
    public static readonly Dictionary<string, Texture2DRegion> TextureCache = new();

    public static Texture2DAtlas IconsAtlas { get; set; }

    // Fonts
    public static FontSystem KoruriSystem { get; set; }
    public static DynamicSpriteFont Koruri50 { get; private set; }

    public new static ContentManager Content { get; private set; }

    public static InputManager Input { get; private set; }

    public static bool ExitOnEscape { get; set; }

    // Menu stuff
    /// <summary>
    /// List of menus that have been traveled through to reach the current menu location
    /// </summary>
    public static readonly Stack<MenuType> NavPath = new();

    // Lists of things to render, in order

    // Low Prio
    // todo sprites
    // todo shapes
    public static readonly List<Label> LabelsLow = [];

    // Med Prio
    // todo sprites
    // todo shapes
    public static readonly List<Label> LabelsMed = [];

    // High Prio
    // todo sprites
    // todo shapes
    public static readonly List<Label> LabelsHigh = [];

    private static Label inputPrompt;

    // Lists of other stuff
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


    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen) {
        // Ensure that multiple cores are not created.
        // You would think this should just be static, but I didn't write MonoGame. There's probably a good reason
        if (sInstance is not null) {
            throw new InvalidOperationException("Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        sInstance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // todo settings
        Graphics.SynchronizeWithVerticalRetrace = false; // Vsync
        this.IsFixedTimeStep = false;
        // todo TargetElapsedTime

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
        Koruri50 = KoruriSystem.GetFont(50);

        // Apply the graphic presentation changes.
        Graphics.ApplyChanges();

        // Set the window title.
        this.Window.Title = title;

        // Set the core's content manager to a reference of the base Game's content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Setup stuff
        RichTextDefaults.ImageResolver = p => {
            if (TextureCache.TryGetValue(p, out Texture2DRegion? region)) {
                return new TextureFragmentColored(region.Texture, region.Bounds);
            }

            region = IconsAtlas.GetRegion(p);

            // Cache the region for future use
            TextureCache[p] = region;

            return new TextureFragmentColored(region.Texture, region.Bounds);
        };

        inputPrompt = new Label {
            Position = World.Vec - new Vector2(10, 10),
            Alignment = Alignment.BottomRight,
            HasBackground = true
        };

        //ContentTypeReaderManager.AddTypeCreator("MonoGame.Extended.Content.ContentReaders.Texture2DAtlasReader", () => new Texture2DAtlasReader());
#if DEBUG
        Settings.EnableModLoader = true;
        this.IsMouseVisible = true;
        ExitOnEscape = true;
#endif
    }

    protected override void Initialize() {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
        WhitePixel.SetData([Color.White]);

        // Create a new input manager.
        Input = new InputManager();
    }

    protected override void Update(GameTime gameTime) {
        // Update the input manager.
        Input.Update(gameTime);

        if (ExitOnEscape && Input.KeyboardState.IsKeyDown(Keys.Escape)) {
            this.Exit();
        }

        base.Update(gameTime);
    }

    public static void AddMenu(MenuType menuType) {
        NavPath.Push(menuType);
        UpdateInputPrompt();
    }

    public static void RemoveMenu() {
        NavPath.Pop();
        UpdateInputPrompt();
    }

    public static void UpdateInputPrompt() {
        inputPrompt.Text = NavPath.Peek().GetInputPrompt();
    }
}