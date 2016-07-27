#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Core.Input.Events;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.GUI.Dialogs;
using Virtex.Lib.Vertices.Localization;


#endregion

namespace Virtex.Lib.Vertices.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class vxSettingsMenuScreen : vxMenuBaseScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSettingsMenuScreen()
            : base("settings")
        {
			
        }

		public override void LoadContent ()
		{
			base.LoadContent ();

            this.MenuTitle = LanguagePack.Get(vxLocalization.Main_Settings);

            // Create our menu entries.
            vxMenuEntry ControlsMenuEntry = new vxMenuEntry(this, LanguagePack.Get(vxLocalization.Settings_Controls));
            vxMenuEntry LocalizationMenuEntry = new vxMenuEntry(this, LanguagePack.Get(vxLocalization.Settings_Localization));
            vxMenuEntry GraphicsMenuEntry = new vxMenuEntry(this, vxEngine.Language.Get(vxLocalization.Settings_Graphics));
			vxMenuEntry AudioMenuEntry = new vxMenuEntry(this, LanguagePack.Get(vxLocalization.Settings_Audio));
            vxMenuEntry displayDebugHUDMenuEntry = new vxMenuEntry(this, "debug");

            vxMenuEntry cancelMenuEntry = new vxMenuEntry(this, LanguagePack.Get(vxLocalization.Misc_Back));

			// Hook up menu event handlers.
			ControlsMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(ControlsMenuEntry_Selected);
			GraphicsMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(GraphicsMenuEntry_Selected);
            LocalizationMenuEntry.Selected += LocalizationMenuEntry_Selected;
            AudioMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(AudioMenuEntry_Selected);
            displayDebugHUDMenuEntry.Selected += DisplayDebugHUDMenuEntry_Selected;
            //Back
            cancelMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(cancelMenuEntry_Selected);



			// Add entries to the menu.
			MenuEntries.Add(ControlsMenuEntry);
            MenuEntries.Add(LocalizationMenuEntry);
            MenuEntries.Add(GraphicsMenuEntry);
			MenuEntries.Add(AudioMenuEntry);

#if DEBUG
            MenuEntries.Add(displayDebugHUDMenuEntry);
#endif

            MenuEntries.Add(cancelMenuEntry);
		}

        void ControlsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxControlsMenuScreen(), e.PlayerIndex);
        }

        void GraphicsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxGraphicSettingsDialog(), e.PlayerIndex);
        }

        private void LocalizationMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxLocalizationDialog(), e.PlayerIndex);
        }

        void AudioMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxAudioMenuScreen(), e.PlayerIndex);
        }

        private void DisplayDebugHUDMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxDebugMenuScreen(), e.PlayerIndex);
        }

        void cancelMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }
        
#endregion
    }
}
