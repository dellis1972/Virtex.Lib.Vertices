#if VRTC_INCLDLIB_NET 

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Core.Input;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.GUI.Events;
using Virtex.Lib.Vertices.GUI.Dialogs;
using Virtex.Lib.Vertices.Network.Events;
using Virtex.Lib.Vertices.GUI;
using Lidgren.Network;
using Virtex.Lib.Vertices.Network;
using Virtex.Lib.Vertices.GUI.MessageBoxs;

namespace Virtex.Lib.Vertices.GUI.Dialogs
{

    enum SessionState
    {
        Idle,
        Countdown,
        Launch
    }

    /// <summary>
    /// This is a Server Lobby Dialog is the 'waiting room' before a session launch.
    /// </summary>
    public class vxSeverLobbyDialog : vxDialogBase
    {
        #region Fields

        /// <summary>
        /// The vxListView GUI item which contains the list of all broadcasting servers on the subnet.
        /// </summary>
        public vxScrollPanel ScrollPanel;

        /// <summary>
        /// Client Callback Loop
        /// </summary>
        public SendOrPostCallback ClientCallBackLoop;

        public SendOrPostCallback ServerCallBackLoop;

        /// <summary>
        /// If the player is acting as Server, then let the player launch the session.
        /// </summary>
        public vxButton Btn_LaunchServer;

        SessionState SessionStateForServer = SessionState.Idle;
        SessionState SessionStateForClient = SessionState.Idle;

        //TODO: Should this be kept?
        //List<LobbyPlayerInfo> LobbyPlayers = new List<LobbyPlayerInfo>();
        List<NetworkPlayerInfo> SeverPlayerList = new List<NetworkPlayerInfo>();

        bool ReadyToAutoLaunch = false;

        public string serverName = "Insert Server Name";

        //A Collection of General Messages From The Server
        const string SVRMSG_SERVER_SHUTDOWN_REASON = "SVRMSG_SERVER_SHUTDOWN_REASON";
        const string SVRMSG_UPDATE_PLAYER_LIST = "SVRMSG_UPDATE_PLAYER_LIST";
        const string SVRMSG_UPDATE_PLAYER_STATUS = "SVRMSG_UPDATE_PLAYER_STATUS";
        const string SVRMSG_LAUNCH_START = "SVRMSG_LAUNCH_START";
        const string SVRMSG_LAUNCH_COUNTDOWN = "SVRMSG_LAUNCH_COUNTDOWN";
        const string SVRMSG_LAUNCH_ABORT = "SVRMSG_LAUNCH_ABORT";

        //A Collection of General Messages From The Clients
        const string CLNTMSG_CLIENT_SHUTDOWN_REASON = "CLNTMSG_CLIENT_SHUTDOWN_REASON";
        const string CLNTMSG_REQUEST_PLAYER_LIST = "CLNTMSG_REQUEST_PLAYER_LIST";
        const string CLNTMSG_UPDATE_PLAYER_STATUS = "CLNTMSG_UPDATE_PLAYER_STATUS";

        int CurrentlySelected = -1;

		List<vxServerLobbyPlayerItem> List_Items = new List<vxServerLobbyPlayerItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;

        bool ReadyState = false;

        int playerCount = 0;

        #endregion

        #region Initialization

        string originalTitle = "";
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSeverLobbyDialog(string Title)
            : base(Title, ButtonTypes.OkCancel)
        {
            originalTitle = Title;
        }


        /// <summary>
        /// Sets up Local Server Dialog. It also sends out the subnet broadcast here searching for any available servers on this subnet.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();



            //The Okay Button here Acts  as the "Ready" button for the Server Lobby
            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);

