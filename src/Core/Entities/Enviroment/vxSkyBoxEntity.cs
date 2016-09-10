#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Cameras;

namespace Virtex.Lib.Vrtc.Core.Entities
{

    public class vxSkyBoxEntity : vxEntity3D
    {
        /// <summary>
        /// The skybox model, which will just be a cube
        /// </summary>
        private Model skyBox;

        /// <summary>
        /// The actual skybox texture
        /// </summary>
        private TextureCube skyBoxTexture;

        /// <summary>
        /// The effect file that the skybox will use to render
        /// </summary>
        private Effect skyBoxEffect;

        /// <summary>
        /// The size of the cube, used so that we can resize the box
        /// for different sized environments.
        /// </summary>
        private float size = 150f;

        vxEngine Engine;

        RasterizerState NewRS;
        RasterizerState OldRS;

        /// <summary>
        /// Creates a new skybox
        /// </summary>
        /// <param name="skyboxTexture">the name of the skybox texture to use</param>
        public vxSkyBoxEntity(vxEngine engine):base(engine, Vector3.Zero)
        {
            Engine = engine;

            skyBox = Engine.EngineContentManager.Load<Model>("Shaders/Skybox/cube");
            skyBoxTexture = Engine.EngineContentManager.Load<TextureCube>("Shaders/Skybox/Skybox");
            skyBoxEffect = Engine.EngineContentManager.Load<Effect>("Shaders/Skybox/SkyBoxShader");
            NewRS = new RasterizerState();
            NewRS.CullMode = CullMode.CullClockwiseFace;
        }

        public override void RenderMeshShadow() { }
        public override void RenderMeshPrepPass() { }
        public override void RenderMeshForWaterReflectionPass(Plane surfacePlane)
        {
            OldRS = Engine.GraphicsDevice.RasterizerState;
            Engine.GraphicsDevice.RasterizerState = NewRS;

            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                // Draw all of the components of the mesh, but we know the cube really
                // only has one mesh
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyBoxEffect;
                        part.Effect.CurrentTechnique = skyBoxEffect.Techniques["Skybox"];
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(Camera.Position));
                        part.Effect.Parameters["View"].SetValue(Camera.GetReflectionView(surfacePlane));
                        part.Effect.Parameters["Projection"].SetValue(Camera.Projection);
                        part.Effect.Parameters["ClipPlane0"].SetValue(new Vector4(surfacePlane.Normal, surfacePlane.D));
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(Camera.Position);
                    }

                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }
            Engine.GraphicsDevice.RasterizerState = OldRS;
        }

        public override void RenderMesh(string RenderTechnique)
        {
            size = Camera.FarPlane * 0.6f;

            OldRS = Engine.GraphicsDevice.RasterizerState;
            Engine.GraphicsDevice.RasterizerState = NewRS;

            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                // Draw all of the components of the mesh, but we know the cube really
                // only has one mesh
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyBoxEffect;
                        part.Effect.CurrentTechnique = skyBoxEffect.Techniques["Skybox"];
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(Camera.Position));
                        part.Effect.Parameters["View"].SetValue(Camera.View);
                        part.Effect.Parameters["Projection"].SetValue(Camera.Projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(Camera.Position);
                    }

                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }
            Engine.GraphicsDevice.RasterizerState = OldRS;
        }
    }
}
#endif