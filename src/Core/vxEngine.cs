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
using Virtex.Lib.Vertices.Core.Input;
using Virtex.Lib.Vertices.Graphics;
using Virtex.Lib.Vertices.Core.Scenes;
using Virtex.Lib.Vertices.Core.Debug;
using Virtex.Lib.Vertices.Core.Settings;
using Virtex.Lib.Vertices.Core.ContentManagement;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Network.Events;
using Virtex.Lib.Vertices.Network;
using Virtex.Lib.Vertices.GUI;
using Virtex.Lib.Vertices.GUI.Themes;
using Virtex.Lib.Vertices.XNA.ContentManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Virtex.Lib.Vertices.Localization;

#if VRTC_PLTFRM_DROID
using Android.Views;
#endif

#endregion

namespace Virtex.Lib.Vertices.Core
{
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

		#if VIRTICES_2D
		/// <summary>
		/// The asset creator is used in 2D Games to create stand in Textures which match(simple) 2D Physics Bodies.
		/// </summary>
		/// <value>The asset creator.</value>
		public AssetCreator2D AssetCreator { get; private set; }

		/// <summary>
		/// Line Batch Manager which draw's a number of 2D Lines to the screen.
		/// </summary>
		/// <value>The line batch.</value>
		public LineBatch LineBatch { get; private set; }
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
        public vxContentManager vxContentManager
        {
            get { return _contentManager; }
            set { _contentManager = value; }
		}
        private vxContentManager _contentManager;

        /// <summary>
        /// Virtex vxEngine Debug Class
        /// </summary>
        public DebugSystem DebugSystem {
			get { return _debugSystem; }
			set { _debugSystem = value; }
		}

		private DebugSystem _debugSystem;

		/// <summary>
		/// Gets or sets the game version.
		/// </summary>
		/// <value>The game version.</value>
		public string GameVersion {
			get { return _gameVersion; }
			set { _gameVersion = _gameName + " - v." + value; }
		}

		string _gameVersion = "v. 0.0.0.0";

