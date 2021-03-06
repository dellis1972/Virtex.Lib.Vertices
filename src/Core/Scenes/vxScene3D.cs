#if VIRTICES_3D
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Linq;

//Virtex vxEngine Declaration
using System.Collections;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Mathematics;
using Virtex.Lib.Vrtc.Core.Particles;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Audio;
using BEPUphysicsDrawer.Models;
using BEPUutilities.Threading;
using Virtex.Lib.Vrtc.Utilities;

using Virtex.Lib.Vrtc.Core.Entities;
using Virtex.Lib.Vrtc.Geometry;
using Virtex.Lib.Vrtc.Core.Debug;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Screens.Menus;

#endregion

namespace Virtex.Lib.Vrtc.Core.Scenes
{
	/// <summary>
	/// The vxScene3D class implements the actual game logic for 3D Games. It is 
	/// the base 3D class which the vxSandboxGamePlay class inherits from. You'll probably want to
	/// either inherit this class or the vxSandboxGamePlay class.
	/// </summary>
	public partial class vxScene3D : vxSceneBase
	{
		#region Fields

		public List<vxEntity3D> List_OverlayItems = new List<vxEntity3D> ();

        vxSkyBoxEntity SkyBox;

		/// <summary>
		/// A Collection of Instance Sets (Yes a collection inside of a collection, ....colleception?!?!)
		/// </summary>
		public List<InstanceSet> InstanceSetCollection = new List<InstanceSet> ();


		public Dictionary<object, InstanceSet> Instances = new Dictionary<object, InstanceSet> ();

		/// <summary>
		/// Collection of Lights in the Level.
		/// </summary>
		public List<vxLightEntity> LightItems = new List<vxLightEntity> ();

		/// <summary>
		/// The main BEPU Physics Simulation Space used in the game.
		/// </summary>
		public Space BEPUPhyicsSpace { get; set; }

		/// <summary>
		/// This is the multithreaded parrallel looper class used by BEPU too
		/// multi-thread the physics engine.
		/// </summary>
		private ParallelLooper BEPUParallelLooper;

		/// <summary>
		/// Model Drawer for debugging the phsyics system
		/// </summary>
		public ModelDrawer BEPUDebugDrawer { get; set; }

        LensFlareComponent lensFlare;

		#region Fog Control

        public bool DoFog
		{
			get { return _doFog; }
			set
			{
				_doFog = value;
				foreach (vxEntity3D entity in Entities)
					entity.DoFog = _doFog;
			}
		}
		bool _doFog = false;

		/// <summary>
		/// Gets or sets the fog near value. Updating this will cascade an update down to all entities in the scene.
		/// </summary>
		/// <value>The fog near.</value>
        public float FogNear 
		{ 
			get { return _fogNear;}
			set
			{
				_fogNear = value;
				foreach (vxEntity3D entity in Entities)
					entity.FogNear = _fogNear;
			}
		}
		float _fogNear = 20;


		/// <summary>
		/// Gets or sets the fog far value. Updating this will cascade an update down to all entities in the scene.
		/// </summary>
		/// <value>The fog far.</value>
		public float FogFar
		{
			get { return _fogFar; }
			set
			{
				_fogFar = value;
				foreach (vxEntity3D entity in Entities)
					entity.FogFar = _fogFar;
			}
		}
		float _fogFar = 1000;


		/// <summary>
		/// Gets or sets the color of the fog.
		/// </summary>
		/// <value>The color of the fog.</value>
        public Color FogColor 
		{
			get { return _fogColor; }
			set
			{
				_fogColor = value;
				foreach (vxEntity3D entity in Entities)
					entity.FogColor = _fogColor;
			}
		}
        Color _fogColor = Color.White;

		#endregion

        /// <summary>
        /// Manages the Sun Class.
        /// </summary>
        public vxSunEntity SunEmitter { get; set; }

		private Vector3 LightPositions {
			get { return SunEmitter.SunPosition; }
		}

		
        /// <summary>
        /// Water Entity Collection
        /// </summary>
        public List<vxWaterEntity> waterItems = new List<vxWaterEntity>();

       
		/// <summary>
		/// Base Camera Class. This can be used on Chase Camera as well as FPS and
		/// Free Roaming.
		/// </summary>
		public vxCamera3D Camera {
			get { return _camera; }
			set { _camera = value; }
		}

