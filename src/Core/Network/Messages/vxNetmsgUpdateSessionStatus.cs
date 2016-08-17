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
    public class vxNetmsgUpdateSessionStatus : INetworkMessage
    {
        public vxEnumSessionStatus SessionStatus { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetmsgUpdateSessionStatus(vxEnumSessionStatus status)
        {
            SessionStatus = status;
        }
        
        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgUpdateSessionStatus(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.SessionStatus;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            SessionStatus = (vxEnumSessionStatus)im.ReadInt32();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            om.Write((int)SessionStatus);
        }
    }
}
#endif