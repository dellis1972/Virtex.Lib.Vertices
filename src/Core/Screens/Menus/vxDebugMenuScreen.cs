#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Controls;


#endregion

namespace Virtex.Lib.Vrtc.Screens.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class vxDebugMenuScreen : vxMenuBaseScreen
    {
        #region Fields

        vxMenuEntry displayDebugHUDMenuEntry;
		vxMenuEntry displayDebugRenderTargets;
		vxMenuEntry displayDebugInformation;

        static bool dispDebugHUD = true;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxDebugMenuScreen()
            : base("Debug")
        {
           
        }

        public override void LoadContent()
        {
            //dispDebug = ScreenManager.GameEngine.DisplayDebugInGame;

			displayDebugHUDMenuEntry = new vxMenuEntry(this, "Display Debug Mesh: " + (false ? "Yes" : "No"));
			displayDebugRenderTargets = new vxMenuEntry(this, "Display Debug Render Targets: " + (false ? "Yes" : "No"));
			displayDebugInformation = new vxMenuEntry(this, "Display Debug Info: " + (false ? "Yes" : "No"));

			//SetMenuEntryText();

			vxMenuEntry backMenuEntry = new vxMenuEntry(this, "Back");

			// Hook up menu event handlers.
			displayDebugHUDMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugHUDMenuEntry_Selected);
			displayDebugRenderTargets.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugRenderTargets_Selected);
			displayDebugInformation.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugInformation_Selected);
			//Accept and Cancel
			backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);



			// Add entries to the menu.
			MenuEntries.Add(displayDebugHUDMenuEntry);
			MenuEntries.Add(displayDebugRenderTargets);
			MenuEntries.Add(displayDebugInformation);
			MenuEntries.Add(backMenuEntry);

            SetMenuEntryText();

            base.LoadContent();
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            displayDebugHUDMenuEntry.Text = "Display Debug HUD: " + (vxEngine.DisplayDebugMesh ? "Yes" : "No");
            displayDebugRenderTargets.Text = "Display Debug Render Targets: " + (vxEngine.DisplayRenderTargets ? "Yes" : "No");
            displayDebugInformation.Text = "Display Debug Info: " + (vxEngine.DisplayDebugInformation ? "Yes" : "No");
        }

        void cancelMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        void backMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            SetNewValues();

            ExitScreen();
        }


        public void SetNewValues()
        {
            GraphicsDeviceManager graphics = vxEngine.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;

            graphics.ApplyChanges();
        }

        #endregion

        #region Handle Input
        
        void displayDebugHUDMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.DisplayDebugMesh = !vxEngine.DisplayDebugMesh;
            SetMenuEntryText();
        }

        void displayDebugRenderTargets_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.DisplayRenderTargets = !vxEngine.DisplayRenderTargets;
            SetMenuEntryText();
        }

        void displayDebugInformation_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxEngine.DisplayDebugInformation = !vxEngine.DisplayDebugInformation;
            SetMenuEntryText();
        }

        #endregion
    }
}
