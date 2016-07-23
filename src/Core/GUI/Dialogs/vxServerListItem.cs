using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Utilities;
using vxVertices.Core;
using vxVertices.GUI.Events;

namespace vxVertices.GUI.Dialogs
{
	/// <summary>
	/// File Chooser Dialor Item.
	/// </summary>
    public class vxServerListItem : vxGUIBaseItem
    {
		/// <summary>
		/// The button image.
		/// </summary>
        public Texture2D ButtonImage;
		Rectangle ImageRectangle;

		/// <summary>
		/// The default width of the button.
		/// </summary>
		public int ButtonWidth = 512;
                

		int ButtonHeight = 64;

		/// <summary>
		/// The name of the Server.
		/// </summary>
		public string ServerName = "";

        /// <summary>
        /// The Server Addess
        /// </summary>
        public string ServerAddress = "";

        /// <summary>
        /// The Server Port
        /// </summary>
        public string ServerPort = "";

        /// <summary>
        /// Sets up a Serve List Dialog Item which holds information pertaining too a Discovered Server.
        /// </summary>
        /// <param name="vxEngine"></param>
        /// <param name="ServerName"></param>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <param name="Position"></param>
        /// <param name="buttonImage"></param>
        /// <param name="ElementIndex"></param>
        public vxServerListItem(vxEngine vxEngine, 
            string ServerName,
            string ServerAddress,
            string ServerPort,
            Vector2 Position, 
			Texture2D buttonImage, 
            int ElementIndex)
        {
			Padding = 4;

            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.ServerPort = ServerPort;
            
            Text = ServerName;
            this.Position = Position;
            OriginalPosition = Position;

            Index = ElementIndex;
            ButtonImage = buttonImage;
            BoundingRectangle = new Rectangle(0, 0, 64, 64);
            this.vxEngine = vxEngine;

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
            vxEngine.SpriteBatch.Begin();
            DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            //Console.WriteLine(this.Text + " _ " + this.Index);

            //
            //Update Rectangle
            //
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y),
                ButtonWidth, ButtonHeight);

            //
            //Draw Button
            //
            float i = 1;
            if (HasFocus)
            {
                i = 1.250f;
            }
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color_Normal * i);
            vxEngine.SpriteBatch.Draw(ButtonImage, new Rectangle((int)(Position.X + 2), (int)(Position.Y + 2),
                ButtonHeight - 4, ButtonHeight - 4), Color.LightGray * i);

            vxEngine.SpriteBatch.DrawString(vxEngine.vxGUITheme.Font, "Name: " + Text,
                new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + 8)),
                Colour_Text);
            vxEngine.SpriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, "Path: " + ServerAddress,
                new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + vxEngine.vxGUITheme.Font.MeasureString(Text).Y + 10)),
    Colour_Text);
        }
    }
}
