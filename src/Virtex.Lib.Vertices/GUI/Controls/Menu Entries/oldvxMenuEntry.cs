#region File Description
//-----------------------------------------------------------------------------
// vxMenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using vxVertices.Core.Input.Events;
using vxVertices.Screens.Menus;
using vxVertices.Mathematics;
using vxVertices.Core;


#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace vxVertices.GUI.Controls
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class vxMenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

		public SpriteFont Font
		{

		}
		SpriteFont font;

        float CurrentFade = 0;
        public int BackWidth = 250;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Color color;


        public bool IsSelected;


        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public vxMenuEntry(string text)
        {
            this.text = text;
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }
        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in yellow, otherwise white.
            color = isSelected ? Color.Orange : Color.Black;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Tanh(time * 6) + 1;

            float scale = 1;// +pulsate * 0.03f * selectionFade;

			CurrentFade = vxSmooth.SmoothFloat(CurrentFade, selectionFade, 1);

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            vxEngine screenManager = screen.vxEngine;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font_GUI;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.Draw(screen.vxEngine.Assets.Textures.Blank,
                new Rectangle
                    (0,
                    (int)position.Y - (int)font.MeasureString(text).Y / 2,
                    screen.menuBackWidth + 50 + screen.buffer,
                    (int)font.MeasureString(text).Y),
                Color.Black * CurrentFade);

            spriteBatch.DrawString(font,
                text,
                position,
                color,
                0,
                origin,
                scale,
                SpriteEffects.None,
                0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.vxEngine.Font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.vxEngine.Font.MeasureString(Text).X;
        }


        #endregion
    }
}
