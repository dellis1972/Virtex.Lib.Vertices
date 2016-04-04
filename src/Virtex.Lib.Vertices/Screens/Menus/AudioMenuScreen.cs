#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core.Input.Events;
using vxVertices.Core;
using vxVertices.GUI.Controls;


#endregion

namespace vxVertices.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class AudioMenuScreen : MenuScreen
    {
        #region Fields

//        vxSliderMenuEntry MusicVolumeMenuEntry;
//		vxSliderMenuEntry SFXVolumeMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public AudioMenuScreen()
            : base("audio")
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();

			//			MusicVolumeMenuEntry = new vxSliderMenuEntry("music volume:", 30, 75, 40);
			//			SFXVolumeMenuEntry = new vxSliderMenuEntry("sfx volume:", 30, 75, 40);
			//            for (int i = 0; i < 11; i++)
			//            {
			//                MusicVolumeMenuEntry.ItemList.Add(i.ToString());
			//                SFXVolumeMenuEntry.ItemList.Add(i.ToString());
			//            }

			vxMenuEntry backMenuEntry = new vxMenuEntry(this, "back");

			//Accept and Cancel
			backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);


			// Add entries to the menu.
			//            MenuEntries.Add(MusicVolumeMenuEntry);
			//            MenuEntries.Add(SFXVolumeMenuEntry);
			MenuEntries.Add(backMenuEntry);

//            //Set Music Volume Menu Entry
//            MusicVolumeMenuEntry.Index = 5;// (int)(10 * vxEngine.Profile.Settings.Double_MUSIC_Volume);
//            SFXVolumeMenuEntry.Index = 5;//(int)(10*vxEngine.Profile.Settings.Double_SFX_Volume);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Set Music Volume in Settings
            //vxEngine.Profile.Settings.Double_MUSIC_Volume = ((float)MusicVolumeMenuEntry.Index) / 10;
            //vxEngine.Profile.Settings.Double_SFX_Volume = ((float)SFXVolumeMenuEntry.Index) / 10;
        }


        void backMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //Save Settings
            //vxEngine.Profile.SaveSettings(vxEngine);

            ExitScreen();
        }

        #endregion
    }
}
