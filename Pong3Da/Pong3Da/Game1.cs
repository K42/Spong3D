using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong3Da
{
    #region Enums
    public enum GameState
    { 
        MainMenu,
        SettingsMenu,
        MpMenu,
        InGameDuringRound,
        InGameAfterScore,
        GameOver
    }

    public enum GameType
    { 
        Split,
        TcpIP
    }
    #endregion
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont topFont;
        public Camera camera1 { get; protected set; }
        public Camera camera2 { get; protected set; }
        private Ball ball;
        private Player player1, player2;
        StaticModel sphere, aX, aY, aZ, SkyDome;
        BoundingSphere playDome;
        bool baal;
        bool ingame = false;
        //-------------------NIE RUSZAĆ------------------------
        GameState gs = new GameState();
        GameType gt = new GameType();
        MenuComponent menu;
        Song dubstep_intro;
        int x, y;
        int turn = -1;
        string[] menuItems = {"Resume", "Start duel", "Quit"};
        Texture2D intro_ball, logo, arrow, bg;
        Vector2 speed;
        Viewport mainViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Viewport topBarViewport;
        float aspectRatio;
        #endregion

        #region Constructor
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = false;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 500;
            aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth /  (2 * GraphicsDeviceManager.DefaultBackBufferHeight);
        }
        #endregion

        #region Initialization
        protected override void Initialize() {
            // TODO: Add your initialization logic here
           
            gs = GameState.MainMenu;
            gt = GameType.Split;
            base.Initialize();
        }
        #endregion

        #region Content
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sphere = new StaticModel(Content.Load<Model>(@"models\sfera2"), 0, 0, 0);
            aX = new StaticModel(Content.Load<Model>(@"models\ring"), 0, 0, 0);
            aY = new StaticModel(Content.Load<Model>(@"models\ring"), 90, 0, 0);
            aZ = new StaticModel(Content.Load<Model>(@"models\ring"), 90, 90, 0);
            SkyDome = new StaticModel(Content.Load<Model>(@"models\sfera2"));
            playDome = new BoundingSphere(Vector3.Zero, sphere.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            intro_ball = Content.Load<Texture2D>(@"textures/intro_ball");
            logo = Content.Load<Texture2D>(@"textures/spong");
            arrow = Content.Load<Texture2D>(@"textures/arrow");
            bg = Content.Load<Texture2D>(@"textures/sky");
            dubstep_intro = Content.Load<Song>(@"intro");            
           // dubstep_intro;
           // MediaPlayer.Play(dubstep_intro);
            menu = new MenuComponent(this, spriteBatch, Content.Load<SpriteFont>("menufont"), menuItems);
            Components.Add(menu);
            menu.Enabled = false;

            topFont = Content.Load<SpriteFont>("TopFont");
                       
            mainViewport = GraphicsDevice.Viewport;
            leftViewport = mainViewport;
            rightViewport = mainViewport;
            leftViewport.Width  /= 2;
            rightViewport.Width /= 2;
            rightViewport.X = leftViewport.Width;
            leftViewport.Height -= 100;
            rightViewport.Height -= 100;
            topBarViewport.Width = GraphicsDevice.Viewport.Width;
            topBarViewport.Height = 100;
            //leftViewport.Height = rightViewport.Height /= 2;
            leftViewport.Y += 100;
            rightViewport.Y += 100;

            Random rnd = new Random();
            x = rnd.Next(100, mainViewport.Width - 300);
            y = rnd.Next(100, mainViewport.Height - 300);
            speed = new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10));


            camera1 = new Camera(this, new Vector3(0, 0, 80), Vector3.Zero, Vector3.Up, 1, leftViewport);
            camera2 = new Camera(this, new Vector3(0, 0, -80), Vector3.Zero, Vector3.Up, 2, rightViewport);

            ball = new Ball(this);
            player1 = new Player(this, 1);
            player2 = new Player(this, 2);

            Components.Add(camera1);
            Components.Add(camera2);
            Components.Add(ball);

            //MediaPlayer.
            // Initialize vertices
            // Initialize the BasicEffect
            //effect = new BasicEffect(GraphicsDevice);
            // Set cullmode to none
            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rs;
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Logic
        protected override void Update(GameTime gameTime) {
            int p = 0;
            if(turn < 0) p = 1;
            else p = 2;
            Window.Title = "P1 " + player1.pts +
                " P2 " + player2.pts +
                " turn: player " + p +
                " rad: " + playDome.Radius;
                //" x = " + ball.position.X
                //+ " y = " + ball.position.Y
                //+ " z = " + ball.position.Z
                //+ " pitch = " + camera1.pitch
                //+ " maxPitch = " + camera1.maxPitch
                //+ " P1 pts = " + player1.pts;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) //this.Exit();        // Allows the game to exit
            {
                gs = GameState.MainMenu;
            }
            if (!baal && Keyboard.GetState().IsKeyDown(Keys.P)) {
                ball.freeze = !ball.freeze;
            }
            baal = Keyboard.GetState().IsKeyDown(Keys.P);
            BoundingSphere b = new BoundingSphere(ball.position, ball.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            if (Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) {
                //punkt!
            } else {
                //ball.ChangeDirectionAtRandom();
                //ball.negate();                
                if (turn < 0)
                {
                    player2.pts++;
                    ball.reset(player1.GetFaceVector());
                }
                else
                {
                    player1.pts++;
                    ball.reset(player2.GetFaceVector());
                }
                
            }
            float hit =0;            
                if (turn < 0)
                {
                    if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                        (hit = Vector3.Distance(b.Center, player1.position)) < 15)
                    {
                        //player1.pts++;
                        //ball.ChangeDirectionAtRandom();
                        ball.reflect(hit, player1.GetFaceVector());
                        turn *= -1;
                    }
                }
                else
                {
                    if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                        (hit = Vector3.Distance(b.Center, player2.position)) < 15)
                    {
                        //player2.pts++;
                        //ball.ChangeDirectionAtRandom();
                        ball.reflect(hit, player2.GetFaceVector());
                        turn *= -1;
                    }
                
            }

            // UNDER CONSTRUCTION
                if (gs == GameState.MainMenu)
                {
                    HandleMainMenuInput();
                    ball.freeze = true;
                }
                else
                {
                    menu.Visible = false;
                    menu.Enabled = false;

                }
            if (gs == GameState.InGameDuringRound) HandleInGameDuringRound();
            base.Update(gameTime);
        }
        protected void ResetGame()
        {
            if (ingame)
            {
                ball.reset();
                player1.reset();
                player2.reset();
                turn = -1;
                camera1.reset();
                camera2.reset();
            }
        }
        protected void HandleMainMenuInput()
        {
            if(!(MediaPlayer.State == MediaState.Playing)) MediaPlayer.Play(dubstep_intro);
            menu.Enabled = true;
            menu.Visible = true;
            if (menu.SelectedIndex == 0)
                if (!ingame)
                    menu.SelectedIndex++;
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (menu.SelectedIndex == 0)
                    if (ingame)
                    {
                        gs = GameState.InGameDuringRound;

                        for (int i = 0; ; i++)
                        {
                            MediaPlayer.Volume -= 0.0001f;
                            if (MediaPlayer.Volume <= 0) break;
                        }
                        MediaPlayer.Stop();
                    }
                    else
                        menu.SelectedIndex++;
                if (menu.SelectedIndex == 1)
                {
                    ResetGame();
                    ingame = true;
                    gs = GameState.InGameDuringRound;

                    for (int i = 0; ; i++)
                    {
                        MediaPlayer.Volume -= 0.0001f;
                        if (MediaPlayer.Volume <= 0) break;
                    }
                    MediaPlayer.Stop();
                }
                if (menu.SelectedIndex == (menuItems.Length - 1)) this.Exit();
            }
            if (y <= GraphicsDevice.Viewport.Bounds.Top || y >= GraphicsDevice.Viewport.Bounds.Bottom - intro_ball.Height)
            {
                speed.Y *= -1;
            }
            if (x <= GraphicsDevice.Viewport.Bounds.Left || x >= GraphicsDevice.Viewport.Bounds.Right - intro_ball.Width)
            {
               speed.X *= -1;
            }
            x += (int)speed.X;
            y += (int)speed.Y;
        }

        protected void HandleInGameDuringRound()
        {

        }
        protected void TopBar()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(topFont, "Player 1:  " + player1.pts, new Vector2(topBarViewport.X + 50, topBarViewport.Y + 10), Color.White);
            if (ball.freeze)
                spriteBatch.DrawString(topFont, "PRESS P TO START", new Vector2(topBarViewport.Width / 2 - 100, topBarViewport.Y + 50), Color.White);
            else
            {
                int p;
                if (turn < 0) p = 1;
                else p = 2;
                spriteBatch.DrawString(topFont, "Player: " + p, new Vector2(topBarViewport.Width / 2 - 100, topBarViewport.Y + 50), Color.White);
            }
            spriteBatch.DrawString(topFont, "Player 2:  " + player2.pts, new Vector2(topBarViewport.Width / 2 + 50, topBarViewport.Y + 10), Color.White);
            spriteBatch.End();
        }
        #endregion

        #region Drawing
        protected void DrawMainMenu()
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(logo, new Vector2(mainViewport.Width/2 - 167, 10), Color.White);
            spriteBatch.Draw(intro_ball, new Vector2(x, y), Color.Gainsboro);
            spriteBatch.End();
        }
        protected void DrawSettingsMenu()
        { 
            
        }

        protected void DrawGameArena()
        {
            //bedzie multi lol? ew single player z kompem
            //ball.Draw(camera1);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //player1.Draw(camera1);
            //sphere.Draw(camera1);
            //GraphicsDevice.BlendState = BlendState.Opaque;
        }
        protected void DrawArrow(int d)
        {
            float x = 30.0f;
            float y = 30.0f;
            float halfArrowSize = 47.0f;
            float rotation = 0;
            Vector2 pos;
            int w = graphics.GraphicsDevice.Viewport.Width/2;
            int h = graphics.GraphicsDevice.Viewport.Height/2;
            switch (d)
            {
                case 1:
                     pos = new Vector2(w - halfArrowSize, h + y - halfArrowSize);
                     rotation = MathHelper.ToRadians(270);
                     break;
                case 2:
                     pos = new Vector2(w + halfArrowSize, h - y + halfArrowSize);
                     rotation = MathHelper.ToRadians(90);
                     break;
                case 3:
                     pos = new Vector2(w - x, h + halfArrowSize);
                     rotation = MathHelper.ToRadians(180);
                     break;
                case 4:
                     pos = new Vector2(w + x, h - halfArrowSize);
                     rotation = 0;
                     break;
                default:
                     pos = Vector2.Zero;
                     break;
            }
            spriteBatch.Begin();

            if(d > 0) spriteBatch.Draw(arrow, pos, new Rectangle(0, 0, 94, 94), Color.Blue, rotation, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.End();
            
        }
        protected void DrawSplitScreenArena(Viewport left_view, Viewport right_view)
        {
            if (ball.freeze)
            {
                camera1.Enabled = false;
                camera2.Enabled = false;
            }
            else
            {
                camera1.Enabled = true;
                camera2.Enabled = true;
            }

            graphics.GraphicsDevice.Viewport = mainViewport;
            TopBar();
            //prawy do lewego!!
            graphics.GraphicsDevice.Viewport = left_view;
            ball.Draw(camera1);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            player1.Draw(camera1);
            //player2.Draw(camera1);
            sphere.Draw(camera1);
            aX.Draw(camera1);
            aY.Draw(camera1);
            aZ.Draw(camera1);
            SkyDome.Draw(camera1);
            GraphicsDevice.BlendState = BlendState.Opaque;
            DrawArrow(camera1.getDirection());

            graphics.GraphicsDevice.Viewport = right_view;
            ball.Draw(camera2);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;            
            player2.Draw(camera2);
            //player1.Draw(camera2);
            sphere.Draw(camera2);
            aX.Draw(camera2);
            aY.Draw(camera2);
            aZ.Draw(camera2);
            SkyDome.Draw(camera1);
            GraphicsDevice.BlendState = BlendState.Opaque;
            DrawArrow(camera2.getDirection());

        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if (gs == GameState.MainMenu)
            {
                graphics.GraphicsDevice.Viewport = mainViewport;
                DrawMainMenu();
            }

            if (gt == GameType.Split && gs == GameState.InGameDuringRound)
            {
                DrawSplitScreenArena(leftViewport, rightViewport);
            }            
           
            //if(gs == GameState.InGameDuringRound) DrawGameArena();
            if (gs == GameState.SettingsMenu) DrawSettingsMenu();           

            base.Draw(gameTime);
        }
        #endregion
    }
}
