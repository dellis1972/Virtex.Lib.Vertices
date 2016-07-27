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
    public class vxNetMsgUpdatePlayerList : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public List<vxNetPlayerInfo> Players { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetMsgUpdatePlayerList(vxNetPlayerManager playerManager)
        {
            Players = new List<vxNetPlayerInfo>();

            //Translate the Dictionary into A list of players.
            foreach (KeyValuePair<long, vxNetPlayerInfo> entry in playerManager.Players)
            {
                Players.Add(entry.Value);
            }
        }
        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetMsgUpdatePlayerList(NetIncomingMessage im)
        {
            Players = new List<vxNetPlayerInfo>();
            this.DecodeMsg(im);
        }

        /// <summary>
        /// The Message Type
        /// </summary>
        public vxNetworkMessageTypes MessageType
        {
            get
            {
                return vxNetworkMessageTypes.UpdatePlayersList;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            int Count = Convert.ToInt32(im.ReadString());

            for (int i = 0; i < Count; i++)
            {
                Players.Add(new vxNetPlayerInfo(
                im.ReadInt64(),
                im.ReadString(),
                (vxEnumNetPlayerStatus)im.ReadInt32()));
            }
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            //First Write the number of elements
            int Count = Players.Count;

            om.Write(Count.ToString());

            foreach (vxNetPlayerInfo plyr in Players)
            {
                int enumIndex = (int)plyr.Status;

                om.Write(plyr.ID);
                om.Write(plyr.UserName);
                om.Write(enumIndex);
            }
        }
    }
}
