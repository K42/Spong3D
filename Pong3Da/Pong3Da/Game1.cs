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

namespace Pong3Da {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public enum GameState
    { 
        MainMenu,
        SettingsMenu,
        MpMenu,
        InGameDuringRound,
        InGameAfterScore,
        GameOver
    }

    public enum MultiType
    { 
        Split,
        TcpIP
    }
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera { get; protected set; }
        private Ball ball;
        private Player player1;//, player2;
        BasicModel sphere;
        BoundingSphere playDome;
        bool baal;
        //-------------------NIE RUSZAĆ------------------------
        GameState gs = new GameState();
        Song dubstep_intro;
        int x, y;
        Texture2D intro_ball;
        Vector2 speed;
        //----------------------------------------------------
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = false;
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1080;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            camera = new Camera(this, new Vector3(0, 0, 60), Vector3.Zero, Vector3.Up);
            
            ball = new Ball(this);
            player1 = new Player(this, camera);
            Random rnd = new Random();
            x = rnd.Next(100,   1000);
            y = rnd.Next(100, 1000);
            speed = new Vector2(rnd.Next(-10,10), rnd.Next(-10,10));
            //player2 = new Player(this, camera);

            Components.Add(camera);
            Components.Add(ball);

           
            

            gs = GameState.MainMenu;
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sphere = new BasicModel(Content.Load<Model>(@"models\sfera2"), "sphere");
            playDome = new BoundingSphere(Vector3.Zero, sphere.model.Meshes[0].BoundingSphere.Radius * 1.0f);

            intro_ball = Content.Load<Texture2D>(@"intro_ball");
            dubstep_intro = Content.Load<Song>(@"intro");
           // dubstep_intro;
            MediaPlayer.Play(dubstep_intro);
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

        protected override void Update(GameTime gameTime) {
            Window.Title = " x = " + ball.position.X
                + " y = " + ball.position.Y
                + " z = " + ball.position.Z
                + " pitch = " + camera.pitch
                + " maxPitch = " + camera.maxPitch
                + " P1 pts = " + player1.pts;
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit();
            if (!baal && Keyboard.GetState().IsKeyDown(Keys.P)) {
                ball.freeze = !ball.freeze;
            }
            baal = Keyboard.GetState().IsKeyDown(Keys.P);
            BoundingSphere b = new BoundingSphere(ball.position, ball.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            if (Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) {
                //punkt!
            } else {
                ball.ChangeDirectionAtRandom();
            }
            if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                Vector3.Distance(b.Center, player1.position) < 10)
            {
                player1.pts++;
                ball.ChangeDirectionAtRandom();
            }

            // UNDER CONSTRUCTION
            if (gs == GameState.MainMenu) HandleMainMenuInput();
            if (gs == GameState.InGameDuringRound) HandleInGameDuringRound();
            base.Update(gameTime);
        }

        protected void HandleMainMenuInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                gs = GameState.InGameDuringRound;

                for (int i = 0; ; i++ )
                {
                    MediaPlayer.Volume -= 0.0001f;
                    if (MediaPlayer.Volume <= 0) break;
                }
                MediaPlayer.Stop();
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
            ball.Draw(camera);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            player1.Draw(camera);
            sphere.Draw(camera);
            GraphicsDevice.BlendState = BlendState.Opaque;
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if(gs == GameState.MainMenu) DrawMainMenu();
            if(gs == GameState.InGameDuringRound) DrawGameArena();
            if (gs == GameState.SettingsMenu) DrawSettingsMenu();
            base.Draw(gameTime);
        }
    }
}
