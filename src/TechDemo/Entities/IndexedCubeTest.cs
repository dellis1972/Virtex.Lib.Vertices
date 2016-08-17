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
using Virtex.Lib.Vrtc.Graphics;

namespace VerticeEnginePort.Base
{
	public class IndexedCubeTest : vxEntity3D
    {
		//VertexPositionTexture[] floorVerts;
		BasicEffect effect;
        vxModelVoxel model;
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public IndexedCubeTest(vxEngine vxEngine, Vector3 StartPosition)
            : base(vxEngine, StartPosition)
        {
			string path = "Content\\Models\\concrete_cube_obj\\concrete_cube.obj";
			#if TECHDEMO_PLTFRM_GL
			path = "Content\\Compiled.WindowsGL\\Models\\concrete_cube_obj\\concrete_cube.obj";
			#endif
			model = new vxModelVoxel(path);

            // We’ll be assigning texture values later
            effect = new BasicEffect (vxEngine.GraphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = vxEngine.Game.Content.Load<Texture2D>("Models\\concrete_cube\\texture");
        }

		//public void Render()
        public override void RenderMesh(string RenderTechnique)
        {

            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.DepthStencilState = DepthStencilState.None;
			/*
            this.vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
            this.vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            this.vxEngine.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
*/
            effect.View = vxEngine.Current3DSceneBase.Camera.View;
			effect.Projection =vxEngine.Current3DSceneBase.Camera.Projection;

            effect.EnableDefaultLighting();
            
            effect.World = this.World;
            

            if(model.MeshVertices.Count > 0)
			foreach (var pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply ();

				vxEngine.GraphicsDevice.DrawUserPrimitives (
					// We’ll be rendering two trinalges
					PrimitiveType.TriangleList,
					// The array of verts that we want to render
					model.MeshVertices.ToArray(),
					// The offset, which is 0 since we want to start 
					// at the beginning of the floorVerts array
					0,
                    // The number of triangles to draw
                    model.MeshVertices.Count/3);
			}
		}

    }
}