		private vxCamera3D _camera;

		/// <summary>
		/// Current Scenes Audio Manager
		/// </summary>
		public vxAudioManager AudioManager {
			get { return _audioManager; }
			set { _audioManager = value; }
		}

		private vxAudioManager _audioManager;

		/// <summary>
		/// Half Pixel.
		/// </summary>
		public Vector2 HalfPixel {
			get { return _halfPixel; }
			set { _halfPixel = value; }
		}

		public Vector2 _halfPixel;

		// average the timings over several frames
		const uint mNumFramesForAverage = 100;


		//public bool mBlockInput = false;

		public vxCamera3D mVirtualCamera = new vxVirtualCamera ();

		vxEnumVirtualCameraMode mVirtualCameraMode = vxEnumVirtualCameraMode.None;
		vxEnumShadowMapOverlayMode mShadowMapOverlay = vxEnumShadowMapOverlayMode.None;
		vxEnumSceneShadowMode mSceneShadowMode = vxEnumSceneShadowMode.Default;

        public Color SkyColour
        {
            get { return _skyColour; }
            set { _skyColour = value; }
        }
        Color _skyColour = Color.CornflowerBlue;


		float pauseAlpha;

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor which starts the game as a Normal (Non-Networked) game.
		/// </summary>
		public vxScene3D ()
		{
			TransitionOnTime = TimeSpan.FromSeconds (0.5);
			TransitionOffTime = TimeSpan.FromSeconds (0.5);

		}


		public override void InitialiseLevel ()
		{
			base.InitialiseLevel ();

			BEPUDebugDrawer = new ModelDrawer (vxEngine.Game);

			//Starts BEPU with multiple Cores
			BEPUParallelLooper = new ParallelLooper ();
			if (Environment.ProcessorCount > 1) {
				for (int i = 0; i < Environment.ProcessorCount; i++) {
					BEPUParallelLooper.AddThread ();
				}
			}
            

            BEPUPhyicsSpace = new Space(BEPUParallelLooper);
			BEPUPhyicsSpace.ForceUpdater.Gravity = new Vector3 (0, -9.81f, 0);
            vxConsole.WriteLine("Starting Physics vxEngine using " + BEPUPhyicsSpace.ParallelLooper.ThreadCount + " Processors");
        }

		public Model sphereModel;

