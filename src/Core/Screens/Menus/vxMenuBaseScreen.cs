#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using Virtex.Lib.Vertices.Core.Input;
using Virtex.Lib.Vertices.Core.Input.Events;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI.Controls;


#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.GUI.GuiArtProvider;
#endregion

namespace Virtex.Lib.Vertices.Screens.Menus
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class vxMenuBaseScreen : vxGameBaseScreen
    {
        #region Properties


        /// <summary>
        /// The Art provider for the Menu Screen.
        /// </summary>
        public vxMenuScreenArtProvider ArtProvider { get; internal set; }

        /// <summary>
        /// The Art provider for the Menu Items.
        /// </summary>
        //public vxMenuItemArtProvider MenuItemArtProvider { get; internal set; }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        public IList<vxMenuEntry> MenuEntries
		{
			get { return menuEntries; }
		}
		List<vxMenuEntry> menuEntries = new List<vxMenuEntry>();
        

        /// <summary>
        /// Menu Screen Title
        /// </summary>
        public string MenuTitle
        {
            get { return menuTitle; }
            set { menuTitle = value; }
        }
        string menuTitle;


		int selectionIndex;
        int selectedEntry = 0;
        

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxMenuBaseScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Handle Input

        public override void LoadContent()
        {
            base.LoadContent();
            
            //Initialise Art Providers.
            ArtProvider = vxEngine.vxGUITheme.ArtProviderForMenuScreen;
        }


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(vxInputManager input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputHelper helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex = PlayerIndex.One;

            if (input.IsMenuSelect())
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if (selectedEntry != -1)
            {
                menuEntries[entryIndex].OnSelectEntry(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
		public virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a vxMenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw

        public virtual void SetArtProvider(vxMenuScreenArtProvider NewArtProvider)
        {
            this.ArtProvider = NewArtProvider;
        }


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //
            //Set Menu Selection if Mouse is over
            //
            selectionIndex = 0;

            selectedEntry = -1;


            if (otherScreenHasFocus == false)
            {
                foreach (vxMenuEntry mEntry in MenuEntries)
                {
                    //Always set this to false intially
                    mEntry.IsSelected = false;
                    if (mEntry.HasFocus == true)
                        selectedEntry = selectionIndex;

                    selectionIndex++;
                }

                // Update each nested vxMenuEntry object.
                for (int i = 0; i < menuEntries.Count; i++)
                {
                    menuEntries[i].Update(vxEngine);
                }
            }

        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = vxEngine.SpriteBatch;

            spriteBatch.Begin();

            this.ArtProvider.Draw(this);

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
				vxMenuEntry vxMenuEntry = menuEntries[i];
                vxMenuEntry.Draw (vxEngine);
            }

            
            spriteBatch.End();
        }
        #endregion        
    }
}
