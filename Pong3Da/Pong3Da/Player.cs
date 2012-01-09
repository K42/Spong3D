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
    /// 
    
    public class Player : Microsoft.Xna.Framework.GameComponent
    {
        public String nickname { get; protected set; }
        public int pts = 0;
        public int colour;
        public Vector3 position { get; protected set; }
        public float speed { get; protected set; }
        private Model model;

        protected Matrix world = Matrix.Identity;

        //o huj tu chodzi?
        public Player(Game game, Camera camera)
            : base(game)
        {
            position = camera.position;
            speed = 0.5f;
            model = Game.Content.Load<Model>(@"models\plate3");
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
            // TODO: Add your update code here
            
            base.Update(gameTime);
        }
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
                    be.World = GetWorld() * mesh.ParentBone.Transform *Matrix.CreateTranslation(position) * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, -15.0f)) ;
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
