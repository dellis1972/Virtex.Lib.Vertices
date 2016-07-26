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
using Virtex.Lib.Vertices.Scenes.Sandbox;
using Virtex.Lib.Vertices.Physics.BEPU.Entities.Prefabs;
using Virtex.Lib.Vertices.Core.Input;
using Virtex.Lib.Vertices.Core.Cameras;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Entities.Sandbox3D;
using Virtex.Lib.Vertices.Scenes.Sandbox3D;

namespace Virtex.Lib.Vertices.Entities.Sandbox3D.Util
{
	/// <summary>
	/// 3D Cursor for use in Sandbox Entity Position Change.
	/// </summary>
	public class vxCursor3D : vxSandboxEntity
    {
        public Box HitBox;

        float CubeSize = 2.5f;

        public float ZoomFactor = 25;

		public List<vxAxis> List_Items = new List<vxAxis>();

        public List<vxSandboxEntity> SelectedItems = new List<vxSandboxEntity>();

        //Reset
        public int scale = 40;

        /// <summary>
        /// The Sandbox that owns this Cursor
        /// </summary>
        public vxSandboxGamePlay CurrentSandbox { get { return (vxSandboxGamePlay)vxEngine.Current3DSceneBase; } }

        Vector3 PreviousPosition = Vector3.Zero;

        bool firstClick = true;
        int cnt = 0;
        Vector3 clickPos = Vector3.Zero;

        /// <summary>
        /// Get's whether the Mouse is currently Hovering under ANY Axis in the Cursor.
        /// </summary>
        public bool IsMouseHovering
        {
            get { return GetIsMouseHovering(); }
        }

        private bool GetIsMouseHovering()
        {
            //Search through each axis too see if either one is Hovered or Selected (i.e. Not Unselected)
            foreach(vxAxis axis in List_Items)
            {
                if (axis.SelectionState != vxEnumSelectionState.Unseleced)
                    return true;
            }

            //If they're all Unselected, then return false
            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vertices.Core.Cursor3D"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public vxCursor3D(vxEngine vxEngine)
            : base(vxEngine, vxEngine.Assets.Models.UnitBox, Vector3.Zero)
        {            
            
            //Always Start The Cursor at the Origin
            HitBox = new Box(Vector3.Zero, CubeSize, CubeSize, CubeSize);
            World = Matrix.CreateScale(CubeSize);

            //Build the Axises
            List_Items.Add(new vxAxis(vxEngine, this, AxisDirections.X));
			List_Items.Add(new vxAxis(vxEngine, this, AxisDirections.Y));
			List_Items.Add(new vxAxis(vxEngine, this, AxisDirections.Z));

            Current3DScene.BEPUPhyicsSpace.Add(HitBox);
            //Current3DScene.Physics3DDebugDrawer.Add(HitBox);

            //Remove from the main list so that it can be drawn over the entire scene
            Current3DScene.Entities.Remove(this);
            Current3DScene.List_OverlayItems.Add(this);
            HitBox.CollisionInformation.CollisionRules.Personal = Virtex.Lib.Vertices.Physics.BEPU.CollisionRuleManagement.CollisionRule.NoSolver;
        }

        public override void SetIndex(int NewIndex)
        {
            HitBox.CollisionInformation.Tag = "CURSOR3D";
            base.SetIndex(NewIndex);
        }


        public void Update(GameTime gameTime, Ray MouseRay)
        {
            //vxConsole.WriteToInGameDebug(CurrentSandbox.List_Bases_Selected.Count);
            PreviousPosition = this.Position;

            //First Search to see if the Cursor has been intersected since it maybe behind other items
            foreach (vxAxis entity in List_Items)
            {
                float? t = MouseRay.Intersects(entity.HitBox.CollisionInformation.BoundingBox);
                if (t != null && entity.SelectionState != vxEnumSelectionState.Selected)
                {
                    entity.SelectionState = vxEnumSelectionState.Hover;
                }
                else if (entity.SelectionState == vxEnumSelectionState.Hover)
                    entity.SelectionState = vxEnumSelectionState.Unseleced;
            }

            //Now Base Update to set Highlighting Colours
            base.Update(gameTime);

            //Set the Zoom Factor based off of distance from camera
            ZoomFactor = Math.Abs(Vector3.Subtract(Position, Current3DScene.Camera.Position).Length());

            //Always re-set the Selection State as it dependas on the child elements
            SelectionState = vxEnumSelectionState.Unseleced;
            foreach (vxSandboxEntity entity in List_Items)
            {
                if (entity.SelectionState == vxEnumSelectionState.Hover || 
                    entity.SelectionState == vxEnumSelectionState.Selected)
                    SelectionState = vxEnumSelectionState.Hover;
                entity.Update(gameTime);
            }

            if (vxEngine.InputManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (firstClick == true)
                {
                    firstClick = false;
                }
                if(cnt == 4)
                {
                    clickPos = Position;
                    //vxConsole.WriteLine("Chosen Pt: " + clickPos);
                    foreach (vxSandboxEntity selectedEntity in CurrentSandbox.SelectedItems)
                    {
                        selectedEntity.PreSelectionWorld = selectedEntity.World;
                    }
                }
                //Set Geometry Changes based off of Child Elements
                Vector3 DeltaPosition = Position - clickPos;

                //Now Reset the Delta Position to the Snap Positions
                DeltaPosition = new Vector3((int)DeltaPosition.X, (int)DeltaPosition.Y, (int)DeltaPosition.Z);

                if (cnt > 4)
                {
                    foreach (vxSandboxEntity selectedEntity in CurrentSandbox.SelectedItems)
                    {
                        selectedEntity.World = selectedEntity.PreSelectionWorld * Matrix.CreateTranslation(DeltaPosition);
                        selectedEntity.SetMesh(selectedEntity.World, true, true);
                    }
                }
                cnt++;
            }
            else if(firstClick == false)
            {
                cnt = 0;
                firstClick = true;
                //vxConsole.WriteLine("firstClick");

            }

            if (this.SelectionState == vxEnumSelectionState.Hover &&
                vxEngine.InputManager.IsNewMouseButtonRelease(MouseButtons.LeftButton))
            {
                //firstClick = false;
            }
        }

        public override void RenderMeshPrepPass()
        {
            //if (Current3DScene.camera.CameraType == CameraType.Freeroam)
            //    base.RenderMeshPrepPass();
        }

        public override void RenderMeshPlain()
        {
            foreach (vxAxis entity in List_Items)
            {
                entity.RenderMeshPlain();
            }
        }

        public override void RenderMesh(string RenderTechnique)
        {
            if (Current3DScene.Camera.CameraType == CameraType.Freeroam)
                base.RenderMesh(RenderTechnique);
        }

        public override void SetOffetOrientation()
        {
            base.SetOffetOrientation();
            HitBox.WorldTransform = World;
        }
    }
}
#endif