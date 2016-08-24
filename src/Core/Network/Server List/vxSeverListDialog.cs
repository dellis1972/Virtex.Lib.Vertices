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



namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// This Dislog Displays all active server's on the connected master server.
    /// </summary>
    public class vxSeverListDialog : vxDialogBase
    {
        #region Fields

        vxListView ScrollPanel;

        private System.ComponentModel.BackgroundWorker BckgrndWrkr_FileOpen;

        string FileExtentionFilter { get; set; }

        int CurrentlySelected = -1;

		List<vxFileDialogItem> List_Items = new List<vxFileDialogItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSeverListDialog()
            : base("Server List", vxEnumButtonTypes.OkApplyCancel)
        {
            
            BckgrndWrkr_FileOpen = new System.ComponentModel.BackgroundWorker();
            BckgrndWrkr_FileOpen.WorkerReportsProgress = true;
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();




            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
            Btn_Cancel.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Cancel_Clicked);
            Btn_Apply.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
            Btn_Apply.Text = "Refresh";

            ScrollPanel = new vxListView(
                new Vector2(
					this.ArtProvider.BoundingGUIRectangle.X + this.ArtProvider.Padding.X, 
					this.ArtProvider.BoundingGUIRectangle.Y + this.ArtProvider.Padding.Y),
				(int)(this.ArtProvider.BoundingGUIRectangle.Width - this.ArtProvider.Padding.X * 2),
				(int)(this.ArtProvider.BoundingGUIRectangle.Height - Btn_Ok.BoundingRectangle.Height - this.ArtProvider.Padding.Y * 3));            

            InternalvxGUIManager.Add(ScrollPanel);


            vxEngine.GameServerListRecieved += new EventHandler<vxGameServerListRecievedEventArgs>(Engine_GameServerListRecieved);
            //Send the request string
            vxEngine.SendMessage("vrtx_request_serverList");
        }

        void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            
        }

        void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {

        }

        void Engine_GameServerListRecieved(object sender, vxGameServerListRecievedEventArgs e)
        {
            int index = 0;
            foreach (string parsestring in e.ServerList)
            {
                if (index != 0)
                {					
					vxConsole.WriteNetworkLine("IP: " + vxUtil.ReadXML(parsestring, "ip") + ", Port: " + vxUtil.ReadXML(parsestring, "port"));

                    vxListViewItem item = new vxListViewItem(vxEngine,                        
						vxUtil.ReadXML(parsestring, "ip"));
					item.ButtonWidth = ScrollPanel.Width - (int)(4 * this.ArtProvider.Padding.X);
                    
                    ScrollPanel.AddItem(item);
                }
                index++;
            }
        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            //Send the request string
            vxEngine.SendMessage("vrtx_request_serverList");
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


        bool FirstLoop = true;
        float LoadingAlpha = 0;
        float LoadingAlpha_Req = 1;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            TimeSinceLastClick++;

            if (FirstLoop)
            {
                FirstLoop = false;

                BckgrndWrkr_FileOpen.RunWorkerAsync();
            }
        }

        #region Draw

        string FileName = "";
        int GetHighlitedItem(int i)
        {
			foreach (vxFileDialogItem fd in List_Items)
            {
                fd.UnSelect();
            }

            List_Items[i].ThisSelect();
            CurrentlySelected = i;

            FileName = List_Items[i].FileName;
            return 0;
        }

        #endregion
    }
}

#endif