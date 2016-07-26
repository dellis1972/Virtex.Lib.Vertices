using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Mathematics;

namespace Virtex.Lib.Vertices.GUI.Controls
{
	/// <summary>
	/// Label Class providing simple one line text as a vxGUI Item.
	/// </summary>
	public class vxLabel : vxGUIBaseItem
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vertices.GUI.Controls.vxLabel"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		public vxLabel(vxEngine vxEngine, string text, Vector2 position):base(position)
        {
			this.vxEngine = vxEngine;

            Text = text;

			Colour_Text = vxEngine.vxGUITheme.vxLabelColorNormal;


            this.Font = vxEngine.vxGUITheme.Font;
            Width = (int)this.Font.MeasureString(Text).X;

            Width = (int)this.Font.MeasureString (Text).X;
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), Width, (int)this.Font.MeasureString(Text).Y + Padding / 2);
        }


		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            //Draw the Text Box
            vxEngine.SpriteBatch.Begin();
            this.DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            Width = (int)this.Font.MeasureString(Text).X;
            vxEngine.SpriteBatch.DrawString(this.Font, Text, new Vector2(Position.X, Position.Y), Colour_Text);
        }
    }
}
