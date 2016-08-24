#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Controls;


#if VRTC_PLTFRM_DROID
using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
#endif

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
			displayDebugInformation = new vxMenuEntry(this, "Display FPS Info: " + (false ? "Yes" : "No"));

			//SetMenuEntryText();

			vxMenuEntry backMenuEntry = new vxMenuEntry(this, "Back");

			// Hook up menu event handlers.
			displayDebugHUDMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugHUDMenuEntry_Selected);
			displayDebugRenderTargets.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugRenderTargets_Selected);
			displayDebugInformation.Selected += new System.EventHandler<PlayerIndexEventArgs>(displayDebugInformation_Selected);
			//Accept and Cancel
			backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);



			// Add entries to the menu.
			#if VRTC_PLTFRM_DROID
			MenuEntries.Add(displayDebugHUDMenuEntry);
			#endif
			//MenuEntries.Add(displayDebugRenderTargets);
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
			displayDebugHUDMenuEntry.Text = "Test Android Keyboard";
			displayDebugRenderTargets.Text = "Display Debug Render Targets: " + ((bool)vxEngine.EnviromentVariables[vxEnumEnvVarType.DEBUG_RNDRTRGT.ToString()].Var ? "Yes" : "No");
			displayDebugInformation.Text = "Display Debug Info: " + ((bool)vxEngine.EnviromentVariables[vxEnumEnvVarType.DEBUG_SHW_FPS.ToString()].Var ? "Yes" : "No");
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

#if VRTC_PLTFRM_DROID
            
            /*
            InputMethodManager imm = (InputMethodManager)vxEngine.Activity.GetSystemService(Activity.INPUT_METHOD_SERVICE);
            //Find the currently focused view, so we can grab the correct window token from it.
            View view = activity.getCurrentFocus();
            //If no view currently has focus, create a new one, just so we can grab a window token from it
            if (view == null)
            {
                view = new View(activity);
            }
            imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
            */
            var pView = vxEngine.Game.Services.GetService<View>();
            var inputMethodManager = vxEngine.Activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
#endif
            SetMenuEntryText();
        }

        void displayDebugRenderTargets_Selected(object sender, PlayerIndexEventArgs e)
        {
            //vxEngine.DisplayRenderTargets = !vxEngine.DisplayRenderTargets;
            SetMenuEntryText();
        }

        void displayDebugInformation_Selected(object sender, PlayerIndexEventArgs e)
        {
			if ((bool)this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_FPS.ToString ()].Var == true) {
				this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_FPS.ToString ()].Var = false;
				this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_TIMERULES.ToString ()].Var = false;
			} else {
				this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_FPS.ToString ()].Var = true;
				this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_TIMERULES.ToString ()].Var = true;
			}

			this.vxEngine.DebugSystem.FpsCounter.Visible = (bool)this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_FPS.ToString ()].Var;
			this.vxEngine.DebugSystem.TimeRuler.Visible = (bool)this.vxEngine.EnviromentVariables [vxEnumEnvVarType.DEBUG_SHW_TIMERULES.ToString ()].Var;
			SetMenuEntryText();
        }

        #endregion
    }
}
