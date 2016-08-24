using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Virtex.Lib.Vrtc.Core.Debug
{
	/// <summary>
	/// The Crash Handler Game is a self contained game class which can be put in a 
	/// try-catch block at the initial entry point of the game. This allows for debug messages
	/// and crash output to be caught and shown on Releases and on non-easily-debugable systems (i.e. Mobile, Consoles etc...)
	/// </summary>
	public static class vxCrashHandler
	{
		/// <summary>
		/// Catchs the crash.
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="exception">Exception.</param>
		public static void CatchCrash (Game game, Exception exception)
		{
			//First Load the Debug Font
			ContentManager CrashContentManager = new ContentManager(game.Services);

            //Set Location of Content Specific too Platform
#if VRTC_PLTFRM_XNA
			Content.RootDirectory = "Virtex.Lib.Vertices.Core.XNA.Content";

#elif VRTC_PLTFRM_GL
			CrashContentManager.RootDirectory = "Vertices.Engine.Content/Compiled.WindowsGL";            
#elif VRTC_PLTFRM_DROID
            CrashContentManager.RootDirectory = "Vertices.Engine.Content/Compiled.Android";

			//this.Activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Activity.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			#endif

			SpriteFont CrashFont = CrashContentManager.Load<SpriteFont> ("Fonts/font_debug");
			SpriteBatch CrashSpriteBatch = new SpriteBatch (game.GraphicsDevice);

			//Get the Time
			string time = DateTime.UtcNow.ToString();


			int scrnwidth = game.GraphicsDevice.Viewport.Width;
			int CharWidth = (int)CrashFont.MeasureString ("A").X;

			int MAX_NUM_OF_CHARS_PER_LINE = scrnwidth / CharWidth - 5;

			string text = "\n";

			text += " __   _____ ___ _____ ___ ___ ___ ___    ___ _  _  ___ ___ _  _ ___     ___ ___    _   ___ _  _    _  _   _   _  _ ___  _    ___ ___ \n";
			text += " \\ \\ / / __| _ \\_   _|_ _/ __| __/ __|  | __| \\| |/ __|_ _| \\| | __|   / __| _ \\  /_\\ / __| || |  | || | /_\\ | \\| |   \\| |  | __| _ \\\n";
			text += "  \\ V /| _||   / | |  | | (__| _|\\__ \\  | _|| .` | (_ || || .` | _|   | (__|   / / _ \\\\__ \\ __ |  | __ |/ _ \\| .` | |) | |__| _||   /\n";
			text += "   \\_/ |___|_|_\\ |_| |___\\___|___|___/  |___|_|\\_|\\___|___|_|\\_|___|   \\___|_|_\\/_/ \\_\\___/_||_|  |_||_/_/ \\_\\_|\\_|___/|____|___|_|_\\\n";

			text += new string ('=', MAX_NUM_OF_CHARS_PER_LINE) + "\n";


			text += "\nWHY AM I SEEING THIS???\n===================================================================\n";
			text += "Despite our best efforts, it seems a bug did make it's way through our QA. Don't worry, this is ";
			text += "an in game catch which displays any and all crash information.\n\n";

			text += "To get outside of this screen, alt-tab or close the progam/app in your system. ";

			text +=	"The log's files for this crash can be found at the root directory of the\ngame under the 'log' folder.\n\n";

			text += "If this is a reoccuring issue, please contact either Virtex Edge Design (contact@virtexedgedesign.com) or the Game Vendor.\n\n";

			text += new string ('-', MAX_NUM_OF_CHARS_PER_LINE);

			text += "\n\nERROR INFORMATION [ " + time + " ]\n=====================================\n";
			text += string.Format("File Source:   {0}\n",exception.Source);
			//Set Location of Content Specific too Platform
			#if VRTC_PLTFRM_XNA
			text += string.Format("Platform:      XNA\n");
			#elif VRTC_PLTFRM_GL
			text += string.Format("Platform:      MonoGame - DesktopGL\n");
			#elif VRTC_PLTFRM_DROID
			text += string.Format("Platform:      MonoGame - Android\n");
			#endif
			text += string.Format("Method Source: {0}\n",exception.TargetSite);
			text += string.Format("Error Messge:  {0}\n",exception.Message);
			text += string.Format("Error Data:    {0}\n",exception.Data);
			text += string.Format("HResult:       {0}\n",exception.HResult);


			text += "\n\nSTACK TRACE\n========================================================================================\n";

			string stacktrace = "";

			using (StringReader reader = new StringReader(exception.StackTrace))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					// Do something with the line

					//If the line is longer than the width of the string, then split it
					if (CrashFont.MeasureString (line).X > scrnwidth) {
						stacktrace += line.Substring (0, MAX_NUM_OF_CHARS_PER_LINE) + "\n";
						stacktrace += line.Substring (MAX_NUM_OF_CHARS_PER_LINE) + "\n";
					} else {
						stacktrace += line + "\n";
					}
				}
			}

			text += stacktrace;


			try
			{
			if (Directory.Exists ("log") == false)
				Directory.CreateDirectory ("log");

			StreamWriter writer = new StreamWriter (string.Format("log/CRASH LOG {0}.log", time));
			writer.Write (text);
			writer.Close ();
			}
			catch (Exception writeException) {
				text = "  NOTE: CRASH LOG COULD NOT BE WRITTEN.\n"+writeException.Message + text;
			}
			int i = 0;
			int j = 0;
			bool loop = true;
			string spinner = "|";
			while(loop)
			{
				game.GraphicsDevice.Clear (Color.DarkMagenta);
				i++;

				KeyboardState ks = Keyboard.GetState ();

				if (ks.IsKeyDown (Keys.A) == true)
					loop = false;

				//Adds a spinner to the top of the BSOD so that the player can still see some movement and they
				// don't worry that their system is locked up
				if (i % 10 == 0) {
					j++;

					switch(j%4)
					{
					case 0:
						spinner = "|";
						break;
						case 1:
						spinner = "/";
						break;
						case 2:
						spinner = "-";
						break;
						case 3:
						spinner = "\\";
						break;
					}
				}


				//TODO: Add your drawing code here
				CrashSpriteBatch.Begin();
				CrashSpriteBatch.DrawString(CrashFont, spinner + " DON'T PANIC"  + text,new Vector2(10,10), Color.White);
				CrashSpriteBatch.End ();
				//GameTime gameTime = new GameTime (new TimeSpan (1), new TimeSpan (1));


				game.GraphicsDevice.Present ();
			}

			//Once the player clicks Space, then exit
			game.Exit ();
		}
	}
}

