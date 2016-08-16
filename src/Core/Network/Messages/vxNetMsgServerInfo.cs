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
    public class vxNetmsgServerInfo : INetworkMessage
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
        public vxNetmsgServerInfo(string ServerName, string ServerIP, string ServerPort)
        {
            this.ServerName = ServerName;
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgServerInfo(NetIncomingMessage im)
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
#endif