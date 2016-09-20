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
using Virtex.Lib.Vrtc.Scenes.Sandbox.Entities;
using Virtex.Lib.Vrtc.Physics.BEPU.Entities;
using Virtex.Lib.Vrtc.Physics.BEPU.Paths.PathFollowing;
using Virtex.Lib.Vrtc.Physics.BEPU.Entities.Prefabs;
using Virtex.Lib.Vrtc.Scenes.Sandbox;
using Virtex.Lib.Vrtc.Core.Debug;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Scenes.Sandbox3D;

namespace Virtex.Lib.Vrtc.Entities.Sandbox3D.Util
{
    public class vxScaleCube : vxSandboxEntity
    {

        /// <summary>
        /// The Pillar Collider Skin
        /// </summary>
        public Box entity { get; set; }

        /// <summary>
        /// Entity Mover which controls the position of the Sandbox Entity.
        /// </summary>
        public EntityMover entityMover { get; set; }

        /// <summary>
        /// The Cube is moved
        /// </summary>
        public event EventHandler<EventArgs> Moved;

        public vxScaleCube(vxEngine vxEngine, Vector3 StartPosition) :
            base(vxEngine, vxEngine.Assets.Models.UnitBox, StartPosition)
        {
            entity = new Box(StartPosition, 1, 1, 1);

            //Set up Entity Mover
            entityMover = new EntityMover(entity);

            //Add Entities too Physics Sim
            vxEngine.Current3DSceneBase.BEPUPhyicsSpace.Add(entity);
            vxEngine.Current3DSceneBase.BEPUDebugDrawer.Add(entity);
            vxEngine.Current3DSceneBase.BEPUPhyicsSpace.Add(entityMover);
            //entity.CollisionInformation.CollisionRules.Personal = Physics.BEPU.CollisionRuleManagement.CollisionRule.NoSolver;

            CurrentSandboxLevel.Items.Add(this);

            //Reset the element indicies.
            CurrentSandboxLevel.SetElementIndicies();

            prePos = Position;

            Saveable = false;
        }

        public override void DisposeEntity()
        {
            base.DisposeEntity();
        }

        /// <summary>
        /// Renders to the Normal Depth Map for Edge Detection
        /// </summary>
        /// <param name="model"></param>
        /// <param name="World"></param>
        /// <param name="camera"></param>
        public override void RenderMeshPrepPass()
        {
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


        float ZoomFactor = 1;
        public override void SetMesh(Matrix NewWorld, bool AddToPhysics, bool ResetWholeMesh)
        {
            AddToPhysicsLibrary = AddToPhysics;

            World = Matrix.CreateScale(ZoomFactor) * NewWorld;
            //entity.WorldTransform = World;

            Position = World.Translation;
        }
        Vector3 prePos;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ZoomFactor = Math.Abs(Vector3.Subtract(Position, Current3DScene.Camera.Position).Length()) / 100;

            World = Matrix.CreateScale(ZoomFactor);
            World *= Matrix.CreateTranslation(Position);

            entity.WorldTransform = World;
            
            entity.HalfLength = ZoomFactor;
            entity.HalfWidth =  ZoomFactor;
            entity.HalfHeight =  ZoomFactor;

            entityMover.TargetPosition = World.Translation;

            if (Math.Abs(Vector3.Subtract(World.Translation, prePos).Length()) > 0.5)
            {
                if (SelectionState == vxEnumSelectionState.Selected)
                {
                    // Raise the 'Moved' event.
                    if (Moved != null)
                        Moved(this, new EventArgs());
                }
            }

            if(SandboxState == vxEnumSandboxGameState.Running)
            {
                entity.CollisionInformation.CollisionRules.Personal = Physics.BEPU.CollisionRuleManagement.CollisionRule.NoSolver;
            }
            else
            {
                entity.CollisionInformation.CollisionRules.Personal = Physics.BEPU.CollisionRuleManagement.CollisionRule.Normal;
            }

            prePos = World.Translation;
        }

		public override void RenderMeshForWaterReflectionPass(Plane surfacePlane)
		{
			if (SandboxState == vxEnumSandboxGameState.EditMode)
			{
				base.RenderMeshForWaterReflectionPass(surfacePlane);
			}
		}
        public override void RenderMeshShadow() { }
        public override void RenderMesh(string RenderTechnique)
        {
            if (SandboxState == vxEnumSandboxGameState.EditMode)
            {
                base.RenderMesh(RenderTechnique);
            }
        }

        public override void SetIndex(int NewIndex)
        {
            entity.CollisionInformation.Tag = NewIndex;
            base.SetIndex(NewIndex);
        }
    }
}

#endif