using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    public class xButton_Toggle : vxGUIBaseItem
    {
        public int length = 150;

        /// <summary>
        /// Button Code
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="function"></param>
		public xButton_Toggle(vxEngine vxEngine, string text, Vector2 position)
        {
            Text = text;        
            Position = position;
            this.vxEngine = vxEngine;
			length = Math.Max(100, (int)(vxEngine.vxGUITheme.Font.MeasureString(Text).X + Padding * 2)); 
        }

        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);
        }

        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //
            //Update Rectangle
            //
            //int length = Math.Max(100, (int)(vxEngine.TinyFont.MeasureString(Text).X + Padding*2));            
			BoundingRectangle = new Rectangle((int)(Position.X-Padding), (int)(Position.Y-Padding/2), length, (int)vxEngine.vxGUITheme.Font.MeasureString(Text).Y + Padding / 2);
			Rectangle BackRectangle = new Rectangle((int)(Position.X - Padding)-1, (int)(Position.Y - Padding / 2)-1, length+2, (int)vxEngine.vxGUITheme.Font.MeasureString(Text).Y + Padding / 2+2);

            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BackRectangle, Color_Highlight);
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour);
			vxEngine.SpriteBatch.DrawString(vxEngine.vxGUITheme.Font, Text, new Vector2(Position.X + length / 2 - vxEngine.vxGUITheme.Font.MeasureString(Text).X / 2 - Padding, Position.Y), Colour_Text);
            vxEngine.SpriteBatch.End();
        }
    }
}
