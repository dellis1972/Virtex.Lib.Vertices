#if VRTC_INCLDLIB_NET 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace Virtex.Lib.Vrtc.Network.Messages
{
    /// <summary>
    /// This message is used during the discovery phase to glean basic server information.
    /// </summary>
    public class vxNetmsgRemovePlayer : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public vxNetPlayerInfo PlayerInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetmsgRemovePlayer(vxNetPlayerInfo playerinfo)
        {
            this.PlayerInfo = playerinfo;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgRemovePlayer(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.PlayerDisconnected;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            PlayerInfo = new vxNetPlayerInfo(
                im.ReadInt64(),
                im.ReadString(),vxEnumNetPlayerStatus.None);
            string dummy = im.ReadString();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            om.Write(this.PlayerInfo.ID);
            om.Write(this.PlayerInfo.UserName);
            om.Write(this.PlayerInfo.Status.ToString());
        }
    }
}
#endif