#region File Description
//-----------------------------------------------------------------------------
// DebugSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using System;
using Virtex.Lib.Vrtc.Utilities;


#endregion

/*
 * To get started with the GameDebugTools, go to your main game class, override the Initialize method and add the
 * following line of code:
 * 
 * GameDebugTools.DebugSystem.Initialize(this, "MyFont");
 * 
 * where "MyFont" is the name a SpriteFont in your content project. This method will initialize all of the debug
 * tools and add the necessary components to your game. To begin instrumenting your game, add the following line of 
 * code to the top of your Update method:
 *
 * GameDebugTools.DebugSystem.Instance.TimeRuler.StartFrame()
 * 
 * Once you have that in place, you can add markers throughout your game by surrounding your code with BeginMark and
 * EndMark calls of the TimeRuler. For example:
 * 
 * GameDebugTools.DebugSystem.Instance.TimeRuler.BeginMark("SomeCode", Color.Blue);
 * // Your code goes here
 * GameDebugTools.DebugSystem.Instance.TimeRuler.EndMark("SomeCode");
 * 
 * Then you can display these results by setting the Visible property of the TimeRuler to true. This will give you a
 * visual display you can use to profile your game for optimizations.
 *
 * The GameDebugTools also come with an FpsCounter and a DebugCommandUI, which allows you to type commands at runtime
 * and toggle the various displays as well as registering your own commands that enable you to alter your game without
 * having to restart.
 */

using Microsoft.Xna.Framework;

namespace Virtex.Lib.Vrtc.Core.Debug
{
    /// <summary>
    /// DebugSystem is a helper class that streamlines the creation of the various GameDebug
    /// pieces. While games are free to add only the pieces they care about, DebugSystem allows
    /// games to quickly create and add all the components by calling the Initialize method.
    /// </summary>
    public class vxDebugSystem
    {
		private static vxDebugSystem singletonInstance;

        /// <summary>
        /// Gets the singleton instance of the debug system. You must call Initialize
        /// to create the instance.
        /// </summary>
		public static vxDebugSystem Instance
        {
            get { return singletonInstance; }
        }

        /// <summary>
        /// Gets the DebugManager for the system.
        /// </summary>
		public vxDebugManager DebugManager { get; private set; }

        /// <summary>
        /// Gets the DebugCommandUI for the system.
        /// </summary>
        public vxDebugCommandUI DebugCommandUI { get; private set; }

        /// <summary>
        /// Gets the FpsCounter for the system.
        /// </summary>
		public vxDebugCntrlFpsCounter FpsCounter { get; private set; }