		/// <summary>
		/// Gets or sets the vxEngine version.
		/// </summary>
		/// <value>The vxEngine version.</value>
		public string EngineVersion {
			get { return _engineVersion; }
			set { _engineVersion = "Virteces Engine - v." + value; }
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

		/// <summary>
		/// Path to save the Profiles
		/// </summary>
		public string Path_Profiles {

			get {
				string path = "Virtex Edge Design/" + GameName + "/Profiles/";
#if VRTC_PLTFRM_XNA
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + path;
#endif
				return path;
			}

		}

		/// <summary>
		/// Path to save the Sandbox Files
		/// </summary>
		public string Path_Sandbox {
			get {
				string path = "Virtex Edge Design/" + GameName + "/Sandbox/";
#if VRTC_PLTFRM_XNA
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + path;
#endif
				return path;
			}
		}


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
        public vxScene3D Current3DSceneBase
        {
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

		public Texture2D SplashScreen { get; set; }

		//
		//Vector3
		//
		/// <summary>
		/// The mouse click position.
		/// </summary>
		public Vector2 Mouse_ClickPos = new Vector2 ();

#endregion


		/// <summary>
		/// This flag tells the engine whether or not to draw the final result to a master
		/// render target at a set resolution. This master rendertarget is then scaled and 
		/// drawn at the native resolution of what ever screen the device has. This is useful
		/// for Phones and Tablets which have a multitude of different resolutions.
		/// </summary>
		public bool FixedRenderTargetEnabled {
			get { return _fixedRenderTargetEnabled; }
			set { _fixedRenderTargetEnabled = value; }
		}
		bool _fixedRenderTargetEnabled = false;

		/// <summary>
		/// Gets or sets the width of the fixed render target.
		/// </summary>
		/// <value>The width of the fixed render target.</value>
		public int FixedRenderTargetWidth {
			get { return _fixedRenderTargetWidth; }
			set { _fixedRenderTargetWidth = value; }
		}
		int _fixedRenderTargetWidth = 1280;

		/// <summary>
		/// Gets or sets the height of the fixed render target.
		/// </summary>
		/// <value>The height of the fixed render target.</value>
		public int FixedRenderTargetHeight {
			get { return _fixedRenderTargetHeight; }
			set { _fixedRenderTargetHeight = value; }
		}
		int _fixedRenderTargetHeight = 720;

		/// <summary>
		/// Gets or sets the fixed render target.
		/// </summary>
		/// <value>The fixed render target.</value>
		public RenderTarget2D FixedRenderTarget {
			get { return _fixedRenderTarget; }
			set { _fixedRenderTarget = value; }
		}
		RenderTarget2D _fixedRenderTarget;

        /// <summary>
        /// A List Containing all Language Packs for this 
        /// </summary>
        public List<vxLanguagePackBase> Languages { get; internal set; }
        
        /// <summary>
        /// The Currently Selected Language
        /// </summary>
        public vxLanguagePackBase Language { get; set; }

        #region Debug Properties

        /// <summary>
        /// Displays Render Targets for Graphics Debugin
        /// </summary>
        public bool DisplayRenderTargets {
			get { return _displayRenderTargets; }
			set { _displayRenderTargets = value; }
		}

		bool _displayRenderTargets = false;


		/// <summary>
		/// Displays Debug Meshes
		/// </summary>
		public bool DisplayDebugMesh {
			get { return _displayDebugMesh; }
			set { _displayDebugMesh = value; }
		}

		bool _displayDebugMesh = false;

		/// <summary>
		/// Displays Debug Information
		/// </summary>
		public bool DisplayDebugInformation {
			get { return _displayDebugInformation; }
			set { _displayDebugInformation = value; }
		}

		bool _displayDebugInformation = false;

		/// <summary>
		/// Displays Debug Information
		/// </summary>
		public bool ShowInGameDebugWindow {
			get { return _showInGameDebugWindow; }
			set { _showInGameDebugWindow = value; }
		}

		bool _showInGameDebugWindow = false;

#endregion

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


#endregion

#region Initialization


		/// <summary>
		/// Constructs a new screen manager component.
		/// </summary>
		public vxEngine (Game game, string GameName)
			: base (game)
		{
			this.GameName = GameName;

			Game.IsMouseVisible = true;

			//This is just temporary, this is re-loaded for global uses when the vxEngine is Initialised.
			string gameVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();

#if !VRTC_PLTFRM_DROID
			try {
				Console.Title = "VIRTICES ENGINE DEBUG CONSOLE v." + gameVersion;
			} catch {
			}
#endif
			vxConsole.WriteLine ("///////////////////////////////////////////////////////////////////////");
			vxConsole.WriteLine (string.Format ("          Vertices Engine - v.{0}", gameVersion));
			vxConsole.WriteLine (string.Format ("          Starting Game   - {0}", GameName));
			vxConsole.WriteLine ("///////////////////////////////////////////////////////////////////////");


        }


		/// <summary>
		/// Initializes the screen manager component.
		/// </summary>
		public override void Initialize ()
		{

			base.Initialize ();

			//Get the vxEngine Version through Reflection
			EngineVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();


            // initialize the debug system with the game and the name of the font 
            // we want to use for the debugging
            _debugSystem = DebugSystem.Initialize (this, "Fonts/font_debug");
			_debugSystem.TimeRuler.ShowLog = true;

			// Register's Command too Show Render Targets on the Screen
			_debugSystem.DebugCommandUI.RegisterCommand (
				"rt",              // Name of command
				"Displays Render Targets",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					DisplayRenderTargets = !DisplayRenderTargets;
				});

			// Register's Command too Show Render Targets on the Screen
			_debugSystem.DebugCommandUI.RegisterCommand (
				"dm",              // Name of command
				"Displays Debug Mesh's",     // Description of command
				(IDebugCommandHost host, string command, IList<string> args) => DisplayDebugMesh = !DisplayDebugMesh);

			// Register's Command too Show Render Targets on the Screen
			_debugSystem.DebugCommandUI.RegisterCommand (
				"bm",              // Name of command
				"Toggle Bloom",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					//this.Profile.Settings.Graphics.Bool_DoBloom = !this.Profile.Settings.Graphics.Bool_DoBloom;
				});

			// Register's Command too Show Render Targets on the Screen
			_debugSystem.DebugCommandUI.RegisterCommand (
				"cn",              // Name of command
				"Toggle In Game Console",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					ShowInGameDebugWindow = !ShowInGameDebugWindow;
				});

			//Initialise the Debug Renderer
			vxDebugShapeRenderer.Initialize (GraphicsDevice);


#if !VRTC_PLTFRM_DROID

			//First Check, if the Profiles Directory Doesn't Exist, Create It
			if (Directory.Exists (Path_Profiles) == false)
				Directory.CreateDirectory (Path_Profiles);

			//First Check, if the Sandbox Directory Doesn't Exist, Create It
			if (Directory.Exists (Path_Sandbox) == false)
				Directory.CreateDirectory (Path_Sandbox);

			//First Check, if the Temp Directory Doesn't Exist, Create It
			if (Directory.Exists ("Temp/Settings") == false)
				Directory.CreateDirectory ("Temp/Settings");

#endif
            vxConsole.vxEngine = this;


#if VRTC_PLTFRM_XNA
			vxConsole.WriteLine("Starting Vertices Engine with XNA Backend...");
#else
            vxConsole.WriteLine ("Starting Vertices Engine with MonoGame Backend...");
#endif



#if VRTC_INCLDLIB_NET
			InitialiseMasterServerConnection();
#endif

