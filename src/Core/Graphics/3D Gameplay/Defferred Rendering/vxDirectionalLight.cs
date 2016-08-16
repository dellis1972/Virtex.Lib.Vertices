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
using Virtex.Lib.Vrtc.Core.Entities;
using Virtex.Lib.Vrtc.Utilities;


namespace Virtex.Lib.Vrtc.Graphics
{
	/// <summary>
	/// Base class for a 3D Light Entity used in Defferred Rendering.
	/// </summary>
	public class vxDirectionalLight : vxLightEntity
    {
		/// <summary>
		/// Direction Entity is facing.
		/// </summary>
		public Vector3 LightDirection { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Graphics.vxDirectionalLight"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="LightDirection">Light direction.</param>
		public vxDirectionalLight(vxEngine vxEngine, Vector3 LightDirection, Color Colour)
			: base(vxEngine, Vector3.Zero, LightType.Directional, Colour, 1, 2)
        {
			this.LightDirection = LightDirection;
        }

		public override void Draw()
		{
			Effect directionalLightEffect = vxEngine.Assets.Shaders.DrfrdRndrDirectionalLight;
			directionalLightEffect.Parameters["lightDirection"].SetValue(this.LightDirection);
			directionalLightEffect.Parameters["Color"].SetValue(this.Color.ToVector3());

			directionalLightEffect.Techniques[0].Passes[0].Apply();
			vxEngine.Renderer.RenderQuad(Vector2.One * -1, Vector2.One);   
		}
    }
}
#endif