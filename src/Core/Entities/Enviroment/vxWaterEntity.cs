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
using vxVertices.Core;
using vxVertices.Physics.BEPU.UpdateableSystems;
using vxVertices.Scenes.Sandbox.Entities;
using vxVertices.Scenes.Sandbox;
using vxVertices.Utilities;
using vxVertices.Core.Debug;
using vxVertices.Entities.Sandbox3D;
using vxVertices.Entities.Sandbox3D.Util;
using vxVertices.Scenes.Sandbox3D;

namespace vxVertices.Core.Entities
{
    public class vxWaterEntity : vxSandboxEntity
    {
        public Plane WrknPlane;

        public Texture2D waterBumpMap;

        public Texture2D waterDistortionMap;

        public Vector2 WaterFlowOffset = new Vector2(0, 0);

        public float xWaveLength = 0.25f;

        public float xWaveHeight = 0.15f;

        public FluidVolume fluidVolume;

        public Vector3 WaterScale = Vector3.One;

        vxScaleCube scPosition;
        vxScaleCube scLeft;
        vxScaleCube scRight;
        vxScaleCube scForward;
        vxScaleCube scBack;

        public static vxSandboxEntityDescription EntityDescription
        {
            get
            {
                return new vxSandboxEntityDescription(
                "vxVertices.Core.Entities.vxWaterEntity",
                "Water",
                "Textures/engine/water/water");
            }
        }

        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public vxWaterEntity(vxEngine vxEngine, Vector3 StartPosition, Vector3 WaterScale)
			: base(vxEngine, vxEngine.LoadModelAsWaterObject("Models/water_plane/water_plane_mg", vxEngine.EngineContentManager), StartPosition)
        {
            waterBumpMap = vxEngine.Assets.Textures.Texture_WaterWaves;
            waterDistortionMap = vxEngine.Assets.Textures.Texture_WaterDistort;
            
            //Render even in debug mode
            RenderEvenInDebug = true;

            this.WaterScale = WaterScale;

            WrknPlane = new Plane(0, 1, 0, StartPosition.Y);
            Position = StartPosition;

            World = Matrix.CreateScale(WaterScale);
            World *= Matrix.CreateTranslation(Position);



            //Set up Scale Cubes
            scPosition = new vxScaleCube(vxEngine, StartPosition);
            scPosition.Moved += ScPosition_Moved;

            scLeft = new vxScaleCube(vxEngine, StartPosition + Vector3.Backward * WaterScale.Z);
            scLeft.Moved += Sc_Moved;
            scRight = new vxScaleCube(vxEngine, StartPosition - Vector3.Backward * WaterScale.Z);
            scRight.Moved += Sc_Moved;

            scForward = new vxScaleCube(vxEngine, StartPosition + Vector3.Right * WaterScale.X);
            scForward.Moved += Sc_Moved;
            scBack = new vxScaleCube(vxEngine, StartPosition - Vector3.Right * WaterScale.X);
            scBack.Moved += Sc_Moved;


            //
            //Physics Body
            //
            var tris = new List<Vector3[]>();
            //Remember, the triangles composing the surface need to be coplanar with the surface.  In this case, this means they have the same height.
            tris.Add(new[]
                         {
                            new Vector3(Position.X - WaterScale.X, Position.Y, Position.Z- WaterScale.Z),
                            new Vector3(Position.X + WaterScale.X, Position.Y, Position.Z- WaterScale.Z),
                            new Vector3(Position.X - WaterScale.X, Position.Y, Position.Z + WaterScale.Z),
                         });
            tris.Add(new[]
                         {
                            new Vector3(Position.X - WaterScale.X, Position.Y, Position.Z+ WaterScale.Z),
                            new Vector3(Position.X + WaterScale.X, Position.Y, Position.Z- WaterScale.Z),
                            new Vector3(Position.X + WaterScale.X, Position.Y, Position.Z + WaterScale.Z),
                         });
            fluidVolume = new FluidVolume(Vector3.Up, -9.81f, tris, WaterScale.Y, density, 0.9f, .4f);

            Current3DScene.BEPUPhyicsSpace.Add(fluidVolume);
        }

        float density = 1.05f;

        public override void SetMesh(Matrix NewWorld, bool AddToPhysics, bool ResetWholeMesh)
        {
            base.SetMesh(NewWorld, false, false);

            //Set the position
            scPosition.Position = this.Position + new Vector3(0, 0, 0);
            ResetScaleCubes();
        }

        private void ScPosition_Moved(object sender, EventArgs e)
        {
            this.Position = scPosition.World.Translation;
            ResetScaleCubes();
        }

        private void Sc_Moved(object sender, EventArgs e)
        {
            //Set the Main Position as the average of the top scale cubes
            this.Position = new Vector3((scForward.Position.X + scBack.Position.X) / 2,
                scPosition.Position.Y,
                (scLeft.Position.Z + scRight.Position.Z) / 2);

            //Set the position
            scPosition.Position = this.Position + new Vector3(0, 0, 0);

            //Set the water scale
            WaterScale = new Vector3(
                (scForward.Position.X - scBack.Position.X) / 2,
                WaterScale.Y,
                (scLeft.Position.Z - scRight.Position.Z) / 2);

            //Now reset all items
            ResetScaleCubes();
        }

