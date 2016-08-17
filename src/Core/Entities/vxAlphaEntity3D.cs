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
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Audio;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Graphics;


namespace Virtex.Lib.Vrtc.Core.Entities
{
    /// <summary>
    /// An Alpha Entity which allows for special see through rendering. Note This will not respond to Edge Detection, Shadows
    /// or deferred rendering.
    /// </summary>
	public class vxAlphaEntity3D : vxEntity3D
    {
       public vxAlphaEntity3D(vxEngine vxEngine, vxModel EntityModel, Vector3 StartPosition) : 
            base(vxEngine, EntityModel, StartPosition)
        {

        }
    }
}

#endif