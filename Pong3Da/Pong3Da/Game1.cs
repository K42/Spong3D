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
        public Camera camera1 { get; protected set; }
        public Camera camera2 { get; protected set; }
        private Ball ball;
        private Player player1, player2;
        BasicModel sphere;
        BoundingSphere playDome;
        bool baal;
        //-------------------NIE RUSZAĆ------------------------
        GameState gs = new GameState();
        GameType gt = new GameType();
        MenuComponent menu;
        Song dubstep_intro;
        int x, y;
        int turn = -1;
        string[] menuItems = {"Start", "Settings", "Quit"};
        Texture2D intro_ball;
        Vector2 speed;
        Viewport mainViewport;
        Viewport leftViewport;
        Viewport rightViewport;
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
            sphere = new BasicModel(Content.Load<Model>(@"models\sfera2"), "sphere");
            playDome = new BoundingSphere(Vector3.Zero, sphere.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            intro_ball = Content.Load<Texture2D>(@"intro_ball");
            dubstep_intro = Content.Load<Song>(@"intro");            
           // dubstep_intro;
            MediaPlayer.Play(dubstep_intro);
            menu = new MenuComponent(this, spriteBatch, Content.Load<SpriteFont>("menufont"), menuItems);
            Components.Add(menu);
            menu.Enabled = false;
                       
            mainViewport = GraphicsDevice.Viewport;
            leftViewport = mainViewport;
            rightViewport = mainViewport;
            leftViewport.Width  /= 2;
            rightViewport.Width /= 2;
            rightViewport.X = leftViewport.Width ;
            //leftViewport.Height = rightViewport.Height /= 2;
            //leftViewport.Y = rightViewport.Y += 200;

            Random rnd = new Random();
            x = rnd.Next(100, mainViewport.Width - 100);
            y = rnd.Next(100, mainViewport.Height - 100);
            speed = new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10));


            camera1 = new Camera(this, new Vector3(0, 0, 80), Vector3.Zero, Vector3.Up, 1, leftViewport);
            camera2 = new Camera(this, new Vector3(0, 0, -80), Vector3.Zero, Vector3.Up, 2, rightViewport);

            ball = new Ball(this);
            player1 = new Player(this, camera1, 1);
            player2 = new Player(this, camera2, 2);

            Components.Add(camera1);
            Components.Add(camera2);
            Components.Add(ball);

            //MediaPlayer.
            // Initialize vertices
            // Initialize the BasicEffect
            //effect = new BasicEffect(GraphicsDevice);
            // Set cullmode to none
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rs;
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Logic
        protected override void Update(GameTime gameTime) {
            Window.Title = " x = " + ball.position.X
                + " y = " + ball.position.Y
                + " z = " + ball.position.Z
                + " pitch = " + camera1.pitch
                + " maxPitch = " + camera1.maxPitch
                + " P1 pts = " + player1.pts;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) //this.Exit();        // Allows the game to exit
                gs = GameState.MainMenu;
            if (!baal && Keyboard.GetState().IsKeyDown(Keys.P)) {
                ball.freeze = !ball.freeze;
            }
            baal = Keyboard.GetState().IsKeyDown(Keys.P);
            BoundingSphere b = new BoundingSphere(ball.position, ball.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            if (Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) {
                //punkt!
            } else {
                //ball.ChangeDirectionAtRandom();
                ball.negate();
            }
            float hit =0;
            
                if (turn < 0)
                {
                    if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                        (hit = Vector3.Distance(b.Center, player1.position)) < 12)
                    {
                        player1.pts++;
                        //ball.ChangeDirectionAtRandom();
                        ball.reflect(hit, player1.GetFaceVector());
                        turn *= -1;
                    }
                }
                else
                {
                    if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                        (hit = Vector3.Distance(b.Center, player2.position)) < 12)
                    {
                        player2.pts++;
                        //ball.ChangeDirectionAtRandom();
                        ball.reflect(hit, player2.GetFaceVector());
                        turn *= -1;
                    }
                
            }

            // UNDER CONSTRUCTION
                if (gs == GameState.MainMenu) HandleMainMenuInput();
                else
                {
                    menu.Visible = false;
                    menu.Enabled = false;
                }
            if (gs == GameState.InGameDuringRound) HandleInGameDuringRound();
            base.Update(gameTime);
        }

        protected void HandleMainMenuInput()
        {
            menu.Enabled = true;
            menu.Visible = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (menu.SelectedIndex == 0)
                {
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
        #endregion

        #region Drawing
        protected void DrawMainMenu()
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
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

        protected void DrawSplitScreenArena(Viewport left_view, Viewport right_view)
        {
            //prawy do lewego!!
            graphics.GraphicsDevice.Viewport = left_view;
            ball.Draw(camera1);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            player1.Draw(camera1);
            sphere.Draw(camera1);
            GraphicsDevice.BlendState = BlendState.Opaque;

            graphics.GraphicsDevice.Viewport = right_view;
            ball.Draw(camera2);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;            
            player2.Draw(camera2);
            sphere.Draw(camera2);
            GraphicsDevice.BlendState = BlendState.Opaque;

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
