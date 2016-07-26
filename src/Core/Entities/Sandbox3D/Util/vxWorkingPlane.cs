#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Core.Entities;
using Virtex.Lib.Vertices.Graphics;

namespace Virtex.Lib.Vertices.Scenes.Sandbox.Entities
{
    public class vxWorkingPlane : vxEntity3D
    {
        /// <summary>
        /// Working Plane Object
        /// </summary>
        public Plane WrknPlane;

        /// <summary>
        /// Vertices Collection of Grid
        /// </summary>
        List<VertexPositionColor> vertices;

        /// <summary>
        /// Basic Effect to Render Working Plane
        /// </summary>
        BasicEffect basicEffect;

        /// <summary>
        /// Creates a New Instance of the Working Plane Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public vxWorkingPlane(vxEngine vxEngine, vxModel entityModel, Vector3 StartPosition)
            : base(vxEngine, entityModel, StartPosition)
        {

            //Render even in debug mode
            RenderEvenInDebug = true;

            WrknPlane = new Plane(Vector3.Up, -Position.Y);

            AlphaValue = 0.05f;

            basicEffect = new BasicEffect(this.vxEngine.GraphicsDevice);

            int size = 10000;

            vertices = new List<VertexPositionColor>();
            for (int i = -size; i < size + 1; i += 10)
            {
                Color color = i % 100 == 0 ? Color.White : Color.Gray * 1.5f;

                vertices.Add(new VertexPositionColor(
                     new Vector3(i, 0, -size),
                     color
                     ));

                vertices.Add(new VertexPositionColor(
                     new Vector3(i, 0, size),
                     color
                     ));


                vertices.Add(new VertexPositionColor(
                    new Vector3(-size, 0, i),
                    color
                    ));

                vertices.Add(new VertexPositionColor(
                     new Vector3(size, 0, i),
                     color
                     ));

            }
        }

        public override void RenderMeshForWaterReflectionPass(Plane ReflectedView) { }
        public override void RenderMeshPrepPass() { }

       /// <summary>
       /// Updates the Working Plane
       /// </summary>
       /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(1);
            World *= Matrix.CreateTranslation(Position - Vector3.Up * 0.5f);

            WrknPlane = new Plane(Vector3.Up, -Position.Y - 0.5f);
            AlphaValue = 0.5f;

            base.Update(gameTime);
        }

        public override void RenderMesh(string RenderTechnique)
        {
            //base.RenderMesh(RenderTechnique);

            //Set Basic Effect Info
            basicEffect.VertexColorEnabled = true;
            basicEffect.View = this.vxEngine.Current3DSceneBase.Camera.View;
            basicEffect.Projection = this.vxEngine.Current3DSceneBase.Camera.Projection;
            basicEffect.World = this.World;

            //Render Vertices List
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.vxEngine.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
            }
        }
    }
}
#endif