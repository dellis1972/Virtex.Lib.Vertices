#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core.Input.Events;
using vxVertices.Core;
using vxVertices.GUI.Controls;


#endregion

namespace vxVertices.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class ControlsMenuScreen : vxMenuBaseScreen
    {
        #region Fields

//        vxSliderMenuEntry MouseInvertedMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlsMenuScreen()
            : base("controls")
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();
			//            MouseInvertedMenuEntry = new vxSliderMenuEntry("mouse:", 5, 150, 50);
			//            MouseInvertedMenuEntry.ItemList.Add("normal");
			//            MouseInvertedMenuEntry.ItemList.Add("inverted");

			vxMenuEntry backMenuEntry = new vxMenuEntry(this, "back");

			//Accept and Cancel
			backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);


			// Add entries to the menu.
			//            MenuEntries.Add(MouseInvertedMenuEntry);
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
