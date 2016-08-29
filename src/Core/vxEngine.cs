#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Core.Debug;
using Virtex.Lib.Vrtc.Core.Settings;
using Virtex.Lib.Vrtc.Core.ContentManagement;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Themes;
using Virtex.Lib.Vrtc.XNA.ContentManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Virtex.Lib.Vrtc.Localization;

//Internal Network Libraries
using Microsoft.Xna.Framework.Input;
using System.IO.IsolatedStorage;


#if VRTC_INCLDLIB_NET
using Virtex.Lib.Vrtc.Network;
using Virtex.Lib.Vrtc.Network.Events;
#endif

//Android Libraries
#if VRTC_PLTFRM_DROID
using Android.Views;
#endif

#endregion

namespace Virtex.Lib.Vrtc.Core
{
	/// <summary>
	/// an enum holding the platform type for this engine build.
	/// </summary>
	public enum vxPlatformType
	{
		XNA,
		DirectX,
		OpenGL,
		Android,
		iOS
	}

	/// <summary>
	/// enum of build configuration types. This can be used instead of compiler flags so
	/// to allows for special debug code that can still be run in a release version.
	/// </summary>
	public enum vxBuildConfigType
	{
		Debug,
		Release
	}

	/// <summary>
	/// The vxEngine is a component which manages one or more vxGameBaseScreen
	/// instances. It maintains a stack of screens, calls their Update and Draw
	/// methods at the appropriate times, and automatically routes input to the
	/// topmost active screen.
	/// </summary>
	public partial class vxEngine : DrawableGameComponent
	{
		#region Fields

		/// <summary>
		/// Gets or sets the name of the game, this needs to be set by each program which uses this library.
		/// </summary>
		/// <value>The name of the game.</value>
		public string GameName {
			get { return _gameName; }
			set { _gameName = value; }
		}

		private string _gameName = "default_gamename";


		/// <summary>
		/// Gets or sets the graphics settings manager.
		/// </summary>
		/// <value>The graphics settings manager.</value>
		public vxGraphicsSettingsManager GraphicsSettingsManager { get; set; }

		#if VIRTICES_2D
		/// <summary>
		/// The asset creator is used in 2D Games to create stand in Textures which match(simple) 2D Physics Bodies.
		/// </summary>
		/// <value>The asset creator.</value>
		public vxAssetCreator2D AssetCreator { get; private set; }

		/// <summary>
		/// Line Batch Manager which draw's a number of 2D Lines to the screen.
		/// </summary>
		/// <value>The line batch.</value>
		public vxLineBatch LineBatch { get; private set; }
		#endif
		/// <summary>
		/// Boolean value of whether or not to allow the resolution to be changeable.
		/// </summary>
		public bool LoadResolution = true;

		/// <summary>
		/// Player Profile Object which stores all settings.
		/// </summary>
		public PlayerProfile Profile = new PlayerProfile ();

		/// <summary>
		/// Publicly Accessible value indicating the Save Icon Alpha.
		/// </summary>
		public float Float_DrawSaveAlpha = 0;


		/// <summary>
		/// Engine Content Manager for specific Virtices Engine fun.
		/// </summary>
		public vxContentManager vxContentManager {
			get { return _contentManager; }
			set { _contentManager = value; }
		}

		private vxContentManager _contentManager;

		/// <summary>
		/// Virtex vxEngine Debug Class
		/// </summary>
		public vxDebugSystem DebugSystem {
			get { return _debugSystem; }
			set { _debugSystem = value; }
		}

		private vxDebugSystem _debugSystem;

		/// <summary>
		/// Gets or sets the game version.
		/// </summary>
		/// <value>The game version.</value>
		public string GameVersion {
			get { return _gameVersion; }
			set { _gameVersion = value; }
		}

		string _gameVersion = "v. 0.0.0.0";

		/// <summary>
		/// Gets or sets the vxEngine version.
		/// </summary>
		/// <value>The vxEngine version.</value>
		public string EngineVersion {
			get { return _engineVersion; }
			set { _engineVersion = value; }
		}

