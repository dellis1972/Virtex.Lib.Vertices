using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.Graphics
{
    public class vxEdgeDetectPostProcess : vxPostProcessor
    {
        /// <summary>
        /// Normal Map.
        /// </summary>
        public RenderTarget2D NormalMap
        {
            set { Parameters["NormalTexture"].SetValue(value); }
        }

        /// <summary>
        /// Depth map of the scene.
        /// </summary>
        public RenderTarget2D DepthMap
        {
            set { Parameters["DepthTexture"].SetValue(value); }
        }

        /// <summary>
        /// Edge Width for edge detection.
        /// </summary>
        public float EdgeWidth
        {
            set { Parameters["EdgeWidth"].SetValue(value); }
        }

        /// <summary>
        /// Intensity of the edge dection.
        /// </summary>
        public float EdgeIntensity
        {
            set { Parameters["EdgeIntensity"].SetValue(value); }
        }


        public float NormalThreshold
        {
            set { Parameters["NormalThreshold"].SetValue(value); }
        }


        public float DepthThreshold
        {
            set { Parameters["DepthThreshold"].SetValue(value); }
        }

        public float NormalSensitivity
        {
            set { Parameters["NormalSensitivity"].SetValue(value); }
        }


        public float DepthSensitivity
        {
            set { Parameters["DepthSensitivity"].SetValue(value); }
        }

        public vxEdgeDetectPostProcess(vxEngine Engine) :base(Engine, Engine.Assets.PostProcessShaders.CartoonEdgeDetection)
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Set Edge Settings
            EdgeWidth = 1;
            EdgeIntensity = 1;


            // How sensitive should the edge detection be to tiny variations in the input data?
            // Smaller settings will make it pick up more subtle edges, while larger values get
            // rid of unwanted noise.
            NormalThreshold = 0.5f;
            DepthThreshold = 0.001f;

            // How dark should the edges get in response to changes in the input data?
            NormalSensitivity = 1.0f;
            DepthSensitivity = 10000.0f;
        }

        public override void Apply()
        {
            //Set Render Target
            Engine.GraphicsDevice.SetRenderTarget(Renderer.RT_EdgeDetected);

            if (Engine.Profile.Settings.Graphics.Bool_DoEdgeDetection)
            {
                // Pass in the Normal Map
                NormalMap = Renderer.RT_NormalMap;

                // Pass in the Depth Map
                DepthMap = Renderer.RT_DepthMap;


                // Activate the appropriate effect technique.
                Effect.CurrentTechnique = Effect.Techniques["EdgeDetect"];

                // Draw a fullscreen sprite to apply the postprocessing effect.
                Engine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, Effect);
                Engine.SpriteBatch.Draw(Renderer.RT_FinalScene, Vector2.Zero, Color.White);
                Engine.SpriteBatch.End();
            }
            else
            {
                //If the user elects to not use the effect, simply draw the previous scene into the current 
                //active render target.
                Engine.SpriteBatch.Begin();
                Engine.SpriteBatch.Draw(Renderer.RT_FinalScene, Vector2.Zero, Color.White);
                Engine.SpriteBatch.End();
            }
        }
    }
}
