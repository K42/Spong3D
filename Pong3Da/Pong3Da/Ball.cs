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
    public class Ball : Microsoft.Xna.Framework.GameComponent
    {
        public Vector3 position;
        public bool freeze { get; set; }
        public float ballSpeed;
        public Model model;

        protected Matrix world = Matrix.Identity;

        public Vector3 velocity = Vector3.Zero;
        public float direction = 0.0f;


        public Ball(Game game)
            : base(game)
        {
            position = Vector3.Zero;
            freeze = true;
            ballSpeed = 0.05f;
            model = Game.Content.Load<Model>(@"models\testbox");
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //// TODO: Add your update code here
            if (freeze != true)
            {
                Bounce();
            }
            base.Update(gameTime);
        }


        private void Bounce()
        {
            position += velocity;
            direction -= ballSpeed * 0.10f;

            Vector3 velocityDelta = Vector3.Zero;

            //wyznaczanie predkosci na podstawie kierunku
            //TODO: trzecia wspolrzedna
            velocityDelta.X = -(float)Math.Sin(direction);
            velocityDelta.Z = -(float)Math.Cos(direction);

            // Now scale our direction by how hard the trigger is down.
            velocityDelta *= ballSpeed;
            velocity += velocityDelta;
        }

        //rysowanie pilki
        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform * Matrix.CreateTranslation(position);
                }
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
       
    }
}
