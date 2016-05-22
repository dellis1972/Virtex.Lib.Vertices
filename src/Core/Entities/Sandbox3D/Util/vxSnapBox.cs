﻿#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Virtex vxEngine Declaration
using vxVertices.Core;
using vxVertices.Scenes.Sandbox;

using vxVertices.Core.Cameras;
using vxVertices.Physics.BEPU.Entities.Prefabs;
using vxVertices.Entities.Sandbox3D;
using vxVertices.Scenes.Sandbox3D;
using vxVertices.Graphics;

namespace vxVertices.Entities.Sandbox3D.Util
{

    public class vxSnapBox : vxSandboxEntity
    {
        Box HitBox;
        Vector3 EndLocalRotation;
        /// <summary>
        /// SnapBox for allowing tracks to snap together
        /// </summary>
        /// <param name="vxEngine"></param>
        public vxSnapBox(vxEngine vxEngine, vxModel SnapBoxModel, int Width, int Height, int Length)
            : base(vxEngine, SnapBoxModel, Vector3.Zero)
        {
            EndLocalRotation = new Vector3(MathHelper.PiOver2, -MathHelper.PiOver4, MathHelper.PiOver4);
            DoShadowMap = false;
            HitBox = new Box(Vector3.Zero, Width, Height, Length);

            Current3DScene.BEPUPhyicsSpace.Add(HitBox);
            //PhysicsSkin_Main.CollisionRules.Personal = Physics.CollisionRuleManagement.CollisionRule.NoSolver;
			HitBox.CollisionInformation.CollisionRules.Personal = vxVertices.Physics.BEPU.CollisionRuleManagement.CollisionRule.NoSolver;
            ((vxSandboxGamePlay)Current3DScene).Items.Add(this);            
        }

        public override void SetIndex(int NewIndex)
        {
            HitBox.CollisionInformation.Tag = NewIndex;
            base.SetIndex(NewIndex);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Parent == null)
                this.DisposeEntity();

            if (PhysicsSkin_Main != null)
				PhysicsSkin_Main.CollisionRules.Personal = vxVertices.Physics.BEPU.CollisionRuleManagement.CollisionRule.NoSolver;
                        
            base.Update(gameTime);
        }
        

        public override void RenderMeshPrepPass()
        {
            if (Current3DScene.Camera.CameraType == CameraType.Freeroam)
                base.RenderMeshPrepPass();
        }

        public override void RenderMesh(string RenderTechnique)
        {
            if(Current3DScene.Camera.CameraType == CameraType.Freeroam)
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