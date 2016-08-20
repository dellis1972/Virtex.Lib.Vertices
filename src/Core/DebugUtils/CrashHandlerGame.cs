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
			Content.RootDirectory = "Vertices.Engine.Content/Compiled.Android";

			this.Activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
			this.Activity.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			#endif

			SpriteFont CrashFont = CrashContentManager.Load<SpriteFont> ("Fonts/font_debug");
			SpriteBatch CrashSpriteBatch = new SpriteBatch (game.GraphicsDevice);

			//Get the Time
			string time = DateTime.UtcNow.ToString();


			int scrnwidth = game.GraphicsDevice.Viewport.Width;
			int CharWidth = (int)CrashFont.MeasureString ("A").X;

			int MAX_NUM_OF_CHARS_PER_LINE = scrnwidth / CharWidth - 5;

			string text = "";

			text += " __   _____ ___ _____ ___ ___ ___ ___    ___ _  _  ___ ___ _  _ ___     ___ ___    _   ___ _  _    _  _   _   _  _ ___  _    ___ ___ \n";
			text += " \\ \\ / / __| _ \\_   _|_ _/ __| __/ __|  | __| \\| |/ __|_ _| \\| | __|   / __| _ \\  /_\\ / __| || |  | || | /_\\ | \\| |   \\| |  | __| _ \\\n";
			text += "  \\ V /| _||   / | |  | | (__| _|\\__ \\  | _|| .` | (_ || || .` | _|   | (__|   / / _ \\\\__ \\ __ |  | __ |/ _ \\| .` | |) | |__| _||   /\n";
			text += "   \\_/ |___|_|_\\ |_| |___\\___|___|___/  |___|_|\\_|\\___|___|_|\\_|___|   \\___|_|_\\/_/ \\_\\___/_||_|  |_||_/_/ \\_\\_|\\_|___/|____|___|_|_\\\n";

			text += new string ('=', MAX_NUM_OF_CHARS_PER_LINE) + "\n";


			text += "\n\nWHY AM I SEEING THIS???\n===================================================================\n";
			text += "Despite our best efforts, it seems a bug did make it's way through our QA. Don't worry, this is\n";
			text += "an in game catch which displays any and all crash information.\n";

			text += "To get outside of this screen, alt-tab or close the progam/app in your system.\n\n";

			text +=	"The log's files for this crash can be found at the root directory of the game under the 'log' folder.\n\n";

			text += "If this is a reoccuring issue, please contact either Virtex Edge Design (contact@virtexedgedesign.com) or the Game Vendor.\n\n";

			text += new string ('-', MAX_NUM_OF_CHARS_PER_LINE);

			text += "\n\nERROR INFORMATION [ " + time + " ]\n=====================================\n";
			text += string.Format("File Source:   {0}\n",exception.Source);
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

			if (Directory.Exists ("log") == false)
				Directory.CreateDirectory ("log");

			StreamWriter writer = new StreamWriter (string.Format("log/CRASH LOG {0}.log", time));
			writer.Write (text);
			writer.Close ();

			int i = 0;
			bool loop = true;
			while(loop)
			{
				game.GraphicsDevice.Clear (Color.Purple);
				i++;

				KeyboardState ks = Keyboard.GetState ();

				if (ks.IsKeyDown (Keys.A) == true || i > 1000)
					loop = false;



				//TODO: Add your drawing code here
				CrashSpriteBatch.Begin();
				CrashSpriteBatch.DrawString(CrashFont, text,new Vector2(10,10), Color.White);
				CrashSpriteBatch.End ();
				//GameTime gameTime = new GameTime (new TimeSpan (1), new TimeSpan (1));


				game.GraphicsDevice.Present ();

			}



			//Once the player clicks Space, then exit
			game.Exit ();
		}
	}
}

