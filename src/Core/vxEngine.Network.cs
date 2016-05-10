#if VRTC_INCLDLIB_NET 
using System.Threading;

using Microsoft.Xna.Framework;

using vxVertices.Utilities;
using vxVertices.Network;

using Lidgren.Network;
using System;
using vxVertices.Network.Events;


namespace vxVertices.Core
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
	public partial class vxEngine : DrawableGameComponent
    {
		/// <summary>
		/// Network boolean of whether or not the current player is logged in or not.
		/// </summary>
		public bool IsLoggedIn = false;

		/// <summary>
		/// The s client.
		/// </summary>
		private NetClient s_client;

		/// <summary>
		/// Gets or sets the connection status.
		/// </summary>
		/// <value>The connection status.</value>
		public vxEnumNetworkConnectionStatus ConnectionStatus {get;set;} 

		public void InitialiseNetwork()
		{
			vxConsole.WriteLine("Setting Up Network System...");
			ConnectionStatus = vxEnumNetworkConnectionStatus.Stopped;
			var config = new NetPeerConfiguration("Virtex_Main_Server");
			s_client = new NetClient(config);
			s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 
		}


		/// <summary>
		/// Method for Receiving Messages kept in a Global Scope for the Engine.
		/// </summary>
		/// <param name="peer">Peer.</param>
		public void GotMessage(object peer)
		{
			NetIncomingMessage im;
			while ((im = s_client.ReadMessage()) != null)
			{
				// handle incoming message
				switch (im.MessageType)
				{
				case NetIncomingMessageType.DebugMessage:
				case NetIncomingMessageType.ErrorMessage:
				case NetIncomingMessageType.WarningMessage:
				case NetIncomingMessageType.VerboseDebugMessage:
					string text = im.ReadString();
					vxConsole.WriteNetworkLine(im.MessageType + " : " + text);
					break;
				case NetIncomingMessageType.StatusChanged:
					NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

					if (status == NetConnectionStatus.Connected)
						ConnectionStatus = vxEnumNetworkConnectionStatus.Running;
					else
						ConnectionStatus = vxEnumNetworkConnectionStatus.Stopped;

					if (status == NetConnectionStatus.Disconnected)
					{
						ConnectionStatus = vxEnumNetworkConnectionStatus.Stopped;
					}

					string reason = im.ReadString();
					vxConsole.WriteNetworkLine(status.ToString() + ": " + reason);

					break;
				case NetIncomingMessageType.Data:

					// incoming chat message from a client
					string chat = im.ReadString();

					//Split the Text By Carriage Return
					string[] result = chat.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

					switch (result[0])
					{
					case "vrtx_serverList":
						vxConsole.WriteNetworkLine("Server List Recieved");

						if (GameServerListRecieved != null)
							GameServerListRecieved(this, new vxGameServerListRecievedEventArgs(result));

						break;
					}
					break;
				default:
					vxConsole.WriteNetworkLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
					break;
				}
				s_client.Recycle(im);
			}
		}

		/// <summary>
		/// Connect the specified ipAddress, port and HailMsg.
		/// </summary>
		/// <param name="ipAddress">Ip address.</param>
		/// <param name="port">Port.</param>
		/// <param name="HailMsg">Hail message.</param>
		public void Connect(string ipAddress, int port, string HailMsg)
		{
			vxConsole.WriteNetworkLine("Connecting to Server...");

			s_client.Start();
			NetOutgoingMessage hail = s_client.CreateMessage(HailMsg);
			s_client.Connect(ipAddress, port, hail);

			vxConsole.WriteNetworkLine("Done!");
		}

		/// <summary>
		/// Shutdown the connection with the specified ShutdownMsg.
		/// </summary>
		/// <param name="ShutdownMsg">Shutdown message.</param>
		public void Shutdown(string ShutdownMsg)
		{
			vxConsole.WriteNetworkLine("Disconnecting from Server...");

			s_client.Disconnect(ShutdownMsg);
			vxConsole.WriteNetworkLine("Done!");

		}

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="stringToSend">String to send.</param>
		public void SendMessage(string stringToSend)
		{
			if (stringToSend != "")
			{
				var message = s_client.CreateMessage();
				message.Write(stringToSend);
				s_client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
				vxConsole.WriteNetworkLine("Sending Message: " + stringToSend);
			}
		}
		
    }
}

#endif