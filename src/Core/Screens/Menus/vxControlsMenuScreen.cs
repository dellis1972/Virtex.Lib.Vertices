#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI.Dialogs;


#endregion

namespace Virtex.Lib.Vrtc.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class vxControlsMenuScreen : vxMenuBaseScreen
    {
        #region Fields

//        vxSliderMenuEntry MouseInvertedMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxControlsMenuScreen()
            : base("controls")
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();

			var keyboardMenuEntry = new vxMenuEntry(this, "Keyboard Settings");
			var gamepadMenuEntry = new vxMenuEntry(this, "Gamepad Settings");
			var mouseMenuEntry = new vxMenuEntry(this, "Mouse Settings");

			var backMenuEntry = new vxMenuEntry(this, "back");

			//Accept and Cancel
			keyboardMenuEntry.Selected += delegate {
				vxEngine.AddScreen(new vxKeyboardSettingsDialog(), PlayerIndex.One);	
			};
			backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);


			// Add entries to the menu.
			MenuEntries.Add(keyboardMenuEntry);
			MenuEntries.Add(gamepadMenuEntry);
			MenuEntries.Add(mouseMenuEntry);
			MenuEntries.Add(backMenuEntry);

            //Set Invert
            //if (vxEngine.Profile.Settings.Int_Mouse_Inverted == 1)
            //    MouseInvertedMenuEntry.Index = 0;
            //else
                //MouseInvertedMenuEntry.Index = 1;
        }


        void backMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //Set Invert
            //if (MouseInvertedMenuEntry.Index == 0)
            //    vxEngine.Profile.Settings.Int_Mouse_Inverted = 1;
            //else
            //    vxEngine.Profile.Settings.Int_Mouse_Inverted = -1;

            ////Save Settings
            //vxEngine.Profile.SaveSettings(vxEngine);

            ExitScreen();
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //Rectangle rec = new Rectangle(vxEngine.GraphicsDevice.Viewport.Width/2 - vxEngine.Texture_Control.Width/2, 
            //    vxEngine.GraphicsDevice.Viewport.Height/2 - vxEngine.Texture_Control.Height/2,
            //    vxEngine.Texture_Control.Width, vxEngine.Texture_Control.Height);

            //vxEngine.SpriteBatch.Begin();
            //vxEngine.SpriteBatch.Draw(vxEngine.Texture_Control, rec, Color.White * TransitionAlpha);
            //vxEngine.SpriteBatch.End();
        }

    }
}
