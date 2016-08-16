using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
	/// <summary>
	/// List view item base class.
	/// </summary>
    public class vxListViewItem : vxGUIBaseItem
    {
        int ButtonHeight = 64;
        public int ButtonWidth = 512;

       /// <summary>
       /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.vxListViewItem"/> class.
       /// </summary>
       /// <param name="vxEngine">Vx engine.</param>
       /// <param name="Text">Text.</param>
        public vxListViewItem(vxEngine vxEngine, string Text)
        {
			Padding = 4;
            
            this.vxEngine = vxEngine;

			this.Font = vxEngine.vxGUITheme.Font;
            
            this.Text = Text;

			ButtonHeight = (int)this.Font.MeasureString(Text).Y + 2 * Padding;
            BoundingRectangle = new Rectangle(0, 0, 400, ButtonHeight);


            Width = 3000;

            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Color_Highlight = Color.DarkOrange;
            Colour_Text = Color.LightGray;
        }



        public void UnSelect()
        {
            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Colour_Text = Color.LightGray;
        }
        public void ThisSelect()
        {
            Color_Normal = Color.DarkOrange;
            Colour_Text = Color.Black;
        }
        
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //
            //Update Rectangle
            //
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y),
                ButtonWidth, ButtonHeight);
            
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
                new Vector2(
                    (int)(Position.X + Padding), 
                    (int)(Position.Y + Padding)),
                Colour_Text);
            
            vxEngine.SpriteBatch.End();
        }
    }
}
