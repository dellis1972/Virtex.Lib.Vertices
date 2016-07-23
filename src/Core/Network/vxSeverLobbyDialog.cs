#if VRTC_INCLDLIB_NET 

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using vxVertices.Core;
using vxVertices.Utilities;
using vxVertices.Core.Input;
using vxVertices.GUI.Controls;
using vxVertices.GUI.Events;
using vxVertices.GUI.Dialogs;
using vxVertices.Network.Events;
using vxVertices.GUI;
using Lidgren.Network;
using vxVertices.Network;
using vxVertices.GUI.MessageBoxs;

namespace vxVertices.GUI.Dialogs
{
    /// <summary>
    /// This is a Server Dialog which searches and retrieves any game servers on this subnet.
    /// It also allows the Player to set up a local server as well.
    /// </summary>
    public class vxSeverLobbyDialog : vxDialogBase
    {
        #region Fields

        /// <summary>
        /// The vxListView GUI item which contains the list of all broadcasting servers on the subnet.
        /// </summary>
        public vxScrollPanel ScrollPanel;
                
        int CurrentlySelected = -1;

		List<vxServerListItem> List_Items = new List<vxServerListItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;

        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSeverLobbyDialog(string Title)
            : base(Title, ButtonTypes.OkCancel)
        {

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
                //Set up the Callback for the Lobby
                vxEngine.GameSever.RegisterReceivedCallback(ServerMsgCallback);

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

        }
        SendOrPostCallback ClientCallBackLoop;


        /// <summary>
        /// The currently highlited server in the list is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {

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


        string serverName = "Big Bob's Kick Ass Cook House";

        //A Collection of General Messages From The Server
        string SVRMSG_SERVER_SHUTDOWN_REASON = "SERVER SHUTDOWN";
        string SVRMSG_RESPONSE_PLAYER_LIST = "RESPONSE_PLAYER_LIST";

        //A Collection of General Messages From The Clients
        string CLNTMSG_CLIENT_SHUTDOWN_REASON = "CLIENT DISCONNECT";
        string CLNTMSG_REQUEST_PLAYER_LIST = "REQUEST_PLAYER_LIST";

        /// <summary>
        /// Method for Receiving Messages. Only Peer Disovery Is Needed for this one.
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
                            case "response_playerlist":

                                //This Resets The Entire List
                                LobbyPlayers.Clear();
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

                                    LobbyPlayerInfo newPlayer = new LobbyPlayerInfo(
                                        vxUtil.ReadXML(plyr, "Name"), 
                                        Convert.ToInt32(vxUtil.ReadXML(plyr, "Level")),
                                        vxUtil.ReadXML(plyr, "Token"));

                                    LobbyPlayers.Add(newPlayer);

                                    //Now Add a New Connection
                                    AddConnectedClient(newPlayer);
                                }

                                ScrollPanel.Clear();

                                foreach (vxServerListItem it in List_Items)
                                    ScrollPanel.AddItem(it);


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

                                LobbyPlayerInfo newPlayer = new LobbyPlayerInfo("Player" + playerCount.ToString(),
                                    rand.Next(0, 100),
                                    msg.SenderConnection.RemoteUniqueIdentifier.ToString());
                                
                                //Now Add a New Connection
                                //AddConnectedClient(newPlayer);
                                SeverPlayerList.Add(newPlayer);

                                //Now Broadcast the New List
                                BroadcastUpdatedClientList(msg);
                            }


                            // Remove/Lost Connection
                            /**************************************************************/
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                //Find Player In Server List to Remove
                                string Token = msg.SenderConnection.RemoteUniqueIdentifier.ToString();

                                for(int i = 0; i < SeverPlayerList.Count; i++)
                                {
                                    LobbyPlayerInfo player = SeverPlayerList[i];

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
                                BroadcastUpdatedClientList(msg);
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

                                case "request_playerlist":
                                    NetOutgoingMessage om = vxEngine.GameSever.CreateMessage();
                                    om.Write(GetPlayerList());
                                    vxEngine.GameSever.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
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



        List<LobbyPlayerInfo> LobbyPlayers = new List<LobbyPlayerInfo>();
        List<LobbyPlayerInfo> SeverPlayerList = new List<LobbyPlayerInfo>();

        string GetPlayerList()
        {            
            //Get Server List
            string serverList = "response_playerlist";
            serverList += "\n";
            foreach (LobbyPlayerInfo player in SeverPlayerList)
            {
                //Now Parse Everything Into a Basic XML Structure
                serverList += 
                    vxUtil.WriteXML(player.Name, "Name") + 
                    vxUtil.WriteXML(player.Level.ToString(), "Level") +
                    vxUtil.WriteXML(player.Token.ToString(), "Token") + "\n";
            }
            return serverList;
        }

        /// <summary>
        /// Broadcasts too All Connected Clients the Updated Client List
        /// </summary>
        private void BroadcastUpdatedClientList(NetIncomingMessage msg)
        {
            // broadcast this to all connections, except sender
            List<NetConnection> all = vxEngine.GameSever.Connections; // get copy
            all.Remove(msg.SenderConnection);

            if (all.Count > 0)
            {
                NetOutgoingMessage om2 = vxEngine.GameSever.CreateMessage();
                om2.Write(GetPlayerList());
                vxEngine.GameSever.SendMessage(om2, all, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }



        int playerCount = 0;
        private void AddConnectedClient(LobbyPlayerInfo newPlayer)
        {
            Texture2D thumbnail = vxEngine.Assets.Textures.Arrow_Right;
            vxServerListItem item = new vxServerListItem(vxEngine,
                newPlayer.Name,
                newPlayer.Level.ToString(),
                "PORT NUMBER",
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
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
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
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            loop++;
            //Get the User List on the First Update
            if (loop == 25)
            {
                //Now request the User List
                NetOutgoingMessage userListMsg = vxEngine.GameClient.CreateMessage();
                userListMsg.Write("request_playerlist");
                vxEngine.GameClient.SendMessage(userListMsg, NetDeliveryMethod.ReliableUnordered);

                Console.WriteLine("REQUESTING PLAYER LIST");
            }

            TimeSinceLastClick++;
        }

        #region Draw

        string FileName = "";



        public void GetHighlitedItem(object sender, vxGuiItemClickEventArgs e)
        {
            foreach (vxServerListItem fd in List_Items)
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