using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vertices.Core;

namespace Virtex.Lib.Vertices.GUI.Controls
{
	/// <summary>
	/// Drop down Combo Box GUI Control.
	/// </summary>
    public class vxComboBoxItem : vxGUIBaseItem
    {
		/// <summary>
		/// The length.
		/// </summary>
        //public int length = 150;

        /// <summary>
        /// Combo Box Item
        /// </summary>
        /// <param name="vxEngine"></param>
        /// <param name="text"></param>
        /// <param name="Index"></param>
        /// <param name="position"></param>
        public vxComboBoxItem(vxEngine vxEngine, string text, int Index, Vector2 position)
        {			
            this.vxEngine = vxEngine;

			this.Font = vxEngine.vxGUITheme.Font;

            Text = text;
            this.Index = Index;
            Position = position;
			Width = Math.Max(100, (int)(this.Font.MeasureString(Text).X + Padding * 2));

            BoundingRectangle = new Rectangle(
                (int)(Position.X - Padding), 
                (int)(Position.Y - Padding / 2), 
				Width, 
				(int)this.Font.MeasureString(Text).Y + Padding / 2);
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //Update Rectangle
			BoundingRectangle = new Rectangle((int)(Position.X-Padding), (int)(Position.Y-Padding/2), Width, (int)this.Font.MeasureString(Text).Y + Padding / 2);
            //Rectangle BackRectangle = new Rectangle((int)(Position.X - Padding)-1, (int)(Position.Y - Padding / 2)-1, length+2, (int)vxEngine.Font_GUI.MeasureString(Text).Y + Padding / 2+2);

            //Draw Button
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour);
			vxEngine.SpriteBatch.DrawString(this.Font, Text, new Vector2(Position.X + Width / 2 - this.Font.MeasureString(Text).X / 2 - Padding, Position.Y), Colour_Text);
            vxEngine.SpriteBatch.End();
        }
    }
}
