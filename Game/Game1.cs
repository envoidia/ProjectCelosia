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

    // temp
    //Matrices for 3D perspective
    private Matrix worldMatrix, viewMatrix, projectionMatrix;

    // Vertex data for rendering
    private VertexPositionColor[] triangleVertices;

// A Vertex format structure that contains position, normal data, and one set of texture coordinates
    private BasicEffect basicEffect;

    // Matrix to translate the drawn primitives to the center of the screen.
    private Matrix translationMatrix;

// Number of vertex points to draw the primitive with.
    private int points = 8;

// The length of the primitive lines to draw.
    private int lineLength = 100;

    // The vertex sata array.
    private VertexPositionColor[] primitiveList;
    private short[] triangleStripIndices;
    private int triangleWidth = 10;
    private int triangleHeight = 10;

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
        // Prevent crash caused by reflection in the atlas reader
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
        this.worldMatrix = Matrix.Identity;

        this.viewMatrix = Matrix.CreateLookAt(
            new Vector3(0, 0, 50),
            Vector3.Zero,
            Vector3.Up
        );

        this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            16 / 9f,
            1.0f, 300.0f);

        /*projectionMatrix = Matrix.CreateOrthographicOffCenter(
            0,
            World.W,
            World.H,
            0,
            1.0f, 1000.0f);*/

        // Calculate the center of the visible screen using the ViewPort.
        Vector2 screenCenter = new(World.W2, World.H2);
        // Calculate the center of the primitives to be drawn.
        Vector2 primitiveCenter = new((((this.points / 2) - 1) * this.lineLength) / 2, this.lineLength / 2);
        // Create a translation matrix to position the drawn primitives in the center of the screen and the center of the primitives.
        this.translationMatrix =
            Matrix.CreateTranslation(screenCenter.X - primitiveCenter.X, screenCenter.Y - primitiveCenter.Y, 0);

        // Initialize an array of indices of type short.
        this.triangleStripIndices = new short[this.points];

        // Populate the array with references to indices in the vertex buffer.
        for (int i = 0; i < this.points; i++) {
            this.triangleStripIndices[i] = (short) i;
        }


        base.Initialize();

        RasterizerState rasterizerState = new();
        rasterizerState.CullMode = CullMode.None;
        GraphicsDevice.RasterizerState = rasterizerState;

        AddMenu(Main);

#if NATIVE_AOT
        celosiaMain = new Celosia.Main();
        celosiaMain.Initialize();
#else
        ModLoader.InitializeAllMods();
#endif

        TestLabel.Text = "";
    }

    protected override void LoadContent() {
        this.basicEffect = new BasicEffect(Graphics.GraphicsDevice);

        this.basicEffect.World = this.worldMatrix;
        this.basicEffect.View = this.viewMatrix;
        this.basicEffect.Projection = this.projectionMatrix;

        // primitive color
        this.basicEffect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
        this.basicEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
        this.basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
        this.basicEffect.SpecularPower = 5.0f;
        this.basicEffect.Alpha = 1.0f;
        // The following MUST be enabled if you want to color your vertices
        this.basicEffect.VertexColorEnabled = true;

        // Use the built in 3 lighting mode provided with BasicEffect            
        this.basicEffect.EnableDefaultLighting();

        this.triangleVertices = new VertexPositionColor[3];

        this.triangleVertices[0].Position = new Vector3(0f, 0f, 0f);
        this.triangleVertices[0].Color = Color.Red;
        this.triangleVertices[1].Position = new Vector3(10f, 10f, 0f);
        this.triangleVertices[1].Color = Color.Yellow;
        this.triangleVertices[2].Position = new Vector3(10f, 0f, -5f);
        this.triangleVertices[2].Color = Color.Green;

        this.primitiveList = new VertexPositionColor[this.points];

        for (int x = 0; x < (this.points / 2); x++) {
            for (int y = 0; y < 2; y++) {
                this.primitiveList[(x * 2) + y] = new VertexPositionColor(
                    new Vector3(x * this.lineLength, y * this.lineLength, 0), Color.White);
            }
        }

        // Translate the position of the vertices by the translation matrix calculated earlier.
        for (int i = 0; i < this.primitiveList.Length; i++) {
            this.primitiveList[i].Position = Vector3.Transform(this.primitiveList[i].Position, this.translationMatrix);
        }

        bg = Content.Load<Texture2D>("img/bg");
        IconsAtlas = Content.Load<Texture2DAtlas>("img/icons");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        this.CheckInput(gameTime);
        base.Update(gameTime);

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

        foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes) {
            pass.Apply();

            GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleStrip, this.primitiveList,
                0, // vertex buffer offset to add to each element of the index buffer
                8, // number of vertices to draw
                this.triangleStripIndices,
                0, // first index element to read
                6 // number of primitives to draw
            );


            GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList, this.triangleVertices,
                0,
                1,
                VertexPositionColor.VertexDeclaration
            );
        }

        base.Draw(gameTime);
    }

    private static void DrawRenderPriority(List<Label> labels) {
        foreach (Label label in labels) label.Draw(SpriteBatch);
    }
}