		string _engineVersion = "v. 0.0.0.0";



  
		/// <summary>
		/// Gets or sets the screen shot variable. This is kept here to be globally accessible.
		/// </summary>
		/// <value>The screen shot.</value>
		public Texture2D ScreenShot {
			get { return _currentScreenshot; }
			set { _currentScreenshot = value; }
		}

		Texture2D _currentScreenshot;


		public Color FadeToBackBufferColor = Color.Black;
		public Color LoadingScreenBackColor = Color.Black;
		public Color LoadingScreenTextColor = Color.White;

		/// <summary>
		/// The Current Gameplay Screen that is being run.
		/// </summary>
		public vxSceneBase CurrentGameplayScreen;




		#if VIRTICES_2D
		/// <summary>
		/// Readonly Value of the current 2D scene base class.
		/// </summary>
		/// <value>Returns the current 2D scene.</value>
		public vxScene2D Current2DSceneBase {
			get{ return (vxScene2D)(this.CurrentGameplayScreen); }
		}
#endif

		#if VIRTICES_3D
		/// <summary>
		/// Readonly Value of the current 3D scene base class.
		/// </summary>
		/// <value>Returns the current 3D scene.</value>
		public vxScene3D Current3DSceneBase {
			get { return (vxScene3D)(this.CurrentGameplayScreen); }
		}
		#endif
		/// <summary>
		/// Tells whether or not the Game has loaded it's Specific Assets.
		/// </summary>
		public bool HasContentBeenLoaded = false;

		/// <summary>
		/// Bool value letting the Renderer know if the render targets need to be resized.
		/// </summary>
		public bool IsBackBufferInvalidated = false;

		/// <summary>
		/// Gets the engine content manager.
		/// </summary>
		/// <value>The engine content manager.</value>
		public ContentManager EngineContentManager {
			get { return _engineContentManager; }
		}

		private ContentManager _engineContentManager;

		#if VIRTICES_3D
		/// <summary>
		/// Virtex Render System
		/// </summary>
		public vxRenderer Renderer {
			get { return _renderer; }
			set { _renderer = value; }
		}

		private vxRenderer _renderer;
		#endif

		/// <summary>
		/// Assets within the vxEngine
		/// </summary>
		public Assets Assets { get; set; }

		/// <summary>
		/// Gets or sets the vxGUItheme use by this game.
		/// </summary>
		public vxGUITheme vxGUITheme { get; set; }


		/// <summary>
		/// Screen List
		/// </summary>
		List<vxGameBaseScreen> screens = new List<vxGameBaseScreen> ();

		/// <summary>
		/// Screens to Update List
		/// </summary>
		List<vxGameBaseScreen> screensToUpdate = new List<vxGameBaseScreen> ();


		/// <summary>
		/// The Game's Input Manager.
		/// </summary>
		public vxInputManager InputManager {
			get { return _inputManager; }
			set { _inputManager = value; }
		}

		private vxInputManager _inputManager;


		//TODO: Add Too Asset's Folder
		/// <summary>
		/// Globally Accessilble Working Plane Model for Sandbox Working Plane
		/// </summary>
		public vxModel Model_Sandbox_WorkingPlane;

		/// <summary>
		/// Boolean to check if the level has been initialised yet.
		/// </summary>
		public bool isInitialized;

		/// <summary>
		/// Gets or sets the splash screen.
		/// </summary>
		/// <value>The splash screen.</value>
		public Texture2D SplashScreen { get; set; }

		/// <summary>
		/// The mouse click position.
		/// </summary>
		public Vector2 Mouse_ClickPos = new Vector2 ();

		#endregion


		#if VRTC_PLTFRM_DROID
        public Android.App.Activity Activity;
#endif

		/// <summary>
		/// A List Containing all Language Packs for this 
		/// </summary>
		public List<vxLanguagePackBase> Languages { get; internal set; }

		/// <summary>
		/// The Currently Selected Language
		/// </summary>
		public vxLanguagePackBase Language { get; set; }


