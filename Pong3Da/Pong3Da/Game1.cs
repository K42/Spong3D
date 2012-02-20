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
    /*
     * Stany gry, kolejno
     *  Menu
     *  Ekran ustawień gry
     *  Ekran rozgrywki
     */
    public enum GameState
    { 
        MainMenu,
        SuitUp,
        InGameDuringRound
    }
    /*
     * Typy gry, kolejno
     *  Limit czasowy
     *  Limit punktów
     */
    public enum GameType
    { 
        Time,
        Points
    }
    #endregion
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont topFont, alertFont;
        SoundEffect menu_go, game_p_1, game_p_2, game_hit, game_point, game_win;

        protected PlayerView menu_view;
        public PlayerView playerView1 { get; protected set; }
        public PlayerView playerView2 { get; protected set; }
        protected PowerUp powerUp;
        private Ball ball;
        private Player player1, player2;
        StaticModel sphere, aX, aY, aZ, skydome, menudome;
        BoundingSphere playDome;
        //boole pomocnicze
        bool baal;
        bool point = false;
        bool ingame = false;
        bool t = false;
        bool playSound = true;
        
        GameState gs = new GameState();
        GameType gt = new GameType();
        TimeSpan spaaace, timer;
        MenuComponent menu, duel_menu;
        Song dubstep_intro, music1, music2, music3;
        //inty pomocnicze
        int x, y;
        int turn = -1; //znacznik tury
        int time = 60, points = 20, helper = 0, gspeed = 1;
        int winner;
        //tablica opcji w menu głównym
        string[] menuItems = { "Resume", "New duel", "Settings", " Quit" };
        //tablica opcji w menu ekranu startowego
        string[] duel_prop = { "Game type: ", "Time limit: ", "Points limit: ", "Speed: ", "   START!" };
        //tekstury do menu i strzalki
        Texture2D intro_ball, logo, arrow, bg1, bg2;
        Vector2 speed;

        Viewport mainViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Viewport topBarViewport;
        #endregion

        #region Constructor
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = false;
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
            this.graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth /  (2 * GraphicsDeviceManager.DefaultBackBufferHeight);
        }
        #endregion

        #region Initialization
        protected override void Initialize() {
            // TODO: Add your initialization logic here
           
            gs = GameState.MainMenu;
            gt = GameType.Time;
            base.Initialize();
        }
        #endregion

        #region Content
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sphere = new StaticModel(Content.Load<Model>(@"models/sphere"), 0, 0, 0);
            aX = new StaticModel(Content.Load<Model>(@"models/ring"), 0, 0, 0);
            aY = new StaticModel(Content.Load<Model>(@"models/ring"), 90, 0, 0);
            aZ = new StaticModel(Content.Load<Model>(@"models/ring"), 90, 90, 0);
            skydome = new StaticModel(Content.Load<Model>(@"models/skydome"), 2.0f, 1.0f, false);
            menudome = new StaticModel(Content.Load<Model>(@"models/skydome"), 2.0f, 1.0f, true); 
            playDome = new BoundingSphere(Vector3.Zero, sphere.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            intro_ball = Content.Load<Texture2D>(@"textures/intro_ball");
            logo = Content.Load<Texture2D>(@"textures/spong");
            arrow = Content.Load<Texture2D>(@"textures/arrow");
            bg1 = Content.Load<Texture2D>(@"textures/bg1");
            bg2 = Content.Load<Texture2D>(@"textures/bg2");
            dubstep_intro = Content.Load<Song>(@"music/intro");
            music1 = Content.Load<Song>(@"music/music1");
            music2 = Content.Load<Song>(@"music/music2");
            music3 = Content.Load<Song>(@"music/music3");
            powerUp = new PowerUp(this);
            menu = new MenuComponent(this, spriteBatch, Content.Load<SpriteFont>(@"fonts/menufont"), menuItems);
            duel_menu = new MenuComponent(this, spriteBatch, Content.Load<SpriteFont>(@"fonts/menufont"), duel_prop);

            topFont = Content.Load<SpriteFont>(@"fonts/topfont");
            alertFont = Content.Load<SpriteFont>(@"fonts/alertfont");

            Components.Add(powerUp);
            Components.Add(menu);
            Components.Add(duel_menu);
            powerUp.Enabled = false;
            menu.Enabled = false;
            duel_menu.Enabled = false;
            spaaace = TimeSpan.FromSeconds(0);
            
            menu_go = Content.Load<SoundEffect>(@"sounds/menu/menu_start");
            game_p_1 = Content.Load<SoundEffect>(@"sounds/game/game_p_1");
            game_p_2 = Content.Load<SoundEffect>(@"sounds/game/game_p_2");
            game_hit = Content.Load<SoundEffect>(@"sounds/game/game_hit");
            game_point = Content.Load<SoundEffect>(@"sounds/game/game_point");
            game_win = Content.Load<SoundEffect>(@"sounds/game/game_win");            
                       
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
            leftViewport.Y += 100;
            rightViewport.Y += 100;

            Random rnd = new Random();
            x = rnd.Next(100, mainViewport.Width - 300);
            y = rnd.Next(100, mainViewport.Height - 300);
            speed = new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10));


            playerView1 = new PlayerView(this, 1, leftViewport);
            playerView2 = new PlayerView(this, 2, rightViewport);
            menu_view = new PlayerView(this, 2, mainViewport);

            ball = new Ball(this);
            player1 = new Player(this, 1);
            player2 = new Player(this, 2);

            Components.Add(playerView1);
            Components.Add(playerView2);
            Components.Add(ball);
            Window.Title = "Spong 3D";
            setSpeed(1);
            MediaPlayer.Volume = 0.7f;
        }
        protected void setSpeed(int k)
        {
            ball.setSpeed(k);
            playerView1.setSpeed(k);
            playerView2.setSpeed(k);
        }
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Logic
        protected override void Update(GameTime gameTime) {
            //Pauza i wyjście do menu
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (gs == GameState.InGameDuringRound) MediaPlayer.Stop();
                gs = GameState.MainMenu;
            }
            //pauza w grze
            if (!baal && Keyboard.GetState().IsKeyDown(Keys.Space)) {
                ball.freeze = !ball.freeze;
                if (ball.freeze) if (playSound) game_p_1.Play();
                    else if (playSound) game_p_2.Play();
            }
            baal = Keyboard.GetState().IsKeyDown(Keys.Space);

            //obsluga stanow
            if (gs == GameState.MainMenu)
            {
                menu.Enabled = true;
                menu.Visible = true;
                HandleMainMenuInput();
                ball.freeze = true;
            }
            else
            {
                menu.Visible = false;
                menu.Enabled = false;
            }
            if (gs == GameState.InGameDuringRound)
            {
                HandleInGameDuringRound(gameTime);
                powerUp.Enabled = true;
            }
            else
                powerUp.Enabled = false;
            if (gs == GameState.SuitUp) 
            {
                duel_menu.Enabled = true;
                duel_menu.Visible = true;
                DuelPreparation();
            }
            else
            {
                duel_menu.Visible = false;
                duel_menu.Enabled = false;
            }
            base.Update(gameTime);
        }
        //obsluga power-upow, niestety na sztywno, ale sie sprawdza
        protected void HandlePowerUp()
        {
            //if (gs != GameState.InGameDuringRound) powerUp.Enabled = false;
            //else powerUp.Enabled = true;
            if (powerUp.active)
            {
                if (powerUp.Intersect(player1.position, 12))
                {
                    if (powerUp.f == flavor.green)
                    {
                        playerView1.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                    if (powerUp.f == flavor.blue || powerUp.f == flavor.yellow)
                    {
                        ball.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                    if (powerUp.f == flavor.red)
                    {
                        playerView2.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                    if (powerUp.f == flavor.black)
                    {
                        playerView1.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                        playerView2.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                        ball.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                }
                if (powerUp.Intersect(player2.position, 12))
                {
                    if (powerUp.f == flavor.green)
                    {
                        playerView2.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                    if (powerUp.f == flavor.blue || powerUp.f == flavor.yellow)
                    {
                        ball.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                    if (powerUp.f == flavor.red)
                    {
                        playerView1.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                    if (powerUp.f == flavor.black)
                    {
                        playerView1.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                        playerView2.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                        ball.PowerUp(powerUp.duration, powerUp.ApplyPowerUp());
                    }
                }
            }

        }
        //muzyka
        protected void MusicHandler()
        {
            if (MediaPlayer.State != MediaState.Playing) MediaPlayer.Play(music1);
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(music1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(music2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(music3);
            }
        }
        //wlacz/wylacz dzwiek
        protected void SetSound(bool set)
        {
            playSound = set;
            menu.playSound = set;
            duel_menu.playSound = set;
        }
        //nvm
        protected void SetWindowTitle(GameTime gameTime)
        {
            int p = 0;
            if (turn < 0) p = 1;
            else p = 2;
            Window.Title = "P1 " + player1.pts +
                " P2 " + player2.pts +
                " turn: player " + p +
                " powerup " + powerUp.active +
                " timer " + powerUp.gettimer();
        }
        //resetowanie stanu gry
        protected void ResetGame()
        {
            if (ingame)
            {
                winner = 0;
                if(turn < 0 )
                    ball.reset(player1.GetFaceVector());
                else
                    ball.reset(player2.GetFaceVector());
                player1.reset();
                player2.reset();
                turn = -1;
                playerView1.reset();
                playerView2.reset();
            }
        }
        //obsluga glownego menu
        protected void HandleMainMenuInput()
        {
            string sound;
            if (playSound) sound = "on";
            else sound = "off";
            if (MediaPlayer.State == MediaState.Stopped) 
                if (playSound) 
                    MediaPlayer.Play(dubstep_intro);
            if (menu.SelectedIndex == 0)
                if (!ingame)
                    menu.SelectedIndex++;
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && menu.Enabled)
            {
                if (menu.SelectedIndex == 0)
                    if (ingame)
                    {
                        if (playSound) menu_go.Play();
                        gs = GameState.InGameDuringRound;
                        MediaPlayer.Stop();
                    }
                    else
                        menu.SelectedIndex++;
                if (menu.SelectedIndex == 1)
                {
                    if (playSound) menu_go.Play();
                    ingame = true;
                    ResetGame();
                    ingame = false;
                    duel_menu.SelectedIndex = 0;
                    gs = GameState.SuitUp;
                }
                if (menu.SelectedIndex == (menuItems.Length - 1))
                {
                    if (playSound) menu_go.Play();
                    this.Exit();
                }

            }
            if (menu.SelectedIndex == 2)
            {                
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    SetSound(true);
                    if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(dubstep_intro);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    SetSound(false);
                    MediaPlayer.Stop();
                }
            }
            menu.SetIndexstring(2, "Sound " + sound);
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
        //obsluga menu ustawien gry
        protected void DuelPreparation()
        {
            string type = "";
            string sSpeed = "";
            if (gt == GameType.Time) type = "time limit";
            else if (gt == GameType.Points) type = "points limit";
            if (gspeed == 1) sSpeed = "Normal";
            if (gspeed == 2) sSpeed = "Fast";
            if (gspeed == 3) sSpeed = "Insane";
            if (t) helper++;
            if (helper > 7)
            {
                helper = 0;
                t = false;
            }
            if (duel_menu.SelectedIndex == 0)
            {                                

                if (Keyboard.GetState().IsKeyDown(Keys.Left) && duel_menu.Enabled && !t)
                {
                    gt = GameType.Time;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && duel_menu.Enabled && !t)
                {
                    gt = GameType.Points;
                }               
                
            }
            if (duel_menu.SelectedIndex == 1)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && duel_menu.Enabled && !t)
                {
                    t = true;
                    if (time > 11) time--;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && duel_menu.Enabled && !t)
                {
                    t = true;
                    time++;
                }
            }
            if (duel_menu.SelectedIndex == 2)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && duel_menu.Enabled && !t)
                {
                    t = true;
                    if (points > 1) points--;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && duel_menu.Enabled && !t)
                {
                    t = true;
                    points++;
                }
            }
            if (duel_menu.SelectedIndex == 3)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && duel_menu.Enabled && !t)
                {
                    t = true;
                    if (gspeed > 1) gspeed--;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && duel_menu.Enabled && !t)
                {
                    t = true;
                    if (gspeed < 3) gspeed++;
                }
            }
            duel_menu.SetIndexstring(0, "Game type: " + type);
            duel_menu.SetIndexstring(1, "Time limit: " + time);
            duel_menu.SetIndexstring(2, "Points limit: " + points);
            duel_menu.SetIndexstring(3, "Speed: " + sSpeed);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && duel_menu.Enabled)
            {
                if (duel_menu.SelectedIndex == 4)
                {
                    if (gt == GameType.Time) timer = TimeSpan.FromSeconds(0);
                    ingame = true;
                    setSpeed(gspeed);
                    if (playSound) menu_go.Play();
                    MediaPlayer.Stop();
                    if (playSound) MediaPlayer.Play(music1);
                    t = true;
                    gs = GameState.InGameDuringRound;                   
                }
            }
        }
        //sprawdzanie zwyciezcy
        protected int WinCheck(GameTime gameTimer)
        {
            if (gt == GameType.Points)
            {
                if (player1.pts >= points)
                    return 1;
                if (player2.pts >= points)
                    return 2;
            }
            if (gt == GameType.Time)
            {
                if (timer.TotalSeconds <= time && !ball.freeze) timer += gameTimer.ElapsedGameTime;
                if (timer.TotalSeconds > time)
                {
                    if (player1.pts == player2.pts)
                        return 3;
                    if (player1.pts > player2.pts)
                        return 1;
                    else
                        return 2;
                }
            }

            return 0;
        }
        //obsluga gry
        protected void HandleInGameDuringRound(GameTime gameTimer)
        {
            if (playSound) MusicHandler();
            BoundingSphere b = new BoundingSphere(ball.position, ball.model.Meshes[0].BoundingSphere.Radius * 1.0f);
            if (Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius)
            {
            }
            else
            {
                if (!point) if (playSound) game_point.Play();
                if (turn < 0)
                {
                    if (!point) spaaace = TimeSpan.FromSeconds(0);
                    point = true;
                    spaaace += gameTimer.ElapsedGameTime;
                    if (spaaace.Seconds > game_point.Duration.Seconds)
                    {
                        ball.reset(player1.GetFaceVector());
                        point = false;
                        player2.pts++;
                    }
                }
                else
                {
                    if (!point) spaaace = TimeSpan.FromSeconds(0);
                    point = true;
                    spaaace += gameTimer.ElapsedGameTime;
                    if (spaaace.Seconds > game_point.Duration.Seconds - 1)
                    {
                        ball.reset(player2.GetFaceVector());
                        point = false;
                        player1.pts++;
                    }
                }

            }
            float hit = 0;
            if (turn < 0)
            {
                if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                    (hit = Vector3.Distance(b.Center, player1.position)) < 15)
                {
                    ball.reflect(hit, player1.GetFaceVector());
                    turn *= -1;
                    if (playSound) game_hit.Play();
                }
            }
            else
            {
                if ((Vector3.Distance(b.Center, playDome.Center) < playDome.Radius - b.Radius) &&
                    (hit = Vector3.Distance(b.Center, player2.position)) < 15)
                {
                    ball.reflect(hit, player2.GetFaceVector());
                    turn *= -1;
                    if (playSound) game_hit.Play();
                }
            }
            winner = WinCheck(gameTimer);
            HandlePowerUp();
        }        
        #endregion

        #region Drawing
        //rysowanie dodatkow menu glownego
        protected void DrawMainMenu()
        {           
            GraphicsDevice.Clear(Color.Black);
            menudome.Update();
            menudome.Draw(menu_view);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(logo, new Vector2(mainViewport.Width/2 - 167, 10), Color.White);
            spriteBatch.Draw(intro_ball, new Vector2(x, y), Color.Gainsboro);
            spriteBatch.End();
        }
        protected void DrawDuelPreparation()
        {
            menudome.Update();
            menudome.Draw(menu_view);
        }
        //gorny panel podczas rozgrywki
        protected void TopBar()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(topFont, "Player 1:  " + player1.pts, new Vector2(topBarViewport.Width / 2 - 300, topBarViewport.Y + 10), Color.White);
            spriteBatch.DrawString(topFont, "Player 2:  " + player2.pts, new Vector2(topBarViewport.Width / 2 + 200, topBarViewport.Y + 10), Color.White);
            
            if (gt == GameType.Time)
            {
                spriteBatch.DrawString(topFont, "Time left:  " + (time - (int)timer.TotalSeconds),
                    new Vector2(topBarViewport.X + 5, topBarViewport.Y + 50), Color.White);
            }
            if (gt == GameType.Points)
            {
                spriteBatch.DrawString(topFont, "Points to win:  " + points,
                    new Vector2(topBarViewport.X + 5, topBarViewport.Y + 50), Color.White);
            }
            if (winner > 0)
            {
                ball.freeze = true;
                ingame = false;
                if (winner < 3)
                {
                    if (t) if (playSound) game_win.Play();
                    t = false;
                    spriteBatch.DrawString(topFont, "PLAYER " + winner + " WINS!",
                        new Vector2(topBarViewport.Width / 2 - 95, topBarViewport.Y + 5), Color.Yellow);
                }
                else
                {
                    spriteBatch.DrawString(topFont, "REMIS!",
                        new Vector2(topBarViewport.Width / 2 - 90, topBarViewport.Y + 5), Color.Yellow);
                }
                spriteBatch.DrawString(topFont, "press ESC to exit",
                    new Vector2(topBarViewport.Width / 2 - 100, topBarViewport.Y + 30), Color.White);
            }
            else
            {
                if(point)
                    spriteBatch.DrawString(topFont, "Point!", new Vector2(topBarViewport.Width / 2 - 30, topBarViewport.Y + 40), Color.Yellow);
                if (ball.freeze)
                    spriteBatch.DrawString(topFont, "PRESS SPACE TO START", new Vector2(topBarViewport.Width / 2 - 100, topBarViewport.Y + 50), Color.White);
                if (powerUp.applied && !ball.freeze)
                {
                    int k = 0;
                    if (playerView1.affected) k = 1;
                    else if (playerView2.affected) k = 2;
                    spriteBatch.DrawString(alertFont, powerUp.GetFlavorText(k) + " " + (float)((int)(((float)powerUp.duration-powerUp.gettimer())*100))/100, new Vector2(topBarViewport.Width / 2 - 180, topBarViewport.Y + 70), powerUp.GetFlavorColor());
                }
                int p;
                if (turn < 0) p = 1;
                else p = 2;
                spriteBatch.DrawString(topFont, "Player: " + p, new Vector2(topBarViewport.Width / 2 - 40, topBarViewport.Y + 5), Color.White);
            }
            
            spriteBatch.End();
        }
        //strzalka kierunku poruszania sie paletki
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
        //rysowanie ekranu gry
        protected void DrawSplitScreenArena(Viewport left_view, Viewport right_view)
        {
            if (ball.freeze)
            {
                playerView1.Enabled = false;
                playerView2.Enabled = false;
            }
            else
            {
                playerView1.Enabled = true;
                playerView2.Enabled = true;
            }

            graphics.GraphicsDevice.Viewport = mainViewport;
            TopBar();
            //prawy do lewego!!
            graphics.GraphicsDevice.Viewport = left_view;
            skydome.Draw(playerView1);
            ball.Draw(playerView1);
            powerUp.Draw(playerView1);
            
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            player1.Draw(playerView1);
            player2.Draw(playerView2, playerView1);
            sphere.Draw(playerView1);
            aX.Draw(playerView1);
            aY.Draw(playerView1);
            aZ.Draw(playerView1);
            GraphicsDevice.BlendState = BlendState.Opaque;
            DrawArrow(playerView1.getDirection());

            graphics.GraphicsDevice.Viewport = right_view;

            skydome.Draw(playerView2);
            ball.Draw(playerView2);
            powerUp.Draw(playerView2);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
           
            player2.Draw(playerView2);
            player1.Draw(playerView1, playerView2);
            //player1.Draw(camera2);
            sphere.Draw(playerView2);
            aX.Draw(playerView2);
            aY.Draw(playerView2);
            aZ.Draw(playerView2);
            GraphicsDevice.BlendState = BlendState.Opaque;
            DrawArrow(playerView2.getDirection());

        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            if (gs == GameState.MainMenu)
            {
                graphics.GraphicsDevice.Viewport = mainViewport;
                DrawMainMenu();
            }
            if (gs == GameState.SuitUp)
                DrawDuelPreparation();
            if (gs == GameState.InGameDuringRound)
            {
                DrawSplitScreenArena(leftViewport, rightViewport);
            }            
            base.Draw(gameTime);
        }
        #endregion
    }
}
