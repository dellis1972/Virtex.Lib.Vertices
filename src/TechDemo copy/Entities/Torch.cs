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
using vxVertices.Scenes.Sandbox.Entities;
using vxVertices.Physics.BEPU.Entities.Prefabs;
using vxVertices.Physics.BEPU.Paths.PathFollowing;
using vxVertices.Physics.BEPU.Entities;
using vxVertices.Core.Debug;
using vxVertices.Mathematics;
using vxVertices.Scenes.Sandbox;
using vxVertices.Physics.BEPU;
using vxVertices.Entities.Sandbox3D;
using vxVertices.Scenes.Sandbox3D;

namespace VerticeEnginePort.Base
{
    public class Torch : vxSandboxEntity
    {
        /// <summary>
        /// The Pillar Collider Skin
        /// </summary>
        public Entity entity { get; set; }

        /// <summary>
        /// Entity Mover which controls the position of the Sandbox Entity.
        /// </summary>
        public EntityMover entityMover { get; set; }

        public bool UseEntityMover = true;

        /// <summary>
        /// The position that it should be set at
        /// </summary>
        public Vector3 RequestedPosition { get; set; }

        /// <summary>
        /// This provides the offset needed between the Phsyics skin and the mesh.
        /// </summary>
        public Vector3 Vector_ModelOffSet = Vector3.Zero;

        public static vxSandboxEntityDescription EntityDescription
        {
            get
            {
                return new vxSandboxEntityDescription(
                "VerticeEnginePort.Base.Torch",
                "Teapot",
                "Models/Torch/Torch");
            }
        }

		Vector3[] staticTriangleVertices;
		int[] staticTriangleIndices;

        public Torch(GameEngine GameEngine, Vector3 StartPosition) :
            base(GameEngine, GameEngine.Model_Items_Teapot, StartPosition)
        {
            NormalMap = vxEngine.Game.Content.Load<Texture2D>("Models/teapot/teapot_nm");
            SpecularMap = vxEngine.Game.Content.Load<Texture2D>("Models/teapot/teapot_sm");
            XRotation_ModelOffset = -MathHelper.PiOver2;
            //Vector_ModelOffSet = new Vector3(-0.5f, 1 / 2, 0.5f);
            //entity = new Box(StartPosition - Vector_ModelOffSet, 1, 1, 1, 1000);

            entity = new Box(StartPosition, 1, 1, 1); //Set up Entity Mover
            entityMover = new EntityMover(entity);

            //Add Entities too Physics Sim
            vxEngine.Current3DSceneBase.BEPUPhyicsSpace.Add(entity);
            vxEngine.Current3DSceneBase.BEPUDebugDrawer.Add(entity);
            vxEngine.Current3DSceneBase.BEPUPhyicsSpace.Add(entityMover);


			ModelDataExtractor.GetVerticesAndIndicesFromModel(model, out staticTriangleVertices, out staticTriangleIndices);

        }
        Matrix preMat = Matrix.Identity;
        public override void ToggleSimulation(bool IsRunning)
        {
            base.ToggleSimulation(IsRunning);

            //if (SandboxState == vxEnumSandboxGameState.Running)
            //{
            //    UseEntityMover = false;
            //    vxEngine.Current3DSceneBase.BEPUPhyicsSpace.Remove(entityMover);
            //    preMat = entity.WorldTransform;
            //    entity.Mass = 1000;
            //}
            //else
            //{
            //    UseEntityMover = true;
            //    vxEngine.Current3DSceneBase.BEPUPhyicsSpace.Add(entityMover);
            //    entityMover.TargetPosition = RequestedPosition;
            //    entity.WorldTransform = preMat;
            //    entity.AngularVelocity = Vector3.Zero;
            //}
        }

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Set Entity Orientation
            if (UseEntityMover)
            {
                entityMover.TargetPosition = vxSmooth.SmoothVector(entity.WorldTransform.Translation,
                    RequestedPosition, 8);
                
                World = Matrix.Identity * Matrix.CreateScale(1);
                World *= Matrix.CreateRotationX(XRotation_ModelOffset);
                World *= Matrix.CreateRotationY(YRotation_ModelOffset);
                World *= Matrix.CreateRotationZ(ZRotation_ModelOffset);
                World *= Matrix.CreateTranslation(entity.WorldTransform.Translation
                    + Vector_ModelOffSet);
                entity.WorldTransform = World;
            }

            else
            {
                World = Matrix.Identity * Matrix.CreateScale(1);
                World *= Matrix.CreateRotationX(XRotation_ModelOffset);
                World *= Matrix.CreateRotationY(YRotation_ModelOffset);
                World *= Matrix.CreateRotationZ(ZRotation_ModelOffset);
                World *= Matrix.CreateTranslation(Vector_ModelOffSet) * entity.WorldTransform;
            }
        }

        public override void SetIndex(int NewIndex)
        {
            entity.CollisionInformation.Tag = NewIndex;
            base.SetIndex(NewIndex);
        }

        public override void SetMesh(Matrix NewWorld, bool AddToPhysics, bool ResetWholeMesh)
        {
            AddToPhysicsLibrary = AddToPhysics;

            World = NewWorld;
            RequestedPosition = World.Translation - Vector_ModelOffSet;

            entity.WorldTransform = Matrix.CreateWorld(World.Translation - Vector_ModelOffSet,
                entity.WorldTransform.Forward, entity.WorldTransform.Up);
            entityMover.TargetPosition = RequestedPosition;
        }

        /// <summary>
        /// Renders to the Normal Depth Map for Edge Detection
        /// </summary>
        /// <param name="model"></param>
        /// <param name="World"></param>
        /// <param name="Camera"></param>
        public override void RenderMesh(string RenderTechnique)
        {
            base.RenderMesh(RenderTechnique);

            if (SandboxState == vxEnumSandboxGameState.EditMode)
            {
                if (SelectionState == vxEnumSelectionState.Hover)
                    vxDebugShapeRenderer.AddBoundingBox(entity.CollisionInformation.BoundingBox, Color.SkyBlue);

                if (SelectionState == vxEnumSelectionState.Selected)
                    vxDebugShapeRenderer.AddBoundingBox(entity.CollisionInformation.BoundingBox, Color.DarkOrange);

                if (SelectionState == vxEnumSelectionState.Hover)
                    SelectionState = vxEnumSelectionState.Unseleced;
            }
        }
    }
}
