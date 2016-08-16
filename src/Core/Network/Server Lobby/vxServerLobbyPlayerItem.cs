#if VRTC_INCLDLIB_NET 
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.Network;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// File Chooser Dialor Item.
    /// </summary>
    public class vxServerLobbyPlayerItem : vxScrollPanelItem
    {
        /// <summary>
        /// Get's the Player's Info
        /// </summary>
        public vxNetPlayerInfo Player;

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
            vxNetPlayerInfo player,
            Vector2 Position,
            Texture2D buttonImage,
            int ElementIndex):base(vxEngine, player.UserName, Position, buttonImage, ElementIndex)
        {
            Player = player;
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            vxEngine.SpriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, "Status: " +vxEngine.ClientManager.PlayerManager.Players[Player.ID].Status,
                new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + vxEngine.vxGUITheme.Font.MeasureString(Text).Y + 10)),
    Colour_Text);

        }
    }
}
#endif