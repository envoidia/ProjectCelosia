using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using API;
using API.Debug;
using API.Graphics;
using API.Input;
using API.Menu;
using FontStashSharp.RichText;
using ResolutionBuddy;

namespace Game;

public class Game1 : Core {
    // Rendering
    private IResolution _resolution; // Ignore unused warning

    private Texture2D _bg;

    private AnimatedSprite _slime;
    private AnimatedSprite _bat;

    // Menu stuff
    private int _index;

    // Debug
    private bool _isDebugInfoEnabled = false;

    public Game1() : base("Project Celosia", 0, 0, false) {
        this._resolution = new ResolutionComponent(this, Graphics, new Point(1920, 1080),
            new Point(1920, 1080), false, false, false);
    }

    private RichTextLayout _richTextLayout;

    private const string TestStr = """
                                   Johny's  Ignis affinity 0 [TODO] +1
                                   James'  Ignis affinity 0 [TODO] +1
                                   Julia's  Ignis affinity 0 [TODO] +1
                                   Josephine's  Ignis affinity 0 [TODO] +1
                                   Jerry's  Ignis affinity 0 [TODO] +1
                                   Jacob's  Ignis affinity 0 [TODO] +1
                                   Jude's  Ignis affinity 0 [TODO] +1
                                   Julian's  Ignis affinity 0 [TODO] +1
                                   Johny's Debuff Given Duration 0 [TODO] +1
                                   Johny's Percentage Dmg Taken 100% [TODO] 50% (-50%)
                                   Johny's  Ignis affinity +1 [TODO] +2
                                   James' Debuff Given Duration 0 [TODO] +1
                                   James' SP 200 [TODO] [TODO]
                                   James'  Ignis affinity +1 [TODO] +2
                                   Julia's Debuff Given Duration 0 [TODO] +1
                                   Julia's  Ignis affinity +1 [TODO] +2
                                   Josephine's Debuff Given Duration 0 [TODO] +1
                                   Josephine's  Ignis affinity +1 [TODO] +2
                                   Jerry's Debuff Given Duration 0 [TODO] +1
                                   Jerry's  Ignis affinity +1 [TODO] +2
                                   Jacob's Debuff Given Duration 0 [TODO] +1
                                   Jacob's  Ignis affinity +1 [TODO] +2
                                   Jude's Debuff Given Duration 0 [TODO] +1
                                   Jude's Percentage Dmg Taken 100% [TODO] 0.1% (-99.9%)
                                   Jude's  Ignis affinity +1 [TODO] +2
                                   Julian's Debuff Given Duration 0 [TODO] +1
                                   Julian's  Ignis affinity +1 [TODO] +2
                                   Turn 1
                                   All units gain 100 SP; both teams gain 100 Bloom
                                   Jacob tries to use  Defend, but can't reach
                                   Josephine uses  Defend 
                                   Josephine gains Defend for 1 turn
                                   Josephine's Shield 0 [TODO] 1,400/7,000 (+1,400)
                                   Josephine's Dmg Taken 100% [TODO] 80% (-20%)
                                   Josephine's SP 200 [TODO] 270 (+70)
                                   James uses  Group Attack Up on Jacob 
                                   Jacob's Atk Stage 0 [TODO] +2, turns 0  [TODO] 5 (Str 11,500 [TODO] 13,800/11,500 (+2,300), Mag 11,500 [TODO] 13,800/11,500 (+2,300))
                                   Jerry's Atk Stage 0 [TODO] +2, turns 0  [TODO] 5 (Str 10,000 [TODO] 12,000/10,000 (+2,000), Mag 10,000 [TODO] 12,000/10,000 (+2,000))
                                   Jude's Atk Stage 0 [TODO] +2, turns 0  [TODO] 5 (Str 11,000 [TODO] 13,200/11,000 (+2,200), Mag 11,000 [TODO] 13,200/11,000 (+2,200))
                                   Jude tries to use  Ice Age on Johny, but doesn't have enough Bloom
                                   Jerry tries to use  Group Attack Up on Julia, but can't reach
                                   Johny uses  Group Agility Up on James 
                                   James' Agi Stage 0 [TODO] +2, turns 0  [TODO] 5 (Agi 14,830 [TODO] 17,796/14,830 (+2,966))
                                   Johny's Agi Stage 0 [TODO] +2, turns 0  [TODO] 5 (Agi 9,500 [TODO] 11,400/9,500 (+1,900))
                                   Julia's Agi Stage 0 [TODO] +2, turns 0  [TODO] 5 (Agi 8,500 [TODO] 10,200/8,500 (+1,700))
                                   Julia uses  Heat Wave on Jude 
                                   Jude's  HP 11,000 [TODO] 10,560/11,000 (-440)
                                   Jacob's  HP 11,500 [TODO] 11,080/11,500 (-420)
                                   Julian's  HP 9,500 [TODO] 8,990/9,500 (-510)
                                   Jude gains Burn with 1 stack and 3 turns
                                   Jude's Str 13,200 [TODO] 12,650/11,000 (-550)
                                   Julian uses  Demon Scythe on Julia 
                                   Julia's  HP 8,500 [TODO] 7,950/8,500 (-550)
                                   Julia gains Curse with 1 stack and 3 turns
                                   Julia's Fth 8,500 [TODO] 8,075/8,500 (-425)
                                   Julia's  HP 7,950 [TODO] 7,400/8,500 (-550)
                                   Julia's Curse stacks 1 [TODO] 2
                                   Julia's Fth 8,075 [TODO] 7,650/8,500 (-425)
                                   Julia's Curse:  HP 7,400 [TODO] 6,975/8,500 (-425)
                                   Josephine loses Defend 
                                   Josephine loses 1,400 Shield
                                   Josephine's Dmg Taken 80% [TODO] 100% (+20%)
                                   Jude's Burn: No effect on 
                                   Turn 2
                                   All units gain 100 SP; both teams gain 100 Bloom
                                   James uses  Defend 
                                   James gains Defend for 1 turn
                                   James' Shield 0 [TODO] 2,600/13,000 (+2,600)
                                   James' Dmg Taken 100% [TODO] 80% (-20%)
                                   James' SP 300 [TODO] 370 (+70)
                                   Jude tries to use  Defend, but can't reach
                                   Johny uses  Defend 
                                   Johny gains Defend for 1 turn
                                   Johny's Shield 0 [TODO] 1,900/9,500 (+1,900)
                                   Johny's Dmg Taken 100% [TODO] 80% (-20%)
                                   Johny's SP 150 [TODO] 220 (+70)
                                   Julia uses  Defend 
                                   Julia gains Defend for 1 turn
                                   Julia's Shield 0 [TODO] 1,700/8,500 (+1,700)
                                   Julia's Dmg Taken 100% [TODO] 80% (-20%)
                                   Julia's SP 138 [TODO] 208 (+70)
                                   Josephine uses  Defend 
                                   Josephine gains Defend for 1 turn
                                   Josephine's Shield 0 [TODO] 1,400/7,000 (+1,400)
                                   Josephine's Dmg Taken 100% [TODO] 80% (-20%)
                                   Josephine's SP 370 [TODO] 440 (+70)
                                   Jacob uses  Demon Scythe on James 
                                   James'  Shield 2,600 [TODO] 2,180/13,000 (-420)
                                   James'  Shield 2,180 [TODO] 1,760/13,000 (-420)
                                   Jerry tries to use  Ice Age on Julia, but doesn't have enough Bloom
                                   Julian tries to use  Ice Age on James, but doesn't have enough Bloom
                                   Johny loses Defend 
                                   Johny loses 1,900 Shield
                                   Johny's Dmg Taken 80% [TODO] 100% (+20%)
                                   James loses Defend 
                                   James loses 1,760 Shield
                                   James' Dmg Taken 80% [TODO] 100% (+20%)
                                   Julia's Curse:  Shield 1,700 [TODO] 1,360/8,500 (-340)
                                   Julia loses Defend 
                                   Julia loses 1,360 Shield
                                   Julia's Dmg Taken 80% [TODO] 100% (+20%)
                                   Josephine loses Defend 
                                   Josephine loses 1,400 Shield
                                   Josephine's Dmg Taken 80% [TODO] 100% (+20%)
                                   Jude's Burn: No effect on 
                                   Turn 3
                                   All units gain 100 SP; both teams gain 100 Bloom
                                   """;