		/// <summary>
		/// Load graphics content for the game.
		/// </summary>
		public override void LoadContent ()
		{
			if (content == null)
				content = new ContentManager (vxEngine.Game.Services, "Content");

			//Get the Graphics Manager.
			mGraphicsManager = vxEngine.Game.Services.GetService (typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;

			//Set the Current Gameplay Screen
			vxEngine.CurrentGameplayScreen = this;


            sphereModel = vxEngine.EngineContentManager.Load<Model> ("Models/lghtng/sphere");

			//Setup Camera
			Camera = new vxCamera3D (vxEngine, new Vector3 (0, 15, 0), 0, 0,
				Matrix.CreatePerspectiveFieldOfView (MathHelper.PiOver4,
					(float)vxEngine.GraphicsDevice.Viewport.Width / vxEngine.GraphicsDevice.Viewport.Height, .1f, 10000), CameraType.ChaseCamera);
			Camera.Yaw = -MathHelper.PiOver2;


            DoFog = false;
            FogNear = Camera.FarPlane/4;
            FogFar = Camera.FarPlane;

            //Setup Sun
            SunEmitter = new vxSunEntity (vxEngine);
            SkyBox = new vxSkyBoxEntity(vxEngine);

            //Setup Audio Manager
            AudioManager = new vxAudioManager (vxEngine);

            // Create and add the lensflare component.
            lensFlare = new LensFlareComponent(vxEngine);
            lensFlare.LoadContent();

            if (GameType == vxEnumGameType.Networked)
                IsPausable = false; 

			//Setup Renderer.
			//vxEngine.Renderer = new vxRenderer (vxEngine);

			// default window size
			mGraphicsManager.PreferMultiSampling = false;

			//vxEngine.Renderer.loadContent (mGraphicsManager);

			Camera.AspectRatio = mGraphicsManager.GraphicsDevice.Viewport.AspectRatio;

#if VRTC_INCLDLIB_NET
            //InitialiseNetwork ();
#endif
		}



		/// <summary>
		/// Unload graphics content used by the game.
		/// </summary>
		public override void UnloadContent ()
        {
#if VRTC_INCLDLIB_NET
			//DeinitialiseNetwork ();
#endif

            content.Unload ();
		}


		public T nextEnumValue<T> (T currentValue)
		{
			// not nice but simplifies a lot of code
			int nextValue = ((int)(object)currentValue + 1) % Enum.GetValues (typeof(T)).Length;
			return (T)(object)nextValue;
		}

#endregion

        #region Update and Draw


		/// <summary>
		/// Updates the state of the game. This method checks the vxGameBaseScreen.IsActive
		/// property, so the game will stop updating when the pause menu is active,
		/// or if you tab away to a different application.
		/// </summary>
		public sealed override void Update (GameTime gameTime, bool otherScreenHasFocus,
		                                    bool coveredByOtherScreen)
		{
			base.Update (gameTime, otherScreenHasFocus, false);
            

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen && IsPausable)
				pauseAlpha = Math.Min (pauseAlpha + 1f / 32, 1);
			else
				pauseAlpha = Math.Max (pauseAlpha - 1f / 32, 0);

			if (IsActive  || IsPausable == false) {
            
                #region Set Debug Info

				BEPUDebugDrawer.Update ();

				// check for other keys pressed on keyboard
				if (vxEngine.InputManager.IsNewKeyPress (Keys.P)) {
					var previousSceneShadowMode = mSceneShadowMode;
					mSceneShadowMode = nextEnumValue (mSceneShadowMode);

					if (previousSceneShadowMode < vxEnumSceneShadowMode.BlockPattern && mSceneShadowMode >= vxEnumSceneShadowMode.BlockPattern ||
					    previousSceneShadowMode >= vxEnumSceneShadowMode.BlockPattern && mSceneShadowMode < vxEnumSceneShadowMode.BlockPattern) {
						vxEngine.Renderer.swapShadowMapWithBlockTexture ();
					}

					foreach (vxEntity entity in Entities)
						((vxEntity3D)entity).renderShadowSplitIndex = mSceneShadowMode >= vxEnumSceneShadowMode.SplitColors;


				}

				vxEngine.Renderer.mSnapShadowMaps = true;
				if (vxEngine.InputManager.IsNewKeyPress (Keys.F)) {
					vxEngine.Renderer.mSnapShadowMaps = !vxEngine.Renderer.mSnapShadowMaps;
				}
				//vxConsole.WriteToInGameDebug("f:" + vxEngine.Renderer.mSnapShadowMaps);

				if (vxEngine.InputManager.IsNewKeyPress (Keys.T)) {
					foreach (vxEntity entity in Entities)
						((vxEntity3D)entity).TextureEnabled = !((vxEntity3D)entity).TextureEnabled;
				}

                #endregion


                //Update Audio Manager
                //**********************************************************************************************
                AudioManager.Listener.Position = Camera.Position / 100;
				AudioManager.Listener.Forward = Camera.View.Forward;
				AudioManager.Listener.Up = Camera.View.Up;
				AudioManager.Listener.Velocity = Camera.View.Forward;

				
				// Update Physics
				//**********************************************************************************************

				// Start measuring time for "Physics".
				vxEngine.DebugSystem.TimeRuler.BeginMark ("Physics", Color.LimeGreen);

                //Update the Physics System.
                //vxConsole.WriteToInGameDebug(((float)gameTime.ElapsedGameTime.Milliseconds)/1000);
                //vxConsole.WriteToInGameDebug((float)gameTime.ElapsedGameTime.TotalSeconds);
                //BEPUPhyicsSpace.Update ((float)gameTime.ElapsedGameTime.TotalSeconds);
                BEPUPhyicsSpace.Update();

				// Stop measuring time for "Draw".
				vxEngine.DebugSystem.TimeRuler.EndMark ("Physics");



                // Update the Scene
                //**********************************************************************************************
                UpdateScene(gameTime, otherScreenHasFocus, false);




                // Update Scene Entities
                //**********************************************************************************************
                for (int i = 0; i < Entities.Count; i++) {
					Entities [i].Update (gameTime);

					if (Entities [i].KeepUpdating == false)
						Entities.RemoveAt (i);
				}

                // Update Particle System
                //**********************************************************************************************
                ParticleSystem.Update (gameTime);


                // Update Camera
                //**********************************************************************************************
                UpdateCameraChaseTarget();
				Camera.Update (gameTime);


				vxEngine.Renderer.setLightPosition (-LightPositions);

				// Tell the lensflare component where our camera is positioned.
				lensFlare.LightDir = SunEmitter.LightDirection;
				lensFlare.View = Camera.View;
				lensFlare.Projection = Camera.Projection;
			} else
				vxEngine.InputManager.ShowCursor = true;
		}



