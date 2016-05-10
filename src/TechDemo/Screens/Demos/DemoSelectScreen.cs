#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using vxVertices.GUI.Controls;
using vxVertices.Core.Input.Events;
using vxVertices.Screens.Async;
using vxVertices.Screens.Menus;
#endregion

namespace VerticeEnginePort.Base
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class DemoSelectScreen : MenuScreen
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
			LoadingScreen.Load(vxEngine, true, e.PlayerIndex,
				new IntroBackground());
		}


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void spoznzaMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(vxEngine, true, e.PlayerIndex,
                 new TechDemoSponza());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void fpsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(vxEngine, true, e.PlayerIndex,
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
