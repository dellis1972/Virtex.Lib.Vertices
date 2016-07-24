#if VRTC_INCLDLIB_NET 
using System.Threading;

using Microsoft.Xna.Framework;

using vxVertices.Utilities;
using vxVertices.Network;

using Lidgren.Network;
using System;
using vxVertices.Network.Events;
using vxVertices.Mathematics;
using System.Collections.Generic;

namespace vxVertices.Core
{


	public partial class vxEngine : DrawableGameComponent
    {


		/// <summary>
		/// Network boolean of whether or not the current player is logged in or not.
		/// </summary>
		public bool IsLoggedIn = false;

		/// <summary>
		/// The s client.
		/// </summary>
		private NetClient MasterSeverClient;

        /// <summary>
        /// The Game Server Object used to Manage Games when the user is the Server.
        /// </summary>
        public NetServer GameSever;

        /// <summary>
        /// The Game Client Object used to Manage Games when the user is the Client.
        /// </summary>
        public NetClient GameClient;

        /// <summary>
        /// A Collection of Players Currently In The Server. This should only be modified only by updates from the connected server.
        /// </summary>
        public List<NetworkPlayerInfo> NetworkPlayers = new List<NetworkPlayerInfo>();

        /// <summary>
        /// Gets or sets the connection status for the Master Server Connection.
        /// </summary>
        /// <value>The connection status.</value>
        public vxEnumNetworkConnectionStatus MasterServerConnectionStatus {get;set;}

        /// <summary>
        /// Gets or sets the connection status for Games.
        /// </summary>
        public vxEnumNetworkConnectionStatus GameConnectionStatus { get; set; }

        /// <summary>
        /// What is the roll of this user in the Networked Game.
        /// </summary>
        public vxEnumNetworkPlayerRole NetworkedGameRoll { get; set; }


        public void InitialiseMasterServerConnection()
		{
			vxConsole.WriteLine("Setting Up Network System...");
			MasterServerConnectionStatus = vxEnumNetworkConnectionStatus.Stopped;
			var config = new NetPeerConfiguration("Virtex_Main_Server");
			MasterSeverClient = new NetClient(config);
			MasterSeverClient.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 
		}


		/// <summary>
		/// Method for Receiving Messages kept in a Global Scope for the Engine.
		/// </summary>
		/// <param name="peer">Peer.</param>
		public void GotMessage(object peer)
		{
			NetIncomingMessage im;
			while ((im = MasterSeverClient.ReadMessage()) != null)
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
						MasterServerConnectionStatus = vxEnumNetworkConnectionStatus.Running;
					else
						MasterServerConnectionStatus = vxEnumNetworkConnectionStatus.Stopped;

					if (status == NetConnectionStatus.Disconnected)
					{
						MasterServerConnectionStatus = vxEnumNetworkConnectionStatus.Stopped;
					}

					string reason = im.ReadString();
					vxConsole.WriteNetworkLine(status.ToString() + " : " + reason);

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
				MasterSeverClient.Recycle(im);
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

			MasterSeverClient.Start();
			NetOutgoingMessage hail = MasterSeverClient.CreateMessage(HailMsg);
			MasterSeverClient.Connect(ipAddress, port, hail);

			vxConsole.WriteNetworkLine("Done!");
		}

		/// <summary>
		/// Shutdown the connection with the specified ShutdownMsg.
		/// </summary>
		/// <param name="ShutdownMsg">Shutdown message.</param>
		public void Shutdown(string ShutdownMsg)
		{
			vxConsole.WriteNetworkLine("Disconnecting from Server...");

			MasterSeverClient.Disconnect(ShutdownMsg);
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
				var message = MasterSeverClient.CreateMessage();
				message.Write(stringToSend);
				MasterSeverClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
				vxConsole.WriteNetworkLine("Sending Message: " + stringToSend);
			}
		}

        int local = -50;
        int Req = -50;
        /// <summary>
        /// Small Debug Utility that draws to the screen Connection Info. Debug Purposes only.
        /// </summary>
        void DrawNetworkGameConnectionInfo()
        {
            Req = -50;
            SpriteBatch.Begin();
            if (GameSever != null)
            {
                Req = 5;
                   local = vxSmooth.SmoothInt(local, Req, 8);
                string output = string.Format(
                    "NETWORK DEBUG INFO: | User Roll: {3} | Server Name: {0} | Port: {1} | Broadcast Address: {2} | Status: {4}",
                    GameSever.Configuration.AppIdentifier,
                    GameSever.Configuration.Port.ToString(),
                    GameSever.Configuration.BroadcastAddress,
                    this.NetworkedGameRoll.ToString(),
                    GameSever.Status.ToString());

                int pad = 3;
                
                SpriteBatch.Draw(this.Assets.Textures.Blank, new Rectangle(0, local + 0, 1000, (int)this.Assets.Fonts.DebugFont.MeasureString(output).Y + 2 * pad), Color.Black * 0.75f);
                SpriteBatch.DrawString(this.Assets.Fonts.DebugFont, output, new Vector2(pad, local + pad), Color.White);
            }
            else if (GameClient != null)
            {
                Req = 5;
                local = vxSmooth.SmoothInt(local, Req, 8);
                string output = string.Format(
                    "NETWORK DEBUG INFO: | User Roll: {3} | Client Name: {0} | Port: {1} | Broadcast Address: {2} | Status: {4}",
                    GameClient.Configuration.AppIdentifier,
                    GameClient.Configuration.Port.ToString(),
                    GameClient.Configuration.BroadcastAddress,
                    this.NetworkedGameRoll.ToString(),
                    GameClient.Status.ToString());

                int pad = 3;

                SpriteBatch.Draw(this.Assets.Textures.Blank, new Rectangle(0, local + 0, 1000, (int)this.Assets.Fonts.DebugFont.MeasureString(output).Y + 2 * pad), Color.Black * 0.75f);
                SpriteBatch.DrawString(this.Assets.Fonts.DebugFont, output, new Vector2(pad, local + pad), Color.White);
            }

            SpriteBatch.End();
        }
		
    }
}

#endif