		/// <summary>
		/// Main Game Update Loop which is accessed outside of the vxEngine
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="otherScreenHasFocus"></param>
		/// <param name="coveredByOtherScreen"></param>
		public override void UpdateScene (GameTime gameTime, bool otherScreenHasFocus,
		                                  bool coveredByOtherScreen)
		{

		}


		/// <summary>
		/// Draws the gameplay screen.
		/// </summary>
		public override void DrawScene (GameTime gameTime)
		{
#if VRTC_PLTFRM_XNA
            foreach (InstanceSet instSet in InstanceSetCollection)
            {
                instSet.Update();
            }

            foreach (KeyValuePair<object, InstanceSet> entry in Instances)
            {
                // do something with entry.Value or entry.Key
                entry.Value.Update();
            }
#endif

            // Draw Shadow Map
            //**********************************************************************************************
            // determine shadow frustums
            var shadowCamera = mVirtualCameraMode != vxEnumVirtualCameraMode.None ? mVirtualCamera : Camera;
            vxEngine.Renderer.setShadowTransforms(shadowCamera);

            // render shadow maps first, then scene
            DrawShadows(gameTime, shadowCamera);


            // Draw Main Render Passes
            //**********************************************************************************************
            DrawMain(gameTime, Camera);

            //Draw the Sun (And any post processing that comes with)
            //lensFlare.Draw(gameTime);

            //Now Draw the Distortion (Note, this will not show up in the edge detection)
            //DrawDistortion(vxEngine.Renderer.RT_MainScene, gameTime);

            //Get Blurred Scene for a number of different Processes (Depth of Field, Menu Background blurring etc...)
            //Do This before the Edge Detection, otherwise you get edge bleeding that over saturates the scene with black.


            // Apply Post Processing Effects
            //**********************************************************************************************
            vxEngine.Renderer.ApplyPostProcessors();


            //Draw Menu Blur Only if this screen is Pausable
			if (IsPausable) {
				vxEngine.SpriteBatch.Begin ();
				vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_BlurredScene, Vector2.Zero, Color.White * pauseAlpha);
				vxEngine.SpriteBatch.End ();
			}

            // Draw any inherited or overriden code
            //**********************************************************************************************
            DrawGameplayScreen(gameTime);



            // Draw Overlay items such as 3D Sandbox Cursor and HUD
            //**********************************************************************************************
            DrawOverlayItems();

			DrawHUD();



            // Debug Rendering
            //**********************************************************************************************
            if (vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_MESH).GetAsBool())
				BEPUDebugDrawer.Draw (Camera.View, Camera.Projection);

			vxDebugShapeRenderer.Draw (gameTime, Camera.View, Camera.Projection);

