﻿#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Microsoft.Xna.Framework.Input;

namespace Virtex.Lib.Vrtc.Core.Entities
{

    public class vxSunEntity : vxEntity3D
    {
        public Vector3 screenPos;
        public float RotationX = 0.9f;
        public float RotationZ = 0.6f;
        public bool IsOnScreen = false;
        public bool IsSunOccluded = false;
        public Vector3 SunPosition;
        public Vector3 LightDirection;

        /// <summary>
        /// SnapBox for allowing tracks to snap together
        /// </summary>
        /// <param name="vxEngine"></param>
        public vxSunEntity(vxEngine vxEngine)
            : base(vxEngine, vxEngine.Assets.Models.Sun_Mask, Vector3.Zero)
        {
            vxEngine.CurrentGameplayScreen.Entities.Remove(this);

			RotationX = 0.75f;
			RotationZ = 0.6f;
            //DoEdgeDetect = false;
			//IsSkyBox = true;
        }
        float inc = 0;
        public int TextureSize = 3;
		public void DrawGlow()
        {
            // Debug
            //vxConsole.WriteToInGameDebug(this.RotationX);
            //vxConsole.WriteToInGameDebug(this.RotationZ);


            if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.Up))
                RotationX += 0.005f;
            if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.Down))
                RotationX -= 0.005f;
            if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.Left))
                RotationZ += 0.005f;
            if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.Right))
                RotationZ -= 0.005f;

            float scale = vxEngine.Current3DSceneBase.Camera.FarPlane;
            inc += 0.001667f;
            //Set the Position for Screen Positioning
            World = Matrix.CreateScale(scale * 0.1f) *
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateRotationY(inc) *
                Matrix.CreateRotationX(-MathHelper.PiOver2) *
                Matrix.CreateRotationX(RotationX) *
                Matrix.CreateRotationZ(RotationZ) *
                Matrix.CreateTranslation(vxEngine.Current3DSceneBase.Camera.Position);

            SunPosition = vxGeometryHelper.RotatePoint(World, new Vector3(0, 0, scale));
            IsOnScreen = false;

            if (Vector3.Dot(Current3DScene.Camera.WorldMatrix.Forward, SunPosition) < 0)
            {
                //vxEngine.Renderer.RT_MaskMap
                IsOnScreen = true;
                screenPos = vxEngine.GraphicsDevice.Viewport.Project(
                    SunPosition,
                    vxEngine.Current3DSceneBase.Camera.Projection,
                    vxEngine.Current3DSceneBase.Camera.View, Matrix.Identity);
				
                int Width = vxEngine.Assets.Textures.Texture_Sun_Glow.Width * TextureSize;
                int Height = vxEngine.Assets.Textures.Texture_Sun_Glow.Height * TextureSize;

                vxEngine.SpriteBatch.Begin();
                vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Texture_Sun_Glow, 
                    new Rectangle((int)(screenPos.X - Width / 2), (int)(screenPos.Y - Height / 2),
                    Width, Height), Color.White);
                vxEngine.SpriteBatch.End();                
			}
        }
        
        public override void RenderMeshShadow() { }
        public override void RenderMeshPrepPass() { }
        public override void RenderMesh(string RenderTechnique) { }
    }
}
#endif