        /// <summary>
        /// Gets the TimeRuler for the system.
        /// </summary>
		public vxDebugCntrlTimeRuler TimeRuler { get; private set; }

#if !WINDOWS_PHONE
        /// <summary>
        /// Gets the RemoteDebugCommand for the system.
        /// </summary>
        //public RemoteDebugCommand RemoteDebugCommand { get; private set; }
#endif
		/// <summary>
		/// Initializes the DebugSystem and adds all components to the game's Components collection.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="debugFont">Debug font.</param>
        public static vxDebugSystem Initialize(vxEngine vxEngine, string debugFont)
        {
            Game game = vxEngine.Game;

            // if the singleton exists, return that; we don't want two systems being created for a game
            if (singletonInstance != null)
                return singletonInstance;

            // Create the system
			singletonInstance = new vxDebugSystem();

            // Create all of the system components
			singletonInstance.DebugManager = new vxDebugManager(vxEngine, game, debugFont);
            #if !XNA
			singletonInstance.DebugManager.Initialize ();
            #endif
            game.Components.Add(singletonInstance.DebugManager);

			singletonInstance.DebugCommandUI = new vxDebugCommandUI(vxEngine);
#if !VRTC_PLTFRM_XNA
			singletonInstance.DebugCommandUI.Initialize ();
#endif
            game.Components.Add(singletonInstance.DebugCommandUI);

			singletonInstance.FpsCounter = new vxDebugCntrlFpsCounter(game);
#if !VRTC_PLTFRM_XNA
			singletonInstance.FpsCounter.Initialize ();
#endif
            game.Components.Add(singletonInstance.FpsCounter);

			singletonInstance.TimeRuler = new vxDebugCntrlTimeRuler(game);
#if !VRTC_PLTFRM_XNA
			singletonInstance.TimeRuler.Initialize ();
#endif
            game.Components.Add(singletonInstance.TimeRuler);



			//Setup Basic Commands
			// Register's Command to Show Render Targets on the Screen
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"rt",              // Name of command
				"Toggle Viewing Individual Render Targets",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_RNDRTRGT).Value = !vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_RNDRTRGT).GetAsBool();
				});


			// Register's Command to Show Render Targets on the Screen
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"dmesh",              // Name of command
				"Toggles Debug Mesh's",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_MESH).Value = !vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_MESH).GetAsBool();
				});



			// Register's Command to Show Render Targets on the Screen
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"gcon",              // Name of command
				"Toggle the in-game console which won't pause the game (Different than this console)",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_INGMECNSL).Value = !vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_INGMECNSL).GetAsBool();
				});





			// Set Resolution Width
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"width",              // Name of command
				"Set's Resoultion Width. (Example: -<cmd> <width>)",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar (vxEnumEnvVarType.RES_X).Value = Convert.ToInt32(args[0]);
				});


			// Set Resolution Height
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"height" ,              // Name of command
				"Set's Resoultion Height. (Example: -<cmd> <height>)",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar (vxEnumEnvVarType.RES_Y).Value = Convert.ToInt32(args[0]);
				});


			// Set Windowed Mode
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"windowed",              // Name of command
				"Forces the Game to start in Windowed Mode.",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar (vxEnumEnvVarType.FLSCRN).Value = false;
				});


			// Set Windowed Mode
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"fullscreen",              // Name of command
				"Forces the Game to start in Fullscreen Mode.",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					vxEnviroment.GetVar (vxEnumEnvVarType.FLSCRN).Value = true;
				});


			// Register's Command to Show Render Targets on the Screen
			/*****************************************************************************************************/
			singletonInstance.DebugCommandUI.RegisterCommand (
				"setvar",              // Name of command
				"Set's a value to a specific variable which is preregisgtered with the debug system. add -help for more information",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {

					switch (args [0]) {
					case "-help":
						singletonInstance.DebugCommandUI.Echo ("");
						singletonInstance.DebugCommandUI.Echo ("Set the Enviroment Variable by issueing the command:");
						singletonInstance.DebugCommandUI.Echo ("");

						singletonInstance.DebugCommandUI.Echo ("set [var] [value]");

						singletonInstance.DebugCommandUI.Echo ("");
						singletonInstance.DebugCommandUI.Echo ("Enviroment Variables List");
						singletonInstance.DebugCommandUI.Echo ("------------------------------");

						int length = 20;
						//Get Length
						foreach (KeyValuePair<object, vxVar> entry in vxEnviroment.Variables) {
							string str = String.Format ("   {0} = {1}", 
								entry.Key, entry.Value.Value);

							length = Math.Max (length, str.Length);
						}
						length += 5;

						//Now Echo the Values
						foreach (KeyValuePair<object, vxVar> entry in vxEnviroment.Variables) {
							string val = String.Format ("   {0} = {1}", 
								entry.Key, entry.Value.Value);

							int cmdlen = val.Length;
							singletonInstance.DebugCommandUI.Echo (String.Format ("   {0}" + new String ('.', length - cmdlen) + " : {1}", 
								val, entry.Value.Description));
						}
						singletonInstance.DebugCommandUI.Echo ("");
						break;
					default:
						if (args.Count > 1) {
							try {
								//Look up the variable. All keys are converted too strings when added, so applying just the
								//first argument as a string
								vxEnviroment.Variables [args [0]].Value = args [1];
							} catch (Exception ex) {
								vxConsole.WriteLine (">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
								vxConsole.WriteLine ("Error Setting Enviroment Variable");
								vxConsole.WriteLine (ex.Message);
								vxConsole.WriteLine (">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
							}
						}
						break;
					}
				});

            return singletonInstance;
        }

        // Private constructor; games should use Initialize
		private vxDebugSystem() { }
    }
}
