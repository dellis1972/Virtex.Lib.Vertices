using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace Virtex.Lib.Vertices.Network.Messages
{
    /// <summary>
    /// This message is used to request a user update from a specific client.
    /// </summary>
    public class vxNetMsgServerToClientInfoRequest : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public string ServerName { get; internal set; }

        /// <summary>
        /// The Server's IP
        /// </summary>
        public string ServerIP { get; internal set; }


        /// <summary>
        /// The Server's Port
        /// </summary>
        public string ServerPort { get; internal set; }

        //public object UserData { get; internal set; }

            /// <summary>
            /// Initialization Constructor to be used on Server Side.
            /// </summary>
            /// <param name="ServerName"></param>
            /// <param name="ServerIP"></param>
            /// <param name="ServerPort"></param>
        public vxNetMsgServerToClientInfoRequest(string ServerName, string ServerIP, string ServerPort)
        {
            this.ServerName = ServerName;
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetMsgServerInfo(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.ServerInfo;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            this.ServerName = im.ReadString();
            this.ServerIP = im.ReadString();
            this.ServerPort = im.ReadString();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            om.Write(this.ServerName);
            om.Write(this.ServerIP);
            om.Write(this.ServerPort);
        }
    }
}