		#region Properties


		/// <summary>
		/// A default SpriteBatch shared by all the screens. This saves
		/// each screen having to bother creating their own local instance.
		/// </summary>
		public SpriteBatch SpriteBatch {
			get { return _spriteBatch; }
		}

		SpriteBatch _spriteBatch;

		/// <summary>
		/// A default font shared by all the screens. This saves
		/// each screen having to bother loading their own local copy.
		/// </summary>
		public SpriteFont Font {
			get { return this.Assets.Fonts.MenuFont; }
		}


		/// <summary>
		/// If true, the manager prints out a list of all the screens
		/// each time it is updated. This can be useful for making sure
		/// everything is being added and removed at the right times.
		/// </summary>
		public bool TraceEnabled {
			get { return _traceEnabled; }
			set { _traceEnabled = value; }
		}

		bool _traceEnabled;

		/// <summary>
		/// Gets the type of the build config. This is set internally but can be changed by launching with specific launch options.
		/// </summary>
		/// <value>The type of the build config.</value>
		public vxBuildConfigType BuildConfigType {
			get;
			internal set;
		}

		/// <summary>
		/// Gets the type of the platform the engine has been compiled for.
		/// </summary>
		/// <value>The type of the platform.</value>
		public vxPlatformType PlatformType {
			get {
				//Set Location of Content Specific too Platform
				#if VRTC_PLTFRM_XNA
				return vxPlatformType.XNA;
				#elif VRTC_PLTFRM_GL
				return vxPlatformType.OpenGL;
				#elif VRTC_PLTFRM_DROID
				return vxPlatformType.Android;
				#elif VRTC_PLTFRM_iOS
				return vxPlatformType.iOS;
				#endif
			}
		}



		#endregion

		#region Initialization


		/// <summary>
		/// Constructs a new screen manager component.
		/// </summary>
		public vxEngine (Game game, string GameName)
			: base (game)
		{
			this.GameName = GameName;

			this.Game.IsMouseVisible = true;
		}

		bool isInitialised = false;

		/// <summary>
		/// Initializes the screen manager component.
		/// </summary>
		public override void Initialize ()
		{
			if (isInitialised == false) {
				isInitialised = true;
				base.Initialize ();

				isInitialised = true;
			}
		}

		/// <summary>
		/// Sets the build config.
		/// </summary>
		/// <param name="configType">Config type.</param>
		internal void SetBuildConfig (vxBuildConfigType configType)
		{
			vxConsole.WriteLine ("Setting Build Config too: " + configType.ToString ());
			this.BuildConfigType = configType;
		}


