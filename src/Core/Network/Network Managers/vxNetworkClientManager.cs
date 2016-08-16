#if VRTC_INCLDLIB_NET 
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Network.Events;
using Virtex.Lib.Vrtc.Network.Messages;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.Network
{
    public class vxNetworkClientManager : INetworkManager
    {
        #region Constants and Fields

        public vxEngine Engine { get; set; }

        /// <summary>
        /// Player Info
        /// </summary>
        public vxNetPlayerInfo PlayerInfo;

        public vxEnumSessionStatus SessionStatus = vxEnumSessionStatus.WaitingForPlayers;

        /// <summary>
        /// The NetPeer Server object
        /// </summary>
        public NetClient Client
        {
            get { return netClient; }
            set { netClient = value; }
        }
        private NetClient netClient;

        /// <summary>
        /// The is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed; }
            set { isDisposed = value; }
        }
        private bool isDisposed;

        public vxNetPlayerManager PlayerManager { get; internal set; }

        SendOrPostCallback ClientCallBackLoop;

        public string UserName { get; set; }

        #endregion


        #region Events

        /// <summary>
        /// This event is fired on the client side whenever a new player connects to the server.
        /// </summary>
        public event EventHandler<vxNetClientEventDiscoverySignalResponse> DiscoverySignalResponseRecieved;

        /// <summary>
        /// This event is fired whenever this player connects to the server.
        /// </summary>
        public event EventHandler<vxNetClientEventConnected> Connected;

        /// <summary>
        /// This event is fired whenever this player disconnects from the server.
        /// </summary>
        public event EventHandler<vxNetClientEventDisconnected> Disconnected;

        /// <summary>
        /// This event is fired on the client side whenever a new player connects to the server.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerConnected> OtherPlayerConnected;

        /// <summary>
        /// This event is fired on the client side whenever a player disconnects from the server.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerDisconnected> OtherPlayerDisconnected;

        /// <summary>
        /// When ever new information of a player is recieved.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerStatusUpdate> UpdatedPlayerInfoRecieved;

        /// <summary>
        /// When the server updates the session status.
        /// </summary>
        public event EventHandler<vxNetClientEventSessionStatusUpdated> UpdateSessionStatus;


        /// <summary>
        /// This event fires when an updated Entity State is recieved from the Server.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerStateUpdate> UpdatePlayerEntityState;


        #endregion

        public vxNetworkClientManager(vxEngine engine)
        {
            this.Engine = engine;

            List<string> UserNames = new List<string>();
            UserNames.Add("Kilijette");
            UserNames.Add("Infamousth");
            UserNames.Add("IteMon");
            UserNames.Add("Lauthro");
            UserNames.Add("Fantilis");
            UserNames.Add("Gramtron");
            UserNames.Add("HappyCent");
            UserNames.Add("Alchend");
            UserNames.Add("Deantingki");
            UserNames.Add("Sereneson");
            UserNames.Add("RocketSoftMax");
            UserNames.Add("Gurligerzo");
            UserNames.Add("Gentagou");
            UserNames.Add("MagicTinV2");
            UserNames.Add("Delkerhe");
            UserNames.Add("FelineTimes");
            UserNames.Add("Goldersear");
            UserNames.Add("Instanteg");
            UserNames.Add("Microckst");

            Random rand = new Random(DateTime.Now.Second);
            UserName = UserNames[rand.Next(0, UserNames.Count)];


			//Why? Bc Linux hates me.
			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        public void Init()
        {

            PlayerManager = new vxNetPlayerManager(this.Engine);
            var config = new NetPeerConfiguration(this.Engine.GameName)
            {
                //SimulatedMinimumLatency = 0.2f,
                // SimulatedLoss = 0.1f
            };

            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            //config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            this.netClient = new NetClient(config);
            this.netClient.Start();

            PlayerInfo = new vxNetPlayerInfo(netClient.UniqueIdentifier, UserName, vxEnumNetPlayerStatus.None);

            ClientCallBackLoop = new SendOrPostCallback(ClientMsgCallback);
            this.netClient.RegisterReceivedCallback(ClientCallBackLoop);
        }
        
        #region Callback Loop

        /// <summary>
		/// Method for Receiving Messages kept in a Global Scope for the Engine.
		/// </summary>
		/// <param name="peer">Peer.</param>
		public void ClientMsgCallback(object peer)
        {
            NetIncomingMessage im;

            while ((im = this.Client.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //LogClient("DEBUG: " + im.ReadString());
                        break;
                    /**************************************************************/
                    //DiscoveryResponse
                    /**************************************************************/
                    case NetIncomingMessageType.DiscoveryResponse:

                        LogClient("     ------- Server found at: " + im.SenderEndPoint);

                        //Fire the RecieveDiscoverySignalResponse Event by passing down the decoded Network Message
                        if (DiscoverySignalResponseRecieved != null)
                            DiscoverySignalResponseRecieved(this, new vxNetClientEventDiscoverySignalResponse(new vxNetmsgServerInfo(im)));
                        
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                {
                                    LogClient(string.Format("{0} Connected", im.SenderEndPoint));

                                    //Fire the Connected Event
                                    if (Connected != null)
                                        Connected(this, new vxNetClientEventConnected());

                                }
                                break;
                            case NetConnectionStatus.Disconnected:
                                {
                                    LogClient(string.Format("{0} Disconnected", im.SenderEndPoint));

                                    //Fire the Connected Event
                                    if (Disconnected != null)
                                        Disconnected(this, new vxNetClientEventDisconnected());
                                }
                                break;
                            default:
                                LogClient(im.ReadString());
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (vxNetworkMessageTypes)im.ReadByte();
                        switch (gameMessageType)
                        {
                            case vxNetworkMessageTypes.UpdatePlayersList:
                                
                                //Get the Message Containing the Updated Player List
                                var updatedPlayerList = new vxNetmsgUpdatePlayerList(im);

                                //Now Loop through each player in the list
                                foreach(vxNetPlayerInfo serverPlayer in updatedPlayerList.Players)
                                {
                                    //First Check if the Server Player is in the clients list. If not, then add the server player to the clients list.
                                    if (PlayerManager.Contains(serverPlayer))
                                    {
                                        PlayerManager.Players[serverPlayer.ID] = serverPlayer;
                                    }
                                    else
                                    {
                                        //Add Player to Player manager
                                        PlayerManager.Add(serverPlayer);

                                        //Now Fire the Event Handler
                                        if (OtherPlayerConnected != null)
                                            OtherPlayerConnected(this, new vxNetClientEventPlayerConnected(serverPlayer));
                                    }
                                }
                                break;

                            case vxNetworkMessageTypes.PlayerDisconnected:

                                //For what ever reason, a player has disconnected, so we need to remove it from the player list

                                //Send a message to all clients to remove this client from their list.
                                var rmvMsg = new vxNetmsgRemovePlayer(im);

                                //Fire the Connected Event
                                if (OtherPlayerDisconnected != null)
                                    OtherPlayerDisconnected(this, new vxNetClientEventPlayerDisconnected(rmvMsg.PlayerInfo));

                                //Finally remove the player from the server's list
                                if(PlayerManager.Players.ContainsKey(rmvMsg.PlayerInfo.ID))
                                    PlayerManager.Players.Remove(rmvMsg.PlayerInfo.ID);

                                break;

                            case vxNetworkMessageTypes.UpdatePlayerLobbyStatus:

                                //Decode the list
                                var updatePlayerState = new vxNetmsgUpdatePlayerLobbyStatus(im);

                                //Update the internal server list
                                PlayerManager.Players[updatePlayerState.PlayerInfo.ID] = updatePlayerState.PlayerInfo;

                                if (UpdatedPlayerInfoRecieved != null)
                                    UpdatedPlayerInfoRecieved(this, new vxNetClientEventPlayerStatusUpdate(updatePlayerState.PlayerInfo));

    
                                break;

                            case vxNetworkMessageTypes.SessionStatus:

                                //Get Old Status for information
                                vxEnumSessionStatus oldStatus = this.SessionStatus;
                                var newSessionStatus = new vxNetmsgUpdateSessionStatus(im);
                                
                                //Set the new Session Status
                                this.SessionStatus = newSessionStatus.SessionStatus;

                                //Fire any connected events
                                if (UpdateSessionStatus != null)
                                    UpdateSessionStatus(this, new vxNetClientEventSessionStatusUpdated(oldStatus, this.SessionStatus));

                                break;


                            case vxNetworkMessageTypes.UpdatePlayerEntityState:
                                
                                //First decode the message
                                var updatePlayerEntityState = new vxNetmsgUpdatePlayerEntityState(im);

                                var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(updatePlayerEntityState.MessageTime));

                                //Console.WriteLine("Client:" + timeDelay);

                                //Only set the information if it isn't our own info
                                if (this.PlayerInfo.ID != updatePlayerEntityState.PlayerInfo.ID)
                                {
                                    //Then Update the Player in the client List
                                    PlayerManager.Players[updatePlayerEntityState.PlayerInfo.ID] = updatePlayerEntityState.PlayerInfo;

                                    //Then fire any connected events
                                    if (UpdatePlayerEntityState != null)
                                        UpdatePlayerEntityState(this, new vxNetClientEventPlayerStateUpdate(updatePlayerEntityState, timeDelay));
                                }
                                break;

                            case vxNetworkMessageTypes.RemoveItem:
                                
                                break;

                            case vxNetworkMessageTypes.Other:
                                
                                break;
                        }
                        break;
                }
                Recycle(im);
            }
        }

        #endregion


        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        public void Connect(string Address, int Port)
        {
            this.netClient.Connect(new IPEndPoint(NetUtility.Resolve(Address), Port));
        }

        /// <summary>
        /// Connect with a hail message
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Port"></param>
        public void Connect(string Address, int Port, NetOutgoingMessage hail)
        {
            this.netClient.Connect(new IPEndPoint(NetUtility.Resolve(Address), Port), hail);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetOutgoingMessage CreateMessage()
        {
            return this.netClient.CreateMessage();
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.netClient.Disconnect("Bye");
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetIncomingMessage ReadMessage()
        {
            return this.netClient.ReadMessage();
        }

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Recycle(NetIncomingMessage im)
        {
            this.netClient.Recycle(im);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        public void SendMessage(INetworkMessage gameMessage)
        {
            NetOutgoingMessage om = this.netClient.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMsg(om);

            this.netClient.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
        }


        /// <summary>
        /// Emit a discovery signal
        /// </summary>
        public void SendDiscoverySignal(int port)
        {
            this.Client.DiscoverLocalPeers(port);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }

                this.isDisposed = true;
            }
        }

        public void LogClient(string log)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CLIENT >> " + log);
            Console.ResetColor();
        }

        public void LogClientError(string log)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("CLIENT >> " + log);
            Console.ResetColor();
        }

        #endregion
    }
}
#endif