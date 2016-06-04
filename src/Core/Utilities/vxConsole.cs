using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core;

namespace vxVertices.Utilities
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
		public static Vector2 DebugStringLocation = new Vector2(25,25);

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
			if (vxEngine != null)
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
			Console.ForegroundColor = ConsoleColor.DarkCyan;
#endif
			Console.WriteLine("<NETWORK>>:" + output);
#if !VRTC_PLTFRM_DROID
			Console.ResetColor();
#endif
            if (vxEngine != null)
                vxEngine.DebugSystem.DebugCommandUI.Echo("<NETWORK>>:" + output.ToString());
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
        public static void WriteError(string SourceFile, string output)
        {
#if !VRTC_PLTFRM_DROID
			Console.ForegroundColor = ConsoleColor.Red;
#endif
            Console.WriteLine("**************************************************");
            Console.WriteLine("Source File   ---   " + SourceFile + "    ---    ");
            Console.WriteLine("ERROR: >>: " + output);
            Console.WriteLine("**************************************************");
#if !VRTC_PLTFRM_DROID
            Console.ResetColor();
#endif
            if (vxEngine != null)
            {
                vxEngine.DebugSystem.DebugCommandUI.Echo("**************************************************");
                vxEngine.DebugSystem.DebugCommandUI.Echo("Source File   ---   " + SourceFile + "    ---    ");
                vxEngine.DebugSystem.DebugCommandUI.Echo("ERROR: >>: " + output);
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
				if (vxEngine.ShowInGameDebugWindow) {
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
