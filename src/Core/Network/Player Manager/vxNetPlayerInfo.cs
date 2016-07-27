using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Network
{
    public enum vxEnumNetPlayerStatus
    {
        /// <summary>
        /// The player has no status, and needs to be set before doing anything else.
        /// </summary>
        None,

        /// <summary>
        /// The player is unconnected and searching for a server.
        /// </summary>
        SearchingForServer,

        /// <summary>
        /// The player is in the lobby, but not ready to start the session.
        /// </summary>
        InServerLobbyNotReady,

        /// <summary>
        /// The player is in the lobby, and ready to start the session.
        /// </summary>
        InServerLobbyReady,

        /// <summary>
        /// The player is transitioning into the game session. This is usually used to keep track of which players
        /// in the session have loaded the level, and which ones are still loading.
        /// </summary>
        TransitioningToGame,

        /// <summary>
        /// The player is ready to play (or possibly is playing already)
        /// </summary>
        ReadyToPlay,

        /// <summary>
        /// The Player is transitioning back to the lobby.
        /// </summary>
        TransitioningToLobby
    }



    /// <summary>
    /// This is a base class which holds basic information for a player in the server/client system.
    /// </summary>
    public class vxNetPlayerInfo
    {
        /// <summary>
        /// Gets the ID of this Net Player. 
        /// </summary>
        public long ID { get; internal set; }

        public string UserName { get; internal set; }

        /// <summary>
        /// An Enumerator that holds where in the "Ready" phase the player is
        /// </summary>
        public vxEnumNetPlayerStatus Status;

        //A Bool to see if the player is ready in the lobby.
        public bool Ready = false;

        //a bool to hold whether the player has loaded the level.
        public bool Loaded = false;

        public vxNetPlayerInfo(long id, string username, vxEnumNetPlayerStatus netplayerstatus)
        {
            this.ID = id;
            this.UserName = username;
            this.Status = netplayerstatus;
        }
    }
}
