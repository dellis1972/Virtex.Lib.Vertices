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
using vxVertices.Core;
using vxVertices.Core.Entities;
using vxVertices.Graphics;

namespace vxVertices.Scenes.Sandbox.Entities
{
    public class vxWorkingPlane : vxEntity3D
    {
        public Plane WrknPlane;

        /// <summary>
        /// This provides the offset needed between the Phsyics skin and the mesh.
        /// </summary>
        public Vector3 Vector_ModelOffSet = Vector3.Zero;

        /// <summary>
        /// Provides the needed X rotation for the model
        /// </summary>
        public float XRotation_ModelOffset = 0;

        /// <summary>
        /// Provides the needed Y rotation for the model
        /// </summary>
        public float YRotation_ModelOffset = 0;

        /// <summary>
        /// Provides the needed Z rotation for the model
        /// </summary>
        public float ZRotation_ModelOffset = 0;



        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public vxWorkingPlane(vxEngine vxEngine, vxModel entityModel, Vector3 StartPosition)
            : base(vxEngine, entityModel, StartPosition)
        {
            //InitShaders();

            //Render even in debug mode
            RenderEvenInDebug = true;

            WrknPlane = new Plane(Vector3.Up, -Position.Y);

            AlphaValue = 0.05f;
        }
        
        public override void RenderMeshForWaterReflectionPass(Plane ReflectedView) { }
        //public override void RenderMeshPrepPass() { }

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(1);
            //World *= Matrix.CreateRotationX(-MathHelper.PiOver2);
            World *= Matrix.CreateTranslation(Position - Vector3.Up * 0.5f);

            WrknPlane = new Plane(Vector3.Up, -Position.Y - 0.5f);
            AlphaValue = 0.5f;

            base.Update(gameTime);
        }
    }
}
#endif