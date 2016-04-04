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
using vxVertices.Utilities;


namespace VerticeEnginePort.Base
{
    public class xEnvrio : vxEntity3D
    {
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public xEnvrio(vxEngine vxEngine, Model entityModel, Vector3 StartPosition)
            : base(vxEngine, entityModel, StartPosition)
        {
            //World = Matrix.CreateRotationX(-MathHelper.PiOver2);
        }
    }
}
