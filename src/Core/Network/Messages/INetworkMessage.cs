using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Network.Messages
{
    /// <summary>
    /// The Network Message Intergace for exchanging data between
    /// server and clients.
    /// </summary>
    public interface INetworkMessage
    {
        /// <summary>
        /// Gets MessageType.
        /// </summary>
        vxNetworkMessageTypes MessageType { get; }

        /// <summary>
        /// Decodes the Incoming Message
        /// </summary>
        /// <param name="im"></param>
        void DecodeMsg(NetIncomingMessage im);

        /// <summary>
        /// Encode's the Network Message.
        /// </summary>
        /// <param name="om"></param>
        void EncodeMsg(NetOutgoingMessage om);
    }

    /// <summary>
    /// Generic Message Types that can be sent by the Vertices Engine.
    /// </summary>
    public enum vxNetworkMessageTypes
    { 
        /// <summary>
        /// Sends Server Info back to a client. Usually done during the discovery signal handshake.
        /// </summary>
        ServerInfo,

        /// <summary>
        /// A basic status check while waiting in the lobby
        /// </summary>
        PlayerLobbyStatusRequest,

        /// <summary>
        /// A player has connected to the server.
        /// </summary>
        PlayerConnected,

        /// <summary>
        /// A player has disconnected from the server.
        /// </summary>
        PlayerDisconnected,

        /// <summary>
        /// Updates the status of the session.
        /// </summary>
        SessionStatus,


        /// <summary>
        /// An Item has been added to the server.
        /// </summary>
        AddItem,

        /// <summary>
        /// An item has been removed in the server.
        /// </summary>
        RemoveItem,

        /// <summary>
        /// Updates the Player list
        /// </summary>
        UpdatePlayersList,

        /// <summary>
        /// The update player state.
        /// </summary>
        UpdatePlayerLobbyStatus,
        
        /// <summary>
        /// Updates the State of a player. This is fired both during the heart beat as well as when ever a player presses a key, it updates
        /// it's state with the server, and the server updates all clients with the new information.
        /// </summary>
        UpdatePlayerEntityState,

        /// <summary>
        /// A Chat message is recieved.
        /// </summary>
        ChatMsgRecieved,

        /// <summary>
        /// A different type of Network Message that isn't covered by the defaults. You can handle how this is handeled 
        /// in the INetworkMessage inherited class Encoding and Decodings.
        /// </summary>
        Other,
    }
}
