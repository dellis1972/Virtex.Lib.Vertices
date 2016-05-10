using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vxVertices.Network
{

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
