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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        // Max yaw/pitch variables
        public float totalYaw = 50000; // MathHelper.Pi / 2;
        public float currentYaw = 0;
        public float totalPitch = 500000;
        public float currentPitch = 0;

        MouseState prevMouseState;

        float speed = 0.06f;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 1000);
        }
        private void reset()
        {
            cameraPosition = new Vector3(0, 0, 20);
            cameraDirection = Vector3.Zero - cameraPosition;
            cameraDirection.Normalize();
            cameraUp = Vector3.Up;
            CreateLookAt();
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R)) reset();

            if (Math.Abs(currentYaw) > totalYaw)
            {
                if (currentYaw < 0) currentYaw = -totalYaw - speed;
                else currentYaw = totalYaw + speed;
            }
            if (Math.Abs(currentPitch) > totalPitch)
            {
                if (currentPitch < 0) currentPitch = -totalPitch - speed;
                else currentPitch = totalPitch + speed;
            }
            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (Math.Abs(currentYaw) <= totalYaw)
                {
                    cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationY(-speed));
                    cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, -speed));
                }
                currentYaw -= speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (Math.Abs(currentYaw) <= totalYaw)
                {
                    cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationY(speed));
                    cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, speed));
                }
                currentYaw += speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (Math.Abs(currentPitch) <= totalPitch)
                {
                    cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(-speed));
                    cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateRotationX(-speed));                    
                }
                currentPitch -= speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (Math.Abs(currentPitch) <= totalPitch)
                {
                    cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(speed));
                    cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateRotationX(speed));                    
                }
                currentPitch += speed;
            }

            // Recreate the camera view matrix
            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }
    }
}
