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
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Core.Entities;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Core.Cameras;

namespace Virtex.Lib.Vertices.Graphics
{
    public enum LightType
    {
        Point,
        Directional
    }

	/// <summary>
	/// Base class for a 3D Light Entity used in Defferred Rendering.
	/// </summary>
	public class vxLightEntity
    {
		/// <summary>
		/// Gives reference to the vxEngine from the entity
		/// </summary>
		public vxEngine vxEngine { get; set; }

        public LightType LightType { get; set; }

        /// <summary>
        /// Location of Entity in world space.
        /// </summary>
        public Vector3 Position
		{
			set { _position = value; }
			get { return _position; }
		}
		private Vector3 _position;

		/// <summary>
		/// The Light Colour.
		/// </summary>
		public Color Color { get; set; }

        float lightRadius { get; set; }

        float lightIntensity { get; set; }

        public vxCamera3D camera
        {
#if VIRTICES_3D
            get { return vxEngine.Current3DSceneBase.Camera; }
#else
			get { return new vxCamera3D(); }
#endif

		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vertices.Graphics.vxLightEntity"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="StartPosition">Start position.</param>
        public vxLightEntity(vxEngine vxEngine, Vector3 StartPosition, LightType LightType, Color Colour, float lightRadius, float lightIntensity)
        {
			this.vxEngine = vxEngine;

#if VIRTICES_3D
            vxEngine.Current3DSceneBase.LightItems.Add(this);
#endif

			Position = StartPosition;

			this.Color = Colour;

            this.LightType = LightType;

            this.lightRadius = lightRadius;

            this.lightIntensity = lightIntensity;
        }

		public virtual void Update(GameTime gameTime)
		{

		}

		public virtual void Draw()
		{
            switch(this.LightType)
            {
                case LightType.Point:
                    DrawPointLight(this.Position, this.Color, this.lightRadius, this.lightIntensity);
                    break;
            }
		}

        private void DrawPointLight(Vector3 lightPosition, Color color, float lightRadius, float lightIntensity)
        {
            Effect pointLightEffect = vxEngine.Assets.Shaders.DrfrdRndrPointLight;

            //set the G-Buffer parameters
            pointLightEffect.Parameters["colorMap"].SetValue(vxEngine.Renderer.RT_ColourMap);
            pointLightEffect.Parameters["normalMap"].SetValue(vxEngine.Renderer.RT_NormalMap);
            pointLightEffect.Parameters["depthMap"].SetValue(vxEngine.Renderer.RT_DepthMap);

            //compute the light world matrix
            //scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.CreateScale(lightRadius) * Matrix.CreateTranslation(lightPosition);
            pointLightEffect.Parameters["World"].SetValue(sphereWorldMatrix);
            pointLightEffect.Parameters["View"].SetValue(camera.View);
            pointLightEffect.Parameters["Projection"].SetValue(camera.Projection);
            //light position
            pointLightEffect.Parameters["lightPosition"].SetValue(lightPosition);

            //set the color, radius and Intensity
            pointLightEffect.Parameters["Color"].SetValue(color.ToVector3());
            pointLightEffect.Parameters["lightRadius"].SetValue(lightRadius);
            pointLightEffect.Parameters["lightIntensity"].SetValue(lightIntensity);

            //parameters for specular computations
            pointLightEffect.Parameters["cameraPosition"].SetValue(camera.Position);
            pointLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(camera.View * camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
#if VIRTICES_3D
            pointLightEffect.Parameters["halfPixel"].SetValue(vxEngine.Current3DSceneBase.HalfPixel);
#endif
			//calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(camera.Position, lightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            if (cameraToCenter < lightRadius)
                vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            else
                vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            pointLightEffect.Techniques[0].Passes[0].Apply();
#if VIRTICES_3D
            foreach (ModelMesh mesh in vxEngine.Current3DSceneBase.sphereModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    vxEngine.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    vxEngine.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);

                    vxEngine.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }
#endif
            vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
#endif