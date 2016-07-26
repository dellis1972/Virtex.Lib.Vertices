using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Core;

namespace Virtex.Lib.Vertices.GUI.Controls
{
    public class vxSandboxItemButton : vxGUIBaseItem
    {
		/// <summary>
		/// Gets or sets the button image.
		/// </summary>
		/// <value>The button image.</value>
		public Texture2D ButtonImage {get; set;}

        public string Key { get; set; }

		public vxSandboxItemButton(){
		}

        //TODO: Events should be used.
        public vxSandboxItemButton(
            vxEngine vxEngine, 
            Texture2D buttonImage,
            string Text,
            string ElementKey, 
            Vector2 position, 
            int Width, 
            int Height)
        {
            Position = position;

            this.Text = Text;

            this.Key = ElementKey;

            ButtonImage = buttonImage;
            BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            this.vxEngine = vxEngine;

            Color_Normal = Color.White;
            Color_Highlight = Color.LightYellow;

            this.Width = Width;
            this.Height = Height;
        }

		/// <summary>
		/// Updates the GUI Item
		/// </summary>
		/// <param name="mouseState">Mouse state.</param>
        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {

            //
            //Update Rectangle
            //     
            Padding = 2;
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), Width, Height + Padding / 2);
            Rectangle BackRectangle = new Rectangle((int)(Position.X - Padding) - 2, (int)(Position.Y - Padding / 2) - 2, Width + 4, Height + Padding / 2 + 4);

            //
            //Draw Button
            //
            //if (HasFocus)
            //{
                
            //    vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color.LightBlue);
            //}
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BackRectangle, HasFocus ? Color.DarkOrange : Color.Black);
            vxEngine.SpriteBatch.Draw(ButtonImage, BoundingRectangle, Colour);


            if (Text != null)
            {
                int BackHeight = (int)(BoundingRectangle.Height - vxEngine.Assets.Fonts.DebugFont.MeasureString(Text).Y - 10);

                vxEngine.SpriteBatch.Draw(
                    vxEngine.Assets.Textures.Blank,
                    new Rectangle(
                        BoundingRectangle.Location.X,
                        BoundingRectangle.Location.Y + BackHeight,
                        BoundingRectangle.Width,
                        BoundingRectangle.Height - BackHeight),
                    Color.Black * 0.5f);

                vxEngine.SpriteBatch.DrawString(
                    vxEngine.Assets.Fonts.DebugFont,
                    Text,
                    new Vector2(
                        BoundingRectangle.Location.X + 2,
                        BoundingRectangle.Location.Y + BackHeight + 5),
                    Color.LightGray);
            }
        }

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            vxEngine.SpriteBatch.Begin();
            this.DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }
    }
}
