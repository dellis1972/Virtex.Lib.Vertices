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
using Virtex.Lib.Vertices.Network.Messages;

namespace Virtex.Lib.Vertices.GUI.Dialogs
{


    /// <summary>
    /// This is a Server Dialog which searches and retrieves any game servers on this subnet.
    /// It also allows the Player to set up a local server as well.
    /// </summary>
    public class vxSeverLocalListDialog : vxDialogBase
    {
        #region Fields

        /// <summary>
        /// The vxListView GUI item which contains the list of all broadcasting servers on the subnet.
        /// </summary>
        public vxScrollPanel ScrollPanel;

        /// <summary>
        /// Let's the player create a new local server.
        /// </summary>
        public vxButton Btn_CreateNewLocalServer;
        NetPeerConfiguration newServerConfig;


        /// <summary>
        /// The Server Name too Give your Game. CHANGE THIS!!!
        /// </summary>
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
        private string _serverName = "default game server name";

        /// <summary>
        /// Server Port. It's a good idea to change this to something else.
        /// </summary>
        public int ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }
        private int _serverPort = 14242;


        int CurrentlySelected = -1;

        List<vxServerListItem> List_Items = new List<vxServerListItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSeverLocalListDialog(string Title)
            : base(Title, ButtonTypes.OkApplyCancel)
        {

        }


        /// <summary>
        /// Sets up Local Server Dialog. It also sends out the subnet broadcast here searching for any available servers on this subnet.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();


            Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