			vxConsole.WriteLine ("Starting Content Manager...");
		}



		/// <summary>
		/// Load your graphics content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Load content belonging to the screen manager.
			ContentManager content = Game.Content;
			_engineContentManager = new ContentManager (Game.Services);

            // Initialise the Engine Speciality Content Manager.
            _contentManager = new vxContentManager(this);

            //string contentLocationTag = "Virtex.Lib.Vertices.XNA.Content";

            //Set Location of Content Specific too Platform
#if VRTC_PLTFRM_XNA
            _engineContentManager.RootDirectory = "Virtex.Lib.Vertices.Core.XNA.Content";

#elif VRTC_PLTFRM_GL
			_engineContentManager.RootDirectory = "Vertices.Engine.Content/Compiled.WindowsGL";
            
#elif VRTC_PLTFRM_DROID
            _engineContentManager.RootDirectory = "Vertices.Engine.Content/Compiled.Android";
            
            Game.Activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
            Game.Activity.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

#endif


#if VIRTICES_2D
            LineBatch = new LineBatch (GraphicsDevice);

			AssetCreator = new AssetCreator2D (GraphicsDevice);
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


#if !VRTC_PLTFRM_DROID
			Model_Sandbox_WorkingPlane = this.Assets.Models.UnitPlane;
#endif


#if VRTC_INCLDLIB_NET
            ServerManager = new vxNetworkServerManager(this, 14242);
            ClientManager = new vxNetworkClientManager(this);
#endif

            #region Load Base Language

            Languages = new List<vxLanguagePackBase>();
            LoadLanguagePacks();

            //Default is English
            this.Language = Languages[0];

            #endregion

            //Load in Profile Data
            vxConsole.WriteLine ("Loading Settings....");
			Profile.LoadSettings (this);
			vxConsole.WriteLine ("\t\t\tDone!");

			//Set Initial Graphics Settings
			SetGraphicsSettings ();

			FixedRenderTarget = new RenderTarget2D (this.GraphicsDevice,
				this.FixedRenderTargetWidth,
				this.FixedRenderTargetHeight,
				false, SurfaceFormat.Color, DepthFormat.None, 
				this.GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);


			GraphicsDeviceManager graphics = Game.Services.GetService (typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
			//graphics.SynchronizeWithVerticalRetrace = false;
			Game.IsFixedTimeStep = false;
			//graphics.PreferMultiSampling = true;
			//graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
			graphics.ApplyChanges ();

            //this.Profile.Settings.Graphics.Bloom = vxEnumQuality.None;
            //this.Profile.Settings.Graphics.DepthOfField = vxEnumQuality.None;// = false;
            //this.Profile.Settings.Graphics.Bool_DoEdgeDetection = true;
#if VIRTICES_3D
			Renderer = new vxRenderer (this);
#endif


            // Tell each of the screens to load their content.
            foreach (vxGameBaseScreen screen in screens) {
				screen.LoadContent ();
			}
		}

		/// <summary>
		/// Loads GUI Theme, override to load a new theme
		/// </summary>
		public virtual void LoadGUITheme ()
		{
			vxConsole.WriteLine ("     Loading GUI...");

		}

        /// <summary>
        /// Loads the Language Packs. Override this method to add your custom packs.
        /// </summary>
        public virtual void LoadLanguagePacks()
        {
            vxConsole.WriteLine("     Loading Language Packs...");

            Languages.Add(new vxLanguagePackEnglishBase());
            Languages.Add(new vxLanguagePackFrenchBase());
            Languages.Add(new vxLanguagePackKoreanBase());
        }

		/// <summary>
		/// This is the Main Entry point for the game external to the Engine.
		/// </summary>
		public virtual void vxEngineMainEntryPoint ()
		{

		}

		/// <summary>
		/// Starts the menu set.
		/// </summary>
		public virtual void StartMenuSet ()
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
			// tell the TimeRuler that we're starting a new frame. you always want
			// to call this at the start of Update
			_debugSystem.TimeRuler.StartFrame ();

			// Start measuring time for "Update".
			_debugSystem.TimeRuler.BeginMark ("Update", Color.Blue);

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
		}


		/// <summary>
		/// Prints a list of all the screens, for debugging.
		/// </summary>
		void TraceScreens ()
		{
			List<string> screenNames = new List<string> ();

			foreach (vxGameBaseScreen screen in screens)
				screenNames.Add (screen.GetType ().Name);

			//Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
		}


		/// <summary>
		/// Tells each screen to draw itself.
		/// </summary>
		public override void Draw (GameTime gameTime)
		{
			// Start measuring time for "Draw".
			_debugSystem.TimeRuler.BeginMark ("Draw", Color.Yellow);

			if (FixedRenderTargetEnabled) {

				this.GraphicsDevice.SetRenderTarget (FixedRenderTarget);

				TouchPanel.DisplayHeight = this.FixedRenderTargetHeight;
				TouchPanel.DisplayWidth = this.FixedRenderTargetWidth;
			}
				foreach (vxGameBaseScreen screen in screens) {
					if (screen.ScreenState == ScreenState.Hidden)
						continue;

					screen.Draw (gameTime);
				}

				_inputManager.Draw ();
#if VRTC_INCLDLIB_NET
            DrawNetworkGameConnectionInfo();
#endif

            vxConsole.Draw ();

			if(FixedRenderTargetEnabled){
				
				this.GraphicsDevice.SetRenderTarget (null);
				this.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
				this.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

				//this.SpriteBatch.Begin ();
				this.SpriteBatch.Draw(FixedRenderTarget, 
					new Rectangle(0,0,
						this.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
						this.GraphicsDevice.Adapter.CurrentDisplayMode.Height),Color.White);
				this.SpriteBatch.End ();
			}

			// Stop measuring time for "Draw".
			_debugSystem.TimeRuler.EndMark ("Draw");
		}


