#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Controls;


#endregion

namespace Virtex.Lib.Vrtc.GUI.MessageBoxs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
	public class vxMessageBoxSaveBeforeQuit : vxGameBaseScreen
    {
        #region Fields

        /// <summary>
        /// Bacground Rectangle of the Message Box
        /// </summary>
        public Rectangle backgroundRectangle { get; set; }

        /// <summary>
        /// Title Rectangle
        /// </summary>
        public Rectangle TitleRectangle { get; set; }

        /// <summary>
        /// Main text for the Message box
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Message Box Title
        /// </summary>
        public string Title { get; set; }

        public Texture2D gradientTexture;

        /// <summary>
        /// Message Box GUI Manager
        /// </summary>
        public vxGuiManager xGUIManager;

        public vxButton Btn_Save;
        public string btn_text_Save = "Save";

        public vxButton Btn_DontSave;
        public string btn_text_DontSave = "Don't Save";

        public vxButton Btn_Cancel;
        public string btn_text_Cancel = "Cancel";


        public SpriteBatch spriteBatch;
        public SpriteFont font;

        // Center the message text in the viewport.
        public Viewport viewport;
        public Vector2 textTitleSize;
        public Vector2 textSize;
        public Vector2 textPosition;
        public Vector2 textTitlePosition;


        // The background includes a border somewhat larger than the text itself.
        public int hPad = 12;
        public int vPad = 12;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Save;
        public event EventHandler<PlayerIndexEventArgs> DontSave;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization



        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
		public vxMessageBoxSaveBeforeQuit(string message, string title)
        {
            this.message = message;

            Title = title;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            xGUIManager = new vxGuiManager();

            Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

            Btn_Save = new vxButton(vxEngine, btn_text_Save, new Vector2(viewportSize.X / 2 - 230, viewportSize.Y / 2 + 20));
			Btn_Save.Clicked += Method_Save;
            Btn_DontSave = new vxButton(vxEngine, btn_text_DontSave, new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
			Btn_DontSave.Clicked += Method_DontSave;
            Btn_Cancel = new vxButton(vxEngine, btn_text_Cancel, new Vector2(viewportSize.X / 2 + 15, viewportSize.Y / 2 + 20));
			Btn_Cancel.Clicked += Btn_Cancel_Clicked;

            xGUIManager.Add(Btn_Save);
            xGUIManager.Add(Btn_DontSave);
            xGUIManager.Add(Btn_Cancel);
            

            spriteBatch = vxEngine.SpriteBatch;
			font = vxEngine.vxGUITheme.Font;;

            // Center the message text in the viewport.
            viewport = vxEngine.GraphicsDevice.Viewport;
            textTitleSize = font.MeasureString(Title);
            textSize = font.MeasureString(message);
            
            //First Get Length of Text
            int textLength = (int)textSize.X;

			int totalBtnWidth = (int)(vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 3 + vxEngine.vxGUITheme.Padding.X * 5);

            textSize = new Vector2(Math.Max(textLength, totalBtnWidth), textSize.Y);

            textPosition = (viewportSize - textSize) / 2;
            textTitlePosition = textPosition - new Vector2(0, 2 * vPad + textTitleSize.Y);

            //Set Gui Item Positions

            backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad + Btn_Cancel.BoundingRectangle.Height + vPad);

            TitleRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - (int)textTitleSize.Y - vPad * 2 - 5,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textTitleSize.Y + vPad);

			Btn_Save.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 3 - vxEngine.vxGUITheme.Padding.X * 4, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight - vxEngine.vxGUITheme.Padding.Y * 2);
			
			Btn_DontSave.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width  - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 2 - vxEngine.vxGUITheme.Padding.X * 3, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight - vxEngine.vxGUITheme.Padding.Y * 2);
			
			Btn_Cancel.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth - vxEngine.vxGUITheme.Padding.X, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight - vxEngine.vxGUITheme.Padding.Y * 2);
        }


        #endregion

        #region Handle Input


		void Method_Save(object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
        {
            // Raise the accepted event, then exit the message box.
            if (Save != null)
                Save(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

            ExitScreen();
        }

		void Method_DontSave(object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
        {
            // Raise the accepted event, then exit the message box.
            if (DontSave != null)
                DontSave(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

            ExitScreen();
        }


		/// <summary>
		// Raise the cancelled event, then exit the message box.
		/// </summary>
		void Btn_Cancel_Clicked (object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
		{
			if (Cancelled != null)
				Cancelled(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

			ExitScreen();
		}

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(vxInputManager input)
        {

        }

        #endregion

        int cnt = 0;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Update GUI Manager
            foreach (vxGUIBaseItem item in xGUIManager.List_GUIItems)
            {
                if (TransitionAlpha < 0.9999f)
                    item.Enabled = false;
                else
                    item.Enabled = true;
            }
            xGUIManager.Update(vxEngine);
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
            Color color = Color.Black * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the Title
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, TitleRectangle, color * 0.65f);
            spriteBatch.DrawString(font, Title, textTitlePosition, Color.White);
            
            // Draw the message box text.
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, backgroundRectangle, color * 0.75f);
            spriteBatch.DrawString(font, message, textPosition, Color.White);

            spriteBatch.End();

            //Draw the GUI
            xGUIManager.Draw(vxEngine);
        }


        #endregion
    }
}
