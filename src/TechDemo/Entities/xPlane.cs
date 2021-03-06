﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;
//
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Input;
//using vxVertices.Core;
//using vxVertices.Core.Entities;
//
////Virtex vxEngine Declaration
// 
//
//namespace MetricRacer.Base
//{
//    public class xPlane : vxEntity3D
//    {
//
//        /// <summary>
//        /// This provides the offset needed between the Phsyics skin and the mesh.
//        /// </summary>
//        public Vector3 Vector_ModelOffSet = Vector3.Zero;
//
//        /// <summary>
//        /// Provides the needed X rotation for the model
//        /// </summary>
//        public float XRotation_ModelOffset = 0;
//
//        /// <summary>
//        /// Provides the needed Y rotation for the model
//        /// </summary>
//        public float YRotation_ModelOffset = 0;
//
//        /// <summary>
//        /// Provides the needed Z rotation for the model
//        /// </summary>
//        public float ZRotation_ModelOffset = 0;
//
//        
//
//        /// <summary>
//        /// Creates a New Instance of the Base Ship Class
//        /// </summary>
//        /// <param name="AssetPath"></param>
//        public xPlane(vxEngine vxEngine, Model entityModel, Vector3 StartPosition)
//            : base(vxEngine, entityModel, StartPosition)
//        {
//           InitShaders();
//
//            //Render even in debug mode
//            RenderEvenInDebug = true;            
//        }
//
//        ///// <summary>
//        ///// Applies a simple rotation to the ship and animates position based
//        ///// on simple linear motion physics.
//        ///// </summary>
//        //public override void Update(GameTime gameTime)
//        //{
//        //    World = Matrix.CreateScale(2);
//        //    base.Update(gameTime);
//        //}
//    }
//}
