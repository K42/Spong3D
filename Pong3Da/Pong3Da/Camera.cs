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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PlayerView : Microsoft.Xna.Framework.GameComponent {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        int p; 
        // max odchylenie g�ra-d�, mo�na pomin��, bo dzia�a w miare normalnie 
        // ale jak si� mija bieguny to si� kierunki mieszaj�, wygodniej jest tak
        protected float maxPitch = (float)Math.PI / 2;
        protected float yaw { get; set; }
        protected float pitch { get; set; }
        public Vector3 position { get; private set; }
        private Vector3 desiredPosition;
        private Vector3 target;
        private Vector3 desiredTarget;
        private Vector3 offsetDistance;
        private Matrix cameraRotation;
        protected float speedup { get; set; }
        private float baseSpeed;
        private float perspective = MathHelper.PiOver2 * 0.9f;
        private float ar;
        private int k = 0;
        private TimeSpan timer = TimeSpan.FromSeconds(0);
        public bool affected { get; private set; }
        protected int bonusDuration = 0;

        public PlayerView(Game game, int p, Viewport view)
            : base(game) {
                this.p = p;
            baseSpeed = 1.0f;
            speedup = baseSpeed;
            ar = (float)view.Width / (float)view.Height;
            reset();
            UpdateView();
            CreateLookAt();            
        }
        public void setSpeed(int s)
        {
            switch(s)
            {
                case 1:
                    baseSpeed = 1.0f;
                    break;
                case 2:
                    baseSpeed = 1.2f;
                    break;
                case 3:
                    baseSpeed = 1.5f;
                    break;
            }
        }
        //reset pozycji
        public void reset() 
        {
            offsetDistance = new Vector3(0, 0, 85);
            if (p == 1)
            {
                yaw = 0.0f;
                pitch = 0.0f;
                position = new Vector3(0, 0, 80);
            }
            if (p == 2)
            {
                yaw = 3.14f;
                pitch = 0.0f;
                position = new Vector3(0, 0, -80);
            }
            desiredPosition = position;
            target = new Vector3();
            desiredTarget = target;
            cameraRotation = Matrix.Identity;
            view = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(
                perspective,
                ar,
                1, 1000);
            UpdateView();
            CreateLookAt();
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize() {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime) {
            
            // Recreate the camera view matrix
            InputHandler();
            UpdateView();
            CreateLookAt();

            //sprawdzanie bonusa
            if (bonusDuration > 0)
            {                
                timer += gameTime.ElapsedGameTime;
                if (timer.TotalSeconds > bonusDuration)
                {
                    speedup = baseSpeed;
                    bonusDuration = 0;
                    affected = false;
                }
            }
            else timer = TimeSpan.FromSeconds(0);
            base.Update(gameTime);
        }
        private void InputHandler()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            float speed = .02f * baseSpeed;
            k = 0;
            if (p == 1)
            {
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    yaw += (speed * speedup);
                    k = 4;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    yaw += -(speed * speedup);
                    k = 3;
                }
                if (keyboardState.IsKeyDown(Keys.W) && pitch > -maxPitch)
                {
                    pitch += -(speed * speedup);
                    k = 1;
                }
                if (keyboardState.IsKeyDown(Keys.S) && pitch < maxPitch)
                {
                    pitch += (speed * speedup);
                    k = 2;
                }
            }
            else if (p == 2)
            {
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    yaw += (speed * speedup);
                    k = 4;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    yaw += -(speed * speedup);
                    k = 3;
                }
                if (keyboardState.IsKeyDown(Keys.Up) && pitch > -maxPitch)
                {
                    pitch += -(speed * speedup);
                    k = 1;
                }
                if (keyboardState.IsKeyDown(Keys.Down) && pitch < maxPitch)
                {
                    pitch += (speed * speedup);
                    k = 2;
                }
            }
        }
        public PlayerView getInstance()
        {
            return this;
        }
        public int getDirection()
        {
            return k;
        }

        private void UpdateView() {
            cameraRotation.Forward.Normalize();

            cameraRotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);

            desiredPosition = Vector3.Transform(offsetDistance, cameraRotation);
            position = desiredPosition;
            target = Vector3.Zero;
        }

        private void CreateLookAt() {
            view = Matrix.CreateLookAt(position, target, cameraRotation.Up);
        }
        //potrzebne przy wyswietlaniu paletki
        public float[] getDim()
        {
            return new float[] { position.X, position.Y, position.Z, pitch, yaw };
        }
        //atywacja bonusa gracza
        public void PowerUp(int active, float value)
        {
            this.bonusDuration = active;
            affected = true;
            speedup = value;
        }
    }
}