#endregion

#region Public Methods


		/// <summary>
		/// Adds a new screen to the screen manager.
		/// </summary>
		public void AddScreen (vxGameBaseScreen screen, PlayerIndex? controllingPlayer)
		{
			vxConsole.WriteLine ("Adding Screen: " + screen.ToString ());

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
		/// Sets the Resolution and Fullscreen State
		/// </summary>
		public void SetGraphicsSettings ()
		{
			GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
#if !DEBUG && !VRTC_PLTFRM_DROID
            if (LoadResolution == true)
            {

            #region Set Resolution

                int ResCount = 0;

                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if (Profile.Settings.Graphics.Int_Resolution == ResCount)
                    {
                        try
                        {
                            graphics.PreferredBackBufferHeight = mode.Height;
                            graphics.PreferredBackBufferWidth = mode.Width;

                            WriteLine_Cyan("Setting Resolution to: '" +
                                graphics.PreferredBackBufferWidth.ToString() + "x" + graphics.PreferredBackBufferHeight.ToString() + "'");

                            graphics.ApplyChanges();
                        }
                        catch (Exception exception)
                        {
                            WriteError("vxEngine.cs", "SetGraphicsSettings", exception.Message);
                        }
                    }
                    //Only Increment if Original Criteria is met
                    if (mode.Width > 599 || mode.Height > 479)
                    {
                        //Increment through Display modes
                        ResCount++;
                    }
                }
            #endregion

            #region Set FullScreen or Not

                if (Profile.Settings.Graphics.Bool_FullScreen == false)
                    graphics.IsFullScreen = false;
                else
                    graphics.IsFullScreen = true;

                WriteLine_Cyan("Fullscreen: " + Profile.Settings.Graphics.Bool_FullScreen);
                
                //Set Graphics
                graphics.ApplyChanges();
#if VIRTICES_3D
                //Reset All Render Targets
                if (this.Renderer !=null)
                    this.Renderer.InitialiseRenderTargetsAll();
#endif
                for (int i = 0; i < 8; i++)
                {
                    graphics.GraphicsDevice.SamplerStates[i] = SamplerState.PointClamp;
                }

#endregion
            }
#endif
            }

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

		//TODO: Add to the Utility Namespace
		/// <summary>
		/// Writes the error, specifing the Method and Error Message.
		/// </summary>
		/// <param name="File">File.</param>
		/// <param name="Method">Method.</param>
		/// <param name="Message">Message.</param>
		public void WriteError (string File, string Method, string Message)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Red;
#endif
			Console.WriteLine ("***********************************************************************");
			Console.WriteLine ("\t\t\tERROR\n");

			Console.WriteLine ("File:\t\t'{0}'", File);
			Console.WriteLine ("Method:\t\t'{0}'", Method);
			Console.WriteLine ("Message:\t{0}", Message);
			Console.WriteLine ("***********************************************************************");
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

		/// <summary>
		/// Writes the line white.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_White (string Text)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.White;
#endif
			Console.WriteLine (Text);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

		/// <summary>
		/// Writes the line green.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Green (string Text)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Green;
#endif
			Console.WriteLine (Text);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

		/// <summary>
		/// Writes the line red.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Red (string Text)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Red;
#endif
			Console.WriteLine (Text);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

		/// <summary>
		/// Writes the line yellow.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Yellow (string Text)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Yellow;
#endif
			Console.WriteLine (Text);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

		/// <summary>
		/// Writes the line dark yellow.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_DarkYellow (string Text)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.DarkYellow;
#endif
            Console.WriteLine (Text);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

		/// <summary>
		/// Writes the line cyan.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Cyan (string Text)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Cyan;
#endif
			Console.WriteLine (Text);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor ();
#endif
		}

	}
}
