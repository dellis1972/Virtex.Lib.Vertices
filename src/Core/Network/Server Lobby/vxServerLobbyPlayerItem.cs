using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI.Events;
using Virtex.Lib.Vertices.Network;

namespace Virtex.Lib.Vertices.GUI.Dialogs
{
    /// <summary>
    /// File Chooser Dialor Item.
    /// </summary>
    public class vxServerLobbyPlayerItem : vxScrollPanelItem
    {
        /// <summary>
        /// Get's the Player's Info
        /// </summary>
        public NetworkPlayerInfo Player;

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
        public vxServerLobbyPlayerItem(vxEngine vxEngine,
            NetworkPlayerInfo player,
            Vector2 Position,
            Texture2D buttonImage,
            int ElementIndex):base(vxEngine, player.Name, Position, buttonImage, ElementIndex)
        {
            Player = player;
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            vxEngine.SpriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, "Ready: " + Player.Ready,
                new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + vxEngine.vxGUITheme.Font.MeasureString(Text).Y + 10)),
    Colour_Text);

        }
    }
}
