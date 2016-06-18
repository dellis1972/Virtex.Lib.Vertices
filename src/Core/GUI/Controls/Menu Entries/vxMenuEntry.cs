using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using vxVertices.Core;
using vxVertices.Core.Input.Events;
using vxVertices.Screens.Menus;
using vxVertices.Utilities;
using Microsoft.Xna.Framework.Audio;

namespace vxVertices.GUI.Controls
{
	/// <summary>
	/// Basic Button GUI Control.
	/// </summary>
	public class vxMenuEntry : vxGUIBaseItem
    {
		/// <summary>
		/// The Parent Menu Screen for this Menu Entry.
		/// </summary>
		public MenuScreen ParentScreen { get; set; }

		/// <summary>
		/// Gets or sets the texture for this Menu Entry Background.
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture { get; set; }

		/// <summary>
		/// Event raised when the menu entry is selected.
		/// </summary>
		public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxMenuEntry"/> class.
        /// </summary>
        /// <param name="ParentScreen">Parent screen.</param>
        /// <param name="text">Text.</param>
		public vxMenuEntry(MenuScreen ParentScreen, string text)
			: base(Vector2.Zero)
		{			
			//Set Engine
			this.vxEngine = ParentScreen.vxEngine;

			//Set Font from Global Engine Font
			this.Font = vxEngine.vxGUITheme.Font;

			//Set Parten Screen
			this.ParentScreen = ParentScreen;

			//Text
			this.Text = text;

			//Engine
			this.vxEngine = vxEngine;

			//Get Settings
            this.Color_Normal = vxEngine.vxGUITheme.vxMenuEntries.BackgroundColour;
            this.Color_Highlight = vxEngine.vxGUITheme.vxMenuEntries.BackgroundHoverColour;

			//Set up Bounding Rectangle
			BoundingRectangle = new Rectangle(
				(int)(Position.X - vxEngine.vxGUITheme.vxMenuEntries.Padding.X/2), 
				(int)(Position.Y - vxEngine.vxGUITheme.vxMenuEntries.Padding.Y/2), 
				(int)(this.Font.MeasureString(Text).X + 2 * vxEngine.vxGUITheme.vxMenuEntries.Padding.X), 
				(int)(this.Font.MeasureString(Text).Y + 2 * vxEngine.vxGUITheme.vxMenuEntries.Padding.Y));

			Texture = vxEngine.vxGUITheme.vxMenuEntries.vxMenuItemBackground;

            this.OnInitialHover += VxMenuEntry_OnInitialHover;
        }

        private void VxMenuEntry_OnInitialHover(object sender, EventArgs e)
		{
#if !NO_DRIVER_OPENAL
            //If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
            SoundEffectInstance MenuHighlight = vxEngine.vxGUITheme.SE_Menu_Hover.CreateInstance();
            MenuHighlight.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume / 6;
            MenuHighlight.Play();
#endif
        }

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
		{
#if !NO_DRIVER_OPENAL
            SoundEffectInstance equipInstance = vxEngine.vxGUITheme.SE_Menu_Confirm.CreateInstance();
            equipInstance.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume;
            equipInstance.Play();
#endif

            if (Selected != null)
				Selected(this, new PlayerIndexEventArgs(playerIndex));
		}

        public override int GetWidth()
        {
			return (int)(vxEngine.vxGUITheme.Font.MeasureString(this.Text).X + vxEngine.vxGUITheme.vxMenuEntries.Padding.X * 2);
        }

        public override int GetHeight()
        {
			return (int)(vxEngine.vxGUITheme.Font.MeasureString(this.Text).Y + vxEngine.vxGUITheme.vxMenuEntries.Padding.Y * 2);
        }

        public override void Draw (vxEngine vxEngine)
		{
			//Update Rectangle
			BoundingRectangle = new Rectangle(
				(int)(Position.X - vxEngine.vxGUITheme.vxMenuEntries.Padding.X/2), 
				(int)(Position.Y - vxEngine.vxGUITheme.vxMenuEntries.Padding.Y/2), 
				(int)(this.Font.MeasureString(Text).X + 2 * vxEngine.vxGUITheme.vxMenuEntries.Padding.X), 
				(int)(this.Font.MeasureString(Text).Y + 2 * vxEngine.vxGUITheme.vxMenuEntries.Padding.Y));

			//Set Opacity from Parent Screen Transition Alpha
			Opacity = ParentScreen.TransitionAlpha;

			//Do a last second null check.
			if (Texture == null)
				Texture = vxEngine.Assets.Textures.Blank;
			
			//Draw Button
            if(vxEngine.vxGUITheme.vxMenuEntries.DrawBackgroungImage)
			    vxEngine.SpriteBatch.Draw(Texture, BoundingRectangle, Colour * Opacity);


			//Set Text Colour Based on Focus
            Colour_Text = HasFocus ? vxEngine.vxGUITheme.vxMenuEntries.TextHover : vxEngine.vxGUITheme.vxMenuEntries.TextColour;

			//Update Left Justifications
			if (vxEngine.vxGUITheme.vxMenuEntries.TextJustification == TextJustification.Left) {
				vxEngine.SpriteBatch.DrawString (this.Font, Text,
					new Vector2 (
						Position.X + Padding,
						Position.Y + Padding),
					Colour_Text * Opacity);

			}

			//Update Center Justifications
			else if (vxEngine.vxGUITheme.vxMenuEntries.TextJustification == TextJustification.Center){

			vxEngine.SpriteBatch.DrawString(
					this.Font, 
					Text, 
					Position + vxEngine.vxGUITheme.vxMenuEntries.Padding/2,
					Colour_Text * Opacity);
			}
		}
    }
}
