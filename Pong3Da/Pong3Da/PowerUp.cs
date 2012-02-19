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

/*
 * green - speed-up player
 * blue - slow-down the ball
 * red - slow-down opponent
 * black - bullet-time 
 */
namespace Pong3Da
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 
    public enum flavor 
        {  
            green,
            blue,
            red,
            black
        }

    public class PowerUp : Microsoft.Xna.Framework.GameComponent
    {
        public Model model, green, blue, red, black;
        protected Matrix world = Matrix.Identity;

        private float size;
        public Vector3 position;

        public flavor f { get; private set; }
        public float value;
        public int duration { get; protected set; }

        public bool active { get; protected set; }
        public bool applied { get; protected set; }
        private int helper = 0;
        private Random r = new Random();

        private TimeSpan timer = TimeSpan.FromSeconds(0);

        public double gettimer()
        {
            return timer.TotalSeconds;
        }
        public PowerUp(Game game)
            : base(game)
        {
            green = Game.Content.Load<Model>(@"models\pu_green");
            blue = Game.Content.Load<Model>(@"models\pu_blue");
            red = Game.Content.Load<Model>(@"models\pu_red");
            black = Game.Content.Load<Model>(@"models\pu_black");
            model = green;
            size = model.Meshes[0].BoundingSphere.Radius;
            RollPosition();
            RollFlavor();
            active = true;
            applied = false;
            // TODO: Construct any child components here
        }
        private bool RollFlavor()
        {
            string s = "pu_green";
            f = flavor.green;
            duration = 10;
            value = 1.3f;
            model = green;
            int k = r.Next(1, 100);
            if (k % 5 == 0)
            {
                f = flavor.black;
                model = black;
                duration = 15;
                value = 0.4f;
                return true;
            }
            if (k % 4 == 0)
            {
                f = flavor.red;
                duration = 5;
                value = 0.5f;
                model = red;
                return true;
            }
            if (k % 3 == 0)
            {
                f = flavor.blue;
                duration = 5;
                value = 0.1f;
                model = blue;
                return true;
            }
            return true;
        }
        private void RollPosition()
        {
            Matrix rotation = Matrix.CreateRotationX(r.Next(1, 30)/10) * Matrix.CreateRotationY(r.Next(1, 30)/10);
            position = Vector3.Transform(new Vector3(0, 0, 50), rotation);
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
        public Color GetFlavorColor()
        {
            if (f == flavor.green)
                return Color.Green;
            if (f == flavor.blue)
                return Color.Blue;
            if (f == flavor.red)
                return Color.Red;
            if (f == flavor.black)
                return Color.White;
            return Color.Black;
        }
        public bool Intersect(Vector3 coords, int rad)
        {
            if (active)
            {
                if (Vector3.Distance(coords, position) <= (rad + size))
                {
                    return true;
                }
                else return false;
            }
            else return false;

        }
        public float ApplyPowerUp()
        {
            applied = true;
            active = false;
            timer = TimeSpan.FromSeconds(0);
            return value;
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (active && !applied)
            {
                timer += gameTime.ElapsedGameTime;
                if (timer.TotalSeconds > r.Next(20, 35))
                {
                    active = false;
                    timer = TimeSpan.FromSeconds(0);
                }
            }
            if (!active && !applied)
            {
                timer += gameTime.ElapsedGameTime;
                if (timer.TotalSeconds > r.Next(10, 15))
                {
                    RollFlavor();
                    RollPosition();
                    active = true;
                }
            }
            if (applied)
            {
                timer += gameTime.ElapsedGameTime;
                if (timer.TotalSeconds > duration)
                {
                    active = false;
                    applied = false;
                }
            }
            base.Update(gameTime);
        }

        public void Draw(Camera camera)
        {
            if (active)
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
                        be.World = GetWorld() * mesh.ParentBone.Transform
                            * Matrix.CreateTranslation(position);
                    }
                    mesh.Draw();
                }
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
    }
}
