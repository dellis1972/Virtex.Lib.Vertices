using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace Virtex.Lib.Vertices.Network.Messages
{
    /// <summary>
    /// This message is used during the discovery phase to glean basic server information.
    /// </summary>
    public class vxNetmsgUpdatePlayerLobbyStatus : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public vxNetPlayerInfo PlayerInfo { get; set; }

            /// <summary>
            /// Initialization Constructor to be used on Server Side.
            /// </summary>
            /// <param name="ServerName"></param>
            /// <param name="ServerIP"></param>
            /// <param name="ServerPort"></param>
        public vxNetmsgUpdatePlayerLobbyStatus(vxNetPlayerInfo playerinfo)
        {
            this.PlayerInfo = playerinfo;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgUpdatePlayerLobbyStatus(NetIncomingMessage im)
        {
            this.DecodeMsg(im);
        }

        /// <summary>
        /// The Message Type
        /// </summary>
        public vxNetworkMessageTypes MessageType
        {
            get
            {
                return vxNetworkMessageTypes.UpdatePlayerLobbyStatus;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            PlayerInfo = new vxNetPlayerInfo(
                im.ReadInt64(),
                im.ReadString(),
                (vxEnumNetPlayerStatus)im.ReadInt32());
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            int enumIndex = (int)this.PlayerInfo.Status;

            om.Write(this.PlayerInfo.ID);
            om.Write(this.PlayerInfo.UserName);
            om.Write(enumIndex);
        }
    }
}
