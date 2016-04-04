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
	public class vxServerScrollBarItem : vxScrollBarItem
    {
        int ButtonHeight = 64;

		/// <summary>
		/// The width of the button.
		/// </summary>
        public int ButtonWidth = 512;

		//TODO: Should this be kept in?
		/// <summary>
		/// The IP address.
		/// </summary>
        public string IPAddress = "";

		/// <summary>
		/// The port.
		/// </summary>
        public string Port = "";

        
                
        /// <summary>
        /// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxScrollBarItem"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="ipAddress">Ip address.</param>
        /// <param name="port">Port.</param>
		public vxServerScrollBarItem(vxEngine vxEngine, string ipAddress, string port):base(vxEngine, ipAddress)
        {
			Padding = 4;
            
            this.vxEngine = vxEngine;

            IPAddress = ipAddress;

            Text = IPAddress;
            Port = port;
            
            BoundingRectangle = new Rectangle(0, 0, 400, 64);


            Width = 3000;

            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Color_Highlight = Color.DarkOrange;
            Colour_Text = Color.LightGray;
        }


		/// <summary>
		/// Uns the select.
		/// </summary>
        public void UnSelect()
        {
            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Colour_Text = Color.LightGray;
        }

		/// <summary>
		/// Select Method
		/// </summary>
        public void ThisSelect()
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
            
			vxEngine.SpriteBatch.DrawString(vxEngine.vxGUITheme.Font, "Name: " + Text, 
                new Vector2((int)(Position.X + ButtonHeight + Padding*2), (int)(Position.Y + 8)),
                Colour_Text);


            vxEngine.SpriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, "Port: " + Port,
				new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + vxEngine.vxGUITheme.Font.MeasureString(Text).Y+10)),
    Colour_Text);
            
            vxEngine.SpriteBatch.End();
        }
    }
}
