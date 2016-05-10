#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core.Entities;
using vxVertices.Core.Cameras;
 

namespace vxVertices.Core
{
    /// <summary>
    /// A Set which includes all the instance data for a specific model
    /// </summary>
    public class InstanceSet
    {
        /// <summary>
        /// The List of Entity Instances to Draw
        /// </summary>
		public List<vxEntity3D> instances = new List<vxEntity3D>();


        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        public static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public Matrix[] instanceTransforms;
        public Model InstancedModel
        {
            get { return instancedModel; }
            set
            {
                instancedModel = value;
                instancedModelBones = new Matrix[instancedModel.Bones.Count];
                instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);                
            }
        }
        Model instancedModel;

        public Matrix[] instancedModelBones;
        public DynamicVertexBuffer instanceVertexBuffer;
        vxEngine vxEngine;

        public InstanceSet(vxEngine vxEngine)
        {
            this.vxEngine = vxEngine;
        }

        public void Initialise(vxEngine vxEngine)
        {
            foreach (ModelMeshPart part in InstancedModel.Meshes.SelectMany(m => m.MeshParts))
            {
                part.Effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

                part.Effect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
                part.Effect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));


                part.Effect.Parameters["Shininess"].SetValue(0.01f);
                part.Effect.Parameters["SpecularIntensity"].SetValue(8.0f);

                part.Effect.Parameters["PoissonKernel"].SetValue(vxEngine.Renderer.poissonKernel);
                part.Effect.Parameters["RandomTexture3D"].SetValue(vxEngine.Renderer.randomTexture3D);
                part.Effect.Parameters["RandomTexture2D"].SetValue(vxEngine.Renderer.randomTexture3D);
                part.Effect.CurrentTechnique = part.Effect.Techniques["Lambert"];
            }
            
            foreach (var eff in InstancedModel.Meshes.SelectMany(m => m.Effects))
            {
                eff.CurrentTechnique = eff.Techniques["ShadowInstanced"];
            }
        }

        public void SetSize()
        {
            // Gather instance transform matrices into a single array.
            Array.Resize(ref instanceTransforms, instances.Count);
        }

        public void Update()
        {

            for (int i = 0; i < instances.Count; i++)
            {
                instanceTransforms[i] = instances[i].World;
            }

            if (instances.Count > 0)
            {
                // Make sure our instance data vertex buffer is big enough. (4x4 float matrix)
                int instanceDataSize = 16 * sizeof(float) * instances.Count;

                if ((instanceVertexBuffer == null) ||
                    (instanceVertexBuffer.VertexCount < instances.Count))
                {
                    if (instanceVertexBuffer != null)
                        instanceVertexBuffer.Dispose();

                    instanceVertexBuffer = new DynamicVertexBuffer(vxEngine.GraphicsDevice,
                        instanceVertexDeclaration, instances.Count, BufferUsage.WriteOnly);
                }

                // Upload transform matrices to the instance data vertex buffer.
                instanceVertexBuffer.SetData(instanceTransforms, 0, instances.Count, SetDataOptions.Discard);
            }
        }


        /// <summary>
        /// Renders the instanced shadow.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="camera">Camera.</param>
        /// <param name="numInstances">Number instances.</param>
        public virtual void RenderInstancedShadow(Model model, vxCamera3D camera, int numInstances)
        {
            
            //Only XNA Supports Instance Drawing Currently
#if VIRTICES_XNA
            for (int i = 0; i < vxEngine.Renderer.NumberOfShadowSplits; ++i)
            {
                {
                    int x = i % 2;
                    int y = i / 2;
                    var viewPort = new Viewport(
                        x * vxEngine.Renderer.ShadowMapSize, 
                        y * vxEngine.Renderer.ShadowMapSize,
                        vxEngine.Renderer.ShadowMapSize,
                        vxEngine.Renderer.ShadowMapSize);

                    vxEngine.GraphicsDevice.Viewport = viewPort;
                }

                // now render the spheres
                if (numInstances > 0)
                {
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (var part in mesh.MeshParts)
                        {
                            part.Effect.CurrentTechnique = part.Effect.Techniques["ShadowInstanced"];
                            part.Effect.Parameters["ViewProjection_Sdw"].SetValue(vxEngine.Renderer.ShadowSplitProjections[i]);
                            part.Effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
                            part.Effect.Parameters["DepthBias_Sdw"].SetValue(new Vector2(
                                vxEngine.Renderer.ShadowDepthBias[i, 0],
                                vxEngine.Renderer.ShadowDepthBias[i, 1]));
                            part.Effect.CurrentTechnique.Passes[0].Apply();
                            
                            // set vertex buffer
                            vxEngine.GraphicsDevice.SetVertexBuffers(new[]
                            {
                                part.VertexBuffer,
                                new VertexBufferBinding(instanceVertexBuffer, 0, 1 )
                            });

                            // set index buffer and draw
                            vxEngine.GraphicsDevice.Indices = part.IndexBuffer;
                            vxEngine.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount, numInstances);

                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Instanced rendering. Draws instancedModel numInstances times. This demo uses hardware
        /// instancing: a secondary vertex stream is created, where the transform matrices of the
        /// individual instances are passed down to the shader. Note that in order to be efficient,
        /// the model should contain as little meshes and meshparts as possible.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="camera">Camera.</param>
        /// <param name="numInstances">Number instances.</param>
        /// <param name="technique">Technique.</param>
        public virtual void RenderInstanced(Model model, vxCamera3D camera, int numInstances, string technique)
        {
            //Only XNA Supports Instance Drawing Currently
#if VIRTICES_XNA
            if (numInstances > 0)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // loop through meshes
                foreach (ModelMesh mesh in model.Meshes)
                {
                    /*
                    foreach (var shadowedEffect in mesh.Effects.Where(e => e.Parameters.Any(p => p.Name == "ShadowMap")))
                    {
                        shadowedEffect.Parameters["ShadowMap"].SetValue(RT_ShadowMap);
                        shadowedEffect.Parameters["ShadowTransform"].SetValue(ShadowSplitProjectionsWithTiling);
                        shadowedEffect.Parameters["TileBounds"].SetValue(ShadowSplitTileBounds);
                    }
                    */
                    // get bone matrix
                    Matrix boneMatrix = transforms[mesh.ParentBone.Index];
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect.CurrentTechnique = part.Effect.Techniques[technique];
                        part.Effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
                        part.Effect.Parameters["View"].SetValue(camera.View);
                        part.Effect.Parameters["Projection"].SetValue(camera.Projection);
                        part.Effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(vxEngine.Renderer.lightPosition));
                        
                        part.Effect.Parameters["ShadowMap"].SetValue(vxEngine.Renderer.RT_ShadowMap);
                        part.Effect.Parameters["ShadowTransform"].SetValue(vxEngine.Renderer.ShadowSplitProjectionsWithTiling);
                        part.Effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);

                        part.Effect.CurrentTechnique.Passes[0].Apply();

                        // set vertex buffer
                        //mGraphicsDevice.SetVertexBuffers(new[]
                        //{
                        //    part.VertexBuffer,
                        //    new VertexBufferBinding(mInstanceDataStream, 0, 1 )
                        //});

                        //// draw primitives 
                        //mGraphicsDevice.Indices = part.IndexBuffer;
                        //mGraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount, numInstances);
                        // set vertex buffer
                        vxEngine.GraphicsDevice.SetVertexBuffers(new[]
                        {
                                part.VertexBuffer,
                                new VertexBufferBinding(instanceVertexBuffer, 0, 1 )
                            });

                        // set index buffer and draw
                        vxEngine.GraphicsDevice.Indices = part.IndexBuffer;
                        vxEngine.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount, numInstances);

                    }
                }
            }
