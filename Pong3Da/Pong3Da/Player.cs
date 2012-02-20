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
        public int nr { get; protected set; }
        public int pts = 0;
        public Vector3 position { get; protected set; }
        private Model model;
        private float[] dim = new float[5];

        protected Matrix world = Matrix.Identity;

        public Player(Game game, int n)
            : base(game)
        {
            this.nr = n;
            position = new Vector3(0, 0, 51); //camera.position;
            model = Game.Content.Load<Model>(@"models\plate");
            //bb = new BoundingBox();
            // TODO: Construct any child components here
        }
        //resetowanie punktow
        public void reset()
        {
            pts = 0;
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
        //rysowanie paletki
        public void Draw(PlayerView camera)
        {
            dim = camera.getDim();
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix plateRotation = Matrix.Identity;
            plateRotation.Forward.Normalize();
            //position = camera.position;
            plateRotation = Matrix.CreateRotationX(camera.getDim()[3]) * Matrix.CreateRotationY(camera.getDim()[4]);
            position = Vector3.Transform(new Vector3(0, 0, 51), plateRotation);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Alpha = 0.25f;
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform *
                        Matrix.CreateRotationX(camera.getDim()[3]) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(90)) * 
                        Matrix.CreateRotationY(camera.getDim()[4]) *
                        Matrix.CreateScale(4.0f) *
                        Matrix.CreateTranslation(position);
                }
                mesh.Draw();
            }
        }
        public void Draw(PlayerView data, PlayerView viewer)
        {
            dim = data.getDim();
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix plateRotation = Matrix.Identity;
            plateRotation.Forward.Normalize();
            //position = camera.position;
            plateRotation = Matrix.CreateRotationX(data.getDim()[3]) * Matrix.CreateRotationY(data.getDim()[4]);
            position = Vector3.Transform(new Vector3(0, 0, 51), plateRotation);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Alpha = 0.1f;
                    be.EnableDefaultLighting();
                    be.Projection = viewer.projection;
                    be.View = viewer.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform *
                        Matrix.CreateRotationX(data.getDim()[3]) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                        Matrix.CreateRotationY(data.getDim()[4]) *
                        Matrix.CreateScale(4.0f) *
                        Matrix.CreateTranslation(position);
                }
                mesh.Draw();
            }
        }
        //zwraca wektor kierunkowy na paletke
        //takie ulatwienie
        //gdy ktos zdobedzie punkt, pilka automatycznie ustawia kierunek na paletke przegranego
        public Vector3 GetFaceVector()
        {
            Vector3 dir = (Vector3.Zero - position);
            dir.Normalize();
            return dir;
        }
        public virtual Matrix GetWorld()
        {
            return world;
        }
        
    }
}
