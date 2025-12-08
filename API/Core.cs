using System;
using System.Collections.Generic;
using System.IO;
using API.Battle;
using API.Graphics;
using API.Input;
using Apos.Shapes;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;

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
    public static DynamicSpriteFont Koruri50 { get; private set; } = null!;

    // todo should this be hiding
    public new static ContentManager Content { get; private set; } = null!;

    public static bool ExitOnEscape { get; set; }

    #endregion

    #region IModItem Lists

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

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen) {
        // Ensure that multiple cores are not created
        if (Instance is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(Core)));
        }

        // Store reference to engine for global member access
        Instance = this;

        // Create a new graphics device manager
        Graphics = new GraphicsDeviceManager(this) {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
            IsFullScreen = fullScreen,
            SynchronizeWithVerticalRetrace = false, // Vsync
            GraphicsProfile = GraphicsProfile.HiDef
            //PreferMultiSampling = true
        };

        // todo settings
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
        //Graphics.PreferMultiSampling = true;
        Graphics.ApplyChanges();

        // Set the window title.
        this.Window.Title = title;

        // Set the core's content manager to a reference of the base Game's content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Setup stuff

        RichTextDefaults.ImageResolver = str => {
            if (TextureCache.TryGetValue(str, out Texture2DRegion? region)) {
                return new TextureFragmentColored(region.Texture, region.Bounds);
            }

            region = IconsAtlas.GetRegion(str);

            // Cache the region for future use
            TextureCache[str] = region;

            return new TextureFragmentColored(region.Texture, region.Bounds);
        };

#if DEBUG
        this.IsMouseVisible = true;
        ExitOnEscape = true;
#endif
    }

    protected override void Initialize() {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create sprite and shape batches
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        ShapeBatch = new ShapeBatch(GraphicsDevice, Content);

        WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
        WhitePixel.SetData([Color.White]);

        // Sort stages
        Stages.Base.Sort();
        Stages.Super.Sort();
    }

    protected override void Update(GameTime gameTime) {
        // Update the input manager.
        InputLib.Update(gameTime);

        if (ExitOnEscape && InputLib._KeyboardState.IsKeyDown(Keys.Escape)) {
            this.Exit();
        }

        base.Update(gameTime);
    }
}