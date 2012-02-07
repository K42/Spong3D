using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Pong3Da {
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Ball : Microsoft.Xna.Framework.GameComponent {
        public Model model;

        protected Matrix world = Matrix.Identity;

        public Vector3 ballSpeed;
        public Vector3 position;
        public Vector3 prevPosition;
        public Vector3 direction;

        public bool freeze { get; set; }

        Random r = new Random();

        public Ball(Game game)
            : base(game) {
            position = Vector3.Zero;
            prevPosition = position;
            freeze = true;
            ballSpeed = Vector3.One * 0.5f;
            direction = new Vector3((float) (r.NextDouble() - 0.5), (float) (r.NextDouble() - 0.5), (float) (r.NextDouble() - 0.5));
            direction.Normalize();
            model = Game.Content.Load<Model>(@"models\testbox");
            // TODO: Construct any child components here
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
            //// TODO: Add your update code here
            if (!freeze) {
                prevPosition = position;
                position += ballSpeed * direction;
            }
            base.Update(gameTime);
        }

        public void ChangeDirectionAtRandom() {
            direction = Vector3.Negate(direction);
            direction = Vector3.Transform(direction,
                Matrix.CreateFromYawPitchRoll((float) r.NextDouble(), (float) r.NextDouble(), (float) r.NextDouble()));
            direction.Normalize();
        }
        public void reset()
        {
            position = Vector3.Zero;
            freeze = true;
            ChangeDirectionAtRandom();
        }
        public void reflect(float hit)
        {
            //bleh
        }

        //rysowanie pilki
        public void Draw(Camera camera) {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect be in mesh.Effects) {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform * Matrix.CreateTranslation(position);
                }
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld() {
            return world;
        }
    }
}