    protected override void Initialize() {
        NavPath.Push(MenuType.Main);

        base.Initialize();
    }

    protected override void LoadContent() {
        this._bg = Content.Load<Texture2D>("img/bg");

        TextureAtlas atlas = TextureAtlas.FromFile(Content, "img/atlas-definition.xml");
        
        this._slime = atlas.CreateAnimatedSprite("slime-animation");
        this._slime.Scale = new Vector2(4.0f, 4.0f);
        this._bat = atlas.CreateAnimatedSprite("bat-animation");
        this._bat.Scale = new Vector2(4.0f, 4.0f);
        
        this._richTextLayout = new RichTextLayout {
            Font = Koruri30,
            Text =
                "A small tree: /i[eating.png] :3c\namong us susssy among us roblox forntite vbucks adkfhsajkasljdlskajdkahsfdjkashdlasjdlkjas\nfwejfweifuowefipwef/i[eating.png]\nsdhjfiousdhfowuefopiuwepofiew[opifopweiufouwrpofg",
            Width = 800
        };

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            this.Exit();
        }

        this.CheckInput(gameTime);


        this._bat.Update(gameTime);
        this._slime.Update(gameTime);

        base.Update(gameTime);
    }

    private void CheckInput(GameTime gameTime) {
        this._isDebugInfoEnabled ^= Input.CheckInput(Keybind.DebugInfo);
        DebugMenu.HandleDebugInfo(this._isDebugInfoEnabled, gameTime);

        switch (NavPath.Peek()) {
            case MenuType.Main:
                this._index = MenuLib.CheckMovement1D(this._index, 5);
                //Console.WriteLine(this._index);
                //Console.WriteLine(1.0f / gameTime.ElapsedGameTime.TotalSeconds);
                // update cursor

                if (Input.CheckInput(Keybind.Confirm)) {
                    // continue based on selected option
                } else if (Input.CheckInput(Keybind.Back)) {
                    // if (index == last) quit, else index = last
                }

                break;
            case MenuType.Popup:
                if (Input.CheckInput(Keybind.Back)) {
                    // cancel
                }

                break;
            case MenuType.Battle:
            case MenuType.Targeting:
            case MenuType.Log:
            case MenuType.InspectTargeting:
            case MenuType.Inspect:
                // Pass to BattleHandler
                break;
            case MenuType.Debug:
            case MenuType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(NavPath.Peek().ToString());
        }
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
            null, null, Resolution.TransformationMatrix());

        SpriteBatch.Draw(this._bg, Vector2.Zero, Color.White);
        this._slime.Draw(SpriteBatch, Vector2.One);
        this._bat.Draw(SpriteBatch, new Vector2(this._slime.Width + 10, 0));

        // todo wip
        /*SpriteBatch.DrawString(
            Koruri25,              // spriteFont
            TestStr, // text
            Vector2.Zero, // position
            Color.White        // color
        );*/

        this._richTextLayout.Draw(SpriteBatch, new Vector2(0, 0), Color.White);
        //Console.WriteLine(KoruriSystem.Atlases.Count); todo test with more diverse chars

        DrawRenderPriority(LabelsLow);
        DrawRenderPriority(LabelsMed);
        DrawRenderPriority(LabelsHigh);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

    private static void DrawRenderPriority(List<Label> labels) {
        foreach (Label label in labels) {
            label.Draw(SpriteBatch);
        }
    }
}