		/// <summary>
		/// Load your graphics content.
		/// </summary>
		protected override void LoadContent ()
		{
			try {

				#if DEBUG
				BuildConfigType = vxBuildConfigType.Debug;
				#else
				BuildConfigType = vxBuildConfigType.Release;
				#endif


				vxConsole.WriteLine ("");
				vxConsole.WriteLine ("");
				vxConsole.WriteLine ("");
				vxConsole.WriteLine ("Starting Vertices Engine");
				vxEnviroment.Initialize (this);


				vxConsole.WriteLine ("Checking Directories");

				//The Android System uses the Isolated Storage for it's GameSaves
				#if VRTC_PLTFRM_DROID

			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
			IsolatedStorageScope.Assembly, null, null);

			string settings = vxEnviroment.GetVar(vxEnumEnvVarType.PATH_SETTINGS).Value.ToString();
			string sandbox = vxEnviroment.GetVar(vxEnumEnvVarType.PATH_SANDBOX).Value.ToString();

			//First Check, if the Profiles Directory Doesn't Exist, Create It
			if (isoStore.DirectoryExists (settings) == false)
			isoStore.CreateDirectory (settings);

			//First Check, if the Sandbox Directory Doesn't Exist, Create It
			if (isoStore.DirectoryExists(sandbox) == false)
			isoStore.CreateDirectory(sandbox);

			//First Check, if the Temp Directory Doesn't Exist, Create It
			if (isoStore.DirectoryExists ("Temp/Settings") == false)
			isoStore.CreateDirectory ("Temp/Settings");

				#else

				string settings = vxEnviroment.GetVar (vxEnumEnvVarType.PATH_SETTINGS).Value.ToString ();
				string sandbox = vxEnviroment.GetVar (vxEnumEnvVarType.PATH_SANDBOX).Value.ToString ();

				//First Check, if the Profiles Directory Doesn't Exist, Create It
				if (Directory.Exists (settings) == false)
					Directory.CreateDirectory (settings);

				//First Check, if the Sandbox Directory Doesn't Exist, Create It
				if (Directory.Exists (sandbox) == false)
					Directory.CreateDirectory (sandbox);

				//First Check, if the Temp Directory Doesn't Exist, Create It
				if (Directory.Exists ("Temp/Settings") == false)
					Directory.CreateDirectory ("Temp/Settings");

				#endif




				vxConsole.WriteLine ("Starting Content Manager");
				// Load content belonging to the screen manager.
				ContentManager content = Game.Content;
				_engineContentManager = new ContentManager (Game.Services);

				// Initialise the Engine Speciality Content Manager.
				_contentManager = new vxContentManager (this);

				//Set Location of Content Specific too Platform
				_engineContentManager.RootDirectory = vxEnviroment.GetVar (vxEnumEnvVarType.PATH_ENGINE_CONTENT).Value.ToString ();
         
#if VRTC_PLTFRM_DROID
            Game.Activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
            Game.Activity.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
#endif


#if VIRTICES_2D
            LineBatch = new vxLineBatch (GraphicsDevice);

			AssetCreator = new vxAssetCreator2D (GraphicsDevice);
			AssetCreator.LoadContent (_engineContentManager);
#endif
				_inputManager = new vxInputManager (this);
				_inputManager.LoadContent ();
				_inputManager.ShowCursor = true;

				_spriteBatch = new SpriteBatch (GraphicsDevice);

				//Initialise vxEngine Assets
				Assets = new Assets (this);

				vxGUITheme = new vxGUITheme (this);
				vxGUITheme.Font = this.Assets.Fonts.MenuFont;


				#region Initialise Debug Console

				//Get the vxEngine Version through Reflection
				EngineVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();


				// initialize the debug system with the game and the name of the font 
				// we want to use for the debugging
				_debugSystem = vxDebugSystem.Initialize (this, "Fonts/font_debug");
				_debugSystem.TimeRuler.ShowLog = true;

				//Initialise the Debug Renderer
				vxDebugShapeRenderer.Initialize (GraphicsDevice);
				vxConsole.Initialize (this);


				//Now Setup Settings Managers
				GraphicsSettingsManager = new vxGraphicsSettingsManager (this);

				#endregion


#if !VRTC_PLTFRM_DROID
				Model_Sandbox_WorkingPlane = this.Assets.Models.UnitPlane;
#endif


#if VRTC_INCLDLIB_NET
			InitialiseMasterServerConnection();

            ServerManager = new vxNetworkServerManager(this, 14242);
            ClientManager = new vxNetworkClientManager(this);
#endif

				#region Load Base Language

				Languages = new List<vxLanguagePackBase> ();
				LoadLanguagePacks ();

				//Default is English
				this.Language = Languages [0];

				#endregion

#if VIRTICES_3D
				//Initialise Renderer
				Renderer = new vxRenderer (this);
#endif

				//Load in Profile Data
				Profile.LoadSettings (this);

				//Once all items are Managers and Engine classes are intitalised, then apply any and all passed cmd line arguments
				vxEnviroment.ApplyCommandLineArgs();

				//Set Initial Graphics Settings
				GraphicsSettingsManager.Apply ();


				// Tell each of the screens to load their content.
				foreach (vxGameBaseScreen screen in screens) {
					screen.LoadContent ();
				}
			} catch (Exception ex) {
				vxCrashHandler.Init (this, ex);
			}
		}

