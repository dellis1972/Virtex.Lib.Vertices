using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vxVertices.Network
{
    public class NetworkPlayerInfo
    {
        public string Name;
        public int Level;
        public string Token;

        //A Bool to see if the player is ready in the lobby.
        public bool Ready = false;

        //a bool to hold whether the player has loaded the level.
        public bool Loaded = false;

        public NetworkPlayerInfo(string name, int level, string token, bool ready)
        {
            Name = name;
            Level = level;
            Token = token;
            Ready = ready;
        }
    }

    /// <summary>
    /// Connection status for networked games.
    /// </summary>
    public enum vxEnumNetworkConnectionStatus
    {
		/// <summary>
		/// Currently Connected.
		/// </summary>
		Running,

		/// <summary>
		/// Connection has Stopped.
		/// </summary>
		Stopped,

		/// <summary>
		/// The connection has timed out.
		/// </summary>
		TimedOut
	}

	/// <summary>
	/// What is the players role in Networked games.
	/// </summary>
	public enum vxEnumNetworkPlayerRole
    {
		/// <summary>
		/// The player is the server for the game.
		/// </summary>
		Server,

		/// <summary>
		/// The player is a client for the game.
		/// </summary>
		Client,
	}
}
