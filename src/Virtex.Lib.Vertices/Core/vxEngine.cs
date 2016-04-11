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
using vxVertices.Core.Input;
using vxVertices.Graphics;
using vxVertices.Core.Scenes;
using vxVertices.Core.Debug;
using vxVertices.Core.Settings;
using vxVertices.Core.ContentManagement;
using vxVertices.Utilities;
using vxVertices.Network.Events;
using vxVertices.Network;
using vxVertices.GUI;
using vxVertices.GUI.Themes;

#endregion

namespace vxVertices.Core
{
	/// <summary>
	/// The vxEngine is a component which manages one or more GameScreen
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
#if VIRTICES_XNA
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
#if VIRTICES_XNA
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
		//Tells whether or not the Game has loaded it's
		///Specific Assets


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

		/// <summary>
		/// Virtex Render System
		/// </summary>
		public vxRenderer Renderer {
			get { return _renderer; }
			set { _renderer = value; }
		}

		private vxRenderer _renderer;

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
		List<GameScreen> screens = new List<GameScreen> ();

		/// <summary>
		/// Screens to Update List
		/// </summary>
		List<GameScreen> screensToUpdate = new List<GameScreen> ();


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
		public Model Model_Sandbox_WorkingPlane;

		/// <summary>
		/// Boolean to check if the level has been initialised yet.
		/// </summary>
		public bool isInitialized;


		//
		//Vector3
		//
		/// <summary>
		/// The mouse click position.
		/// </summary>
		public Vector2 Mouse_ClickPos = new Vector2 ();

		#endregion

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

			try {
				Console.Title = "VIRTICES ENGINE DEBUG CONSOLE v." + gameVersion;
			} catch {
			}
			vxConsole.WriteLine ("///////////////////////////////////////////////////////////////////////");
			vxConsole.WriteLine (string.Format ("          Vertices Engine - v.{0}", gameVersion));
			vxConsole.WriteLine (string.Format ("          Starting Game   - {0}", GameName));
			vxConsole.WriteLine ("///////////////////////////////////////////////////////////////////////");

			//Console.SetWindowPosition(0, 0);

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

			//First Check, if the Profiles Directory Doesn't Exist, Create It
			if (Directory.Exists (Path_Profiles) == false)
				Directory.CreateDirectory (Path_Profiles);

			//First Check, if the Sandbox Directory Doesn't Exist, Create It
			if (Directory.Exists (Path_Sandbox) == false)
				Directory.CreateDirectory (Path_Sandbox);

			//First Check, if the Temp Directory Doesn't Exist, Create It
			if (Directory.Exists ("Temp/Settings") == false)
				Directory.CreateDirectory ("Temp/Settings");

			vxConsole.vxEngine = this;


#if VIRTICES_XNA
			vxConsole.WriteLine("Starting Vertices Engine with XNA Backend...");
#else
			vxConsole.WriteLine ("Starting Vertices Engine with MonoGame Backend...");
#endif



#if VRTC_INCLDLIB_NET
			InitialiseNetwork();
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

			//string contentLocationTag = "Virtex.Lib.Vertices.XNA.Content";

//Set Location of Content Specific too Platform
#if VIRTICES_XNA
            _engineContentManager.RootDirectory = "Virtex.Lib.Vertices.XNA.Content";

#elif VRTC_PLTFRM_GL
			_engineContentManager.RootDirectory = "Vertices.Engine.Content/Compiled.WindowsGL";
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

			Model_Sandbox_WorkingPlane = this.Assets.Models.UnitPlane;

			//Load in Profile Data
			vxConsole.WriteLine ("Loading Settings....");
			Profile.LoadSettings (this);
			vxConsole.WriteLine ("\t\t\tDone!");

			//Set Initial Graphics Settings
			SetGraphicsSettings ();


