#if VIRTICES_2D
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using vxVertices.Core;
using vxVertices.Physics.Farseer.Dynamics;
using vxVertices.Physics.Farseer.Factories;
using vxVertices.Physics.Farseer;
using System.Collections.Generic;
using vxVertices.Utilities;
using vxVertices.Core.Entities;
using vxVertices.Core.Input;
using vxVertices.Core.Cameras;

namespace vxVertices.Core.Scenes
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class vxScene2D : vxSceneBase
	{
		// Resources for drawing.
		//private GraphicsDeviceManager graphics;
		public SpriteBatch spriteBatch;

		public List<vxEntity2D> List_vxEntity2D = new List<vxEntity2D> ();

		public Vector2 baseScreenSize = new Vector2(800, 480);

		/// <summary>
		/// Global Transformation for Screen Scaling
		/// </summary>
		public Matrix globalTransformation;

		/// <summary>
		/// Physics World Space
		/// </summary>
		public World World;


		public vxCamera2D Camera;

		// We store our input states so that we only poll once per frame, 
		// then we use the same input state wherever needed
		public GamePadState gamePadState;
		public KeyboardState keyboardState;
		public TouchCollection touchState;
		public AccelerometerState accelerometerState;

		public VirtualGamePad virtualGamePad;

		// Global content.
		public SpriteFont hudFont;

		GameTime gameTimeBase;

		public vxScene2D ()
		{

		}

		public override void InitialiseLevel ()
		{
			base.InitialiseLevel ();

			//graphics = new GraphicsDeviceManager (this);
			//vxEngine.Game.Content.RootDirectory = "Content";	

			Accelerometer.Initialize();            

			ConvertUnits.SetDisplayUnitToSimUnitRatio(24f);
			//Initialise Physics vxEngine
			World = new World(Vector2.UnitY*20);

			#if WINDOWS_PHONE
			TargetElapsedTime = TimeSpan.FromTicks(333333);
			#endif
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public override void LoadContent ()
		{			
			InitialiseLevel ();


			Camera = new vxCamera2D(vxEngine.GraphicsDevice);
			Camera.ResetCamera();

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (vxEngine.GraphicsDevice);

			// Load fonts
			//hudFont = vxEngine.Game.Content.Load<SpriteFont>("Fonts/font_gui");


			//Work out how much we need to scale our graphics to fill the screen
			float horScaling = vxEngine.GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
			float verScaling = vxEngine.GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
			Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
			globalTransformation = Matrix.CreateScale(screenScalingFactor);

			//virtualGamePad = new VirtualGamePad(baseScreenSize, globalTransformation, vxEngine.EngineVersion.Content.Load<Texture2D>("Sprites/VirtualControlArrow"));

		}
		public bool AllowCameraInput = true;

        float pauseAlpha;

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public sealed override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                            bool coveredByOtherScreen)
        {//vxEngine.Game.IsFixedTimeStep = false;

            base.Update(gameTime, otherScreenHasFocus, false);

            // GUIManager.Update(vxEngine);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen && IsPausable)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive || IsPausable == false)
            {
                UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus,
			bool coveredByOtherScreen)
		{
			// FOREACH LOOPS DO NOT ALLOW ADDING MORE ADDITIONAL ENTITIES DURING LOOPING, FOR LOOPS GET AROUND THIS
			/* 
			foreach (vxEntity2D entity in List_vxEntity2D)
				entity.Update (gameTime);
			*/
			for (int i = 0; i < List_vxEntity2D.Count; i++)
				List_vxEntity2D [i].Update (gameTime);

			if(AllowCameraInput)
				HandleCamera (vxEngine.InputManager, gameTime);

			Camera.Update(gameTime);

			// variable time step but never less then 30 Hz
			try
			{
			World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 60f)));
			}
			catch(Exception ex) {
				vxConsole.WriteLine ("ERROR IN FARSEER: " + ex.Message);
			}
			gameTimeBase = gameTime;

			base.UpdateScene(gameTime, otherScreenHasFocus,coveredByOtherScreen);
		}


        private void HandleCamera(vxInputManager input, GameTime gameTime)
		{
			Vector2 camMove = Vector2.Zero;

			if (input.KeyboardState.IsKeyDown(Keys.Up))
				camMove.Y -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (input.KeyboardState.IsKeyDown(Keys.Down))
				camMove.Y += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (input.KeyboardState.IsKeyDown(Keys.Left))
				camMove.X -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (input.KeyboardState.IsKeyDown(Keys.Right))
				camMove.X += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (input.KeyboardState.IsKeyDown(Keys.PageUp))
				Camera.Zoom += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
			if (input.KeyboardState.IsKeyDown(Keys.PageDown))
				Camera.Zoom -= 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;


#if !VRTC_PLTFRM_XNA
			//Panning Control
			if (input.MouseState.MiddleButton == ButtonState.Pressed) {
				camMove = -Vector2.Subtract(input.MouseState.Position.ToVector2(), 
					input.PreviousMouseState.Position.ToVector2())/20;
			}
#else
            //Panning Control
            if (input.MouseState.MiddleButton == ButtonState.Pressed)
            {
                camMove = -Vector2.Subtract(new Vector2(input.MouseState.X, input.MouseState.Y),
                    new Vector2(input.PreviousMouseState.X, input.PreviousMouseState.Y)) / 20;
            }
#endif

            float scrollSens = 1;
			if(input.MouseState.ScrollWheelValue - input.PreviousMouseState.ScrollWheelValue > 0)
				Camera.Zoom += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / scrollSens;
			else if(input.MouseState.ScrollWheelValue - input.PreviousMouseState.ScrollWheelValue < 0)
				Camera.Zoom -= 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / scrollSens;

			if (camMove != Vector2.Zero)
				Camera.MoveCamera(camMove);
			if (input.IsNewKeyPress(Keys.Home))
				Camera.ResetCamera();



		}

		/// <summary>
		/// Handles the input base.
		/// </summary>
		/// <param name="input">Input.</param>
        public override void HandleInputBase(vxInputManager input)
		{
			// get all of our input states
			keyboardState = Keyboard.GetState();
			touchState = TouchPanel.GetState();
			//gamePadState = virtualGamePad.GetState(touchState, GamePad.GetState(PlayerIndex.One));
			accelerometerState = Accelerometer.GetState();

			//virtualGamePad.Update(gameTimeBase);

			//			#if !NETFX_CORE
			//			// Exit the game when back is pressed.
			//			if (gamePadState.Buttons.Back == ButtonState.Pressed)
			//				vxEngine.Game.Exit();
			//			#endif

		}

		/// <summary>
		/// Loads the next level.
		/// </summary>
		public virtual void LoadNextLevel()
		{

		}

		/// <summary>
		/// Reloads the current level.
		/// </summary>
		public virtual void ReloadCurrentLevel()
		{

		}

		/// <summary>
		/// Draws the game from background to foreground.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void DrawScene(GameTime gameTime)
		{
			vxEngine.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

			foreach (vxEntity2D entity in List_vxEntity2D)
				entity.Draw ();

			vxEngine.SpriteBatch.End();

			base.DrawScene(gameTime);
		}

		/// <summary>
		/// Draws the hud.
		/// </summary>
		public virtual void DrawHud()
		{


		}

		/// <summary>
		/// Draws the shadowed string.
		/// </summary>
		/// <param name="font">Font.</param>
		/// <param name="value">Value.</param>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		public virtual void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
		{
			spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
			spriteBatch.DrawString(font, value, position, color);
		}
	}
}
#endif