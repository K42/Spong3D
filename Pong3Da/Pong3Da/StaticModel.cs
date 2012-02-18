using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//dziwna bezuzyteczna klasa.
namespace Pong3Da
{
    class StaticModel
    {
        public String modelID { get; protected set; }
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        private float rotationX = 0, rotationY = 0, rotationZ = 0;

        public StaticModel(Model m, float rX, float rY, float rZ)
        {
            this.model = m;
            this.rotationX = MathHelper.ToRadians(rX);
            this.rotationY = MathHelper.ToRadians(rY);
            this.rotationZ = MathHelper.ToRadians(rZ);
        }
        public StaticModel(Model m)
        {
            this.model = m;
        }

        public virtual void Update()
        {
        }

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Alpha = 0.25f;
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform
                        * Matrix.CreateRotationX(rotationX)
                        * Matrix.CreateRotationY(rotationY)
                        * Matrix.CreateRotationZ(rotationZ);
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
