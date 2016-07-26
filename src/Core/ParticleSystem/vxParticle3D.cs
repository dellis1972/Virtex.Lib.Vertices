#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Core.Entities;
using Virtex.Lib.Vertices.Graphics;

namespace Virtex.Lib.Vertices.Core.Particles
{
	/// <summary>
	/// 3D Particle Object for use in the vxParticleSystem3D Manager Class.
	/// </summary>
    public class vxParticle3D : vxEntity3D
    {
		/// <summary>
		/// Boolean of whether to keep the Particle Alive or not.
		/// </summary>
        public bool Alive = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vertices.Core.Particles.vxParticle3D"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="model">Model.</param>
		/// <param name="StartPosition">Start position.</param>
		public vxParticle3D(vxEngine vxEngine, vxModel model, Vector3 StartPosition) 
            : base(vxEngine, model, StartPosition)
        {

        }
    }
}
#endif