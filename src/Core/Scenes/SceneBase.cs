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

using vxVertices.Core;
using vxVertices.Mathematics;
using vxVertices.Graphics;
using vxVertices.Utilities;


#if VIRTICES_3D
using vxVertices.Audio;
using vxVertices.Core.Particles;
#endif
using vxVertices.GUI;
using vxVertices.Core.Entities;
using vxVertices.Core.Input;
using vxVertices.Screens.Menus;

#endregion

namespace vxVertices.Core.Scenes
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class vxSceneBase : GameScreen
    {
        #region Fields

		/// <summary>
		/// The content manager for this scene
		/// </summary>
        public ContentManager content;

		/// <summary>
		/// The type of the game, whether its a Local game or Networked.
		/// </summary>
        public vxEnumGameType GameType = vxEnumGameType.Local;

		/// <summary>
		/// The Sandbox GUI Manager
		/// </summary>
		public vxGuiManager GUIManager { get; set; }

        /// <summary>
        /// Connected Server IP
        /// </summary>
        public string IPAddress = "127.0.0.1";

        /// <summary>
        /// This Systems Current Public IP
        /// </summary>
        public string ThisPublicIp = "0.0.0.0";

        /// <summary>
        /// Connection Port
        /// </summary>
        public int Port = 14242;

		//TODO: Should this be kept?
		/// <summary>
		/// Should the GUI be shown?
		/// </summary>
		public bool ShowGUI = true;

        /// <summary>
        /// Graphics Device Manger for accessing Graphics info
        /// </summary>
        public GraphicsDeviceManager mGraphicsManager { get; set; }

        /// <summary>
        /// Main Entity Collection for Draw and Update
        /// </summary>
        public List<vxEntity> Entities = new List<vxEntity>();

		#if VIRTICES_3D
        /// <summary>
        /// vxEngine Partile System, Note this is managed seperate from the Entity List.
        /// </summary>
		public vxParticleSystem3D ParticleSystem = new vxParticleSystem3D();
		#endif

		/// <summary>
		/// This is the Pause Alpha Amount based off of the poisition the screen is in terms of
		/// transitioning too a new screen.
		/// </summary>
        float pauseAlpha;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is pausable.
		/// </summary>
		/// <value><c>true</c> if this instance is pausable; otherwise, <c>false</c>.</value>
		public bool IsPausable
		{
			get{ return _isPausable; }
			set { _isPausable = value; }
		}
		bool _isPausable = true;

		public Texture2D LastFrame;

        #endregion
        
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSceneBase()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }

		/// <summary>
		/// Initialises the level.
		/// </summary>
        public virtual void InitialiseLevel()
        {
            vxEngine.CurrentGameplayScreen = this;

			GUIManager = new vxGuiManager();

            if (content == null)
                content = new ContentManager(vxEngine.Game.Services, "Content");
			
			mGraphicsManager = vxEngine.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;


			LastFrame = new Texture2D (vxEngine.GraphicsDevice,
				vxEngine.GraphicsDevice.PresentationParameters.BackBufferWidth,
				vxEngine.GraphicsDevice.PresentationParameters.BackBufferHeight);

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {           
			

        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
			if(content != null)
            	content.Unload();
        }


        #endregion

        #region Update and Draw

		/// <summary>
		/// Updates the state of the game. This method checks the GameScreen.IsActive
		/// property, so the game will stop updating when the pause menu is active,
		/// or if you tab away to a different application.
		/// </summary>
		public override void Update(GameTime gameTime, bool otherScreenHasFocus,
			bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

			GUIManager.Update(vxEngine);

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen)
				pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
			else
				pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

			if (IsActive || _isPausable == false)
			{
				//UpdateScene (gameTime, otherScreenHasFocus,coveredByOtherScreen);
			}
		}

		/// <summary>
		/// Main Game Update Loop which is accessed outside of the vxEngine
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="otherScreenHasFocus"></param>
		/// <param name="coveredByOtherScreen"></param>
		public virtual void UpdateScene(GameTime gameTime, bool otherScreenHasFocus,
			bool coveredByOtherScreen)
		{

		}



		/// <summary>
		/// Lets the game respond to player input. Unlike the Update method,
		/// this will only be called when the gameplay screen is active.
		/// </summary>
		public override void HandleInput(vxInputManager input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			// Look up inputs for the active player profile.
			int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.KeyboardState;// input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.GamePadState;// input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected;

            /*            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];*/
			
            //if (input.IsPauseGame() || gamePadDisconnected)
            if (input.IsPauseGame())
            {
                ShowPauseScreen();
            }
			else
			{
				HandleInputBase (input);
			}
		}

		/// <summary>
		/// Handles the input base.
		/// </summary>
		/// <param name="input">Input.</param>
		public virtual void HandleInputBase(vxInputManager input)
		{

		}



        /// <summary>
        /// This Method Loads the Engine Base Pause Screen (PauseMenuScreen()), but 
        /// more items might be needed to be added. Override to
        /// load your own Pause Screen.
        /// </summary>
        /// <example> 
        /// This sample shows how to override the <see cref="ShowPauseScreen"/> method. 'MyGamesCustomPauseScreen()' inheirts
        /// from the <see cref="vxVertices.Screens.Menus.MenuScreen"/> Class.
        /// <code>
        /// //This Allows to show your own custom pause screen.
        /// public override void ShowPauseScreen()
        /// {
        ///     vxEngine.AddScreen(new MyGamesCustomPauseScreen(), ControllingPlayer);
        /// }
        /// </code>
        /// </example>
        public virtual void ShowPauseScreen()
        {
            vxEngine.AddScreen(new PauseMenuScreen(), ControllingPlayer);
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
		{
			// This game has a blue background. Why? Because!
			vxEngine.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

			// Our player and enemy are both actually just text strings.
			SpriteBatch spriteBatch = vxEngine.SpriteBatch;

			DrawScene (gameTime);

			if(ShowGUI)
				GUIManager.Draw(vxEngine);

			//vxEngine._input.Draw ();

			LastFrame = (Texture2D)vxEngine.GraphicsDevice.Textures [0];

			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

				vxEngine.FadeBackBufferToBlack(alpha);
			}
		}

		/// <summary>
		/// Draws the scene.
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		public virtual void DrawScene(GameTime gameTime)
		{

		}

        #endregion
    }
}
