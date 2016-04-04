#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using vxVertices.Network;


#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using vxVertices.Screens.Menus;
using vxVertices.GUI.Dialogs;
using vxVertices.GUI.Controls;
using vxVertices.Core.Input.Events;
using vxVertices.Core;
using vxVertices.GUI.MessageBoxs;
using vxVertices.Screens.Async;
#endregion

namespace VerticeEnginePort.Base
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization
        
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Vertice Engine - Tech Demo")
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Create our menu entries.
            vxMenuEntry playGameMenuEntry = new vxMenuEntry(this, "Play");
            vxMenuEntry sandboxGameMenuEntry = new vxMenuEntry(this, "Sandbox");
            vxMenuEntry modelViewGameMenuEntry = new vxMenuEntry(this, "Model Viewer");
            vxMenuEntry demoGameMenuEntry = new vxMenuEntry(this, "Demo Screen");
            vxMenuEntry optionsMenuEntry = new vxMenuEntry(this, "Options");
            vxMenuEntry exitMenuEntry = new vxMenuEntry(this, "Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            sandboxGameMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(sandboxGameMenuEntry_Selected);
            modelViewGameMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(modelViewGameMenuEntry_Selected);
            demoGameMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(demoGameMenuEntry_Selected);
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            /*
			MenuEntries.Add(multiplayerMenuEntry);
            MenuEntries.Add(sandboxGameMenuEntry);
            MenuEntries.Add(modelViewGameMenuEntry);
            MenuEntries.Add(demoGameMenuEntry);
            */
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

        #region Handle Input
        
        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
            vxEngine.AddScreen(new DemoSelectScreen(), PlayerIndex.One);
        }
        
        void logInForm_LogInSuccessful(object sender, EventArgs e)
        {
            //vxEngine.AddScreen(new MultiplayerSelectScreen(), PlayerIndex.One);            
        }

        void logInForm_LogInUNSuccessful(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sandbox Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sandboxGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //vxEngine.AddScreen(new SandboxSelectScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Model Viewer Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void modelViewGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(vxEngine, true, e.PlayerIndex, new ModelViewerScreen());
        }

        /// <summary>
        /// Eventhandler for the Demo Selection Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void demoGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new DemoSelectScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.AddScreen(new SettingsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        public override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to quit?";
            vxMessageBox confirmExitMessageBox = new vxMessageBox(message, "quit?");
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            vxEngine.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.Game.Exit();
        }


        #endregion
    }
}
