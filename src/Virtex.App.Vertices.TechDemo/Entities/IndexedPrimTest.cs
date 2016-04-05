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
	public class IndexedPrimTest : vxEntity3D
    {
		VertexPositionTexture[] floorVerts;
		BasicEffect effect;
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
		public IndexedPrimTest(vxEngine vxEngine, Model entityModel, Vector3 StartPosition)
            : base(vxEngine, entityModel, StartPosition)
        {

			floorVerts = new VertexPositionTexture[6];
			floorVerts [0].Position = new Vector3 (20, 0, 20);
			floorVerts [1].Position = new Vector3 (-20, 0,  20);
			floorVerts [2].Position = new Vector3 ( 20, 0, -20);
			floorVerts [3].Position = floorVerts[1].Position;
			floorVerts [4].Position = new Vector3 ( -20, 0,  -20);
			floorVerts [5].Position = floorVerts[2].Position;


			floorVerts [0].TextureCoordinate = new Vector2 (0, 0);
			floorVerts [1].TextureCoordinate = new Vector2 (0, 1);
			floorVerts [2].TextureCoordinate = new Vector2 (1, 0);

			floorVerts [3].TextureCoordinate = floorVerts[1].TextureCoordinate;
			floorVerts [4].TextureCoordinate = new Vector2 (1, 1);
			floorVerts [5].TextureCoordinate = floorVerts[2].TextureCoordinate;
			// We’ll be assigning texture values later
			effect = new BasicEffect (vxEngine.GraphicsDevice);
        }

		public override void RenderMesh(string RenderTechnique)
		{

			effect.View = vxEngine.Current3DSceneBase.Camera.View;
			effect.Projection =vxEngine.Current3DSceneBase.Camera.Projection;

			effect.TextureEnabled = true;
			effect.Texture = vxEngine.Assets.Textures.Texture_Sun_Glow;


			foreach (var pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply ();

				vxEngine.GraphicsDevice.DrawUserPrimitives (
					// We’ll be rendering two trinalges
					PrimitiveType.TriangleList,
					// The array of verts that we want to render
					floorVerts,
					// The offset, which is 0 since we want to start 
					// at the beginning of the floorVerts array
					0,
					// The number of triangles to draw
					2);
			}
		}

    }
}
