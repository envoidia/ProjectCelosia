using System;
using System.Collections.Generic;
using System.IO;
using API.Debug;
using API.Graphics;
using API.Input;
using API.Menu;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace API;

public class Core : Game {
    internal static Core sInstance;

    public static Core Instance => sInstance;

    // Rendering
    public static GraphicsDeviceManager Graphics { get; private set; }
    public new static GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static Texture2D WhitePixel { get; private set; }

    // Fonts
    private static FontSystem KoruriSystem { get; set; }
    public static SpriteFontBase Koruri25 { get; private set; }
    public static SpriteFontBase Koruri30 { get; private set; }

    public new static ContentManager Content { get; private set; }

    public static InputManager Input { get; private set; }

    public static bool ExitOnEscape { get; set; }

    // Menu stuff
    /// <summary>
    /// List of menus that have been traveled through to reach the current menu location
    /// </summary>
    public static readonly IList<MenuType> NavPath = new List<MenuType>();

    // Lists of things to render, in order

    // Low Prio
    // todo sprites
    // todo shapes
    public static readonly IList<Label> LabelsLow = new List<Label>();

    // Med Prio
    // todo sprites
    // todo shapes
    public static readonly IList<Label> LabelsMed = new List<Label>();

    // High Prio
    // todo sprites
    // todo shapes
    public static readonly IList<Label> LabelsHigh = new List<Label>();

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen) {
        // Ensure that multiple cores are not created.
        if (sInstance != null) {
            throw new InvalidOperationException($"Only a single Core instance can be created");
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
        Graphics.SynchronizeWithVerticalRetrace = true; // Vsync
        this.IsFixedTimeStep = false;

        // Setup font
        KoruriSystem = new FontSystem();
        KoruriSystem.AddFont(File.ReadAllBytes(@"fnt/koruri.ttf"));
        Koruri25 = KoruriSystem.GetFont(25);
        Koruri30 = KoruriSystem.GetFont(30);

        // Apply the graphic presentation changes.
        Graphics.ApplyChanges();

        // Set the window title.
        this.Window.Title = title;

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

#if DEBUG
        this.IsMouseVisible = true;
        ExitOnEscape = true;
#endif
    }

    protected override void Initialize() {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
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
}