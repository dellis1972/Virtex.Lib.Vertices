#region File Description
//-----------------------------------------------------------------------------
// MainvxMenuBaseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Screens.Async;
using Virtex.Lib.Vrtc.Screens.Menus;
#endregion

namespace Virtex.vxGame.VerticesTechDemo
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class DemoSelectScreen : vxMenuBaseScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public DemoSelectScreen()
            : base("Select Demo")
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();
			// Create our menu entries.
			vxMenuEntry introLevelMenuEntry = new vxMenuEntry(this, "Intro Level");
            vxMenuEntry spoznzaMenuEntry = new vxMenuEntry(this, "Sponza Sandbox Level");
            vxMenuEntry fpsMenuEntry = new vxMenuEntry(this, "First Person Level");
            vxMenuEntry exitMenuEntry = new vxMenuEntry(this, "Back");

			// Hook up menu event handlers.
			introLevelMenuEntry.Selected +=	introLevelMenuEntrySelected;
            spoznzaMenuEntry.Selected += spoznzaMenuEntrySelected;
            fpsMenuEntry.Selected += fpsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

			// Add entries to the menu.
			MenuEntries.Add(introLevelMenuEntry);
            MenuEntries.Add(spoznzaMenuEntry);
            MenuEntries.Add(fpsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

		#region Handle Input

		/// <summary>
		/// Event handler for when the Play Game menu entry is selected.
		/// </summary>
		void introLevelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
			vxLoadingScreen.Load(vxEngine, true, e.PlayerIndex,
				new IntroBackground());
		}


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void spoznzaMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vxLoadingScreen.Load(vxEngine, true, e.PlayerIndex,
                 new TechDemoSponza());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void fpsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vxLoadingScreen.Load(vxEngine, true, e.PlayerIndex,
                               new FPSGamePlay());
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        public override void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        #endregion
    }
}
