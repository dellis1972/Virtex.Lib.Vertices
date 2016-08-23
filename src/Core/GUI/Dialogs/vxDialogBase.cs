using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.GUI.GuiArtProvider;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxDialogBase : vxGameBaseScreen
    {
        #region Fields



        /// <summary>
        /// Main text for the Message box
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Message Box Title
        /// </summary>
        public string Title { get; set; }


		/// <summary>
		/// The given Art Provider of the Menu Entry. 
		/// </summary>
		public vxDialogArtProvider ArtProvider { get; internal set; }

        //public Texture2D gradientTexture;

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


		List<vxScrollPanelItem> List_Items = new List<vxScrollPanelItem>();

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Apply;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

		vxEnumButtonTypes ButtonTypes;

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
		public vxDialogBase(string Title, vxEnumButtonTypes ButtonTypes)
        {
            this.Title = Title;

            this.ButtonTypes = ButtonTypes;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }
        #endregion

        public Vector2 viewportSize;

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            InternalvxGUIManager = new vxGuiManager();

			this.ArtProvider = (vxDialogArtProvider)vxEngine.vxGUITheme.ArtProviderForDialogs.Clone();

            viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);

            Btn_Apply = new vxButton(vxEngine, btn_Apply_text, new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
            Btn_Ok = new vxButton(vxEngine, btn_ok_text, new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
            Btn_Cancel = new vxButton(vxEngine, btn_ok_Cancel, new Vector2(viewportSize.X / 2 + 15, viewportSize.Y / 2 + 20));

            //Btn_Apply.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
            Btn_Cancel.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Cancel_Clicked);

			if(ButtonTypes == vxEnumButtonTypes.OkApplyCancel)
                InternalvxGUIManager.Add(Btn_Apply);

            InternalvxGUIManager.Add(Btn_Ok);
            InternalvxGUIManager.Add(Btn_Cancel);

            spriteBatch = vxEngine.SpriteBatch;
			font = vxEngine.vxGUITheme.Font;

            

            //Reset Gui Item Positions Based off of Background Rectangle
			Btn_Apply.Position = new Vector2(
				this.ArtProvider.BoundingGUIRectangle.X, 
				this.ArtProvider.BoundingGUIRectangle.Y) + this.ArtProvider.PosOffset
				+ new Vector2(
					this.ArtProvider.BoundingGUIRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 3 - vxEngine.vxGUITheme.Padding.X * 3, 
					this.ArtProvider.BoundingGUIRectangle.Bottom - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight * 3 - vxEngine.vxGUITheme.Padding.Y * 2);
			
			Btn_Ok.Position =  new Vector2(
				this.ArtProvider.BoundingGUIRectangle.X, 
				this.ArtProvider.BoundingGUIRectangle.Y) + this.ArtProvider.PosOffset
				+ new Vector2(
					this.ArtProvider.BoundingGUIRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth * 2 - vxEngine.vxGUITheme.Padding.X * 2, 
					this.ArtProvider.BoundingGUIRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight * 2 - vxEngine.vxGUITheme.Padding.Y * 2);
			
			Btn_Cancel.Position =  new Vector2(
				this.ArtProvider.BoundingGUIRectangle.X, this.ArtProvider.BoundingGUIRectangle.Y) + this.ArtProvider.PosOffset
				+ new Vector2(
					this.ArtProvider.BoundingGUIRectangle.Width - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultWidth - vxEngine.vxGUITheme.Padding.X, 
					this.ArtProvider.BoundingGUIRectangle.Height - vxEngine.vxGUITheme.ArtProviderForButtons.DefaultHeight * 2 - vxEngine.vxGUITheme.Padding.Y * 2);

        }

		public virtual void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
		{
			// Raise the accepted event, then exit the message box.
			if (Accepted != null)
				Accepted(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

			ExitScreen();

		}

        public virtual void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
			// Raise the cancelled event, then exit the message box.
			if (Cancelled != null)
				Cancelled(this, new PlayerIndexEventArgs(ControllingPlayer.Value));
			
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
			//First Draw the Dialog Art Provider
			this.ArtProvider.Draw(this);

            //Draw the GUI
            InternalvxGUIManager.Draw(vxEngine);
		}

		public virtual void SetArtProvider(vxDialogArtProvider NewArtProvider)
		{
			this.ArtProvider = (vxDialogArtProvider)NewArtProvider.Clone();
		}

        #endregion
    }
}
