using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.Mathematics;
using System.Xml.Serialization;
using Virtex.Lib.Vrtc.Scenes.Sandbox3D;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// Open File Dialog
    /// </summary>
    public class vxOpenFileDialog : vxDialogBase
    {
        #region Fields

        vxScrollPanel ScrollPanel;

        public System.ComponentModel.BackgroundWorker BckgrndWrkr_FileOpen;

        string FileExtentionFilter { get; set; }

        int CurrentlySelected = -1;

		List<vxFileDialogItem> List_Items = new List<vxFileDialogItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;
        
        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

        public string FileName = "";


        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Core.vxOpenFileDialog"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="path">Path.</param>
        /// <param name="FileExtentionFilter">File extention filter.</param>
        public vxOpenFileDialog(vxEngine vxEngine, string path, string FileExtentionFilter)
            : base("Open a Sandbox File", ButtonTypes.OkCancel)
        {
            this.vxEngine = vxEngine;

            this.Path = path;
            this.FileExtentionFilter = FileExtentionFilter;
            
            BckgrndWrkr_FileOpen = new System.ComponentModel.BackgroundWorker();
            BckgrndWrkr_FileOpen.WorkerReportsProgress = true;
            //BckgrndWrkr_FileOpen.DoWork += new System.ComponentModel.DoWorkEventHandler(BckgrndWrkr_FileOpen_DoWork);
            BckgrndWrkr_FileOpen.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(BckgrndWrkr_FileOpen_ProgressChanged);
            BckgrndWrkr_FileOpen.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(BckgrndWrkr_FileOpen_RunWorkerCompleted);
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

            ScrollPanel = new vxScrollPanel(
                new Vector2(
                    backgroundRectangle.X + hPad, 
                    backgroundRectangle.Y + vPad),
                backgroundRectangle.Width - hPad * 2,
                backgroundRectangle.Height - Btn_Ok.BoundingRectangle.Height - vPad * 3);

            InternalvxGUIManager.Add(ScrollPanel);
        }

        public override void UnloadContent()
        {
			foreach (vxFileDialogItem fd in List_Items)
                fd.ButtonImage.Dispose();

            base.UnloadContent();
        }

        #endregion

        #region Handle Input


        void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            if (FileName != "")
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

                ExitScreen();
            }
        }

        void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            // Raise the cancelled event, then exit the message box.
            if (Cancelled != null)
                Cancelled(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

            ExitScreen();
        }
        


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(vxInputManager input)
        {
            //if (input.CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
            //    input.LastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
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


        /// <summary>
        /// Draws the message box.
        /// </summary>
        string WaitingText = "";
        int periodCount = 0;
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();

			LoadingAlpha = vxSmooth.SmoothFloat(LoadingAlpha, LoadingAlpha_Req, 8);

            WaitingText = "<Please Wait/>";
            WaitingText += new string('.', (int)((periodCount/2) % 5));
            
			spriteBatch.DrawString(vxEngine.vxGUITheme.Font, WaitingText, 
                new Vector2(viewport.Width / 2 - 50, viewport.Height / 2), Color.WhiteSmoke * LoadingAlpha*0.75f);

            spriteBatch.End();
        }
        
        public virtual void BckgrndWrkr_FileOpen_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string[] filePaths = Directory.GetFiles(vxEngine.Path_Sandbox, "*.sbx");

            List<vxFileDialogItem> List_Temp_Items = new List<vxFileDialogItem>();

            int index = 0;
            foreach (string file in filePaths)
            {
                //Temp Level File
                vxSandBoxFileStructure tempLevelFile;

                //Deserialize the input xml file
                XmlSerializer deserializer = new XmlSerializer(typeof(vxSandBoxFileStructure));
                TextReader reader = new StreamReader(file);
                tempLevelFile = (vxSandBoxFileStructure)deserializer.Deserialize(reader);
                reader.Close();


                Texture2D thumbnail = new Texture2D(vxEngine.GraphicsDevice, tempLevelFile.textureWidth, tempLevelFile.textureHeight);//vxEngine.Assets.Textures.Arrow_Right;

                try
                {
                    thumbnail.SetData<byte>(tempLevelFile.texture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }

                //Create a New File Dialog Button
                vxFileDialogItem fileDialogButton = new vxFileDialogItem(
                    vxEngine,
                    file,
                    new Vector2(
                        (int)(2 * hPad),
                        vPad + (vPad / 10 + 68) * (index + 1)),
                    thumbnail, index);

                fileDialogButton.Clicked += GetHighlitedItem;

                //Set Button Width
                fileDialogButton.ButtonWidth = vxEngine.GraphicsDevice.Viewport.Width - (4 * hPad);

                List_Temp_Items.Add(fileDialogButton);

                index++;
                BckgrndWrkr_FileOpen.ReportProgress(index);
            }
            e.Result = List_Temp_Items;
        }

        void BckgrndWrkr_FileOpen_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            periodCount = e.ProgressPercentage;
        }

        void BckgrndWrkr_FileOpen_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                List_Items.AddRange((List<vxFileDialogItem>)e.Result);
                ScrollPanel.AddRange((List<vxFileDialogItem>)e.Result);
            }
            LoadingAlpha_Req = 0;
        }


		public void GetHighlitedItem(object sender, vxGuiItemClickEventArgs e)
        {
			foreach (vxFileDialogItem fd in List_Items)
            {
                fd.UnSelect();
            }
			int i = e.GUIitem.Index;

            List_Items[i].ThisSelect();
            CurrentlySelected = i;

            FileName = List_Items[i].FileName;            
        }

        #endregion
    }
}
