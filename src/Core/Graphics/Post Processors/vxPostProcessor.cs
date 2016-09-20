using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;
using static Virtex.Lib.Vrtc.Graphics.vxRenderer;

namespace Virtex.Lib.Vrtc.Graphics
{
    public class vxPostProcessor : vxPostProcessorInterface
    {
        public vxEngine Engine;

        public vxRenderer Renderer
        {
            get { return Engine.Renderer; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return Engine.SpriteBatch; }
        }

        public Effect Effect;

        public EffectParameterCollection Parameters;

        public Vector2 ScreenResolution
        {
            set
            {
                if(Parameters["ScreenResolution"] != null)
                    Parameters["ScreenResolution"].SetValue(value);
            }
        }

        public Matrix OrthogonalProjection;

        public Matrix HalfPixelOffset;

        public Matrix MatrixTransform
        {
            set
            {
                if (Parameters["MatrixTransform"] != null)
                    Parameters["MatrixTransform"].SetValue(value);
            }
        }

        public vxPostProcessor(vxEngine Engine, Effect Effect)
        {
            //Get a Reference to the Engine
            this.Engine = Engine;

            this.Effect = Effect;
            this.Parameters = Effect.Parameters;
        }


        public virtual void LoadContent()
        {
            //Then Add this Post Processor to the Renderer's Collection 
            this.Engine.Renderer.PostProcessors.Add(this);

            SetResoultion();
        }

        public virtual void SetResoultion()
        {   
            ScreenResolution = new Vector2(Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);

            OrthogonalProjection = Matrix.CreateOrthographicOffCenter(0,
                Engine.GraphicsDevice.Viewport.Width,
                Engine.GraphicsDevice.Viewport.Height,
                0, 0, 1);

            HalfPixelOffset = Matrix.CreateTranslation(.5f / (float)Engine.GraphicsDevice.Viewport.Width,
                                        .5f / (float)Engine.GraphicsDevice.Viewport.Height, 0);

            MatrixTransform = HalfPixelOffset * OrthogonalProjection;
        }

        public virtual void Apply()
        {

        }

        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        public void DrawRenderTargetIntoOther(vxEngine vxEngine, Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            Engine.GraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect);
        }


        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="effect"></param>
        public void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect)
        {
            SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draws With No Effect Used
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void DrawFullscreenQuad(Texture2D texture, int width, int height)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            SpriteBatch.End();
        }
    }
}
