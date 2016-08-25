using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.Core
{
	/// <summary>
	/// A static class which holds all Enviroment Variables
	/// </summary>
	public class vxEnviroment
    {
		/// <summary>
		/// The current instance of the running Engine.
		/// </summary>
        public static vxEngine vxEngine;

		/// <summary>
		/// Gets or sets the enviroment variables object.
		/// </summary>
		/// <value>The variables.</value>
		public static Dictionary<object, vxVar> Variables { get; internal set; }

		/// <summary>
		/// Initialises the vxConsole Static Object.
		/// </summary>
		/// <param name="engine">Engine.</param>
		public static void Initialize(vxEngine engine)
		{
			vxEngine = engine;

			Variables = new Dictionary<object, vxVar> ();


			#region Graphics Variables
			AddVar (new vxVar (vxEnumEnvVarType.RES_X, 
				engine.GraphicsDevice.PresentationParameters.BackBufferWidth,
				"The Resolution Width", 
				delegate (vxVar v) {
					Console.WriteLine("Setting '" + v.Name + "' to " + v.Value);
				}
			));

			AddVar (new vxVar (vxEnumEnvVarType.RES_Y,
				engine.GraphicsDevice.PresentationParameters.BackBufferHeight,
				"The Resolution Height", 
				delegate (vxVar v) {
					Console.WriteLine("Setting '" + v.Name + "' to " + v.Value);
				}
			));

			vxVar newvar = new vxVar (vxEnumEnvVarType.FLSCRN, 
				engine.GraphicsDevice.PresentationParameters.IsFullScreen, 
				"Full Screen", 
				delegate (vxVar v) {
					Console.WriteLine("Setting '" + v.Name + "' to " + v.Value);
				});
			AddVar (newvar);

			AddVar (new vxVar (vxEnumEnvVarType.VSYNC,
				true,
				"Vsync", 
				delegate (vxVar v) {
					Console.WriteLine("Setting '" + v.Name + "' to " + v.Value);
				}
			));

			#endregion

			#region DEBUG Variables

			AddVar (new vxVar (vxEnumEnvVarType.DEBUG_MESH, false,
				"Toggles the Debug Mesh for Physics"));

			AddVar (new vxVar (vxEnumEnvVarType.DEBUG_RNDRTRGT, false,
				"Toggles viewing the Individual Render Targets"));

			AddVar (new vxVar (vxEnumEnvVarType.DEBUG_INGMECNSL, false,
				"Toggles the In-Game Debug Window"));

			AddVar (new vxVar (vxEnumEnvVarType.DEBUG_SHW_FPS, false,
				"Toggles the FPS Counter"));

			AddVar (new vxVar (vxEnumEnvVarType.DEBUG_SHW_TIMERULES, false,
				"Toggles the Time Ruler Debuger"));

			#endregion

			#region Enviroment File Paths

			string enginecontentpath = "Virtex.Lib.Vertices.Core";

			#if VRTC_PLTFRM_XNA
			enginecontentpath = "Virtex.Lib.Vertices.Core.XNA.Content";

			#elif VRTC_PLTFRM_GL
			enginecontentpath = "Vertices.Engine.Content/Compiled.DesktopGL";            
			#elif VRTC_PLTFRM_DROID
			enginecontentpath = "Vertices.Engine.Content/Compiled.Android";
			#endif
			AddVar(new vxVar (vxEnumEnvVarType.PATH_ENGINE_CONTENT, enginecontentpath,
				"Path to the Engine Content Folder"));




			string path = "Profiles/";
			string virtexrootfolder = "virtexedgegames";
			#if VRTC_PLTFRM_XNA
			path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\"+virtexrootfolder+"\\" + vxEngine.GameName + "\\" +  path;
			#elif VRTC_PLTFRM_GL
			//In Unix Systems, Hide the Folder
			path = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments) + "/." + virtexrootfolder + "/" + vxEngine.GameName + "/" + path;
			#endif
			AddVar(new vxVar (vxEnumEnvVarType.PATH_SETTINGS, path,
				"Path to the Settings Folder"));


			string sndpath = "Sandbox/";
			#if VRTC_PLTFRM_XNA
			sndpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\"+virtexrootfolder+"\\" + vxEngine.GameName + "\\" +  sndpath;
			#elif VRTC_PLTFRM_GL
			sndpath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments) + "/." + virtexrootfolder + "/" + vxEngine.GameName + "/" + sndpath;
			#endif
			AddVar(new vxVar (vxEnumEnvVarType.PATH_SANDBOX,sndpath,
				"Path to the Sandbox Folder"));

			#endregion
		}

		/// <summary>
		/// Add a new variable to the Enviroment Variable Collection.
		/// </summary>
		/// <param name="variable">Variable.</param>
		public static void AddVar(vxVar variable)
		{
			Variables.Add (variable.Name.ToString(), variable);
		}

		/// <summary>
		/// Gets the variable with the specified key.
		/// </summary>
		/// <returns>The variable.</returns>
		/// <param name="key">Key.</param>
		public static vxVar GetVar(object key)
		{
			if (Variables.ContainsKey (key.ToString()))
				return Variables [key.ToString()];
			else
				return null;
		}

    }
}
