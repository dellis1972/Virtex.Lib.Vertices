using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Screens.Menus;
using Virtex.Lib.Vrtc.Utilities;
using Microsoft.Xna.Framework.Audio;
using Virtex.Lib.Vrtc.GUI.GuiArtProvider;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
	/// <summary>
	/// Basic Button GUI Control.
	/// </summary>
	public class vxMenuEntry : vxGUIBaseItem
    {
		/// <summary>
		/// The Parent Menu Screen for this Menu Entry.
		/// </summary>
		public vxMenuBaseScreen ParentScreen { get; set; }

        /// <summary>
        /// Gets or Sets the Icon for this Menu Entry.
        /// </summary>
        public Texture2D Icon { get; set; }

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
        /// The given Art Provider of the Menu Entry. 
        /// </summary>
        public vxMenuItemArtProvider ArtProvider { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxMenuEntry"/> class.
        /// </summary>
        /// <param name="ParentScreen">Parent screen.</param>
        /// <param name="text">Text.</param>
		public vxMenuEntry(vxMenuBaseScreen ParentScreen, string text)
            : this(ParentScreen , text, null)
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxMenuEntry"/> class.
        /// </summary>
        /// <param name="ParentScreen"></param>
        /// <param name="text"></param>
        /// <param name="icon"></param>
        public vxMenuEntry(vxMenuBaseScreen ParentScreen, string text, Texture2D icon)
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

            this.Icon = icon;

			//Engine
			//this.vxEngine = vxEngine;

			//Get Settings
			this.Color_Normal = vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.BackgroundColour;
			this.Color_Highlight = vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.BackgroundHoverColour;

			//Set up Bounding Rectangle
			BoundingRectangle = new Rectangle(
				(int)(Position.X - vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Padding.X/2), 
				(int)(Position.Y - vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Padding.Y/2), 
				(int)(this.Font.MeasureString(Text).X + 2 * vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Padding.X), 
				(int)(this.Font.MeasureString(Text).Y + 2 * vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Padding.Y));

			Texture = vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.BackgroundImage;

            this.OnInitialHover += VxMenuEntry_OnInitialHover;

            this.ArtProvider = (vxMenuItemArtProvider)vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Clone();
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
			return (int)(vxEngine.vxGUITheme.Font.MeasureString(this.Text).X + vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Padding.X * 2);
        }

        public override int GetHeight()
        {
			return (int)(vxEngine.vxGUITheme.Font.MeasureString(this.Text).Y + vxEngine.vxGUITheme.ArtProviderForMenuScreenItems.Padding.Y * 2);
        }

        public virtual void SetArtProvider(vxMenuItemArtProvider NewArtProvider)
        {
            this.ArtProvider = (vxMenuItemArtProvider)NewArtProvider.Clone();
        }

        public override void Draw (vxEngine vxEngine)
		{
            this.ArtProvider.Draw(this);
		}
    }
}
