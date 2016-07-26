using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Core.Input.Events;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.GUI.Events;

namespace Virtex.Lib.Vertices.GUI.Dialogs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxDialogBase : vxGameBaseScreen
    {
        #region Fields

        /// <summary>
        /// Bacground Rectangle of the Message Box
        /// </summary>
        public Rectangle backgroundRectangle { get; set; }
        public Rectangle brdr_backgroundRectangle { get; set; }

        /// <summary>
        /// Title Rectangle
        /// </summary>
        public Rectangle TitleRectangle { get; set; }
        public Rectangle brdr_TitleRectangle { get; set; }

        /// <summary>
        /// Main text for the Message box
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Message Box Title
        /// </summary>
        public string Title { get; set; }

        public Texture2D gradientTexture;

        /// <summary>
        /// The Internal GUI Manager so the Dialog Box can handle it's own items.
        /// </summary>
        public vxGuiManager InternalvxGUIManager {get; set;}
        
        public vxButton Btn_Ok;
        public string btn_ok_text = "Okay";

        public vxButton Btn_Apply;
        public string btn_Apply_text = "Apply";

        public vxButton Btn_Cancel;
        public string btn_ok_Cancel = "Cancel";

        public SpriteBatch spriteBatch;
        public SpriteFont font;

        // Center the message text in the viewport.
        public Viewport viewport;
        public Vector2 textTitleSize;

        public Vector2 textTitlePosition;

		List<vxFileDialogItem> List_Items = new List<vxFileDialogItem>();
        
        // The background includes a border somewhat larger than the text itself.
        public int hPad = 16;
        public int vPad = 16;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Apply;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

        ButtonTypes ButtonTypes;

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public vxDialogBase(string Title, ButtonTypes ButtonTypes)
        {
            this.Title = Title;

            this.ButtonTypes = ButtonTypes;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }
        #endregion


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            InternalvxGUIManager = new vxGuiManager();

            Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

            Btn_Apply = new vxButton(vxEngine, btn_Apply_text, new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
            Btn_Ok = new vxButton(vxEngine, btn_ok_text, new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
            Btn_Cancel = new vxButton(vxEngine, btn_ok_Cancel, new Vector2(viewportSize.X / 2 + 15, viewportSize.Y / 2 + 20));

            //Btn_Apply.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
            //Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
            Btn_Cancel.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Cancel_Clicked);

            if(ButtonTypes == ButtonTypes.OkApplyCancel)
                InternalvxGUIManager.Add(Btn_Apply);

            InternalvxGUIManager.Add(Btn_Ok);
            InternalvxGUIManager.Add(Btn_Cancel);

            spriteBatch = vxEngine.SpriteBatch;
			font = vxEngine.vxGUITheme.Font;

            // Center the message text in the viewport.
            viewport = vxEngine.GraphicsDevice.Viewport;
            textTitleSize = font.MeasureString(Title);
            

            textTitlePosition = new Vector2(2 * hPad, 1.5f * vPad);

            TitleRectangle = new Rectangle((int)hPad, (int)vPad,
                                                           viewport.Width - hPad * 2,
                                                          (int)textTitleSize.Y + vPad);


            brdr_TitleRectangle = new Rectangle(
                (int)hPad - vxEngine.vxGUITheme.vxDialogs.Header_BorderWidth,
                (int)vPad - vxEngine.vxGUITheme.vxDialogs.Header_BorderWidth,
                TitleRectangle.Width + vxEngine.vxGUITheme.vxDialogs.Header_BorderWidth * 2,
                TitleRectangle.Height + vxEngine.vxGUITheme.vxDialogs.Header_BorderWidth * 2);


            backgroundRectangle = new Rectangle((int)hPad, TitleRectangle.Y + TitleRectangle.Height + vPad / 4,
                                                          viewport.Width - hPad * 2,
                                                          viewport.Height - vPad - TitleRectangle.Y - TitleRectangle.Height - vPad / 4);


            //Reset Gui Item Positions Based off of Background Rectangle
			Btn_Apply.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.vxButtons.Width * 3 - vxEngine.vxGUITheme.Padding.X * 3, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);
			
			Btn_Ok.Position =  new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.vxButtons.Width * 2 - vxEngine.vxGUITheme.Padding.X * 2, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);
			
			Btn_Cancel.Position =  new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.vxButtons.Width - vxEngine.vxGUITheme.Padding.X, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);

        }

        public virtual void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            ExitScreen();
        }
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Update GUI Manager
            InternalvxGUIManager.Update(vxEngine);
        }

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            vxEngine.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            
            // Fade the popup alpha during transitions.
            Color color = Color.DimGray * TransitionAlpha;

            spriteBatch.Begin();
            
            // Draw the message box text.
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, backgroundRectangle, vxEngine.vxGUITheme.vxDialogs.BackgroundColour);

            // Draw the Title
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, brdr_TitleRectangle, Color.Black);
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, TitleRectangle, vxEngine.vxGUITheme.vxDialogs.Header_BackgroundColour);
            spriteBatch.DrawString(font, Title, textTitlePosition, vxEngine.vxGUITheme.vxDialogs.Header_TextColour);

            spriteBatch.End();

            //Draw the GUI
            InternalvxGUIManager.Draw(vxEngine);
        }

        #endregion
    }
}
