#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using vxVertices.GUI;
using vxVertices.GUI.MessageBoxs;
using vxVertices.GUI.Controls;


#endregion

namespace vxVertices.Scenes.Sandbox.MessageBoxs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
	public class vxMessageBoxSaveAs : vxMessageBox
    {
        #region Fields
        
        public vxTextbox Textbox;
        
        //File Name
        string FileName = "";
        
        #endregion
        
        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
		public vxMessageBoxSaveAs(string message, string title, string fileName)
            : base(message, title)
        {
            FileName = fileName;
            this.message = message;

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
            btn_ok_text = "Save As";
            base.LoadContent();

			//First Add Textbox
            Textbox = new vxTextbox(vxEngine, FileName, 
				textPosition + new Vector2(0, textSize.Y + vPad), (int)textSize.X + 2 * hPad);

            Textbox.Textbox_Length = 350;

            xGUIManager.Add(Textbox);

            Vector2 viewportSize = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);


            // Center the message text in the viewport.
            viewport = vxEngine.GraphicsDevice.Viewport;
            textTitleSize = font.MeasureString(Title);
            textSize = font.MeasureString(message);
            textPosition = (viewportSize - textSize) / 2;
            textTitlePosition = textPosition - new Vector2(0, 2 * vPad + textTitleSize.Y);

            //Next Set Window Size as 
            backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
				(int)textPosition.Y - vPad,
				(int)Textbox.Textbox_Length + hPad * 2,
				(int)textSize.Y + vPad*2 + (int)Textbox.Textbox_height + Btn_Cancel.BoundingRectangle.Height + vPad * 2);

			TitleRectangle = new Rectangle((int)textPosition.X - hPad,
				(int)textPosition.Y - (int)textTitleSize.Y - vPad * 2 - 5,
				(int)Textbox.Textbox_Length + hPad * 2,
				(int)textTitleSize.Y + vPad);

            //Reset the Button Positions
            //Set Gui Item Positions

            Btn_Ok.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
                backgroundRectangle.Width - Btn_Ok.Width - Btn_Cancel.Width - vxEngine.vxGUITheme.Padding.X * 2,
                backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);

            Btn_Cancel.Position = new Vector2(backgroundRectangle.X, backgroundRectangle.Y) + new Vector2(
                backgroundRectangle.Width - vxEngine.vxGUITheme.vxButtons.Width - vxEngine.vxGUITheme.Padding.X,
                backgroundRectangle.Height - vxEngine.vxGUITheme.vxButtons.Height - vxEngine.vxGUITheme.Padding.Y * 2);
        }
        #endregion
    }
}
