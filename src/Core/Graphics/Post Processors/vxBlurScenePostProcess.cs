using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Settings;
using static Virtex.Lib.Vrtc.Graphics.vxRenderer;

namespace Virtex.Lib.Vrtc.Graphics
{
    public class vxBlurScenePostProcess : vxPostProcessor
    {
        /// <summary> 
		/// Controls how bright a pixel needs to be before it will bloom.
		/// Zero makes everything bloom equally, while higher values select
		/// only brighter colors. Somewhere between 0.25 and 0.5 is good.
		/// </summary>
        public float BloomThreshold = 0;


        /// <summary>
        /// Controls how much blurring is applied to the bloom image.
        /// The typical range is from 1 up to 10 or so.
        /// </summary>
        public float BlurAmount = 2;


        /// <summary>
        /// Controls the amount of the bloom and base images that
        /// will be mixed into the final scene. Range 0 to 1.
        /// </summary>
        public float BloomIntensity = 1;

        /// <summary>
        /// The base intensity.
        /// </summary>
        public float BaseIntensity = 0.1f;


        /// <summary>
        /// Independently control the color saturation of the bloom and
        /// base images. Zero is totally desaturated, 1.0 leaves saturation
        /// unchanged, while higher values increase the saturation level.
        /// </summary>
        public float BloomSaturation = 1;

        /// <summary>
        /// The base saturation.
        /// </summary>
        public float BaseSaturation = 1;


        float[] sampleWeightsX, sampleWeightsY;// = new float[sampleCount];
        Vector2[] sampleOffsetsX, sampleOffsetsY;// = new Vector2[sampleCount];

        int SampleCount = 1;

        public vxBlurScenePostProcess(vxEngine Engine) :base(Engine, Engine.Assets.PostProcessShaders.GaussianBlurEffect)
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();
            
        }

        public override void SetResoultion()
        {
            base.SetResoultion();

            //First Setup the Sample Count
            SampleCount = Effect.Parameters["SampleWeights"].Elements.Count;

            sampleWeightsX = new float[SampleCount]; 
            sampleOffsetsX = new Vector2[SampleCount];

            sampleWeightsY = new float[SampleCount];
            sampleOffsetsY = new Vector2[SampleCount];

            //Now Generate the Sample Weights and Sample Offsets for this given Resolution
            SetBloomEffectParameters(1.0f / (float)Renderer.RT_BlurTempOne.Width, 0, BlurAmount, 
                IntermediateBuffer.BlurredHorizontally, SampleCount);


            SetBloomEffectParameters(1.0f / (float)Renderer.RT_BlurTempOne.Width, 0, BlurAmount,
                IntermediateBuffer.BlurredBothWays, SampleCount);
        }

        public override void Apply()
        {
            //Set First Temp Blur Rendertarget 
            Effect.Parameters["SampleWeights"].SetValue(sampleWeightsX);
            Effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsX);

            DrawRenderTargetIntoOther(Engine, Renderer.RT_FinalScene, Renderer.RT_BlurTempTwo,
                               Effect, IntermediateBuffer.BlurredHorizontally);

            Effect.Parameters["SampleWeights"].SetValue(sampleWeightsY);
            Effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsY);

            DrawRenderTargetIntoOther(Engine, Renderer.RT_BlurTempTwo, Renderer.RT_BlurredScene,
                               Engine.Assets.PostProcessShaders.GaussianBlurEffect,
                               IntermediateBuffer.BlurredBothWays);
        }


        void SetBloomEffectParameters(float dx, float dy, float theta, IntermediateBuffer BlurDirection, int sampleCount)
        {
            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeBloomGaussian(0, theta);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeBloomGaussian(i + 1, theta);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            if(BlurDirection == IntermediateBuffer.BlurredHorizontally)
            {
                sampleWeightsX = sampleWeights;
                sampleOffsetsX = sampleOffsets;
            }
            else
            {
                sampleWeightsY = sampleWeights;
                sampleOffsetsY = sampleOffsets;
            }
        }


        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeBloomGaussian(float n, float theta)
        {
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
