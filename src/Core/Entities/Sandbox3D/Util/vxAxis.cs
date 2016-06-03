#if VIRTICES_3D
using System;
using Microsoft.Xna.Framework;
using vxVertices.Core;
using vxVertices.Utilities;
using vxVertices.Physics.BEPU.Entities.Prefabs;
using vxVertices.Core.Debug;
using vxVertices.Core.Input;
using vxVertices.Core.Cameras;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Scenes.Sandbox3D;

namespace vxVertices.Entities.Sandbox3D.Util
{
    public enum AxisDirections
    {
        X,
        Y,
        Z,
    }

	/// <summary>
	/// Axis Object for editing Sandbox Entity Position in the Sandbox Enviroment.
	/// </summary>
    public class vxAxis : vxSandboxEntity
    {
        public Box HitBox;
        public AxisDirections AxisDirections;

		vxCursor3D ParentCursor;

        Vector3 MainAxis = Vector3.Zero;
        Vector3 PerpendicularAxis = Vector3.Zero;

        Vector3 StartPosition = Vector3.Zero;
        private object vxcConsole;

        Model model;

        /// <summary>
        /// Initializes a new instance of the <see cref="vxVertices.Core.Axis"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="ParentCursor">Parent cursor.</param>
        /// <param name="AxisDirections">Axis directions.</param>
        public vxAxis(vxEngine vxEngine, vxCursor3D ParentCursor, AxisDirections AxisDirections)
            : base(vxEngine, null, Vector3.Zero)
        {
            model = vxEngine.Assets.Models.UnitArrow;
            this.ParentCursor = ParentCursor;
            DoShadowMap = false;
            HitBox = new Box(Vector3.Zero, 2, 2, 25);
            World = Matrix.CreateScale(2, 2, 25);

            this.AxisDirections = AxisDirections;
            switch (AxisDirections)
            {
                case AxisDirections.X:
                    PlainColor = new Color(128,0,0);
                    World = Matrix.CreateScale(25, 2, 2);
                    break;
                case AxisDirections.Y:
                    PlainColor = Color.Green;
                    World = Matrix.CreateScale(2, 25, 2);
                    break;
                case AxisDirections.Z:
                    PlainColor = new Color(0, 0, 128);
                    World = Matrix.CreateScale(2, 2, 25);
                    break;
            }

            Current3DScene.BEPUPhyicsSpace.Add(HitBox);
            //Current3DScene.Physics3DDebugDrawer.Add(HitBox);
            HitBox.CollisionInformation.Tag = AxisDirections;

			HitBox.CollisionInformation.CollisionRules.Personal = vxVertices.Physics.BEPU.CollisionRuleManagement.CollisionRule.NoSolver;

            //Remove from the main list so that it can be drawn over the entire scene
            //Current3DScene.List_Entities.Remove(this);
            //Current3DScene.List_OverlayItems.Add(this);
        }

        public override void SetIndex(int NewIndex)
        {
            HitBox.CollisionInformation.Tag = AxisDirections;
            base.SetIndex(NewIndex);
        }

        float hightLiteFactor = 1;
        public override void RenderMeshPlain()
        {
            if (Current3DScene.Camera.CameraType == CameraType.Freeroam && model != null)
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.DiffuseColor = this.PlainColor.ToVector3() * hightLiteFactor * hightLiteFactor;
                        effect.World = Matrix.CreateScale(10) * World;
                        effect.View = vxEngine.Current3DSceneBase.Camera.View;
                        effect.Projection = vxEngine.Current3DSceneBase.Camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        public override void RenderMesh(string RenderTechnique) { }
        public override void RenderMeshPrepPass() { }

        public override void Update(GameTime gameTime)
        {
            switch (AxisDirections)
            {
                case AxisDirections.X:
                    MainAxis = ParentCursor.World.Forward;
                    PerpendicularAxis = ParentCursor.World.Up;
                    break;
                case AxisDirections.Y:
                    MainAxis = ParentCursor.World.Up;
                    PerpendicularAxis = ParentCursor.World.Forward;
                    break;
                case AxisDirections.Z:
                    MainAxis = ParentCursor.World.Right;
                    PerpendicularAxis = ParentCursor.World.Forward;
                    break;
            }

            //Set the World of the Arrows
            World = Matrix.CreateScale(ParentCursor.ZoomFactor / (ParentCursor.scale * 2)) *
                Matrix.CreateWorld(ParentCursor.Position, MainAxis, PerpendicularAxis);

            //Set the World of the Bit Box
            HitBox.WorldTransform = Matrix.CreateScale(1) * Matrix.CreateWorld(
                ParentCursor.Position + MainAxis * ParentCursor.ZoomFactor / ((ParentCursor.scale)),
                MainAxis,
                PerpendicularAxis);

            HitBox.HalfLength = 2 * ParentCursor.ZoomFactor / ParentCursor.scale;
            HitBox.HalfWidth = 2 * ParentCursor.ZoomFactor / (ParentCursor.scale * 7.5f);
            HitBox.HalfHeight =2 * ParentCursor.ZoomFactor / (ParentCursor.scale * 7.5f);

            base.Update(gameTime);

            hightLiteFactor = 1;
            if (SelectionState == vxEnumSelectionState.Hover)
            {
                hightLiteFactor = 1.25f;
                //DebugShapeRenderer.AddBoundingBox(HitBox.CollisionInformation.BoundingBox, Color.HotPink);
            }              


            //Handle if Selected
            else if (SelectionState == vxEnumSelectionState.Selected)
            {
                hightLiteFactor = 2.0f;
                Vector3 MovementAxis = MainAxis;
                MovementAxis.Normalize();
                if(vxEngine.InputManager.IsNewMouseButtonPress(MouseButtons.LeftButton))
                    StartPosition = ParentCursor.Position;
                
                ParentCursor.Position += MovementAxis * (
                    vxEngine.InputManager.MouseState.X - vxEngine.InputManager.PreviousMouseState.X
                    - vxEngine.InputManager.MouseState.Y + vxEngine.InputManager.PreviousMouseState.Y) * ParentCursor.ZoomFactor / (ParentCursor.scale*40);
                //DebugShapeRenderer.AddBoundingBox(HitBox.CollisionInformation.BoundingBox, Color.LimeGreen);
            }
            
            if (SelectionState == vxEnumSelectionState.Selected && vxEngine.InputManager.IsNewMouseButtonRelease(MouseButtons.LeftButton))
                SelectionState = vxEnumSelectionState.Unseleced;

        }
    }
}
#endif