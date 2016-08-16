#if VRTC_INCLDLIB_NET 

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.GUI.Dialogs;
using Virtex.Lib.Vrtc.Network.Events;
using Virtex.Lib.Vrtc.GUI;
using Lidgren.Network;
using Virtex.Lib.Vrtc.Network;
using Virtex.Lib.Vrtc.GUI.MessageBoxs;
using Virtex.Lib.Vrtc.Network.Messages;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
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
        //List<NetworkPlayerInfo> SeverPlayerList = new List<NetworkPlayerInfo>();

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

            //Setup Client Events
            vxEngine.ClientManager.OtherPlayerConnected += ClientManager_OtherPlayerConnected;
            vxEngine.ClientManager.OtherPlayerDisconnected += ClientManager_OtherPlayerDisconnected;
            InternalvxGUIManager.Add(ScrollPanel);




            //Set up the Server Code
            if (vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Server)
            {
                Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

                //Create The New Server Button
                Btn_LaunchServer = new vxButton(vxEngine, "Launch Game", new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));

                //Set the Button's Position relative too the background rectangle.
                Btn_LaunchServer.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
                    vxEngine.vxGUITheme.Padding.X * 2,
                    backgroundRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight - vxEngine.vxGUITheme.Padding.Y * 2);

                Btn_LaunchServer.Clicked += Btn_LaunchServer_Clicked; ;
                InternalvxGUIManager.Add(Btn_LaunchServer);


                //Now Start The Server
                vxEngine.ServerManager.Start();


                //Now Connect to it's self if it's the Server
                NetOutgoingMessage approval = vxEngine.ClientManager.CreateMessage();
                approval.Write("secret");
                vxEngine.ClientManager.Connect(NetUtility.Resolve("localhost").ToString(), vxEngine.ServerManager.Server.Configuration.Port, approval);

            }
            //vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Client
            //The Server acts as a Client as well, so no need for an 'else if' block here.

            Btn_Ok.Text = "Not Ready";

        }


        #region Client Events


        //This event fires if a new player is found in the updated list
        private void ClientManager_OtherPlayerConnected(object sender, vxNetClientEventPlayerConnected e)
        {
            //First Add a New Player in the Manager. The details will come in an update.
            Texture2D thumbnail = vxEngine.Assets.Textures.Arrow_Right;
            vxServerLobbyPlayerItem item = new vxServerLobbyPlayerItem(vxEngine,
                e.ConnectedPlayer,
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

            ScrollPanel.Clear();

            foreach (vxServerLobbyPlayerItem it in List_Items)
                ScrollPanel.AddItem(it);
        }
        private void ClientManager_OtherPlayerDisconnected(object sender, vxNetClientEventPlayerDisconnected e)
        {
            Console.WriteLine("DISCONNECTED IN LOBBY!: " + e.DisconnectedPlayer.UserName);
            for (int i = 0; i < List_Items.Count; i++)
            {
                if (List_Items[i].Player.ID == e.DisconnectedPlayer.ID)
                {
                    List_Items.RemoveAt(i);
                    //Now Decrement the index since the count just dropped one
                    i--;
                }
            }

            //Now Re-Introduce list
            ScrollPanel.Clear();

            foreach (vxServerLobbyPlayerItem it in List_Items)
                ScrollPanel.AddItem(it);
        }

        #endregion



        /// <summary>
        /// This method is called at the end of the countdown in the lobby to launch the session.
        /// </summary>
        public virtual void LaunchSession()
        {
            if (vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Server)
            {
                //Turn off the Server
                vxEngine.ServerManager.Server.Configuration.AcceptIncomingConnections = false;
            }

            //Now untether handles

            vxEngine.ClientManager.OtherPlayerConnected -= ClientManager_OtherPlayerConnected;
            vxEngine.ClientManager.OtherPlayerDisconnected -= ClientManager_OtherPlayerDisconnected;
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


            vxEngine.ClientManager.PlayerInfo.Status = ReadyState ? vxEnumNetPlayerStatus.InServerLobbyReady : vxEnumNetPlayerStatus.InServerLobbyNotReady;
            vxEngine.ClientManager.SendMessage(new vxNetmsgUpdatePlayerLobbyStatus(vxEngine.ClientManager.PlayerInfo));

        }

        /// <summary>
        /// Closes the Local Server Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            //Always Disconnect The Client
            vxEngine.ClientManager.Disconnect();

            //If The User is the Server And Leaves, then the Shutdown Signal Needs to be Sent Also
            if (vxEngine.NetworkedGameRoll == vxEnumNetworkPlayerRole.Server)
            {
                vxEngine.ServerManager.Disconnect();
            }
            base.Btn_Cancel_Clicked(sender, e);
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

        int loop = 0;
        bool FirstLoop = true;
        float LoadingAlpha = 0;
        float LoadingAlpha_Req = 1;

        float LaunchCountdown = 1.5f;

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
                //First, automatically send user data
                vxEngine.ClientManager.PlayerInfo.Status = vxEnumNetPlayerStatus.InServerLobbyNotReady;

                //Send message with User Data
                vxEngine.ClientManager.SendMessage(new vxNetmsgAddPlayer(vxEngine.ClientManager.PlayerInfo));
            }

            if (vxEngine.ClientManager.PlayerManager.Players.Count > 1 && LaunchCountdown > 0)
            {
                SessionStateForClient = SessionState.Countdown;
                // Launch Control
                /**********************************************************************************************/
                foreach (KeyValuePair<long, vxNetPlayerInfo> entry in vxEngine.ClientManager.PlayerManager.Players)
                {
                    if (entry.Value.Status != vxEnumNetPlayerStatus.InServerLobbyReady)
                    {
                        SessionStateForClient = SessionState.Idle;
                        break;
                    }
                }
            }

            if (SessionStateForClient == SessionState.Idle)
            {
                LaunchCountdown = 1.5f;
                this.Title = originalTitle;
            }
            else if (SessionStateForClient == SessionState.Countdown)
            {
                LaunchCountdown += -1 * (float)gameTime.ElapsedGameTime.TotalSeconds;

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