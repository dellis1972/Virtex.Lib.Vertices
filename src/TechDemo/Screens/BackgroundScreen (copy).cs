#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using vxVertices.Core;
using vxVertices.Graphics;
using vxVertices.Screens.Async;
#endregion

namespace MetricRacer.Base
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;

        Model shipModel;
        Matrix modelWorld;

        // Custom rendertargets.
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalDepthRenderTarget;

        // Effect used to apply the edge detection and pencil sketch postprocessing.
        Effect postprocessEffect;
        Effect cartoonShader;

        #endregion

        #region Initialization

        NonPhotoRealisticSettings Settings
        {
            get { return NonPhotoRealisticSettings.PresetSettings[0]; }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(vxEngine.Game.Services, "Content");

            //backgroundTexture = content.Load<Texture2D>("background");

            shipModel = content.Load<Model>("mdls/BackgroundShip/BackgroundShip");
            
            cartoonShader = content.Load<Effect>("Effects/CartoonEffect");
            postprocessEffect = vxEngine.Assets.PostProcessShaders.EdgeDetection;

            PresentationParameters pp = vxEngine.Game.GraphicsDevice.PresentationParameters;
            sceneRenderTarget = new RenderTarget2D(vxEngine.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            normalDepthRenderTarget = new RenderTarget2D(vxEngine.Game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);


            DrawModel.ChangeEffectUsedByModel(shipModel, cartoonShader);

            //
            //Loads Global Content
            //
            if (vxEngine.HasContentBeenLoaded == false)
                vxEngine.LoadGlobalContent(vxEngine.Game.Content);
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            if (sceneRenderTarget != null)
            {
                sceneRenderTarget.Dispose();
                sceneRenderTarget = null;
            }

            if (normalDepthRenderTarget != null)
            {
                normalDepthRenderTarget.Dispose();
                normalDepthRenderTarget = null;
            }
            content.Unload();
            //backgrounMusic.Dispose();
        }


        #endregion

        #region Update and Draw

        bool firstloop = false;
        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (firstloop)
            {
                firstloop = false;
                LoadingScreen.Load(vxEngine, true, PlayerIndex.One, new TrackBaseGamePlay());
            }
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            #region Set Up
            SpriteBatch spriteBatch = vxEngine.SpriteBatch;
            Viewport viewport = vxEngine.GraphicsDevice.Viewport;

            vxEngine.GraphicsDevice.Clear(ClearOptions.Target, Color.GhostWhite, 0, 0);
            GraphicsDevice device = vxEngine.Game.GraphicsDevice;

            string effectTechniqueName = "Toon";

            //
            //Mess with Position and Rotation
            //
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            float scale = 1;
            if (time < 5)
            {
                scale = (float)Math.Tan((double)time / 10);
                if (scale > 1)
                    scale = 1;
            }
            float position = (float)Math.Sin(time / 20);

            //
            //Set World Matrix
            //
            modelWorld =
                Matrix.CreateScale(200) *
                Matrix.CreateRotationX(-MathHelper.PiOver2) * 
                Matrix.CreateRotationY(-time / 20) *
                Matrix.CreateTranslation(0, 10 * (float)Math.Sin(time / 8), 200 * position);

            float height = 150;
            Matrix view = Matrix.CreateLookAt(new Vector3(-800, height, -200),
                                              new Vector3(0, height, -200),
                                              Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    vxEngine.GraphicsDevice.Viewport.AspectRatio,
                                                                    0.1f, 10000);
            #endregion

            #region Setup Edge Detect
            //If we are doing edge detection, first off we need to render the
            //normals and depth of our model into a special rendertarget.
            if (Settings.EnableEdgeDetect)
            {
                device.SetRenderTarget(normalDepthRenderTarget);

                device.Clear(Color.Black);

                DrawShipModel(shipModel, modelWorld, view, projection, "NormalDepth");
            }

            //If we are doing edge detection and/or pencil sketch processing, we
            //need to draw the model into a special rendertarget which can then be
            //fed into the postprocessing shader. Otherwise can just draw it
            //directly onto the backbuffer.
            if (Settings.EnableEdgeDetect || Settings.EnableSketch)
                device.SetRenderTarget(sceneRenderTarget);
            else
                device.SetRenderTarget(null);

            device.Clear(Color.GhostWhite*TransitionAlpha);
            #endregion

            //Draw Models
            DrawShipModel(shipModel, modelWorld, view, projection, effectTechniqueName);

            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            //Draw SpriteBatch shit
            vxEngine.Game.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
            spriteBatch.Begin();

            //Draw Version Information
            int Padding = 5;
            int TextWidth = (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.GameVersion).X;
            int TextHeight = (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.GameVersion).Y;

            //Position Of Text
            Vector2 PosOfText = new Vector2(
                vxEngine.GraphicsDevice.Viewport.Width - 2 * Padding - TextWidth,
                vxEngine.GraphicsDevice.Viewport.Height - Padding - TextHeight);

            //Draw Background
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, 
                new Rectangle(
                    (int)PosOfText.X - Padding,
                    (int)PosOfText.Y,
                    TextWidth + 2 * Padding, 
                    TextHeight),
                Color.Black * TransitionAlpha * 0.75f);

            spriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, vxEngine.GameVersion, PosOfText, 
                Color.White * TransitionAlpha * 0.85f);
            
            spriteBatch.End();
            
            #region ApplyPostProcess
            //Run the postprocessing filter over the scene that we just rendered.
            if (Settings.EnableEdgeDetect || Settings.EnableSketch)
            {
                device.SetRenderTarget(null);

                ApplyPostprocess();
            }
            #endregion
        }

        void DrawShipModel(Model model, Matrix world, Matrix view, Matrix projection, string effectTechniqueName)
        {            
            // Set suitable renderstates for drawing a 3D model.
            vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            vxEngine.Game.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

            // Look up the bone transform matrices.
            Matrix[] transforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    // Specify which effect technique to use.
                    effect.CurrentTechnique = effect.Techniques[effectTechniqueName];

                    Matrix localWorld = transforms[mesh.ParentBone.Index] * world;

                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }

                mesh.Draw();
            }
        }

        void ApplyPostprocess()
        {
            EffectParameterCollection parameters = postprocessEffect.Parameters;
            string effectTechniqueName;

            // Set effect parameters controlling the pencil sketch effect.
            if (Settings.EnableSketch)
            {
                parameters["SketchThreshold"].SetValue(Settings.SketchThreshold);
                parameters["SketchBrightness"].SetValue(Settings.SketchBrightness);
            }

            // Set effect parameters controlling the edge detection effect.
            if (Settings.EnableEdgeDetect)
            {
                Vector2 resolution = new Vector2(sceneRenderTarget.Width,
                                                 sceneRenderTarget.Height);

                Texture2D normalDepthTexture = normalDepthRenderTarget;

                parameters["EdgeWidth"].SetValue(Settings.EdgeWidth);
                parameters["EdgeIntensity"].SetValue(Settings.EdgeIntensity);
                parameters["ScreenResolution"].SetValue(resolution);
                parameters["NormalDepthTexture"].SetValue(normalDepthTexture);

                // Choose which effect technique to use.
                if (Settings.EnableSketch)
                {
                    if (Settings.SketchInColor)
                        effectTechniqueName = "EdgeDetectColorSketch";
                    else
                        effectTechniqueName = "EdgeDetectMonoSketch";
                }
                else
                    effectTechniqueName = "EdgeDetect";
            }
            else
            {
                // If edge detection is off, just pick one of the sketch techniques.
                if (Settings.SketchInColor)
                    effectTechniqueName = "ColorSketch";
                else
                    effectTechniqueName = "MonoSketch";
            }

            // Activate the appropriate effect technique.
            postprocessEffect.CurrentTechnique = postprocessEffect.Techniques[effectTechniqueName];

            // Draw a fullscreen sprite to apply the postprocessing effect.
            vxEngine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, postprocessEffect);
            vxEngine.SpriteBatch.Draw(sceneRenderTarget, Vector2.Zero, Color.White * TransitionAlpha);
            vxEngine.SpriteBatch.End();
        }


        #endregion
    }


    public class DrawModel
    {
        #region Draw Model Normally
        /// <summary>
        /// Simple model drawing method. The interesting part here is that
        /// the view and projection matrices are taken from the camera object.
        /// </summary>        
        static public void Normal(Model model, Matrix world, GraphicsDevice graphicsDevice,
            Matrix view, Matrix projection, bool drawFog, bool DrawShading, float AlphaValue)
        {

            graphicsDevice.BlendState = BlendState.AlphaBlend;

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;

                    // Use the matrices provided by the chase camera
                    effect.View = view;
                    effect.Projection = projection;
                    effect.LightingEnabled = DrawShading;
                    effect.EmissiveColor = new Vector3(0.8f);
                    effect.Alpha = AlphaValue;

                    if (drawFog == true)
                    {
                        effect.FogEnabled = true;
                        effect.FogColor = Color.CornflowerBlue.ToVector3();
                        effect.FogStart = 1200;
                        effect.FogEnd = 3000;

                    }
                }
                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                mesh.Draw();
            }
        }
        #endregion

        #region Draw Model Cartoony

        //Changes the Effect applied to the inputted mesh
        static public void ChangeEffectUsedByModel(Model model, Effect replacementEffect)
        {
            try
            {
                // Table mapping the original effects to our replacement versions.
                Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

                foreach (ModelMesh mesh in model.Meshes)
                {
                    // Scan over all the effects currently on the mesh.
                    foreach (BasicEffect oldEffect in mesh.Effects)
                    {
                        // If we haven't already seen this effect...
                        if (!effectMapping.ContainsKey(oldEffect))
                        {
                            // Make a clone of our replacement effect. We can't just use
                            // it directly, because the same effect might need to be
                            // applied several times to different parts of the model using
                            // a different texture each time, so we need a fresh copy each
                            // time we want to set a different texture into it.
                            Effect newEffect = replacementEffect.Clone();

                            // Copy across the texture from the original effect.
                            newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);
                            newEffect.Parameters["TextureEnabled"].SetValue(oldEffect.TextureEnabled);

                            effectMapping.Add(oldEffect, newEffect);
                        }
                    }

                    // Now that we've found all the effects in use on this mesh,
                    // update it to use our new replacement versions.
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = effectMapping[meshPart.Effect];
                    }
                }
            }
            catch { }//If it fails, its prob bc it already changed the effect
        }

        //Draw the Model with a Cartoon Effect, 
        //NOTE: ChangeEffectUsedByModel(...) must be called first or it won't draw        
        static public void DrawCartoonModel(Model model, Matrix world, Matrix view, Matrix projection, GraphicsDevice graphicsDevice, string effectTechnique)
        {
            // Set suitable renderstates for drawing a 3D model.
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

            // Look up the bone transform matrices.
            Matrix[] transforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    // Specify which effect technique to use.
                    effect.CurrentTechnique = effect.Techniques[effectTechnique];

                    Matrix localWorld = transforms[mesh.ParentBone.Index] * world;
                    effect.Parameters["World"].SetValue(localWorld);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }
                graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                mesh.Draw();
            }
        }

        /// <summary>
        /// Helper applies the edge detection and pencil sketch postprocess effect.
        /// </summary>
        static public void ApplyPostprocess(Effect postprocessEffect, SpriteBatch spriteBatch,
            RenderTarget2D sceneRenderTarget, RenderTarget2D normalDepthRenderTarget)
        {
            EffectParameterCollection parameters = postprocessEffect.Parameters;
            string effectTechniqueName;

            Vector2 resolution = new Vector2(sceneRenderTarget.Width,
                                             sceneRenderTarget.Height);

            Texture2D normalDepthTexture = normalDepthRenderTarget;

            parameters["EdgeWidth"].SetValue(1);
            parameters["EdgeIntensity"].SetValue(1);
            parameters["ScreenResolution"].SetValue(resolution);
            parameters["NormalDepthTexture"].SetValue(normalDepthTexture);

            effectTechniqueName = "EdgeDetect";

            // Activate the appropriate effect technique.
            postprocessEffect.CurrentTechnique = postprocessEffect.Techniques[effectTechniqueName];

            // Draw a fullscreen sprite to apply the postprocessing effect.
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postprocessEffect);
            spriteBatch.Draw(sceneRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        #endregion
    }
}
