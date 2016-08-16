using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    /// <summary>
    /// A Seperator Used in the vxScrollPanel.
    /// </summary>
    public class vxScrollPanelSpliter : vxGUIBaseItem
    {
        /// <summary>
        /// Gets or sets the button image.fdsf
        /// </summary>
        /// <value>The button image.</value>
        public Texture2D ButtonImage { get; set; }

        public vxScrollPanelSpliter(
            vxEngine vxEngine,
            string Text)
        {
            this.vxEngine = vxEngine;

            this.Text = Text;

            Position = Vector2.Zero;

            ButtonImage = vxEngine.Assets.Textures.Blank;

            this.Width = 1500;
            this.Height = 36;

            BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            Color_Normal = Color.White;
            Color_Highlight = Color.LightBlue;
        }

        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            //
            //Update Rectangle
            //     
            Padding = 1;
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), Width, Height + Padding / 2);
            Rectangle BackRectangle = new Rectangle((int)(Position.X - Padding) - 2, (int)(Position.Y - Padding / 2) - 2, Width + 4, Height + Padding / 2 + 4);

            /*
           if (HasFocus)
           {
               vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BackRectangle, Color.LightBlue * 1.1f);
               vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color.LightBlue);
           }
           */
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BackRectangle, Color.Black);
            vxEngine.SpriteBatch.Draw(ButtonImage, BoundingRectangle, Color.DarkOrange);


            if (Text != null)
            {
                int BackHeight = 4;

                vxEngine.SpriteBatch.Draw(
                    vxEngine.Assets.Textures.Blank,
                    new Rectangle(
                        BoundingRectangle.Location.X,
                        BoundingRectangle.Location.Y + BackHeight + (int)vxEngine.Assets.Fonts.MenuFont.MeasureString(Text).Y,
                        BoundingRectangle.Width,
                        1),
                    Color.Black * 0.5f);
                
                vxEngine.SpriteBatch.DrawString(
                    vxEngine.Assets.Fonts.MenuFont,
                    Text,
                    new Vector2(
                        BoundingRectangle.Location.X + 5,
                        BoundingRectangle.Location.Y + BackHeight + 5),
                    Color.Black);
            }
        }

        public override void Draw(vxEngine vxEngine)
        {            
            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();

            DrawByOwner(vxEngine);

            vxEngine.SpriteBatch.End();
        }
    }
}
