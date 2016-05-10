#if VRTC_INCLDLIB_NET 
using System;
using Microsoft.Xna.Framework;
using vxVertices.GUI.Controls;
using vxVertices.GUI.MessageBoxs;

namespace vxVertices.GUI.MessageBoxs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
	public class vxMessageBoxNetworkLogIn : vxMessageBox
    {
        #region Fields
        
        public vxTextbox Textbox;
        
        //File Name
        private string IpAddressToConnectTo = "127.0.0.1";
        
        #endregion
        
        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
		public vxMessageBoxNetworkLogIn(string ipAddressToConnectTo)
            : base("Enter Your Log In Information", "Log In")
        {
            IpAddressToConnectTo = ipAddressToConnectTo;

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
            btn_ok_text = "Log In";
            btn_ok_Cancel = "Offline";
            base.LoadContent();

            Textbox = new vxTextbox(vxEngine, IpAddressToConnectTo, 
                textPosition + new Vector2(0, textSize.Y + vPad), 300);

            xGUIManager.Add(Textbox);

            //Reset the Button Positions
            //Set Gui Item Positions
            Btn_Ok.Position = textPosition + new Vector2((int)Textbox.Textbox_Length - Btn_Ok.BoundingRectangle.Width - 125, textSize.Y + vPad * 2 + (int)Textbox.Textbox_height);
            Btn_Cancel.Position = textPosition + new Vector2((int)Textbox.Textbox_Length - Btn_Cancel.BoundingRectangle.Width, textSize.Y + vPad * 2 + (int)Textbox.Textbox_height);

            backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)Textbox.Textbox_Length + hPad * 2,
                                                          (int)textSize.Y + vPad*2 + (int)Textbox.Textbox_height + Btn_Cancel.BoundingRectangle.Height + vPad * 2);
            
            TitleRectangle = new Rectangle((int)textPosition.X - hPad,
                                                  (int)textPosition.Y - (int)textTitleSize.Y - vPad * 2 - 5,
                                                  (int)Textbox.Textbox_Length + hPad * 2,
                                                  (int)textTitleSize.Y + vPad);
        }
        #endregion
    }
}

#endif