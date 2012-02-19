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
    public class Camera : Microsoft.Xna.Framework.GameComponent {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        int p; //tymczasowo, przeniesc do playera razem z inputem
        public Vector3 cameraPosition { get; protected set; }
        // max odchylenie góra-dó³, mo¿na pomin¹æ, bo dzia³a w miare normalnie 
        // ale jak siê mija bieguny to siê kierunki pierdol¹, wiêc lepiej niech jest
        public float maxPitch = (float)Math.PI/2;
        // chaos w dostêpach, póŸniej mo¿na poprawiæ co ma byæ public a co private
        public float yaw { get; set; }
        public float pitch { get; set; }
        public Vector3 position { get; protected set; }
        private Vector3 desiredPosition;
        private Vector3 target;
        private Vector3 desiredTarget;
        private Vector3 offsetDistance;
        private Matrix cameraRotation;
        public float speedup { get; set; }
        private float basespeed;
        private float perspective = MathHelper.PiOver2 * 0.9f;
        private float ar;
        private int k = 0;
        TimeSpan timer = TimeSpan.FromSeconds(0);
        int duration= 0 ;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up, int p, Viewport view)
            : base(game) {
                this.p = p;
            position = pos;
            desiredPosition = position;
            target = new Vector3();
            desiredTarget = target;
            basespeed = 1.0f;
            speedup = basespeed;
            offsetDistance = new Vector3(0, 0, 85);

            yaw = 0.0f;
            pitch = 0.0f;

            cameraRotation = Matrix.Identity;
            ar = (float)view.Width / (float)view.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(
                perspective,
                ar,
                //(float) Game.Window.ClientBounds.Width /
                //(float) Game.Window.ClientBounds.Height,
                1, 1000);

            CreateLookAt();
        }
        public void reset() {
            //int i = 1;
            //if (p == 2) i = -1;
            position = new Vector3(0, 0, 80);
            desiredPosition = position;
            target = new Vector3();
            desiredTarget = target;

            offsetDistance = new Vector3(0, 0, 85);

            yaw = 0.0f;
            pitch = 0.0f;

            cameraRotation = Matrix.Identity;
            view = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(
                perspective,
                ar,
                1, 1000);
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
            
            KeyboardState keyboardState = Keyboard.GetState();

            //obrót kamery
            //if (keyboardState.IsKeyDown(Keys.P)) test++;
            float speed = .02f;
            //if (pitch > 2 * maxPitch) pitch = -maxPitch;
            //if (pitch < 2 * (-maxPitch)) pitch = maxPitch;
            //int q;
            //if ((pitch > maxPitch) || (pitch < -maxPitch)) q = -1;
            //else q = 1;
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
            // Recreate the camera view matrix
            UpdateView();
            CreateLookAt();
            if (duration > 0)
            {                
                timer += gameTime.ElapsedGameTime;
                if (timer.TotalSeconds > duration)
                {
                    speedup = basespeed;
                    duration = 0;
                }
            }
            else timer = TimeSpan.FromSeconds(0);
            base.Update(gameTime);
        }
        public Camera getInstance()
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
            //desiredPosition += chasedObjectsWorld.Translation;
            position = desiredPosition;
            target = Vector3.Zero;
        }

        private void CreateLookAt() {
            //view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
            view = Matrix.CreateLookAt(position, target, cameraRotation.Up);
        }

        public float[] getDim()
        {
            return new float[] { position.X, position.Y, position.Z, pitch, yaw };
        }
        public void PowerUp(int active, float value)
        {
            this.duration = active;
            speedup = value;
        }
        //zalaczanie powerupa tutaj, liczenmie czasu tutaj
        //klasa powerup liczy dla siebie
    }
}
