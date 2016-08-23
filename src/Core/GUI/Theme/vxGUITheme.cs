using System;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Virtex.Lib.Vrtc.GUI.GuiArtProvider;

namespace Virtex.Lib.Vrtc.GUI.Themes
{	
	

	/// <summary>
	/// Vx GUI theme.
	/// </summary>
	public class vxGUITheme{
		
		vxEngine Engine;
		ContentManager Content
		{
			get{ return Engine.Game.Content; }
		}
        string PathTooFiles = "Gui/DfltThm/";

		/*******************************************/
		//					MASTER VALUES
		/*******************************************/


		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font.</value>
		public SpriteFont Font{
			get{ return font; }
			set{ font = value; }
		}
		SpriteFont font;

		/// <summary>
		/// Gets or sets the default padding for all GUI items
		/// </summary>
		/// <value>The padding.</value>
		public Vector2 Padding{
			get{ return padding; }
			set{ padding = value; }
		}
		Vector2 padding = new Vector2 (10, 10);

		//Misc
		public Texture2D SplitterTexture { get; set; }

        /*******************************************/
        //				ART PROVIDERS
		/*******************************************/
		public vxButtonArtProvider ArtProviderForButtons { get; set; }
        public vxMenuScreenArtProvider ArtProviderForMenuScreen { get; set; }
        public vxMenuItemArtProvider ArtProviderForMenuScreenItems { get; set; }
		public vxMessageBoxArtProvider ArtProviderForMessageBoxes { get; set; }
		public vxDialogArtProvider ArtProviderForDialogs{ get; set; }

        /*******************************************/
        //					LABEL
        /*******************************************/
        public Color vxLabelColorNormal = Color.White;




		public vxThemeTextbox vxTextboxes {get;set; }
        //public vxThemeDialog vxDialogs { get; set; }
		public vxLoadingScreen vxLoadingScreen { get; set; }




        /*******************************************/
        //					Sound Effects
        /*******************************************/
#if !NO_DRIVER_OPENAL
        public SoundEffect SE_Menu_Hover { get; set; }
        public SoundEffect SE_Menu_Confirm { get; set; }
        public SoundEffect SE_Menu_Cancel { get; set; }
		#endif


        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.vxGUITheme"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxGUITheme(vxEngine Engine)
		{
			this.Engine = Engine;

            //vxButtons = new vxThemeButton(Engine);
            //vxMenuEntries = new vxMenuEntryTheme(Engine);
            vxTextboxes = new vxThemeTextbox(Engine);
            //vxDialogs = new vxThemeDialog(Engine);
			vxLoadingScreen = new vxLoadingScreen (Engine);

            //Load the Default Theme first
            //LoadTheme (PathTooFiles, Engine.EngineContentManager);

            //Initialise Art Providers
			ArtProviderForButtons = new vxButtonArtProvider(Engine);
            ArtProviderForMenuScreen = new vxMenuScreenArtProvider(Engine);
            ArtProviderForMenuScreenItems = new vxMenuItemArtProvider(Engine);
			ArtProviderForMessageBoxes = new vxMessageBoxArtProvider (Engine);
			ArtProviderForDialogs = new vxDialogArtProvider (Engine);

            SetDefaultTheme ();
		}

		public void SetDefaultTheme()
		{

			this.vxLoadingScreen.SplashScreen = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxButton/Bckgrnd_Nrml");


			// Misc ITEM
			/*******************************************/
			SplitterTexture = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxMenuEntry/Splttr_Nrml");


            // Sound Effects
            /*******************************************/

            //Menu Values
#if !NO_DRIVER_OPENAL
            SE_Menu_Hover = Engine.EngineContentManager.Load<SoundEffect>("Gui/DfltThm/vxGUITheme/SndFx/Menu/Click/Menu_Click");
			SE_Menu_Confirm = SE_Menu_Hover;// Engine.EngineContentManager.Load<SoundEffect>("Gui/DfltThm/vxGUITheme/SndFx/Menu/MenuConfirm");
			SE_Menu_Cancel = SE_Menu_Hover;//Engine.EngineContentManager.Load<SoundEffect>("Gui/DfltThm/vxGUITheme/SndFx/Menu/MenuError");
#endif
        }


        /// <summary>
		/// Loads the theme.
		/// </summary>
		/// <param name="PathTooFiles">Path to Theme</param>
        public void LoadTheme(string PathTooFiles)
        {
            LoadTheme(PathTooFiles, Engine.Game.Content);
        }

        /// <summary>
        /// Loads the Theme
        /// </summary>
        /// <param name="PathTooFiles">Path to Theme</param>
        /// <param name="contentManager">Content Manager to use.</param>
		public void LoadTheme(string PathTooFiles, ContentManager contentManager)
		{
			this.PathTooFiles = PathTooFiles;

			//loading Screen
			this.vxLoadingScreen.SplashScreen = LoadTexture(contentManager, "vxGUITheme/vxLoadingScreen/SplashScreen");
			SplitterTexture = LoadTexture(contentManager, "vxGUITheme/vxMenuEntry/Splttr_Nrml");
		}


		/// <summary>
		/// Loads the texture.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="path">Path.</param>
        Texture2D LoadTexture(ContentManager contentManager, string path)
		{
			try{
                return contentManager.Load<Texture2D>(PathTooFiles + path);
			}
			catch(Exception ex){
				vxConsole.WriteError (ex);

				return Engine.Assets.Textures.Blank;
			}
		}
	}

}

