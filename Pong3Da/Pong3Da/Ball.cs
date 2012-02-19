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
        public Vector3 baseSpeed;
        public Vector3 position;
        public Vector3 prevPosition;
        public Vector3 direction;

        public bool freeze { get; set; }
        TimeSpan timer = TimeSpan.FromSeconds(0);
        int bonusDuration = 0;

        Random r = new Random();

        public Ball(Game game)
            : base(game) {
            position = Vector3.Zero;
            prevPosition = position;
            freeze = true;
            ballSpeed = Vector3.One * 0.5f;
            baseSpeed = ballSpeed;
            direction = new Vector3((float) (r.NextDouble() - 0.5), (float) (r.NextDouble() - 0.5), (float) (r.NextDouble() - 0.5));
            direction.Normalize();
            model = Game.Content.Load<Model>(@"models\playbox");
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
            if (bonusDuration == 0) ballSpeed = baseSpeed;
            if (!freeze) {
                prevPosition = position;
                position += ballSpeed * direction;
            }
            //sprawdzanie bonusa
            if (bonusDuration > 0)
            {
                timer += gameTime.ElapsedGameTime;
                if (timer.TotalSeconds > bonusDuration)
                {
                    ballSpeed = baseSpeed;
                    bonusDuration = 0;
                }
            }
            else timer = TimeSpan.FromSeconds(0);
            base.Update(gameTime);
        }
        public void setSpeed(int s)
        {
            switch (s)
            {
                case 1:
                    baseSpeed = Vector3.One * 0.5f;
                    break;
                case 2:
                    baseSpeed = Vector3.One * 0.8f;
                    break;
                case 3:
                    baseSpeed = Vector3.One * 1.1f;
                    break;
            }
        }
        //losowe wyznaczanie kierunku
        //public void ChangeDirectionAtRandom() {
        //    direction = Vector3.Negate(direction);
        //    direction = Vector3.Transform(direction,
        //        Matrix.CreateFromYawPitchRoll((float) r.NextDouble(), (float) r.NextDouble(), (float) r.NextDouble()));
        //    direction.Normalize();
        //}
        //do testow
        public void negate()
        {
            direction = Vector3.Negate(direction);
            direction.Normalize();
        }
        //reset pozycji pilki, losowy kierunek
        //public void reset()
        //{
        //    position = Vector3.Zero;
        //    freeze = true;
        //    ChangeDirectionAtRandom();
        //}
        //reset pozycji pilki, okreslony kierunek
        //wywolywana po zdobyciu punktu
        public void reset(Vector3 direction)
        {
            position = Vector3.Zero;
            freeze = true;
            this.direction = direction;
            negate();
            direction.Normalize();
        }
        //odbicie
        public void reflect(float hit, Vector3 surface)
        {
            hit *= 10;
            direction = Vector3.Reflect(direction, surface) * Math.Abs(hit);            
            direction.Normalize();
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
                    be.World = GetWorld() * mesh.ParentBone.Transform
                        * Matrix.CreateTranslation(position);
                }
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld() {
            return world;
        }

        //aktywacja bonusu dla pilki
        public void PowerUp(int active, float value)
        {
            this.bonusDuration = active;
            ballSpeed = Vector3.One * (baseSpeed * value);
            //else ballSpeed = baseSpeed;
        }
    }
}