			GraphicsDeviceManager graphics = Game.Services.GetService (typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
			//graphics.SynchronizeWithVerticalRetrace = false;
			Game.IsFixedTimeStep = false;
			//graphics.PreferMultiSampling = true;
			//graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
			graphics.ApplyChanges ();

			//this.Profile.Settings.Graphics.Bloom = vxEnumQuality.None;
			//this.Profile.Settings.Graphics.DepthOfField = vxEnumQuality.None;// = false;
			//this.Profile.Settings.Graphics.Bool_DoEdgeDetection = true;

			Renderer = new vxRenderer (this);

			// Tell each of the screens to load their content.
			foreach (GameScreen screen in screens) {
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

		//TODO: Add too Netork Event File.
		public event EventHandler<vxGameServerListRecievedEventArgs> GameServerListRecieved;


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
			foreach (GameScreen screen in screens) {
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

				foreach (GameScreen screen in screens)
					screensToUpdate.Add (screen);

				bool otherScreenHasFocus = !Game.IsActive;
				bool coveredByOtherScreen = false;

				// Loop as long as there are screens waiting to be updated.
				while (screensToUpdate.Count > 0) {
					// Pop the topmost screen off the waiting list.
					GameScreen screen = screensToUpdate [screensToUpdate.Count - 1];

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

			foreach (GameScreen screen in screens)
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

			foreach (GameScreen screen in screens) {
				if (screen.ScreenState == ScreenState.Hidden)
					continue;

				screen.Draw (gameTime);
			}

			_inputManager.Draw ();

			vxConsole.Draw ();

			// Stop measuring time for "Draw".
			_debugSystem.TimeRuler.EndMark ("Draw");
		}


		#endregion

		#region Public Methods


		/// <summary>
		/// Adds a new screen to the screen manager.
		/// </summary>
		public void AddScreen (GameScreen screen, PlayerIndex? controllingPlayer)
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
		/// use GameScreen.ExitScreen instead of calling this directly, so
		/// the screen can gradually transition off rather than just being
		/// instantly removed.
		/// </summary>
		public void RemoveScreen (GameScreen screen)
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
		public GameScreen[] GetScreens ()
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
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path"></param>
		/// <returns>A Model Object With the Distortion Shader Applied</returns>
		public Model LoadDistortionModel (string Path)
		{
			Model modelToReturn;
			Effect replacementEffect = this.Assets.Shaders.DistortionShader;

			modelToReturn = this.Game.Content.Load<Model> (Path);

			try {
				// Table mapping the original effects to our replacement versions.
				Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect> ();

				foreach (ModelMesh mesh in modelToReturn.Meshes) {
					// Scan over all the effects currently on the mesh.
					foreach (BasicEffect oldEffect in mesh.Effects) {
						// If we haven't already seen this effect...
						if (!effectMapping.ContainsKey (oldEffect)) {
							// Make a clone of our replacement effect. We can't just use
							// it directly, because the same effect might need to be
							// applied several times to different parts of the model using
							// a different texture each time, so we need a fresh copy each
							// time we want to set a different texture into it.
							Effect newEffect = replacementEffect.Clone ();

							//// Copy across the texture from the original effect.
							//newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

							//newEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

							//newEffect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
							//newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));

							effectMapping.Add (oldEffect, newEffect);
						}
					}

					// Now that we've found all the effects in use on this mesh,
					// update it to use our new replacement versions.
					foreach (ModelMeshPart meshPart in mesh.MeshParts) {
						meshPart.Effect = effectMapping [meshPart.Effect];
					}
				}
			} catch {
			}

			return modelToReturn;
		}

		/// <summary>
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path">The Model File Path</param>
		/// <returns>A Model Object With the Main Shader Applied</returns>
		public Model LoadModel (string Path)
		{
			return LoadModel (Path, this.Game.Content);
		}

		/// <summary>
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path">The Model File Path</param>
		/// <param name="Content">The Content Manager to load the Model with</param>
		/// <returns>A Model Object With the Main Shader Applied</returns>
		public Model LoadModel (string Path, ContentManager Content)
		{
			Model modelToReturn;
			Effect replacementEffect;
			Texture2D baseNormalMap;
			Texture2D baseSpecularMap;

			modelToReturn = Content.Load<Model> (Path);
            
			if (this.Assets != null) {
				replacementEffect = this.Assets.Shaders.MainShader;
				baseNormalMap = this.Assets.Textures.Texture_NormalMap_Null;
				baseSpecularMap = this.Assets.Textures.Texture_SpecularMap_Null;
			} else {
				string prefixtag = "";

				//Model Shaders
#if VIRTICES_XNA
#else
				prefixtag = "MonoGame/";
#endif
				replacementEffect = this.EngineContentManager.Load<Effect> (prefixtag + "Shaders/MainModelShader");

				baseNormalMap = this.EngineContentManager.Load<Texture2D> ("Textures/null_normal");
				baseSpecularMap = this.EngineContentManager.Load<Texture2D> ("Textures/null_specular");
			}
			try {
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine ("\t\tImporting Model: " + Path);
				// Table mapping the original effects to our replacement versions.
				Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect> ();
                
                foreach (ModelMesh mesh in modelToReturn.Meshes) {

                    Console.WriteLine("\t\t\tMesh Mesh: " + mesh.Name);
					mesh.Tag = Path;
					// Scan over all the effects currently on the mesh.
					foreach (BasicEffect oldEffect in mesh.Effects) {
						// If we haven't already seen this effect...
						if (!effectMapping.ContainsKey (oldEffect)) {
							// Make a clone of our replacement effect. We can't just use
							// it directly, because the same effect might need to be
							// applied several times to different parts of the model using
							// a different texture each time, so we need a fresh copy each
							// time we want to set a different texture into it.
							Effect newEffect = replacementEffect.Clone ();

							// Copy across the texture from the original effect.
							if (newEffect.Parameters ["Texture"] != null)
								newEffect.Parameters ["Texture"].SetValue (oldEffect.Texture);

							if (newEffect.Parameters ["TextureEnabled"] != null)
								newEffect.Parameters ["TextureEnabled"].SetValue (oldEffect.TextureEnabled);

							if (newEffect.Parameters ["IsSun"] != null)
								newEffect.Parameters ["IsSun"].SetValue (false);
                            
                          
                                if (newEffect.Parameters["NormalMap"] != null &&
                                    File.Exists("Content/" + vxUtil.GetParentPathFromFilePath(Path) + "/" + mesh.Name + "_nm"))
                                {
                                    newEffect.Parameters["NormalMap"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(Path) + "/" + mesh.Name + "_nm"));
                                    Console.WriteLine("\t\t\t\tNormal Map Found");
                                }


                            if (newEffect.Parameters["SpecularMap"] != null &&
                                    File.Exists("Content/" + vxUtil.GetParentPathFromFilePath(Path) + "/" + mesh.Name + "_sm"))
                            {
                                
                                    newEffect.Parameters["SpecularMap"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(Path) + "/" + mesh.Name + "_sm"));
                                Console.WriteLine("\t\t\t\tSpecular Map Found");
                            }
                            

#if VIRTICES_XNA
                            if (newEffect.Parameters["LightDirection"] != null)
                                newEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

                            if (newEffect.Parameters["LightColor"] != null)
                                newEffect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

                            if (newEffect.Parameters["AmbientLightColor"] != null)
                                newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
#endif

							effectMapping.Add (oldEffect, newEffect);
						}
					}

					// Now that we've found all the effects in use on this mesh,
					// update it to use our new replacement versions.
					foreach (ModelMeshPart meshPart in mesh.MeshParts) {
						meshPart.Effect = effectMapping [meshPart.Effect];
					}
				}
			} catch (Exception ex) {
				vxConsole.WriteLine ("ERROR IMPORTING FILE: " + Path + "\n" + ex.Message);
			}
			//#endif
			modelToReturn.Tag = Path;
			return modelToReturn;
		}
        

        /// <summary>
        /// Load a Model and apply the Main Shader Effect to it.
        /// </summary>
        /// <param name="Path">The Model File Path</param>
        /// <param name="Content">The Content Manager to load the Model with</param>
        /// <returns>A Model Object With the Main Shader Applied</returns>
        public Model LoadModelAsWaterObject (string Path, ContentManager Content)
		{
			Model modelToReturn;
			Effect replacementEffect;

			modelToReturn = Content.Load<Model> (Path);

#if VIRTICES_XNA
            replacementEffect = this.Assets.Shaders.WaterReflectionShader;

            try
            {
                // Table mapping the original effects to our replacement versions.
                Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

                foreach (ModelMesh mesh in modelToReturn.Meshes)
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
            catch { }
#endif

			return modelToReturn;
		}

		/// <summary>
		/// Sets the Resolution and Fullscreen State
		/// </summary>
		public void SetGraphicsSettings ()
		{
#if !DEBUG
            if (LoadResolution == true)
            {

#region Set Resolution

                GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;

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

                //Reset All Render Targets
                if(this.Renderer !=null)
                    this.Renderer.InitialiseRenderTargetsAll();
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
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("***********************************************************************");
			Console.WriteLine ("\t\t\tERROR\n");

			Console.WriteLine ("File:\t\t'{0}'", File);
			Console.WriteLine ("Method:\t\t'{0}'", Method);
			Console.WriteLine ("Message:\t{0}", Message);
			Console.WriteLine ("***********************************************************************");
			Console.ResetColor ();
		}

		/// <summary>
		/// Writes the line white.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_White (string Text)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine (Text);
			Console.ResetColor ();
		}

		/// <summary>
		/// Writes the line green.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Green (string Text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine (Text);
			Console.ResetColor ();
		}

		/// <summary>
		/// Writes the line red.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Red (string Text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine (Text);
			Console.ResetColor ();
		}

		/// <summary>
		/// Writes the line yellow.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Yellow (string Text)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine (Text);
			Console.ResetColor ();
		}

		/// <summary>
		/// Writes the line dark yellow.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_DarkYellow (string Text)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine (Text);
			Console.ResetColor ();
		}

		/// <summary>
		/// Writes the line cyan.
		/// </summary>
		/// <param name="Text">Text.</param>
		public void WriteLine_Cyan (string Text)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine (Text);
			Console.ResetColor ();
		}

	}
}