			if (vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_RNDRTRGT).GetAsBool())
				DrawDebugRenderTargetsToScreen ();

			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
				vxEngine.FadeBackBufferToBlack (MathHelper.Lerp (1f - TransitionAlpha, 1f, pauseAlpha / 2));
		}

		/// <summary>
		/// Draws the HUD Once the entire scene has been rendered.
		/// </summary>
		public virtual void DrawHUD ()
		{

		}


		public void DrawPointLight (Vector3 lightPosition, Color color, float lightRadius, float lightIntensity)
		{
			Effect pointLightEffect = vxEngine.Assets.Shaders.DrfrdRndrPointLight;

			//set the G-Buffer parameters
			pointLightEffect.Parameters ["colorMap"].SetValue (vxEngine.Renderer.RT_ColourMap);
			pointLightEffect.Parameters ["normalMap"].SetValue (vxEngine.Renderer.RT_NormalMap);
			pointLightEffect.Parameters ["depthMap"].SetValue (vxEngine.Renderer.RT_DepthMap);

			//compute the light world matrix
			//scale according to light radius, and translate it to light position
			Matrix sphereWorldMatrix = Matrix.CreateScale (lightRadius) * Matrix.CreateTranslation (lightPosition);
			pointLightEffect.Parameters ["World"].SetValue (sphereWorldMatrix);
			pointLightEffect.Parameters ["View"].SetValue (Camera.View);
			pointLightEffect.Parameters ["Projection"].SetValue (Camera.Projection);
			//light position
			pointLightEffect.Parameters ["lightPosition"].SetValue (lightPosition);

			//set the color, radius and Intensity
			pointLightEffect.Parameters ["Color"].SetValue (color.ToVector3 ());
			pointLightEffect.Parameters ["lightRadius"].SetValue (lightRadius);
			pointLightEffect.Parameters ["lightIntensity"].SetValue (lightIntensity);

			//parameters for specular computations
			pointLightEffect.Parameters ["cameraPosition"].SetValue (Camera.Position);
			pointLightEffect.Parameters ["InvertViewProjection"].SetValue (Matrix.Invert (Camera.View * Camera.Projection));
			//size of a halfpixel, for texture coordinates alignment
			pointLightEffect.Parameters ["halfPixel"].SetValue (_halfPixel);
			//calculate the distance between the camera and light center
			float cameraToCenter = Vector3.Distance (Camera.Position, lightPosition);
			//if we are inside the light volume, draw the sphere's inside face
			if (cameraToCenter < lightRadius)
				vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
			else
				vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

			vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

			pointLightEffect.Techniques [0].Passes [0].Apply ();
			foreach (ModelMesh mesh in sphereModel.Meshes) {
				foreach (ModelMeshPart meshPart in mesh.MeshParts) {
					vxEngine.GraphicsDevice.Indices = meshPart.IndexBuffer;
					vxEngine.GraphicsDevice.SetVertexBuffer (meshPart.VertexBuffer);

					vxEngine.GraphicsDevice.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
				}
			}

			vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		public virtual void DrawOverlayItems ()
		{
            //if (vxEngine.DisplayDebugMesh == false)
            //    foreach (vxEntity3D entity in List_OverlayItems)
            //        entity.RenderMeshPlain();
		}

		public void DrawShadows (GameTime gameTime, vxCamera3D camera)
		{
			// only render shadow map if we're not filling it with a block pattern
			if (mSceneShadowMode < vxEnumSceneShadowMode.BlockPattern) {
				vxEngine.GraphicsDevice.SetRenderTarget (vxEngine.Renderer.RT_ShadowMap);
				vxEngine.GraphicsDevice.Clear (ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

				vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
				vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
				vxEngine.GraphicsDevice.SamplerStates [0] = SamplerState.PointClamp;

                if (vxEngine.Profile.Settings.Graphics.ShadowQuality > Settings.vxEnumQuality.None)
                {
                    foreach (vxEntity3D entity in Entities)
                        entity.RenderMeshShadow();

                    foreach (InstanceSet instSet in InstanceSetCollection)
                        instSet.RenderInstancedShadow(instSet.InstancedModel.ModelShadow, camera, instSet.instances.Count);

                    foreach (KeyValuePair<object, InstanceSet> entry in Instances)
                    {
                        // do something with entry.Value or entry.Key
                        entry.Value.RenderInstancedShadow(entry.Value.InstancedModel.ModelShadow, camera, entry.Value.instances.Count);
                    }
                }

			}
		}


		public void DrawMain (GameTime gameTime, vxCamera3D camera)
		{
			//vxEngine.Profile.Settings.Graphics.Bool_DoGodRays = false;
			_halfPixel = -new Vector2 (.5f / (float)vxEngine.GraphicsDevice.Viewport.Width,
				.5f / (float)vxEngine.GraphicsDevice.Viewport.Height);



            /****************************************************************************/
            /*						PREP PASS											*/
            /****************************************************************************/
            vxEngine.GraphicsDevice.SetRenderTargets(
                vxEngine.Renderer.RT_ColourMap,
                vxEngine.Renderer.RT_NormalMap,
                vxEngine.Renderer.RT_DepthMap);

            //Setup initial graphics states for prep pass            
            vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            //
            //Reset Appropriate Values for Rendertargets
            //vxEngine.Assets.Shaders.DrfrdRndrClearGBuffer.Parameters["SkyColour"].SetValue(Color.White.ToVector4());
            vxEngine.Assets.Shaders.DrfrdRndrClearGBuffer.Techniques[0].Passes[0].Apply();
            vxEngine.Renderer.RenderQuad(Vector2.One * -1, Vector2.One);

            //Set the Depth Buffer appropriately
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (vxEntity entity in Entities)
				((vxEntity3D)entity).RenderMeshPrepPass();


#if VRTC_PLTFRM_XNA
            foreach (InstanceSet instSet in InstanceSetCollection)
                instSet.RenderInstanced(instSet.InstancedModel.ModelMain, camera, instSet.instances.Count, "Technique_PrepPass_Instanced");

            foreach (KeyValuePair<object, InstanceSet> entry in Instances)
            {
                // do something with entry.Value or entry.Key
                entry.Value.RenderInstanced(entry.Value.InstancedModel.ModelMain, camera, entry.Value.instances.Count, "Technique_PrepPass_Instanced");
            }
#endif


            // Render God Rays
            //**********************************************************************************************
            vxEngine.GraphicsDevice.SetRenderTarget(vxEngine.Renderer.RT_SunMap);
            vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            vxEngine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            SunEmitter.DrawGlow();

            vxEngine.GraphicsDevice.SetRenderTarget(vxEngine.Renderer.RT_MaskMap);
            vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            vxEngine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            Matrix orthoproj = Matrix.CreateOrthographicOffCenter(0,
                                       vxEngine.GraphicsDevice.Viewport.Width,
                                       vxEngine.GraphicsDevice.Viewport.Height,
                                       0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            vxEngine.Assets.PostProcessShaders.MaskedSunEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * orthoproj);
            vxEngine.Assets.PostProcessShaders.MaskedSunEffect.Parameters["DepthMap"].SetValue(vxEngine.Renderer.RT_DepthMap);
            
            vxEngine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, 
                vxEngine.Assets.PostProcessShaders.MaskedSunEffect);
            vxEngine.SpriteBatch.Draw(vxEngine.Renderer.RT_SunMap, Vector2.Zero, Color.White);
            vxEngine.SpriteBatch.End();



            // Render the Water Reflection Map for the nearest water entity
            //**********************************************************************************************
            vxEngine.GraphicsDevice.SetRenderTarget(vxEngine.Renderer.RT_WaterReflectionMap);
            vxEngine.GraphicsDevice.Clear(Color.CornflowerBlue);
            vxEngine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (waterItems.Count > 0)
                foreach (vxEntity3D entity in Entities)
                    entity.RenderMeshForWaterReflectionPass(waterItems[0].WrknPlane);





            //Now Render the Scene Into the Scene Render Target
            //**********************************************************************************************
            vxEngine.GraphicsDevice.SetRenderTarget(vxEngine.Renderer.RT_MainScene);
            vxEngine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            vxEngine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, SkyColour, 1.0f, 0);

			if (vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_MESH).GetAsBool() == false)
            {
                switch (mSceneShadowMode)
                {
                    case vxEnumSceneShadowMode.Default:
                    case vxEnumSceneShadowMode.SplitColors:
                    case vxEnumSceneShadowMode.BlockPattern:

#if VRTC_PLTFRM_XNA
                        foreach (InstanceSet instSet in InstanceSetCollection)
                            instSet.RenderInstanced(instSet.InstancedModel.ModelMain, camera, instSet.instances.Count, "Technique_Main_Instanced");

                        foreach (KeyValuePair<object, InstanceSet> entry in Instances)
                        {
                            // do something with entry.Value or entry.Key
                            entry.Value.RenderInstanced(entry.Value.InstancedModel.ModelMain, camera, entry.Value.instances.Count, "Technique_Main_Instanced");
                        }
#endif

                        // Render Main Pass
                        //**********************************************************************************************
                        foreach (vxEntity3D entity in Entities)
                            entity.RenderMesh("Technique_Main");


                        // Now Generate Water Reflection Map
                        //**********************************************************************************************
                        foreach (vxWaterEntity water in waterItems)
                            water.DrawWater(vxEngine.Renderer.RT_WaterReflectionMap, camera.GetReflectionView(water.WrknPlane));

                        break;
                }
            }
            

            /****************************************************************************/
            /*						LIGHT MAP PASS										*/
            /****************************************************************************/
            //Render Light Pass
            vxEngine.GraphicsDevice.SetRenderTarget(vxEngine.Renderer.RT_LightMap);
            vxEngine.GraphicsDevice.Clear(Color.Transparent);
            vxEngine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.None;


            DrawDirectionalLight(-Vector3.Normalize(vxEngine.Renderer.lightPosition), Color.White*0);
            
            vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (vxLightEntity lightEntity in LightItems)
                lightEntity.Draw();


            /****************************************************************************/
            /*						FINAL PASS											*/
            /****************************************************************************/

            //Set Render State
            vxEngine.GraphicsDevice.SetRenderTarget(vxEngine.Renderer.RT_FinalScene);
            vxEngine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            vxEngine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;


            Effect finalCombineEffect = vxEngine.Assets.Shaders.DrfrdRndrCombineFinal;
            //Combine everything
            finalCombineEffect.Parameters["colorMap"].SetValue(vxEngine.Renderer.RT_MainScene);
            finalCombineEffect.Parameters["lightMap"].SetValue(vxEngine.Renderer.RT_LightMap);
            finalCombineEffect.Parameters["halfPixel"].SetValue(_halfPixel);

            finalCombineEffect.Techniques[0].Passes[0].Apply();
            vxEngine.Renderer.RenderQuad(Vector2.One * -1, Vector2.One);
        }


		private void DrawDirectionalLight (Vector3 lightDirection, Color color)
		{
			Effect directionalLightEffect = vxEngine.Assets.Shaders.DrfrdRndrDirectionalLight;

			directionalLightEffect.Parameters ["colorMap"].SetValue (vxEngine.Renderer.RT_ColourMap);
			directionalLightEffect.Parameters ["normalMap"].SetValue (vxEngine.Renderer.RT_NormalMap);
			directionalLightEffect.Parameters ["depthMap"].SetValue (vxEngine.Renderer.RT_DepthMap);

			directionalLightEffect.Parameters ["lightDirection"].SetValue (lightDirection);
			directionalLightEffect.Parameters ["Color"].SetValue (color.ToVector3 ());

			directionalLightEffect.Parameters ["cameraPosition"].SetValue (Camera.Position);
			directionalLightEffect.Parameters ["InvertViewProjection"].SetValue (Matrix.Invert (Camera.View * Camera.Projection));

			directionalLightEffect.Parameters ["halfPixel"].SetValue (_halfPixel);

			directionalLightEffect.Techniques [0].Passes [0].Apply ();

			vxEngine.Renderer.RenderQuad (Vector2.One * -1, Vector2.One);
		}

		public virtual void UpdateCameraChaseTarget ()
		{
		}

		/// <summary>
		/// Grab a scene that has already been rendered, 
		/// and add a distortion effect over the top of it.
		/// </summary>
		public void DrawDistortion (RenderTarget2D RenderTarget, GameTime gameTime)
		{
			// if we want to show the distortion map, then the backbuffer is done.
			// if we want to render the scene distorted, then we need to resolve the
			// backbuffer as the distortion map and use it to distort the scene
			vxEngine.GraphicsDevice.SetRenderTarget (vxEngine.Renderer.RT_DistortionScene);

			// draw the scene image again, distorting it with the distortion map
			vxEngine.GraphicsDevice.Textures [1] = vxEngine.Renderer.RT_DistortionMap;
			vxEngine.GraphicsDevice.SamplerStates [1] = SamplerState.PointClamp;

			Viewport viewport = vxEngine.GraphicsDevice.Viewport;
			Effect distortEffect = vxEngine.Assets.PostProcessShaders.distortEffect;
			vxEngine.Assets.PostProcessShaders.distortEffect.CurrentTechnique = false ? vxEngine.Assets.PostProcessShaders.distortBlurTechnique : vxEngine.Assets.PostProcessShaders.distortTechnique;
			vxEngine.Renderer.DrawFullscreenQuad (RenderTarget,
				viewport.Width, viewport.Height, distortEffect);

		}



		/// <summary>
		/// Main Gameplay Draw Loop which is accessed outside of the vxEngine
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void DrawGameplayScreen (GameTime gameTime)
		{
		}


		/// <summary>
		/// Render the shadow map texture to the screen
		/// </summary>
		void DrawDebugRenderTargetsToScreen ()
		{
			int width = 200;
			int height = 128;
			int padding = 2;
			string TitleText = "Title";

			vxEngine.SpriteBatch.Begin (0, BlendState.Opaque, SamplerState.PointClamp, null, null);

			vxEngine.SpriteBatch.Draw (new Texture2D (vxEngine.GraphicsDevice, 1, 1),
				new Rectangle (0, 0, vxEngine.GraphicsDevice.Viewport.Width, height + vxEngine.Assets.Fonts.DebugFont.LineSpacing + 2 * padding), Color.DarkGray * 0.5f);
			
			//
			//Plain Render
			//
			TitleText = "Colour Map";
			vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_ColourMap, new Rectangle (0, 0, width, height), Color.White);
			vxEngine.SpriteBatch.DrawString (vxEngine.Assets.Fonts.DebugFont, TitleText,
				new Vector2 (width / 2 - vxEngine.Assets.Fonts.DebugFont.MeasureString (TitleText).X / 2, height + padding), Color.LightGray);

			//
			//Normal Map
			//
			TitleText = "Normal Map";
			vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_NormalMap, new Rectangle (width, 0, width, height), Color.White);
			vxEngine.SpriteBatch.DrawString (vxEngine.Assets.Fonts.DebugFont, TitleText,
				new Vector2 (width * 3 / 2 - vxEngine.Assets.Fonts.DebugFont.MeasureString (TitleText).X / 2, height + padding), Color.LightGray);


			//
			//Depth Map
			//
			TitleText = "Depth Map";
			vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_DepthMap, new Rectangle (2 * width, 0, width, height), Color.White);
			vxEngine.SpriteBatch.DrawString (vxEngine.Assets.Fonts.DebugFont, TitleText,
				new Vector2 (width * 5 / 2 - vxEngine.Assets.Fonts.DebugFont.MeasureString (TitleText).X / 2, height + padding), Color.LightGray);


			//
			//Distortion Map
			//
			TitleText = "Edge Detection";

			vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_EdgeDetected, new Rectangle (3 * width, 0, width, height), Color.White);
			vxEngine.SpriteBatch.DrawString (vxEngine.Assets.Fonts.DebugFont, TitleText,
				new Vector2 (width * 7 / 2 - vxEngine.Assets.Fonts.DebugFont.MeasureString (TitleText).X / 2, height + padding), Color.LightGray);

			//
			//Blur Mask
			//
			TitleText = "Shadow Map";

			vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_ShadowMap, new Rectangle (4 * width, 0, width, height), Color.White);
			vxEngine.SpriteBatch.DrawString (vxEngine.Assets.Fonts.DebugFont, TitleText,
				new Vector2 (width * 9 / 2 - vxEngine.Assets.Fonts.DebugFont.MeasureString (TitleText).X / 2, height + padding), Color.LightGray);

			//
			//Distortion Map
			//
			TitleText = "Mask Map";

			vxEngine.SpriteBatch.Draw (vxEngine.Renderer.RT_GodRaysScene, new Rectangle (5 * width, 0, width, height), Color.White);
			vxEngine.SpriteBatch.DrawString (vxEngine.Assets.Fonts.DebugFont, TitleText,
				new Vector2 (width * 11 / 2 - vxEngine.Assets.Fonts.DebugFont.MeasureString (TitleText).X / 2, height + padding), Color.LightGray);


			vxEngine.SpriteBatch.End ();

			//vxEngine.Game.GraphicsDevice.Textures[0] = null;
			//vxEngine.Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

		}

#endregion
	}
}
#endif