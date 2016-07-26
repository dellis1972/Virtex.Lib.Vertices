#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Core.Input.Events;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI.Controls;


#endregion

namespace Virtex.Lib.Vertices.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class GraphicsSettingsScreen : vxMenuBaseScreen
    {
        #region Fields

//        vxSliderMenuEntry fullScreenMenuEntry;
//		vxSliderMenuEntry resolutionMenuEntry;
//		vxSliderMenuEntry MenuEntryShowEdgeDetect;
//		vxSliderMenuEntry MenuEntryShowDistortion;
//		vxSliderMenuEntry MenuEntryShowBloom;
        vxMenuEntry displayDebugHUDMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GraphicsSettingsScreen()
            : base("Graphics Settings")
        {
			
        }

        public override void LoadContent()
		{
			//            
			//            //
			//            //Set Fullscreen or Windowed Options
			//            //
			//			fullScreenMenuEntry = new vxSliderMenuEntry("screen", 50, 150, 35);
			//            fullScreenMenuEntry.ItemList.Add("full screen");
			//            fullScreenMenuEntry.ItemList.Add("windowed");
			//
			//
			//            //
			//            //Set Resolutions
			//            //
			//			resolutionMenuEntry = new vxSliderMenuEntry("resolution", 50, 150, 35);
			//            // Create our menu entries.
			//            bool AddItem = true;
			//            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
			//            {
			//                AddItem = true;
			//
			//                //Don't Show All Resolutions
			//                if (mode.Width > 599 || mode.Height > 479)
			//                {
			//                    string menuItemText = string.Format("{0}x{1}", mode.Width, mode.Height);
			//
			//                    //If Good Resolution and Not being repeated, Add Item
			//                    if (AddItem)
			//                        resolutionMenuEntry.ItemList.Add(menuItemText);
			//                }
			//
			//            }
			//
			//            //
			//            //Show Bloom
			//            //
			//			MenuEntryShowBloom = new vxSliderMenuEntry("bloom", 50, 150, 35);
			//            MenuEntryShowBloom.ItemList.Add("true");
			//            MenuEntryShowBloom.ItemList.Add("false");
			//
			//            displayDebugHUDMenuEntry = new vxMenuEntry("debug");
			//            vxMenuEntry backMenuEntry = new vxMenuEntry("back");
			//            
			//            // Hook up menu event handlers.
			//            displayDebugHUDMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugHUDMenuEntry_Selected);
			//
			//            //Accept and Cancel
			//            backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);
			//
			//
			//            // Add entries to the menu.
			//            
			//            MenuEntries.Add(fullScreenMenuEntry);
			//            MenuEntries.Add(resolutionMenuEntry);
			//            MenuEntries.Add(MenuEntryShowBloom);
			//            
			//            //MenuEntries.Add(displayDebugHUDMenuEntry);

			//MenuEntries.Add(backMenuEntry);
            //Set Initial Fullscreen Values
//            if (vxEngine.Profile.Settings.Bool_FullScreen == true)
//                fullScreenMenuEntry.Index = 0;
//            else
//                fullScreenMenuEntry.Index = 1;
//
//                resolutionMenuEntry.Index = vxEngine.Profile.Settings.Int_Resolution;
//
//
//            //Set Initial Bloom Values
//            if (vxEngine.ShowBloom == true)
//                MenuEntryShowBloom.Index = 0;
//            else
//                MenuEntryShowBloom.Index = 1;

            base.LoadContent();
        }

        void cancelMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        void backMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
//            //
//            //Set Resolution
//            //
//            vxEngine.Profile.Settings.Int_Resolution = resolutionMenuEntry.Index;
//
//
//
//            //
//            //Set Fulscreen.Window
//            //
//            if (fullScreenMenuEntry.SelectedItem == "full screen")
//                vxEngine.Profile.Settings.Bool_FullScreen = true;
//            else
//                vxEngine.Profile.Settings.Bool_FullScreen = false;
//
//
//
//            //
//            //Set Bloom Settings
//            //
//            if (MenuEntryShowBloom.Index == 0)
//                vxEngine.ShowBloom = true;
//            else
//                vxEngine.ShowBloom = false;


            //
            //Save Settings
            //
            vxEngine.Profile.SaveSettings(vxEngine);


            //
            //Set Graphics
            //
            vxEngine.SetGraphicsSettings();

            ExitScreen();
        }
        #endregion

        #region Handle Input


        void displayDebugHUDMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new DebugMenuScreen(), e.PlayerIndex);
        }

        #endregion
    }
}
