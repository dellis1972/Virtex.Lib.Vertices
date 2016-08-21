#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using Virtex.Lib.Vrtc.GUI.GuiArtProvider;


#endregion

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
using Virtex.Lib.Vrtc.Localization;
#endregion

namespace Virtex.Lib.Vrtc.GUI.MessageBoxs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxMessageBox : vxGameBaseScreen
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

        public vxButton Btn_Ok;
        public string btn_ok_text = "OK";
        public vxButton Btn_Cancel;
        public string btn_ok_Cancel = "Cancel";


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


		/// <summary>
		/// The given Art Provider of the Menu Entry. 
		/// </summary>
		public vxMessageBoxArtProvider ArtProvider { get; internal set; }

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization



        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
		public vxMessageBox(string message, string title)
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

            btn_ok_text = LanguagePack.Get(vxLocalization.Misc_OK);
            btn_ok_Cancel = LanguagePack.Get(vxLocalization.Misc_Cancel);

            Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

			Btn_Ok = new vxButton (vxEngine, btn_ok_text, new Vector2 (viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
			Btn_Ok.Clicked += Btn_Ok_Clicked;;
            Btn_Cancel = new vxButton(vxEngine, btn_ok_Cancel, new Vector2(viewportSize.X / 2 + 15, viewportSize.Y / 2 + 20));
			Btn_Cancel.Clicked += Btn_Cancel_Clicked;;

            xGUIManager.Add(Btn_Ok);
            xGUIManager.Add(Btn_Cancel);
            

            spriteBatch = vxEngine.SpriteBatch;
			font = vxEngine.vxGUITheme.Font;

            // Center the message text in the viewport.
            viewport = vxEngine.GraphicsDevice.Viewport;
            textTitleSize = font.MeasureString(Title);
            textSize = font.MeasureString(message);
            textPosition = (viewportSize - textSize) / 2;
            textTitlePosition = textPosition - new Vector2(0, 2 * vPad + textTitleSize.Y);

            //Set Gui Item Positions

			int length = Math.Max (vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 2 + (int)vxEngine.vxGUITheme.Padding.X * 2, (int)textSize.X);

            backgroundRectangle = new Rectangle(
				(int)textPosition.X - hPad,
                (int)textPosition.Y - vPad,
				length + hPad * 2,
				(int)textSize.Y + vPad + vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight + vPad * 2);

            TitleRectangle = new Rectangle(
				(int)textPosition.X - hPad,
                (int)textPosition.Y - (int)textTitleSize.Y - vPad * 2 - 5,
				length + hPad * 2,
				(int)textTitleSize.Y + vPad);

			Btn_Ok.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 2 - vxEngine.vxGUITheme.Padding.X * 2, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight - vxEngine.vxGUITheme.Padding.Y * 2);

			Btn_Cancel.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
				backgroundRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth - vxEngine.vxGUITheme.Padding.X, 
				backgroundRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight - vxEngine.vxGUITheme.Padding.Y * 2);


			this.ArtProvider = (vxMessageBoxArtProvider)vxEngine.vxGUITheme.ArtProviderForMessageBoxes.Clone();
        }


        #endregion

        #region Handle Input


		/// <summary>
		/// Raise the accepted event, then exit the message box.
		/// </summary>
		void Btn_Ok_Clicked (object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
		{
			if (Accepted != null)
				Accepted(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

			ExitScreen();
		}

		/// <summary>
		// Raise the cancelled event, then exit the message box.
		/// </summary>
		void Btn_Cancel_Clicked (object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
		{
			Method_Cancel ();
		}


		public void Method_Cancel()
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

        }
        
        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Update GUI Manager
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

			this.ArtProvider.Draw(this);

            //Draw the GUI
            xGUIManager.Draw(vxEngine);
        }


		public virtual void SetArtProvider(vxMessageBoxArtProvider NewArtProvider)
		{
			this.ArtProvider = (vxMessageBoxArtProvider)NewArtProvider.Clone();
		}


        #endregion
    }
}
