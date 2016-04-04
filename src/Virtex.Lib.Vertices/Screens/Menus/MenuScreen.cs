#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using vxVertices.Core.Input;
using vxVertices.Core.Input.Events;
using vxVertices.Core;
using vxVertices.GUI.Controls;


#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using vxVertices.Utilities;
#endregion

namespace vxVertices.Screens.Menus
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
		#region Properties

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
		/// Gets or sets the menu start position.
		/// </summary>
		/// <value>The menu start position.</value>
		public Vector2 MenuStartPosition { 
			get{ return menuStartPosition; }
			set{ menuStartPosition = value; }
		}
		Vector2 menuStartPosition = new Vector2(200, 200);
		Vector2 position = new Vector2(0, 0);

		/// <summary>
		/// Gets or sets the offset between Menu Item
		/// </summary>
		/// <value>The offset for the next menu item.</value>
		public Vector2 NextMenuItemOffset { 
			get{ return nextMenuItemOffset; }
			set{ nextMenuItemOffset = value; }
		}
		Vector2 nextMenuItemOffset = new Vector2(0,0);



        /// <summary>
        /// Menu Screen Title
        /// </summary>
        public string MenuTitle
        {
            get { return menuTitle; }
            set { menuTitle = value; }
        }
        string menuTitle;

        /// <summary>
        /// Title Position.
        /// </summary>
        public Vector2 TitlePosition
        {
            get { return titlePosition; }
            set { titlePosition = value; }
        }
		public Vector2 titlePosition = new Vector2(0,0);

        /// <summary>
        /// Is there a background image on the title
        /// </summary>
        public bool DoTitleBackground
        {
            get { return doTitleBackground; }
            set { doTitleBackground = value; }
        }
        public bool doTitleBackground = true;

		int selectionIndex;
        int selectedEntry = 0;

        MouseState mouseState;
        int offset;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
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
            titlePosition = new Vector2(vxEngine.GraphicsDevice.Viewport.Width / 2, 80);

			//Set Menu Item Location
			if(vxEngine.vxGUITheme.vxMenuEntries.TextJustification == vxVertices.GUI.TextJustification.Left)
				menuStartPosition = new Vector2(200, 200);
			else if(vxEngine.vxGUITheme.vxMenuEntries.TextJustification == vxVertices.GUI.TextJustification.Center)
				menuStartPosition = new Vector2(vxEngine.GraphicsDevice.Viewport.Width / 2, 200);

            titlePosition = vxEngine.vxGUITheme.vxMenuEntries.TitlePosition;
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


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //Set the Top Menu Start Position
			position = menuStartPosition;

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
				vxMenuEntry vxMenuEntry = menuEntries[i];
				NextMenuItemOffset = new Vector2 (0, vxMenuEntry.Height + vxEngine.vxGUITheme.vxMenuEntries.Padding.Y);    
                
				//Set Menu Item Location
				if(vxEngine.vxGUITheme.vxMenuEntries.TextJustification == vxVertices.GUI.TextJustification.Left)
                	position.X = menuStartPosition.X;// -vxMenuEntry.Width / 2;
				else if(vxEngine.vxGUITheme.vxMenuEntries.TextJustification == vxVertices.GUI.TextJustification.Center)
						position.X = menuStartPosition.X - vxMenuEntry.Width / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                vxMenuEntry.Position = position;
                //vxConsole.WriteToInGameDebug(position);

                // move down for the next entry the size of this entry
				position += NextMenuItemOffset;
            }
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
            mouseState = Mouse.GetState();
            selectionIndex = 0;

            offset = 10;
            selectedEntry = -1;

			foreach (vxMenuEntry mEntry in MenuEntries)
            {
                //Always set this to false intially
                mEntry.IsSelected = false;
				if(mEntry.HasFocus == true)
					selectedEntry = selectionIndex;
				
				selectionIndex++;
            }

            // Update each nested vxMenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
				menuEntries [i].Update (mouseState);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = vxEngine.GraphicsDevice;
            SpriteBatch spriteBatch = vxEngine.SpriteBatch;
			SpriteFont font = vxEngine.vxGUITheme.Font;

            spriteBatch.Begin();
			/*
            //Draw Background
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank,
                new Rectangle(50 - buffer, 0, menuBackWidth + 2 * buffer, vxEngine.GraphicsDevice.Viewport.Height), Color.Orange * TransitionAlpha);
			*/
            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
				vxMenuEntry vxMenuEntry = menuEntries[i];
				vxMenuEntry.Draw (vxEngine);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);



            // Draw the menu title centered on the screen
            //titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            //Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = vxEngine.vxGUITheme.vxMenuEntries.TitleColor * TransitionAlpha;

            float titleScale = 1;



            //titlePosition.Y -= transitionOffset * 100;
			Rectangle BoundingRectangle = new Rectangle(
				(int)(titlePosition.X - font.MeasureString(menuTitle).X/2 - vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.X), 
				(int)(titlePosition.Y - font.MeasureString(menuTitle).Y/2 - vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.Y),
				(int)(font.MeasureString(menuTitle).X + vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.X), 
				(int)(font.MeasureString(menuTitle).Y + vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.Y));

            if (DoTitleBackground)
            {
				vxEngine.SpriteBatch.Draw(vxEngine.vxGUITheme.vxMenuEntries.TitleBackground, 
					BoundingRectangle, vxEngine.vxGUITheme.vxMenuEntries.TitleBackgroundColor * TransitionAlpha);
            }

			spriteBatch.DrawString(font, menuTitle, titlePosition - (font.MeasureString(menuTitle)+vxEngine.vxGUITheme.vxMenuEntries.TitlePadding)/2, 
				vxEngine.vxGUITheme.vxMenuEntries.TitleColor * TransitionAlpha,
				0, Vector2.Zero, titleScale, SpriteEffects.None, 0);
            
            spriteBatch.End();
        }


        #endregion
    }
}
