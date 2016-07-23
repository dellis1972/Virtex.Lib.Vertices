#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core.Input.Events;
using vxVertices.GUI.Controls;
using vxVertices.GUI.Dialogs;


#endregion

namespace vxVertices.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class SettingsMenuScreen : vxMenuBaseScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsMenuScreen()
            : base("settings")
        {
			
        }

		public override void LoadContent ()
		{
			base.LoadContent ();

			// Create our menu entries.
			vxMenuEntry ControlsMenuEntry = new vxMenuEntry(this, "controls");
			vxMenuEntry GraphicsMenuEntry = new vxMenuEntry(this, vxEngine.Language.Graphics);
			vxMenuEntry AudioMenuEntry = new vxMenuEntry(this, "audio");
            vxMenuEntry LocalizationMenuEntry = new vxMenuEntry(this, "Localization");

            vxMenuEntry cancelMenuEntry = new vxMenuEntry(this, "back");

			// Hook up menu event handlers.
			ControlsMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(ControlsMenuEntry_Selected);
			GraphicsMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(GraphicsMenuEntry_Selected);
			AudioMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(AudioMenuEntry_Selected);
            LocalizationMenuEntry.Selected += LocalizationMenuEntry_Selected;
            //Back
            cancelMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(cancelMenuEntry_Selected);



			// Add entries to the menu.
			MenuEntries.Add(ControlsMenuEntry);
            MenuEntries.Add(LocalizationMenuEntry);
            MenuEntries.Add(GraphicsMenuEntry);
			MenuEntries.Add(AudioMenuEntry);

            MenuEntries.Add(cancelMenuEntry);
		}

        void ControlsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new ControlsMenuScreen(), e.PlayerIndex);
        }

        void GraphicsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxGraphicSettingsDialog(), e.PlayerIndex);
        }

        void AudioMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new AudioMenuScreen(), e.PlayerIndex);
        }

        private void LocalizationMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxLocalizationDialog(), e.PlayerIndex);
        }

        void cancelMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }
        
        #endregion
    }
}
