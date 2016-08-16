#if VRTC_INCLDLIB_NET 
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Network.Messages;

namespace Virtex.Lib.Vrtc.Network
{
    public interface INetworkManager
    {
        /// <summary>
        /// Initialises The Network Manager
        /// </summary>
        void Init();

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        NetOutgoingMessage CreateMessage();

        /// <summary>
        /// The disconnect.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        NetIncomingMessage ReadMessage();

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        void Recycle(NetIncomingMessage im);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        void SendMessage(INetworkMessage gameMessage);
    }
}
#endif