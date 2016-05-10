#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using vxVertices.Core.Input.Events;
using vxVertices.GUI.Controls;
using vxVertices.GUI.Dialogs;
using vxVertices.GUI.MessageBoxs;
using vxVertices.Screens.Async;


#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using vxVertices.Core;
#endregion

namespace vxVertices.Screens.Menus
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    public class PauseMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
			
        }

		public override void LoadContent ()
		{
			base.LoadContent ();

			// Create our menu entries.
			vxMenuEntry resumeGameMenuEntry = new vxMenuEntry(this, "Resume Game");
			vxMenuEntry MenuEntry_Graphics = new vxMenuEntry(this, "Graphics");
			vxMenuEntry quitGameMenuEntry = new vxMenuEntry(this, "Quit Game");

			// Hook up menu event handlers.
			resumeGameMenuEntry.Selected += OnCancel;
			MenuEntry_Graphics.Selected += new System.EventHandler<PlayerIndexEventArgs>(MenuEntry_Graphics_Selected);
			quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

			// Add entries to the menu.
			MenuEntries.Add(resumeGameMenuEntry);
			MenuEntries.Add(MenuEntry_Graphics);
			MenuEntries.Add(quitGameMenuEntry);
		}

        void MenuEntry_Graphics_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new vxGraphicSettingsDialog(), e.PlayerIndex);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

			vxMessageBox confirmQuitMessageBox = new vxMessageBox(message, "Pause");

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            vxEngine.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(vxEngine, false, null, new TitleScreen(2));
        }


        #endregion
    }
}