		/// <summary>
		/// Loads GUI Theme, override to load a new theme
		/// </summary>
		public virtual void LoadGUITheme ()
		{
			vxConsole.WriteLine ("Loading GUI");

		}

		/// <summary>
		/// Loads the Language Packs. Override this method to add your custom packs.
		/// </summary>
		public virtual void LoadLanguagePacks ()
		{
			vxConsole.WriteLine ("Starting Language Manager");

			Languages.Add (new vxLanguagePackEnglishBase ());
			Languages.Add (new vxLanguagePackFrenchBase ());
			Languages.Add (new vxLanguagePackKoreanBase ());
		}

		/// <summary>
		/// This is the Main Entry point for the game external to the Engine.
		/// </summary>
		public virtual void vxEngineMainEntryPoint ()
		{

		}




		/// <summary>
		/// Loads Global Content Specific for the Game, Accessed OutSide of vxEngine
		/// </summary>
		/// <param name="content"></param>
		public virtual void LoadGlobalContent (ContentManager content)
		{
			//Once the Main items are loaded, load the GUI
			LoadGUITheme ();
		}


		/// <summary>
		/// Unload your graphics content.
		/// </summary>
		protected override void UnloadContent ()
		{
			ClearTempDirectory ();

			// Tell each of the screens to unload their content.
			foreach (vxGameBaseScreen screen in screens) {
				screen.UnloadContent ();
			}
		}


		#endregion

		#region Update and Draw


