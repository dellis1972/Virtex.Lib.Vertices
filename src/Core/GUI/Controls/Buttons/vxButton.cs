using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using vxVertices.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace vxVertices.GUI.Controls
{
    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public class vxButton : vxGUIBaseItem
    {
        /// <summary>
        /// Gets or sets the texture for this Menu Entry Background.
        /// </summary>
        /// <value>The texture.</value>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxButton"/> class.
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
            this.Color_Normal = vxEngine.vxGUITheme.vxButtons.BackgroundColour;
            this.Color_Highlight = vxEngine.vxGUITheme.vxButtons.BackgroundHoverColour;
            this.Colour_Text = vxEngine.vxGUITheme.vxButtons.TextColour;

            //Set Width and Height
            Width = Math.Max(vxEngine.vxGUITheme.vxButtons.Width, (int)(this.Font.MeasureString(Text).X + Padding * 2));
            Height = Math.Max(vxEngine.vxGUITheme.vxButtons.Height, (int)this.Font.MeasureString(Text).Y + Padding * 2);

            BoundingRectangle = new Rectangle(
                (int)(Position.X - Padding),
                (int)(Position.Y - Padding / 2),
                Width, Height);


            BackgroundTexture = vxEngine.vxGUITheme.vxButtons.BackgroundImage;

            this.OnInitialHover += VxMenuEntry_OnInitialHover;
        }

        private void VxMenuEntry_OnInitialHover(object sender, EventArgs e)
        {
            //If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
            SoundEffectInstance MenuHighlight = vxEngine.vxGUITheme.SE_Menu_Hover.CreateInstance();
            MenuHighlight.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume / 6;
            MenuHighlight.Play();
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


            Rectangle BorderRectangle = new Rectangle(
                (int)(Position.X - Padding) - vxEngine.vxGUITheme.vxButtons.BorderWidth,
                (int)(Position.Y - Padding / 2) - vxEngine.vxGUITheme.vxButtons.BorderWidth,
                Width + vxEngine.vxGUITheme.vxButtons.BorderWidth * 2, 
                Height + vxEngine.vxGUITheme.vxButtons.BorderWidth * 2);

            //Draw Button
            if(vxEngine.vxGUITheme.vxButtons.DoBorder)
                vxEngine.SpriteBatch.Draw(BackgroundTexture, BorderRectangle, Color.Black * Opacity);
            vxEngine.SpriteBatch.Draw(BackgroundTexture, BoundingRectangle, Colour * Opacity);
            vxEngine.SpriteBatch.DrawString(this.Font, Text,
                new Vector2(
                    Position.X + Width / 2 - this.Font.MeasureString(Text).X / 2 - Padding,
                    Position.Y + Height / 2 - this.Font.MeasureString(Text).Y / 2),
                Colour_Text * Opacity);
        }
    }
}
