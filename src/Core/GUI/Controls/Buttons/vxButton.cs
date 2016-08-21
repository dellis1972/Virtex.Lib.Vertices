using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Virtex.Lib.Vrtc.GUI.GuiArtProvider;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public class vxButton : vxGUIBaseItem
    {
		vxButtonArtProvider ArtProvider {get; set;}

        /// <summary>
        /// Gets or sets the texture for this Menu Entry Background.
        /// </summary>
        /// <value>The texture.</value>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxButton"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="text">Text.</param>
        /// <param name="position">Position.</param>
		public vxButton(vxEngine vxEngine, string text, Vector2 position)
            : base(position)
        {
            //Text
            this.Text = text;

            //Engine
            this.vxEngine = vxEngine;

            //Set up Font
            this.Font = vxEngine.vxGUITheme.Font;

            //Get Settings
//            this.Color_Normal = vxEngine.vxGUITheme.vxButtons.BackgroundColour;
//            this.Color_Highlight = vxEngine.vxGUITheme.vxButtons.BackgroundHoverColour;
//            this.Colour_Text = vxEngine.vxGUITheme.vxButtons.TextColour;
//
//            //Set Width and Height
//            Width = Math.Max(vxEngine.vxGUITheme.vxButtons.Width, (int)(this.Font.MeasureString(Text).X + Padding * 2));
//            Height = Math.Max(vxEngine.vxGUITheme.vxButtons.Height, (int)this.Font.MeasureString(Text).Y + Padding * 2);
//
            BoundingRectangle = new Rectangle(
                (int)(Position.X - Padding),
                (int)(Position.Y - Padding / 2),
                Width, Height);
//
//
//            BackgroundTexture = vxEngine.vxGUITheme.vxButtons.BackgroundImage;

			//Have this button get a clone of the current Art Provider
			this.ArtProvider = (vxButtonArtProvider)vxEngine.vxGUITheme.ArtProviderForButtons.Clone ();

			this.OnInitialHover += this_OnInitialHover;
			this.Clicked += this_Clicked;
		}

		private void this_OnInitialHover(object sender, EventArgs e)
        {
            //If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
#if !NO_DRIVER_OPENAL
            SoundEffectInstance MenuHighlight = vxEngine.vxGUITheme.SE_Menu_Hover.CreateInstance();
            MenuHighlight.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume / 6;
            MenuHighlight.Play();

#endif
		}

		void this_Clicked (object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
		{
			#if !NO_DRIVER_OPENAL
			SoundEffectInstance equipInstance = vxEngine.vxGUITheme.SE_Menu_Confirm.CreateInstance();
			equipInstance.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume;
			equipInstance.Play();
			#endif
		}

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            //Draw Button            
            vxEngine.SpriteBatch.Begin();
            this.DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }


        public override void DrawByOwner(vxEngine vxEngine)
        {
            //Update Rectangle
            BoundingRectangle = new Rectangle(
                (int)(Position.X - Padding),
                (int)(Position.Y - Padding / 2),
                Width, Height);

			//Now get the Art Provider to draw the scene
			this.ArtProvider.Draw (this);
        }
    }
}
