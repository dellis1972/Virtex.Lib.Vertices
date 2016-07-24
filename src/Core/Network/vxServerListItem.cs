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
    public class vxServerListItem : vxScrollPanelItem
    {
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
            int ElementIndex):base(vxEngine, ServerName, Position, buttonImage, ElementIndex)
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

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            vxEngine.SpriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, "Address: " + ServerAddress,
                new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + vxEngine.vxGUITheme.Font.MeasureString(Text).Y + 10)),
    Colour_Text);

        }
    }
}
