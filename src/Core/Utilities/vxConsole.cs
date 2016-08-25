using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.Utilities
{
	/// <summary>
	/// Console Utility which provides output too both the in-game console window as well as 
    /// the system console if available.
	/// </summary>
    public class vxConsole
    {
		/// <summary>
		/// The current instance of the running Engine.
		/// </summary>
        public static vxEngine vxEngine;

        /// <summary>
        /// The collection of Debug Strings.
        /// </summary>
		static List<string> DebugStringsToDraw = new List<string>();

		/// <summary>
		/// The debug string location.
		/// </summary>
		public static Vector2 DebugStringLocation = new Vector2(5,5);

		/// <summary>
		/// Initialises the vxConsole Static Object.
		/// </summary>
		/// <param name="engine">Engine.</param>
		public static void Initialize(vxEngine engine)
		{
			vxEngine = engine;

			//This is just temporary, this is re-loaded for global uses when the vxEngine is Initialised.
			string gameVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();

			#if !VRTC_PLTFRM_DROID
			try {
				Console.Title = "VIRTICES ENGINE DEBUG CONSOLE v." + gameVersion;
			} catch {
			}
			#endif

			string backend = "COMPILER FLAG NOT FOUND";
			//There's only two choices for a backend, XNA or MonoGame. The entire code base will be eventually
			//be moved over ONLY too MonoGame as XNA is no longer supported.
			#if VRTC_PLTFRM_XNA
			backend = "XNA";
			#elif VRTC_PLTFRM_DRTCX
			backend = "MonoGame [DirectX]";
			#elif VRTC_PLTFRM_GL
			backend = "MonoGame [OpenGL]";
			#elif VRTC_PLTFRM_DROID
			backend = "MonoGame [Android]";
			#elif VRTC_PLTFRM_IOS
			backend = "MonoGame [iOS]";
			#endif

			vxConsole.WriteLine ("____   ____             __  .__                     ");
			vxConsole.WriteLine ("\\   \\ /   /____________/  |_|__| ____  ____   ______");
			vxConsole.WriteLine (" \\   Y   // __ \\_  __ \\   __\\  |/ ___\\/ __ \\ /  ___/");
			vxConsole.WriteLine ("  \\     /\\  ___/|  | \\/|  | |  \\  \\__\\  ___/ \\___ \\ ");
			vxConsole.WriteLine ("   \\___/  \\___  >__|   |__| |__|\\___  >___  >____  >");
			vxConsole.WriteLine ("              \\/                    \\/    \\/     \\/ ");
			vxConsole.WriteLine ("VERTICES ENGINE - (C) VIRTEX EDGE DESIGN");
			vxConsole.WriteLine ("///////////////////////////////////////////////////////////////////////");
			vxConsole.WriteLine (string.Format ("Engine Version      v.{0}", gameVersion));
			vxConsole.WriteLine (string.Format ("Game Name:          {0}", engine.GameName));
			vxConsole.WriteLine (string.Format ("Graphical Backend:  {0}", backend));
			vxConsole.WriteLine ("///////////////////////////////////////////////////////////////////////");
		}



        /// <summary>
        /// Writes a debug line which is outputed to both the engine debug window and the system console.
        /// </summary>
        /// <remarks>If debug information is needed, this method is useful for outputing any object.ToString() value 
        /// too both the in-engine debug window as well as the system console if it is available.</remarks>
        /// <param name="output">The object too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteLine"/> method.
        /// <code>
        /// vxConsole.WriteLine("Output of Foo is: " + foo.Output.ToString());
        /// </code>
        /// </example>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteLine"/> method with different variable types as inputs.
        /// <code>
        /// vxConsole.WriteLine(string.Format("X,Y,Z Max: {0}, {1}, {2}", Level_X_Max, Level_Y_Max, Level_Z_Max));
        /// </code>
        /// </example>
        public static void WriteLine(object output)
        {
#if !VRTC_PLTFRM_DROID
            Console.ForegroundColor = ConsoleColor.Green;
#endif
			Console.WriteLine(">>: " + output);
#if !VRTC_PLTFRM_DROID
            Console.ResetColor();
#endif
			if (vxEngine != null && vxEngine.DebugSystem != null)
				vxEngine.DebugSystem.DebugCommandUI.Echo(">>: " + output.ToString());
		}

		public static void WriteLine(object output, ConsoleColor consoleColor)
		{
			#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = consoleColor;
			#endif
			Console.WriteLine(">>: " + output);
			#if !VRTC_PLTFRM_DROID
			Console.ResetColor();
			#endif
			if (vxEngine != null && vxEngine.DebugSystem != null)
				vxEngine.DebugSystem.DebugCommandUI.Echo(">>: " + output.ToString());
		}

        /// <summary>
        /// Similar to the <see cref="WriteLine"/> method. This method writes out a line of text which is 
        /// is prefixed with a "Networking" tag to the console output line to help distuignish it against regular 
        /// console output.
        /// </summary>
        /// <param name="output">The object too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteNetworkLine"/> method.
        /// <code>
        /// vxConsole.WriteNetworkLine("Ping: " + foo.Ping.ToString());
        /// </code>
        /// </example>
        public static void WriteNetworkLine(object output)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Yellow;
#endif
			Console.WriteLine("\tNET >>:" + output);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor();
#endif
            if (vxEngine != null)
                vxEngine.DebugSystem.DebugCommandUI.Echo("     NET >>:" + output.ToString());
        }

        /// <summary>
        /// Writes out an error the error.
        /// </summary>
        /// <param name="SourceFile">Source file where the error is being sent from. Helpful for tracking where error's 
        /// are being generated. </param>
        /// <param name="output">The object holding the error data too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteError"/> method.
        /// <code>
        ///     try
        ///     {
        ///         foo.bar();
        ///     }
        ///     catch(Exception ex)
        ///     {
        ///         vxConsole.WriteError(this.ToString(), ex.Message);
        ///     }
        /// </code>
        /// </example>
        public static void WriteError(Exception ex)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Red;
#endif
            Console.WriteLine("**************************************************");
			Console.WriteLine("ERROR: >>: " + ex.Message);
			Console.WriteLine("Stack Trace File " + ex.StackTrace);
            Console.WriteLine("**************************************************");
#if !VRTC_PLTFRM_DROID
            Console.ResetColor();
#endif
            if (vxEngine != null)
            {
                vxEngine.DebugSystem.DebugCommandUI.Echo("**************************************************");
				vxEngine.DebugSystem.DebugCommandUI.Echo("ERROR: >>: " + ex.Message);
				vxEngine.DebugSystem.DebugCommandUI.Echo("ERROR: >>: " + ex.StackTrace);
                vxEngine.DebugSystem.DebugCommandUI.Echo("**************************************************");
            }
        }



        /// <summary>
        /// Writes out a warning to the debug and system console.
        /// </summary>
        /// <param name="SourceFile">Source file where the warning is being sent from. Helpful for tracking where warning's 
        /// are being generated. </param>
        /// <param name="output">The object holding the warning data too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteWarning"/> method.
        /// <code>
        ///     try
        ///     {
        ///         foo.bar();
        ///     }
        ///     catch(Exception ex)
        ///     {
        ///         vxConsole.WriteWarning(this.ToString(), ex.Message);
        ///     }
        /// </code>
        /// </example>
        public static void WriteWarning(string SourceFile, string output)
        {
#if !VRTC_PLTFRM_DROID
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
#endif
            Console.WriteLine("**************************************************");
            Console.WriteLine("Source File   ---   " + SourceFile + "    ---    ");
            Console.WriteLine("WARNING: >>: " + output);
            Console.WriteLine("**************************************************");
#if !VRTC_PLTFRM_DROID
            Console.ResetColor();
#endif
            if (vxEngine != null)
            {
                vxEngine.DebugSystem.DebugCommandUI.Echo("**************************************************");
                vxEngine.DebugSystem.DebugCommandUI.Echo("Source File   ---   " + SourceFile + "    ---    ");
                vxEngine.DebugSystem.DebugCommandUI.Echo("WARNING: >>: " + output);
                vxEngine.DebugSystem.DebugCommandUI.Echo("**************************************************");
            }
        }

        /// <summary>
        /// Writes to in game debug. Activate the in-game debug window by running the 'cn' command.
        /// </summary>
        /// <remarks>NOTE: This is different than the Engine Debug console.</remarks>
        /// <param name="output">The object holding the error data too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteToInGameDebug"/> method.
        /// <code>
        /// vxConsole.WriteToInGameDebug("Player Position: " + foo.Position.ToString());
        /// </code>
        /// </example>
        public static void WriteToInGameDebug(object output)
		{
			if(output != null)
				DebugStringsToDraw.Add (output.ToString());
			else
				DebugStringsToDraw.Add ("NULL");
		}

        /// <summary>
        /// Draw the InGame debug window. To access this debug window, run the 'cn' command in the engine console.
        /// </summary>
        /// <remarks>NOTE: This is different than the Engine Debug console.</remarks>
        public static void Draw()
		{
            SpriteFont font = vxEngine.Assets.Fonts.DebugFont;
			if (vxEngine != null) {
				if (vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_INGMECNSL).GetAsBool()== true) {
					vxEngine.SpriteBatch.Begin ();

					string outputText = "InGame Debug Console:";
					outputText += "\n=========================";
					foreach (string str in DebugStringsToDraw) {
						outputText += "\n" + str;
					}
					int padding = 5;
					vxEngine.SpriteBatch.Draw (vxEngine.Assets.Textures.Blank,
						new Rectangle (
							(int)DebugStringLocation.X - padding, 
							(int)DebugStringLocation.Y - padding,
							(int)font.MeasureString (outputText).X + 2 * padding,
							(int)font.MeasureString (outputText).Y + 2 * padding),
						Color.Black * 0.5f);
					
							vxEngine.SpriteBatch.DrawString (font,
						outputText,
						DebugStringLocation,
						Color.White);
				
					vxEngine.SpriteBatch.End ();
				}
				//Clear it ever loop to prevent memory leaks
				DebugStringsToDraw.Clear ();
			}
		}
    }
}