		/// <summary>
		/// Allows each screen to run logic.
		/// </summary>
		public override void Update (GameTime gameTime)
		{
#if !DEBUG
            if (vxCrashHandler.IsInitialised == false) {
				try {
#endif
					Profile.Settings.Audio.Double_SFX_Volume = 0.2f;
					// tell the TimeRuler that we're starting a new frame. you always want
					// to call this at the start of Update
					_debugSystem.TimeRuler.StartFrame ();

					// Start measuring time for "Update".
					_debugSystem.TimeRuler.BeginMark ("Update", Color.Red);

					//If the Debug Console is Open, Then don't update or take input
					if (!_debugSystem.DebugCommandUI.Focused) {
						// Read the keyboard and gamepad.
						_inputManager.Update (gameTime);

						// Make a copy of the master screen list, to avoid confusion if
						// the process of updating one screen adds or removes others.
						screensToUpdate.Clear ();

						foreach (vxGameBaseScreen screen in screens)
							screensToUpdate.Add (screen);

						bool otherScreenHasFocus = !Game.IsActive;
						bool coveredByOtherScreen = false;

						// Loop as long as there are screens waiting to be updated.
						while (screensToUpdate.Count > 0) {
							// Pop the topmost screen off the waiting list.
							vxGameBaseScreen screen = screensToUpdate [screensToUpdate.Count - 1];

							screensToUpdate.RemoveAt (screensToUpdate.Count - 1);

							// Update the screen.
							screen.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);

							if (screen.ScreenState == ScreenState.TransitionOn ||
							    screen.ScreenState == ScreenState.Active) {
								// If this is the first active screen we came across,
								// give it a chance to handle input.
								if (!otherScreenHasFocus) {
									screen.HandleInput (_inputManager);

									otherScreenHasFocus = true;
								}

								// If this is an active non-popup, inform any subsequent
								// screens that they are covered by it.
								if (!screen.IsPopup)
									coveredByOtherScreen = true;
							}
						}

						// Print debug trace?
						if (_traceEnabled)
							TraceScreens ();
					}

					// Stop measuring time for "Update".
					_debugSystem.TimeRuler.EndMark ("Update");

#if !DEBUG
				} catch (Exception ex) {
					vxCrashHandler.Init (this, ex);
					Crashed ();
				}
			}
#endif
        }

        /// <summary>
        /// Function for what the engine to handle if a crash is set off internally
        /// </summary>
        void Crashed ()
		{
			if (_debugSystem != null) {

				if (_debugSystem.TimeRuler != null)
					_debugSystem.TimeRuler.Visible = false;
			}
		}

		/// <summary>
		/// Prints a list of all the screens, for debugging.
		/// </summary>
		void TraceScreens ()
		{
			List<string> screenNames = new List<string> ();

			foreach (vxGameBaseScreen screen in screens)
				screenNames.Add (screen.GetType ().Name);
		}


		/// <summary>
		/// Tells each screen to draw itself.
		/// </summary>
		public override void Draw (GameTime gameTime)
		{
#if !DEBUG
            if (vxCrashHandler.IsInitialised == false) {
				try {
#endif
					// Start measuring time for "Draw".
					_debugSystem.TimeRuler.BeginMark ("Draw", Color.Yellow);


					foreach (vxGameBaseScreen screen in screens) {
						if (screen.ScreenState == ScreenState.Hidden)
							continue;

						screen.Draw (gameTime);
					}

					_inputManager.Draw ();
#if VRTC_INCLDLIB_NET && DEBUG
            DrawNetworkGameConnectionInfo();
#endif

					vxConsole.Draw ();
					// Stop measuring time for "Draw".
					_debugSystem.TimeRuler.EndMark ("Draw");

					_debugSystem.TimeRuler.DrawGraph ();
#if !DEBUG
				} catch (Exception ex) {
					vxCrashHandler.Init (this, ex);
					Crashed ();
				}


				//If the Crash Handler has been initialised, then a crash has occured somewhere and it'll fall to this.
			} else {
				vxCrashHandler.Draw (gameTime);
			}
#endif
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen (vxGameBaseScreen screen, PlayerIndex? controllingPlayer)
		{
			//Console.WriteLine ("Adding Screen: " + screen.ToString ());

			screen.IsInitialised = true;
			screen.ControllingPlayer = controllingPlayer;
			screen.vxEngine = this;
			screen.IsExiting = false;

			// If we have a graphics device, tell the screen to load content.
			if (isInitialized) {
				screen.LoadContent ();
			}

			screens.Add (screen);

		}


		/// <summary>
		/// Removes a screen from the screen manager. You should normally
		/// use vxGameBaseScreen.ExitScreen instead of calling this directly, so
		/// the screen can gradually transition off rather than just being
		/// instantly removed.
		/// </summary>
		public void RemoveScreen (vxGameBaseScreen screen)
		{
			//Console.WriteLine ("Removing Screen: " + screen.ToString ());


			// If we have a graphics device, tell the screen to unload content.
			if (isInitialized) {
				screen.UnloadContent ();
			}

			screens.Remove (screen);
			screensToUpdate.Remove (screen);

			// if there is a screen still in the manager, update TouchPanel
			// to respond to gestures that screen is interested in.
			if (screens.Count > 0) {
			}
		}


		/// <summary>
		/// Expose an array holding all the screens. We return a copy rather
		/// than the real master list, because screens should only ever be added
		/// or removed using the AddScreen and RemoveScreen methods.
		/// </summary>
		public vxGameBaseScreen[] GetScreens ()
		{
			return screens.ToArray ();
		}


		/// <summary>
		/// Helper draws a translucent black fullscreen sprite, used for fading
		/// screens in and out, and for darkening the background behind popups.
		/// </summary>
		public void FadeBackBufferToBlack (float alpha)
		{
			Viewport viewport = GraphicsDevice.Viewport;

			_spriteBatch.Begin ();

			_spriteBatch.Draw (this.Assets.Textures.Blank,
				new Rectangle (0, 0, viewport.Width, viewport.Height),
				FadeToBackBufferColor * alpha);
			
			_spriteBatch.End ();
		}


#endregion


		/// <summary>
		/// Clears the temp directory.
		/// </summary>
		public void ClearTempDirectory ()
		{
			//Clear Out the Temp Directory
			DirectoryInfo tempDirectory = new DirectoryInfo ("Temp");

			foreach (FileInfo file in tempDirectory.GetFiles()) {
				file.Delete ();
			}
			foreach (DirectoryInfo dir in tempDirectory.GetDirectories()) {
				try {
					dir.Delete (true);
				} catch {
				}
			}
		}
	}
}