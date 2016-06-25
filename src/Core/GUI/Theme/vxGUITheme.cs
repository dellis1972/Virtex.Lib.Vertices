using System;
using vxVertices.Core;
using vxVertices.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace vxVertices.GUI.Themes
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
        

        /*******************************************/
        //					LABEL
        /*******************************************/
        public Color vxLabelColorNormal = Color.White;



        public vxThemeButton vxButtons { get; set; }
        public vxMenuEntryTheme vxMenuEntries {get;set;}
		public vxThemeTextbox vxTextboxes {get;set; }
        public vxThemeDialog vxDialogs { get; set; }
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
        /// Initializes a new instance of the <see cref="vxVertices.GUI.vxGUITheme"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxGUITheme(vxEngine Engine)
		{
			this.Engine = Engine;

            vxButtons = new vxThemeButton(Engine);
            vxMenuEntries = new vxMenuEntryTheme(Engine);
            vxTextboxes = new vxThemeTextbox(Engine);
            vxDialogs = new vxThemeDialog(Engine);
			vxLoadingScreen = new vxLoadingScreen (Engine);

            //Load the Default Theme first
            //LoadTheme (PathTooFiles, Engine.EngineContentManager);

			SetDefaultTheme ();
		}

		public void SetDefaultTheme()
		{
			//this.vxMenuEntries.TextJustification = TextJustification.Left;
			//this.vxButtons.BackgroundImage = vxEngine.Assets.Textures.Blank;
			this.vxButtons.DoBorder = true;
			this.vxButtons.BorderWidth = 2;
			this.vxButtons.TextColour = Color.Black;
			this.vxButtons.BackgroundColour = Color.DarkOrange;
			this.vxButtons.BackgroundHoverColour = Color.DarkOrange * 1.2f;

			this.vxDialogs.Header_BackgroundColour = Color.DarkOrange;
			this.vxDialogs.Header_BorderWidth = 2;
			this.vxDialogs.BackgroundColour = Color.Black*0.75f;

			this.vxLoadingScreen.SplashScreen = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxButton/Bckgrnd_Nrml");


			vxButtons.BackgroundImage = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxLoadingScreen/SplashScreen");

			/*******************************************/
			//					MENU ITEM
			/*******************************************/
			//this.vxMenuEntries.TitleColor = Color.Black;
			vxMenuEntries.Padding = new Vector2 (10, 4);
			vxMenuEntries.vxMenuItemBackground = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxMenuEntry/Bckgrnd_Nrml");
			vxMenuEntries.vxMenuSplitterTexture = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxMenuEntry/Splttr_Nrml");
			vxMenuEntries.TitleBackground = LoadTexture(Engine.EngineContentManager, "vxGUITheme/vxMenuScreen/Bckgrnd_Nrml");




            /*******************************************/
            //					Sound Effects
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

            /*******************************************/
            //					BUTTON
            /*******************************************/
            vxButtons.BackgroundImage = LoadTexture(contentManager, "vxGUITheme/vxButton/Bckgrnd_Nrml");

			/*******************************************/
			//					MENU ITEM
			/*******************************************/
			vxMenuEntries.vxMenuItemBackground = LoadTexture(contentManager, "vxGUITheme/vxMenuEntry/Bckgrnd_Nrml");
			vxMenuEntries.vxMenuSplitterTexture = LoadTexture(contentManager, "vxGUITheme/vxMenuEntry/Splttr_Nrml");
			vxMenuEntries.TitleBackground = LoadTexture(contentManager, "vxGUITheme/vxMenuScreen/Bckgrnd_Nrml");

			//loading Screen
			this.vxLoadingScreen.SplashScreen = LoadTexture(contentManager, "vxGUITheme/vxLoadingScreen/SplashScreen");
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
				vxConsole.WriteError ("vxGUITheme", 
					string.Format("ERROR LOADING THEME TEXTURE: PATH: '{0}'      --------------         ER MSG: {1}", PathTooFiles + path, ex.Message));

				return Engine.Assets.Textures.Blank;
			}
		}
	}

}

