
//XNA/MONOGAME
using Microsoft.Xna.Framework;

//Virtex vxEngine Declaration
using vxVertices.Core;
using vxVertices.Core.Entities;
using vxVertices.Graphics;

namespace VerticeEnginePort.Base
{
    public class xEnvrio : vxEntity3D
    {
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public xEnvrio(vxEngine vxEngine, vxModel entityModel, Vector3 StartPosition)
            : base(vxEngine, entityModel, StartPosition)
        {
            //World = Matrix.CreateRotationX(-MathHelper.PiOver2);
        }
    }
}