#endif
        }


        /// <summary>
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(InstanceSet instanceSet, vxCamera3D Camera, string technique)
        {
            Model model = instanceSet.InstancedModel;
            Matrix[] modelBones = instanceSet.instancedModelBones;
            Matrix[] instances = instanceSet.instanceTransforms;

            if (instances == null || instances.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceSet.instanceVertexBuffer == null) ||
                (instances.Length > instanceSet.instanceVertexBuffer.VertexCount))
            {
                if (instanceSet.instanceVertexBuffer != null)
                    instanceSet.instanceVertexBuffer.Dispose();

                instanceSet.instanceVertexBuffer = new DynamicVertexBuffer(vxEngine.GraphicsDevice, InstanceSet.instanceVertexDeclaration,
                                                               instances.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceSet.instanceVertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    vxEngine.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceSet.instanceVertexBuffer, 0, 1)
                    );

                    vxEngine.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    // Specify which effect technique to use.
                    effect.CurrentTechnique = effect.Techniques["Technique_PrepPass"];

                    effect.Parameters["World"].SetValue(Matrix.Identity);
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);

                    //if (effect.Parameters["LightDirection"] != null)
                    effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(vxEngine.Renderer.lightPosition));

                    if (effect.Parameters["ShadowMap"] != null)
                        effect.Parameters["ShadowMap"].SetValue(vxEngine.Renderer.RT_ShadowMap);
                    if (effect.Parameters["ShadowTransform"] != null)
                        effect.Parameters["ShadowTransform"].SetValue(vxEngine.Renderer.ShadowSplitProjectionsWithTiling);
                    if (effect.Parameters["TileBounds"] != null)
                        effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
                    if (effect.Parameters["SplitColors"] != null)
                        effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());
						

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        vxEngine.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instances.Length);
                    }
                }
            }
        }
    }
}
#endif