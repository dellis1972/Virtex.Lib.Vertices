using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Utilities;
using vxVertices.Core;

namespace vxVertices.GUI.Controls
{
	/// <summary>
	/// Scrollbar Item base class. This can be inherited too expand controls within one scrollbar item.
	/// </summary>
    public class vxScrollBarItem : vxGUIBaseItem
    {
       
		/// <summary>
		/// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxScrollBarItem"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="text">Text.</param>
        public vxScrollBarItem(vxEngine vxEngine, string text)
        {
			Padding = 4;
            
            this.vxEngine = vxEngine;    

			Width = 500;
			Height = 64;

            Text = text;            
			BoundingRectangle = new Rectangle(0, 0, Width, Height);

            //Width = 3000;

            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Color_Highlight = Color.DarkOrange;
            Colour_Text = Color.LightGray;

			this.Font = vxEngine.Assets.Fonts.MenuFont;
        }


		/// <summary>
		/// Uns the select.
		/// </summary>
        public virtual void UnSelect()
        {
            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Colour_Text = Color.LightGray;
        }

		/// <summary>
		/// Select Method
		/// </summary>
		public virtual void ThisSelect()
        {
            Color_Normal = Color.DarkOrange;
            Colour_Text = Color.Black;
        }
        
		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //
            //Update Rectangle
            //
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y),
                Width, Height);
            
            //
            //Draw Button
            //
            float i = 1;
            vxEngine.SpriteBatch.Begin();

            if (IsSelected)
                ThisSelect();
            else
                UnSelect();
            if (HasFocus)
            {
                i = 1.250f;
            }

            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color_Normal * i);
            
			vxEngine.SpriteBatch.DrawString(this.Font, Text, 
				new Vector2((int)(Position.X + Padding), (int)(Position.Y + Padding)),
                Colour_Text);
            
            vxEngine.SpriteBatch.End();
        }
    }
}