        void ResetScaleCubes()
        {
            if (scLeft.SelectionState != vxEnumSelectionState.Selected)
                scLeft.Position = this.Position + Vector3.Backward * WaterScale.Z;
            if (scRight.SelectionState != vxEnumSelectionState.Selected)
                scRight.Position = this.Position - Vector3.Backward * WaterScale.Z;

            if (scForward.SelectionState != vxEnumSelectionState.Selected)
                scForward.Position = this.Position + Vector3.Right * WaterScale.X;
            if (scBack.SelectionState != vxEnumSelectionState.Selected)
                scBack.Position = this.Position - Vector3.Right * WaterScale.X;



            //
            //Reset the Physics Body
            //
            Current3DScene.BEPUPhyicsSpace.Remove(fluidVolume);
            var tris = new List<Vector3[]>();
            //Remember, the triangles composing the surface need to be coplanar with the surface.  In this case, this means they have the same height.
            tris.Add(new[]
                         {
                            new Vector3(Position.X - WaterScale.X, Position.Y, Position.Z- WaterScale.Z),
                            new Vector3(Position.X + WaterScale.X, Position.Y, Position.Z- WaterScale.Z),
                            new Vector3(Position.X - WaterScale.X, Position.Y, Position.Z + WaterScale.Z),
                         });
            tris.Add(new[]
                         {
                            new Vector3(Position.X - WaterScale.X, Position.Y, Position.Z+ WaterScale.Z),
                            new Vector3(Position.X + WaterScale.X, Position.Y, Position.Z- WaterScale.Z),
                            new Vector3(Position.X + WaterScale.X, Position.Y, Position.Z + WaterScale.Z),
                         });
            fluidVolume = new FluidVolume(Vector3.Up, -9.81f, tris, WaterScale.Y, density, 0.9f, .4f);

            Current3DScene.BEPUPhyicsSpace.Add(fluidVolume);
        }

        public override void InitShaders() { }
        public override void UpdateRenderTechnique() { }
        public override void RenderMeshForWaterReflectionPass(Plane ReflectedView) { }
        public override void RenderMeshPrepPass() { }
        public override void RenderMesh(string RenderTechnique) { }
        public override void RenderMeshShadow() { }


        public override void PreSave()
        {
            base.PreSave();

            UserDefinedData02 = string.Format("{0};{1};{2}", WaterScale.X, WaterScale.Y, WaterScale.Z);
        }

        public override void PostLoad()
        {            
            string[] vars = UserDefinedData02.Split(';');

            WaterScale = new Vector3(float.Parse(vars[0]), float.Parse(vars[1]), float.Parse(vars[2]));
            ResetScaleCubes();

            base.PostLoad();
        }

        public override void DisposeEntity()
        {
            if(scPosition != null)
                scPosition.DisposeEntity();

            scLeft.DisposeEntity();
            scRight.DisposeEntity();
            scForward.DisposeEntity();
            scBack.DisposeEntity();
            
            //CurrentSandboxLevel.Items.Remove(this);
#if VRTC_PLTFRM_XNA
			CurrentSandboxLevel.waterItems.Remove (this);
#endif
            Current3DScene.BEPUPhyicsSpace.Remove(fluidVolume);
            base.DisposeEntity();
        }

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(WaterScale);
            World *= Matrix.CreateTranslation(Position);

            base.Update(gameTime);
        }

        public void DrawWater(RenderTarget2D reflectionMap, Matrix reflectionViewMatrix)
        {
            //vxDebugShapeRenderer.AddBoundingBox(fluidVolume.BoundingBox, Color.Blue);
            WaterFlowOffset += Vector2.UnitX/3000;
            if (model != null)
            {
                // Look up the bone transform matrices.
                Matrix[] transforms = new Matrix[model.Bones.Count];

                model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        // Specify which effect technique to use.
                        effect.CurrentTechnique = effect.Techniques["Water"];

                        effect.Parameters["World"].SetValue(World);
                        effect.Parameters["View"].SetValue(Camera.View);
                        effect.Parameters["Projection"].SetValue(Camera.Projection);
                        effect.Parameters["ReflectionView"].SetValue(reflectionViewMatrix);
                        effect.Parameters["ReflectionMap"].SetValue(reflectionMap);
                        effect.Parameters["xWaterBumpMap"].SetValue(waterBumpMap);
                        effect.Parameters["xWaveLength"].SetValue(xWaveLength);
                        effect.Parameters["xWaveHeight"].SetValue(xWaveHeight);
                        effect.Parameters["WaterFlowOffset"].SetValue(WaterFlowOffset);

                        effect.Parameters["xCamPos"].SetValue(Camera.Position);
                        effect.Parameters["xLightDirection"].SetValue(vxEngine.Renderer.lightPosition);
                    }
                    mesh.Draw();
                }
            }
        }
            /// <summary>
        /// Draws the Models to the Distortion Target
        /// </summary>
        public override void DrawModelDistortion(vxEngine vxEngine, GameTime gameTime)
        {
            /*
                // draw the distorter
                Matrix worldView = World * Camera.View;

                // make sure the depth buffering is on, so only parts of the scene
                // behind the distortion effect are affected
                vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique =
                            effect.Techniques["Distortion"];
                        effect.Parameters["WorldView"].SetValue(worldView);
                        effect.Parameters["WorldViewProjection"].SetValue(worldView * Camera.Projection);
                        effect.Parameters["DisplacementMap"].SetValue(waterDistortionMap);
                        effect.Parameters["offset"].SetValue(0);
                        
                        effect.Parameters["DistortionScale"].SetValue(DistortionScale);
                        effect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
                    }
                    mesh.Draw();
                }
            */
        }        
    }
}
#endif