            //The Cancel Button is Naturally the 'Back' button
            Btn_Cancel.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Cancel_Clicked);
            

            ScrollPanel = new vxScrollPanel(
                new Vector2(
                    backgroundRectangle.X + hPad,
                    backgroundRectangle.Y + vPad),
                backgroundRectangle.Width - hPad * 2,
                backgroundRectangle.Height - Btn_Ok.BoundingRectangle.Height - vPad * 3);

            ScrollPanel.ScrollBarWidth = 15;

            InternalvxGUIManager.Add(ScrollPanel);

            if (vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Server)
            {
                Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

                //Create The New Server Button
                Btn_LaunchServer = new vxButton(vxEngine, "Launch Game", new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));

                //Set the Button's Position relative too the background rectangle.
                Btn_LaunchServer.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
                    vxEngine.vxGUITheme.Padding.X * 2,
                    backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);

                Btn_LaunchServer.Clicked += Btn_LaunchServer_Clicked; ;
                InternalvxGUIManager.Add(Btn_LaunchServer);


                //Set up the Callback for the Lobby
                ServerCallBackLoop = new SendOrPostCallback(ServerMsgCallback);
                vxEngine.GameSever.RegisterReceivedCallback(ServerCallBackLoop);

                //Now Start The Server
                vxEngine.GameSever.Start();

                
                //Now Connect to it's self if it's the Server
                NetOutgoingMessage approval = vxEngine.GameClient.CreateMessage();
                approval.Write("secret");
                vxEngine.GameClient.Connect(NetUtility.Resolve("localhost").ToString(), vxEngine.GameSever.Configuration.Port, approval);
            }
            //vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Client
            //The Server acts as a Client as well, so no need for an 'else if' block here.

            ClientCallBackLoop = new SendOrPostCallback(ClientMsgCallback);
            vxEngine.GameClient.RegisterReceivedCallback(ClientCallBackLoop);

            Btn_Ok.Text = "Not Ready";

        }

        /// <summary>
        /// This method is called at the end of the countdown in the lobby to launch the session.
        /// </summary>
        public virtual void LaunchSession()
        {
            //Remove Call Backs
            vxEngine.GameClient.UnregisterReceivedCallback(ClientCallBackLoop);

            if (vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Server)
            {
                vxEngine.GameSever.UnregisterReceivedCallback(ServerCallBackLoop);
            }
        }

        /// <summary>
        /// Launches the Server. This is only available to the player acting as the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_LaunchServer_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            LaunchSession();
        }


        /// <summary>
        /// The currently highlited server in the list is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            ReadyState = !ReadyState;

            if (ReadyState)
                Btn_Ok.Text = "Ready";
            else
                Btn_Ok.Text = "Not Ready";

            //Now Send the Status Update too the Server
            NetOutgoingMessage om = vxEngine.GameClient.CreateMessage();

            string mesg = CLNTMSG_UPDATE_PLAYER_STATUS + "\n";
            mesg += vxUtil.WriteXML(vxEngine.GameClient.UniqueIdentifier.ToString(), "Token");
            mesg += vxUtil.WriteXML(this.ReadyState.ToString(),"Ready");
            om.Write(mesg);
            vxEngine.GameClient.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Closes the Local Server Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            //Always Disconnect The Client
            vxEngine.GameClient.Disconnect(CLNTMSG_CLIENT_SHUTDOWN_REASON);

            //If The User is the Server And Leaves, then the Shutdown Signal Needs to be Sent Also
            if (vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Server)
            {
                vxEngine.GameSever.Shutdown(SVRMSG_SERVER_SHUTDOWN_REASON);
            }
            base.Btn_Cancel_Clicked(sender, e);
        }


        string GetPlayerList(string header)
        {
            //Get Server List
            string serverList = header;
            serverList += "\n";
            foreach (NetworkPlayerInfo player in SeverPlayerList)
            {
                //Now Parse Everything Into a Basic XML Structure
                serverList +=
                    vxUtil.WriteXML(player.Name, "Name") +
                    vxUtil.WriteXML(player.Level.ToString(), "Level") +
                    vxUtil.WriteXML(player.Token.ToString(), "Token") +
                    vxUtil.WriteXML(player.Ready.ToString(), "Ready") + "\n";
            }
            return serverList;
        }

        /// <summary>
        /// Method for Client Receiving Messages
        /// </summary>
        /// <param name="peer">Peer.</param>
        public void ClientMsgCallback(object peer)
        {
            NetIncomingMessage im;
            while ((im = vxEngine.GameClient.ReadMessage()) != null)
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


                    /**************************************************************/
                    //StatusChanged
                    /**************************************************************/
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        string reason = im.ReadString();
                        vxConsole.WriteNetworkLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            //If the Player is a Client, then Notify Them.
                            if (reason == SVRMSG_SERVER_SHUTDOWN_REASON && vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Client)
                            {
                                vxMessageBox msgBox = new vxMessageBox("The Connection to the Server Was Dropped", "Are You Still There?");
                                vxEngine.AddScreen(msgBox, PlayerIndex.One);
                            }
                            ExitScreen();
                        }

                        break;

                    case NetIncomingMessageType.ConnectionLatencyUpdated:

                        Console.WriteLine("CLIENT >>"+im.ReadFloat());

                        break;

                    /**************************************************************/
                    //Data
                    /**************************************************************/
                    case NetIncomingMessageType.Data:
                        // incoming chat message from a client
                        string chat = im.ReadString();

                        //Split the Text By Carriage Return
                        string[] result = chat.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                        switch (result[0])
                        {
                            case SVRMSG_UPDATE_PLAYER_LIST:

                                //This Resets The Entire List

                                vxEngine.NetworkPlayers.Clear();
                                List_Items.Clear();

                                for (int i = 1; i<result.Length; i++)
                                {
                                    string plyr = result[i];
                                    /*
                                    Console.WriteLine("----------------------------");
                                    Console.WriteLine("Name: " + vxUtil.ReadXML(plyr, "Name"));
                                    Console.WriteLine("Level: " + vxUtil.ReadXML(plyr, "Level"));
                                    Console.WriteLine("Token: " + vxUtil.ReadXML(plyr, "Token"));
                                    */

                                    NetworkPlayerInfo newPlayer = new NetworkPlayerInfo(
                                        vxUtil.ReadXML(plyr, "Name"), 
                                        Convert.ToInt32(vxUtil.ReadXML(plyr, "Level")),
                                        vxUtil.ReadXML(plyr, "Token"),
                                        bool.Parse(vxUtil.ReadXML(plyr, "Ready")));

                                    vxEngine.NetworkPlayers.Add(newPlayer);

                                    //Now Add a New Connection
                                    AddConnectedClient(newPlayer);
                                }

                                ScrollPanel.Clear();

                                foreach (vxServerLobbyPlayerItem it in List_Items)
                                    ScrollPanel.AddItem(it);


                                break;
                            case SVRMSG_UPDATE_PLAYER_STATUS:
                                
                                for (int i = 1; i < result.Length; i++)
                                {
                                    string plyr = result[i];

                                    //Parse out the info into new Lobby Player
                                    NetworkPlayerInfo newPlayer = new NetworkPlayerInfo(
                                        vxUtil.ReadXML(plyr, "Name"),
                                        Convert.ToInt32(vxUtil.ReadXML(plyr, "Level")),
                                        vxUtil.ReadXML(plyr, "Token"),
                                        bool.Parse(vxUtil.ReadXML(plyr, "Ready")));

                                    //Now Search through the lobby list, 
                                    foreach (vxServerLobbyPlayerItem it in List_Items)
                                    {
                                        if (it.Player.Token == newPlayer.Token)
                                        {
                                            it.Player.Ready = newPlayer.Ready;
                                        }                                        
                                    }
                                }

                                break;

                            case SVRMSG_LAUNCH_START:

                                break;
                            case SVRMSG_LAUNCH_COUNTDOWN:
                                SessionStateForClient = SessionState.Countdown;
                                Console.WriteLine("Count Down Starting...");
                                break;
                            case SVRMSG_LAUNCH_ABORT:
                                SessionStateForClient = SessionState.Idle;
                                Console.WriteLine("Count Down ABORTED!!");
                                break;
                            default:
                                vxConsole.WriteNetworkLine("Unknown Data Package '" + chat + "'");                                
                                break;
                        }
                        break;
                    default:
                        vxConsole.WriteNetworkLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                vxEngine.GameClient.Recycle(im);
            }
        }

        /// <summary>
        /// Method for Server Receiving Messages.
        /// </summary>
        /// <param name="peer"></param>
        public virtual void ServerMsgCallback(object peer)
        {
            NetIncomingMessage msg;
            if (vxEngine.GameSever != null)
            {
                while ((msg = vxEngine.GameSever.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        /**************************************************************/
                        //Handle Discovery
                        /**************************************************************/
                        case NetIncomingMessageType.DiscoveryRequest:

                            // Create a response and write some example data to it
                            NetOutgoingMessage response = vxEngine.GameSever.CreateMessage();

                            //Send Back Connection Information, the client will need to Authenticate with a Secret though
                            string respString = vxUtil.WriteXML(serverName, "name");
                            respString += vxUtil.WriteXML(vxEngine.GameSever.Configuration.BroadcastAddress.ToString(), "ip");
                            respString += vxUtil.WriteXML(vxEngine.GameSever.Configuration.Port.ToString(), "port");
                            response.Write(respString);
                            
                            // Send the response to the sender of the request
                            vxEngine.GameSever.SendDiscoveryResponse(response, msg.SenderEndPoint);
                            vxConsole.WriteNetworkLine(string.Format("Discovery Request Recieved from: {0}", msg.SenderEndPoint));
                            break;


                        /**************************************************************/
                        //Handle Connection Approval
                        /**************************************************************/
                        case NetIncomingMessageType.ConnectionApproval:
                            string s = msg.ReadString();
                            if (s == "secret")
                            {
                                NetOutgoingMessage approve = vxEngine.GameSever.CreateMessage();

                                msg.SenderConnection.Approve(approve);
                            }
                            else
                                msg.SenderConnection.Deny();
                            break;

                        case NetIncomingMessageType.ConnectionLatencyUpdated:

                            //Console.WriteLine("SERVER >>" + msg.ReadString());

                            break;

                        /**************************************************************/
                        //DEBUG
                        /**************************************************************/
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            vxConsole.WriteNetworkLine(msg.MessageType + " : " + msg.ReadString());
                            break;


                        /**************************************************************/
                        //StatusChanged
                        /**************************************************************/
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                            vxConsole.WriteNetworkLine("Status changed too:     " + status);
                            string reason = msg.ReadString();
                            vxConsole.WriteNetworkLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);


                            // New Connection
                            /**************************************************************/
                            if (status == NetConnectionStatus.Connected)
                            {
                                //Add the New Connection to the List
                                string respStringNew = vxUtil.WriteXML(msg.SenderEndPoint.ToString(), "ip");

                                playerCount++;

                                Random rand = new Random(playerCount * playerCount);

                                //Initialise Lobby Player Info
                                NetworkPlayerInfo newPlayer = new NetworkPlayerInfo("Player" + playerCount.ToString(),
                                    rand.Next(0, 100),
                                    msg.SenderConnection.RemoteUniqueIdentifier.ToString(),false);
                                
                                //Now Add a New Connection
                                SeverPlayerList.Add(newPlayer);

                                //Now Broadcast the New List
                                BroadcastUpdatedClientList(msg, SVRMSG_UPDATE_PLAYER_LIST);
                            }


                            // Remove/Lost Connection
                            /**************************************************************/
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                //Find Player In Server List to Remove
                                string Token = msg.SenderConnection.RemoteUniqueIdentifier.ToString();

                                for(int i = 0; i < SeverPlayerList.Count; i++)
                                {
                                    NetworkPlayerInfo player = SeverPlayerList[i];

                                    //Check to see if the Tokens Are the same
                                    if(Token == player.Token)
                                    {
                                        //Now Remove the Player
                                        SeverPlayerList.Remove(player);

                                        //Since one entry was removed, we need to decrement by one.
                                        i--;
                                    }
                                }
                                
                                //Now Broadcast the New List
                                BroadcastUpdatedClientList(msg, SVRMSG_UPDATE_PLAYER_LIST);
                            }
                            break;


                        /**************************************************************/
                        //Data
                        /**************************************************************/
                        case NetIncomingMessageType.Data:
                            // incoming chat message from a client
                            string chat = msg.ReadString();

                            //Split the Text By Carriage Return
                            string[] result = chat.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                            switch (result[0])
                            {

                                    //Respond to New Player Addition
                                case CLNTMSG_REQUEST_PLAYER_LIST:
                                    NetOutgoingMessage om = vxEngine.GameSever.CreateMessage();
                                    om.Write(GetPlayerList(SVRMSG_UPDATE_PLAYER_LIST));
                                    vxEngine.GameSever.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    break;

                                //A Player Has Updated their status
                                case CLNTMSG_UPDATE_PLAYER_STATUS:
                                    
                                    for (int i = 1; i < result.Length; i++)
                                    {
                                        string plyr = result[i];

                                        bool newReady = bool.Parse(vxUtil.ReadXML(plyr, "Ready"));
                                        string token = vxUtil.ReadXML(plyr, "Token");

                                        //Now Search through the lobby list,                                         
                                        foreach (NetworkPlayerInfo it in SeverPlayerList)
                                        {
                                            if (it.Token == token)                         
                                                it.Ready = newReady;
                                        }
                                    }

                                    ReadyToAutoLaunch = false;

                                    //Now Check If All Players are ready
                                    if (SeverPlayerList.Count > 1)
                                    {
                                        //Now Search through the lobby list,                                         
                                        foreach (NetworkPlayerInfo it in SeverPlayerList)
                                        {
                                            //Set Ready To Launch too what the player's status is
                                            ReadyToAutoLaunch = it.Ready;

                                            //If any player is no, then abort launch
                                            if (it.Ready == false)
                                                break;
                                        }
                                    }

                                    //Handle One off Events
                                    if(ReadyToAutoLaunch && SessionStateForServer == SessionState.Idle)
                                    {
                                        SessionStateForServer = SessionState.Countdown;
                                        Broadcast(msg, SVRMSG_LAUNCH_COUNTDOWN, false);
                                    }
                                    else
                                    {
                                        SessionStateForServer = SessionState.Idle;
                                        Broadcast(msg, SVRMSG_LAUNCH_ABORT, false);
                                    }

                                    vxConsole.WriteNetworkLine(ReadyToAutoLaunch);

                                    //Now Broadcast the Update
                                    BroadcastUpdatedClientList(msg, SVRMSG_UPDATE_PLAYER_STATUS, false);
                                    break;

                                default:
                                    vxConsole.WriteNetworkLine("SERVER >> Unknown Data Package '" + chat + "'");
                                    break;
                            }
                            break;
                        default:
                            vxConsole.WriteNetworkLine("Unhandled type: " + msg.MessageType + " " + msg.LengthBytes + " bytes " + msg.DeliveryMethod + "|" + msg.SequenceChannel);
                            break;
                    }
                    vxEngine.GameSever.Recycle(msg);
                }
            }
        }


        /// <summary>
        /// Broadcasts too All Connected Clients the Updated Client List. This version removes the original 
        /// sender from the reciepents list.
        /// </summary>
        /// <param name="sender">The Original Message Sender</param>
        /// <param name="header">The Header for the string to be sent.</param>
        private void BroadcastUpdatedClientList(NetIncomingMessage sender, string header)
        {
            BroadcastUpdatedClientList(sender, header, true);
        }

        /// <summary>
        /// Broadcasts too All Connected Clients the Updated Client List.
        /// </summary>
        /// <param name="sender">The Original Message Sender</param>
        /// <param name="header">The Header for the string to be sent.</param>
        /// <param name="removeSender">Should the original Sender of this message be removed from the broadcast list.</param>
        private void BroadcastUpdatedClientList(NetIncomingMessage sender, string header, bool removeSender)
        {
            // broadcast this to all connections, except sender
            List<NetConnection> all = vxEngine.GameSever.Connections; // get copy

            if(removeSender)
                all.Remove(sender.SenderConnection);

            if (all.Count > 0)
            {
                NetOutgoingMessage om2 = vxEngine.GameSever.CreateMessage();
                om2.Write(GetPlayerList(header));
                vxEngine.GameSever.SendMessage(om2, all, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }


        /// <summary>
        /// Broadcasts a string of text to all clients
        /// </summary>
        /// <param name="sender">The Originator of the Broadcast. Usually the Server.</param>
        /// <param name="textToBrodcast">The string of Text to broadcast too all clients.</param>
        /// <param name="removeSender">Should the originator of the broadcast be removed from the broadcast.</param>
        private void Broadcast(NetIncomingMessage sender, string textToBrodcast, bool removeSender)
        {
            // broadcast this to all connections, except sender
            List<NetConnection> all = vxEngine.GameSever.Connections; // get copy

            if (removeSender)
                all.Remove(sender.SenderConnection);

            if (all.Count > 0)
            {
                NetOutgoingMessage om2 = vxEngine.GameSever.CreateMessage();
                om2.Write(textToBrodcast);
                vxEngine.GameSever.SendMessage(om2, all, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }



        private void AddConnectedClient(NetworkPlayerInfo newPlayer)
        {
            Texture2D thumbnail = vxEngine.Assets.Textures.Arrow_Right;
            vxServerLobbyPlayerItem item = new vxServerLobbyPlayerItem(vxEngine,
                newPlayer,
        new Vector2(
            (int)(2 * hPad),
            vPad + (vPad / 10 + 68) * (List_Items.Count + 1)),
        thumbnail,
        List_Items.Count);


            //Set Item Width
            item.ButtonWidth = ScrollPanel.Width - (int)(2 * hPad) - ScrollPanel.ScrollBarWidth;

            //Set Clicked Event
            item.Clicked += GetHighlitedItem;

            //Add item too the list
            List_Items.Add(item);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Loop to handle inpurt
        /// </summary>
        /// <param name="input">Input Manager</param>
        public override void HandleInput(vxInputManager input)
        {
            if (input.IsNewMouseButtonPress(MouseButtons.LeftButton))
            {
                if (TimeSinceLastClick < 20)
                {
                    if(CurrentlySelected == HighlightedItem_Previous)
                        Btn_Ok.Select();
                }
                else
                {
                    TimeSinceLastClick = 0;
                }

                HighlightedItem_Previous = CurrentlySelected;
            }
        }

        #endregion

        int loop = 0;
        bool FirstLoop = true;
        float LoadingAlpha = 0;
        float LoadingAlpha_Req = 1;

        float LaunchCountdown = 5;

        bool HasLaunched = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            loop++;
            //Get the User List on the First Update
            if (loop == 25)
            {
                //Now request the User List
                NetOutgoingMessage userListMsg = vxEngine.GameClient.CreateMessage();
                userListMsg.Write(CLNTMSG_REQUEST_PLAYER_LIST);
                vxEngine.GameClient.SendMessage(userListMsg, NetDeliveryMethod.ReliableUnordered);
            }


            // Launch Control
            /**********************************************************************************************/
            if (SessionStateForClient == SessionState.Idle)
            {
                LaunchCountdown = 5;
                this.Title = originalTitle;
            }
            else if (SessionStateForClient == SessionState.Countdown)
            {
                LaunchCountdown += -0.0167f;

                if (LaunchCountdown < 0)
                    SessionStateForClient = SessionState.Launch;

                this.Title = originalTitle + string.Format(" - [ Launching in: {0} ]", LaunchCountdown);
            }
            else if (SessionStateForClient == SessionState.Launch && HasLaunched == false)
            {
                HasLaunched = true;

                LaunchSession();
            }

            TimeSinceLastClick++;
        }

        #region Draw
        
        public void GetHighlitedItem(object sender, vxGuiItemClickEventArgs e)
        {
            foreach (vxServerLobbyPlayerItem fd in List_Items)
            {
                fd.UnSelect();
            }
            int i = e.GUIitem.Index;

            List_Items[i].ThisSelect();
            CurrentlySelected = i;

            //SelectedServerIp = List_Items[i].ServerAddress;
        }


        #endregion
    }
}

#endif