            //Create The New Server Button
            Btn_CreateNewLocalServer = new vxButton(vxEngine, "Create New Local Server", new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));

            //Set the Button's Position relative too the background rectangle.
            Btn_CreateNewLocalServer.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
                vxEngine.vxGUITheme.Padding.X * 2,
                backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);

            Btn_CreateNewLocalServer.Clicked += Btn_CreateNewLocalServer_Clicked;
            InternalvxGUIManager.Add(Btn_CreateNewLocalServer);

            //The Okay Button here Selects the Selected Server in the List
            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);

            //The Cancel Button is Naturally the 'Back' button
            Btn_Cancel.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Cancel_Clicked);

            //The Apply Button here is used to start a New Server
            Btn_Apply.Clicked += Btn_Refresh_Clicked;// new EventHandler<vxGuiItemClickEventArgs>(Btn_Refresh_Clicked);
            Btn_Apply.Text = "Refresh";

            ScrollPanel = new vxScrollPanel(
                new Vector2(
                    backgroundRectangle.X + hPad,
                    backgroundRectangle.Y + vPad),
                backgroundRectangle.Width - hPad * 2,
                backgroundRectangle.Height - Btn_Ok.BoundingRectangle.Height - vPad * 3);

            ScrollPanel.ScrollBarWidth = 15;

            InternalvxGUIManager.Add(ScrollPanel);

            //Initialise the network client
            vxEngine.ClientManager.Init();

            //Now setup the Event Handlers
            vxEngine.ClientManager.DiscoverySignalResponseRecieved += ClientManager_DiscoverySignalResponseRecieved;

            //By Default, The Game will start looking for other networked games as a client.
            vxEngine.NetworkedGameRoll = vxEnumNetworkPlayerRole.Client;
            
            //Finally at the end, send out a pulse of discovery signals 
            vxConsole.WriteLine("Sending Discovery Signal...");
            SendDiscoverySignal();
        }

        private void ClientManager_DiscoverySignalResponseRecieved(object sender, vxNetClientEventDiscoverySignalResponse e)
        {
            AddDiscoveredServer(e.NetMsgServerInfo);
        }

        void SendDiscoverySignal()
        {
            List_Items.Clear();

            ScrollPanel.Clear();

            //TODO: increase port range (send out a 100 signals?)
            // Emit a discovery signal
            this.vxEngine.ClientManager.SendDiscoverySignal(14242);
        }


        #region Client Networking Code


        ///// <summary>
        ///// Method for Receiving Messages. Only Peer Disovery Is Needed for this one.
        ///// </summary>
        ///// <param name="peer">Peer.</param>
        //public void ClientMsgCallback(object peer)
        //{
        //    NetIncomingMessage im;
        //    while ((im = vxEngine.GameClient.ReadMessage()) != null)
        //    {
        //        // handle incoming message
        //        switch (im.MessageType)
        //        {
        //            case NetIncomingMessageType.DebugMessage:
        //            case NetIncomingMessageType.ErrorMessage:
        //            case NetIncomingMessageType.WarningMessage:
        //            case NetIncomingMessageType.VerboseDebugMessage:
        //                string text = im.ReadString();
        //                vxConsole.WriteNetworkLine(im.MessageType + " : " + text);
        //                break;


        //            /**************************************************************/
        //            //DiscoveryResponse
        //            /**************************************************************/
        //            case NetIncomingMessageType.DiscoveryResponse:

        //                //Read In The Discovery Response
        //                string receivedMsg = im.ReadString();

        //                vxConsole.WriteNetworkLine("Server found at: " + im.SenderEndPoint + "\nmsg: " + receivedMsg);
        //                AddDiscoveredServer(receivedMsg);
        //                break;


        //            /**************************************************************/
        //            //StatusChanged
        //            /**************************************************************/
        //            case NetIncomingMessageType.StatusChanged:
        //                NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        
        //                string reason = im.ReadString();
        //                vxConsole.WriteNetworkLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
        //                break;



        //            /**************************************************************/
        //            //ConnectionApproval
        //            /**************************************************************/
        //            case NetIncomingMessageType.ConnectionApproval:

        //                Console.WriteLine("Connetion Approval From: " + im.SenderEndPoint + "\nmsg: " + im.ReadString());

        //                break;


        //            /**************************************************************/
        //            //Data
        //            /**************************************************************/
        //            case NetIncomingMessageType.Data:
        //                // incoming chat message from a client
        //                string chat = im.ReadString();

        //                //Split the Text By Carriage Return
        //                string[] result = chat.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        //                switch (result[0])
        //                {
        //                    default:
        //                        vxConsole.WriteNetworkLine("Broadcasting '" + chat + "'");
        //                        /*
        //                        // broadcast this to all connections, except sender
        //                        List<NetConnection> all = vxEngine.GameSever.Connections; // get copy
        //                        all.Remove(im.SenderConnection);

        //                        if (all.Count > 0)
        //                        {
        //                            NetOutgoingMessage om2 = vxEngine.GameSever.CreateMessage();
        //                            om2.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
        //                            vxEngine.GameSever.SendMessage(om2, all, NetDeliveryMethod.ReliableOrdered, 0);
        //                        }
        //                        */
        //                        break;
        //                }
        //                break;
        //            default:
        //                vxConsole.WriteNetworkLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
        //                break;
        //        }
        //        vxEngine.GameClient.Recycle(im);
        //    }
        //}


        private void AddDiscoveredServer(vxNetmsgServerInfo response)
        {
            Texture2D thumbnail = vxEngine.Assets.Textures.Arrow_Right;
            vxServerListItem item = new vxServerListItem(vxEngine,
                response.ServerName,
                response.ServerIP,
               response.ServerPort,
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


            foreach (vxServerListItem it in List_Items)
                ScrollPanel.AddItem(it);
        }

        string SelectedServerIp = "";

        public void GetHighlitedItem(object sender, vxGuiItemClickEventArgs e)
        {
            foreach (vxServerListItem fd in List_Items)
            {
                fd.UnSelect();
            }
            int i = e.GUIitem.Index;

            List_Items[i].ThisSelect();
            CurrentlySelected = i;

            SelectedServerIp = List_Items[i].ServerAddress;
        }


        /// <summary>
        /// Connect the specified ipAddress, port and HailMsg.
        /// </summary>
        /// <param name="ipAddress">Ip address.</param>
        /// <param name="port">Port.</param>
        /// <param name="HailMsg">Hail message.</param>
        public void Connect(string ipAddress, int port)
        {
            vxEngine.ClientManager.LogClient(string.Format("Connecting to Server: {0} : {1}", ipAddress, port));
            
            NetOutgoingMessage approval = vxEngine.ClientManager.CreateMessage();
            approval.Write("secret");
            vxEngine.ClientManager.Connect(ipAddress, port, approval);

            vxEngine.ClientManager.LogClient("Done!");
        }

        /*
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
*/
        #endregion


        /// <summary>
        /// The event Fired when the user wants to create their own Local Server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_CreateNewLocalServer_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            try
            {
                vxEngine.ServerManager.Connect(14242);


                //Set the User's Network Roll to be Server.
                vxEngine.NetworkedGameRoll = vxEnumNetworkPlayerRole.Server;

                OpenServerLobby();
                ExitScreen();
            }
            catch
            { Console.WriteLine("SERVER COULD NOT BE STARTED!"); }
        }

        /// <summary>
        /// This Method is Called to Open the Server Lobby. If your game uses an inherited version of vxSeverLobbyDialog then
        /// you should override this function.
        /// </summary>
        public virtual void OpenServerLobby()
        {
            vxEngine.AddScreen(new vxSeverLobbyDialog("Lobby"), PlayerIndex.One);
        }

        /// <summary>
        /// The currently highlited server in the list is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {

            //Connect to the Selected Server
            Connect(NetUtility.Resolve("localhost").ToString(),
                Convert.ToInt32(List_Items[CurrentlySelected].ServerPort));

            //Now Add go to the Server Lobby. The Lobby info will be added in by the global Client Connection Object.
            OpenServerLobby();
            ExitScreen();
        }

        /// <summary>
        /// Closes the Local Server Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            base.Btn_Cancel_Clicked(sender, e);
        }

        /// <summary>
        /// Refreshes the Local Server List. Note this uses the general Dialog 'Apply' button and renames it's event's and internal text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_Refresh_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SendDiscoverySignal();
        }


        public override void UnloadContent()
        {
            //Now Deactivate all Event Handlers
            vxEngine.ClientManager.DiscoverySignalResponseRecieved -= ClientManager_DiscoverySignalResponseRecieved;

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
                    if (CurrentlySelected == HighlightedItem_Previous)
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


        bool FirstLoop = true;
        float LoadingAlpha = 0;
        float LoadingAlpha_Req = 1;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            TimeSinceLastClick++;
        }

    }
}

#endif