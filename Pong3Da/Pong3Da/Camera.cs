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

        public Vector3 cameraPosition { get; protected set; }
        // max odchylenie góra-dó³, mo¿na pomin¹æ, bo dzia³a w miare normalnie 
        // ale jak siê mija bieguny to siê kierunki pierdol¹, wiêc lepiej niech jest
        public float maxPitch = 1.45f;
        // chaos w dostêpach, póŸniej mo¿na poprawiæ co ma byæ public a co private
        public float yaw { get; set; }
        public float pitch { get; set; }
        public Vector3 position { get; protected set; }
        private Vector3 desiredPosition;
        private Vector3 target;
        private Vector3 desiredTarget;
        private Vector3 offsetDistance;
        private Matrix cameraRotation;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game) {
            position = pos;
            desiredPosition = position;
            target = new Vector3();
            desiredTarget = target;

            offsetDistance = new Vector3(0, 0, 80);

            yaw = 0.0f;
            pitch = 0.0f;

            cameraRotation = Matrix.Identity;

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver2 * 0.9f,
                (float) Game.Window.ClientBounds.Width /
                (float) Game.Window.ClientBounds.Height,
                1, 1000);

            CreateLookAt();
        }
        private void reset() {
            position = new Vector3(0, 0, 50);
            desiredPosition = position;
            target = new Vector3();
            desiredTarget = target;

            offsetDistance = new Vector3(0, 0, 50);

            yaw = 0.0f;
            pitch = 0.0f;

            cameraRotation = Matrix.Identity;
            view = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float) Game.Window.ClientBounds.Width /
                (float) Game.Window.ClientBounds.Height,
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
            if (Keyboard.GetState().IsKeyDown(Keys.R)) reset();
            KeyboardState keyboardState = Keyboard.GetState();

            //Rotate Camera
            //if (keyboardState.IsKeyDown(Keys.P)) test++;

            if (keyboardState.IsKeyDown(Keys.D)) {
                yaw += .05f;
            }
            if (keyboardState.IsKeyDown(Keys.A)) {
                yaw += -.05f;
            }
            if (keyboardState.IsKeyDown(Keys.W) && pitch > -maxPitch) {
                pitch += -.05f;
            }
            if (keyboardState.IsKeyDown(Keys.S) && pitch < maxPitch) {
                pitch += .05f;
            }
            // Recreate the camera view matrix
            UpdateView();
            CreateLookAt();

            base.Update(gameTime);
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
    }
}
