#if VIRTICES_3D
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vrtc.Core.Entities;
using Virtex.Lib.Vrtc.Graphics;

namespace Virtex.Lib.Vrtc.Core
{
	/// <summary>
	/// This special Alpha Entity is a vxEntity3D which does not send it's depth or normal's to the graphics
	/// renderer. it is useful for particles, smoke, etc...
	/// </summary>
	public class vxAlphaEntity3D : vxEntity3D
	{
        public Texture2D Texture;

		public vxAlphaEntity3D(vxEngine Engine, vxModel model, Vector3 Pos) : 
			base(Engine, model, Pos)
		{
            Texture = Engine.Assets.Textures.Blank;
        }

		public override void RenderMeshPrepPass() { }
		public override void RenderMeshShadow() { }
		public override void RenderMesh(string RenderTechnique) {
			RenderAlpha();
		}

		/// <summary>
		/// Renders the alpha model.
		/// </summary>
		public virtual void RenderAlpha()
		{
			if (vxModel.ModelMain != null)
			{
				// Copy any parent transforms.
				Matrix[] transforms = new Matrix[vxModel.ModelMain.Bones.Count];
				vxModel.ModelMain.CopyAbsoluteBoneTransformsTo(transforms);

				// Draw the model. A model can have multiple meshes, so loop.
				foreach (ModelMesh mesh in vxModel.ModelMain.Meshes)
				{
					// This is where the mesh orientation is set, as well 
					// as our camera and projection.
					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.World = this.World;
						effect.View = this.Camera.View;
                        effect.Texture = Texture;
						effect.Projection = this.Camera.Projection;
                        effect.TextureEnabled = true;
                        effect.EmissiveColor = Color.White.ToVector3();
					}
					// Draw the mesh, using the effects set above.
					mesh.Draw();
				}
			}
		}
	}
}
#endif