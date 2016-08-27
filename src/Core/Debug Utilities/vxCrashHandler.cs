using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Mathematics;

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
		/// The crash font.
		/// </summary>
		static SpriteFont CrashFont;

		/// <summary>
		/// The crash sprite batch.
		/// </summary>
		static SpriteBatch CrashSpriteBatch;

		/// <summary>
		/// The engine.
		/// </summary>
		static vxEngine Engine;

		/// <summary>
		/// The is initialised.
		/// </summary>
		public static bool IsInitialised = false;

		/// <summary>
		/// The text.
		/// </summary>
		static string text = "";

		/// <summary>
		/// The scrollpos.
		/// </summary>
		static float scrollpos = 0;

		/// <summary>
		/// The rect.
		/// </summary>
		static Rectangle rect;


		/// <summary>
		/// Initialise the Crash Handler with the specified engine and exception.
		/// </summary>
		/// <param name="engine">Engine.</param>
		/// <param name="exception">Exception.</param>
		public static void Init(vxEngine engine, Exception exception)
		{
			Engine = engine;

			//First Load the Debug Font
			ContentManager CrashContentManager = new ContentManager(Engine.Game.Services);

			CrashContentManager.RootDirectory = vxEnviroment.GetVar (vxEnumEnvVarType.PATH_ENGINE_CONTENT).Value.ToString ();

			CrashFont = CrashContentManager.Load<SpriteFont> ("Fonts/font_debug");

			CrashSpriteBatch = new SpriteBatch (Engine.Game.GraphicsDevice);

			Engine.Game.IsMouseVisible = true;

			IsInitialised = true;



			//Build the Error Message
			int scrnwidth = Engine.Game.GraphicsDevice.Viewport.Width;
			int CharWidth = (int)CrashFont.MeasureString ("A").X;

			int MAX_NUM_OF_CHARS_PER_LINE = scrnwidth / CharWidth - 5;

			string header = "";

			header = "\n";


			header += new string ('/', MAX_NUM_OF_CHARS_PER_LINE)+"\n";
			header += " __   _____ ___ _____ ___ ___ ___ ___    ___ _  _  ___ ___ _  _ ___     ___ ___    _   ___ _  _    _  _   _   _  _ ___  _    ___ ___ \n";
			header += " \\ \\ / / __| _ \\_   _|_ _/ __| __/ __|  | __| \\| |/ __|_ _| \\| | __|   / __| _ \\  /_\\ / __| || |  | || | /_\\ | \\| |   \\| |  | __| _ \\\n";
			header += "  \\ V /| _||   / | |  | | (__| _|\\__ \\  | _|| .` | (_ || || .` | _|   | (__|   / / _ \\\\__ \\ __ |  | __ |/ _ \\| .` | |) | |__| _||   /\n";
			header += "   \\_/ |___|_|_\\ |_| |___\\___|___|___/  |___|_|\\_|\\___|___|_|\\_|___|   \\___|_|_\\/_/ \\_\\___/_||_|  |_||_/_/ \\_\\_|\\_|___/|____|___|_|_\\\n";

			header += "\n" + new string ('/', MAX_NUM_OF_CHARS_PER_LINE) + "\n";


			text += "\n\n\nVERTICES ENGINE CRASH HANDLER - [ v 0.1 ] \n";
			text += new string ('=', MAX_NUM_OF_CHARS_PER_LINE) + " \n";
			text += "WHY AM I SEEING THIS???\n";
			text += "----------------------------\n";
			text += "Despite our best efforts, it seems a bug did make it's way through our QA. Don't worry, this is ";
			text += "an in game catch which displays any and all crash information.\n\n";

			text += "\nWHAT CAN I DO???\n";
			text += "----------------------------\n";
			text += "To get outside of this screen, you can [alt] + [tab] out or press [Enter] to close the game. ";

			text +=	"The log's files for this crash can be found at the root directory of the game under the 'log' folder.\n\n";

			text += "If this is a reoccuring issue, please contact either Virtex Edge Design (contact@virtexedgedesign.com) or the Game Vendor.\n";

			text += "\n\n\n\nTECHNICAL DATA\n";
			text += new string ('=', MAX_NUM_OF_CHARS_PER_LINE) + " \n";

			text += "\nERROR INFORMATION [ " + DateTime.Now.ToString() + " ]\n";
			text += "----------------------------\n";
			text += string.Format("Game Name:             {0}\n",Engine.GameName);
			text += string.Format("Game Version:          {0}\n",Engine.GameVersion);
			text += "-\n";
			text += string.Format("Engine Version:        {0}\n",Engine.EngineVersion);
			text += string.Format("Engine Platform:       {0}\n",Engine.PlatformType);
			text += string.Format("Engine Build Config:   {0}\n",Engine.BuildConfigType);
			text += "-\n";
			text += string.Format("Error Source Method:   {0}\n",exception.TargetSite);
			text += string.Format("Error Messge:          {0}\n",exception.Message);
			text += string.Format("Error Data:            {0}\n",exception.Data);



			//Writeout the Stack Trace
			text += "\n\nSTACK TRACE\n";
			text += new string ('-', MAX_NUM_OF_CHARS_PER_LINE);
			text += exception.StackTrace;

			string endmsg = " END OF ERROR MESSAGE ";
			string sides = new string ('=', MAX_NUM_OF_CHARS_PER_LINE / 2 - endmsg.Length);
			text += "\n\n" + sides + endmsg + sides + "\n";

			//Finally wrap the text
			text = vxUtil.WrapMultilineString(text, MAX_NUM_OF_CHARS_PER_LINE);

			//Now add on the initial header
			//text = header + text;
			string writeresult = "";
			try
			{
				if (Directory.Exists ("log") == false)
					Directory.CreateDirectory ("log");
				string filename = string.Format("log/CRASH LOG {0}.log", DateTime.Now.ToString());
				StreamWriter writer = new StreamWriter (filename);
				writer.Write (header + text);
				writer.Close ();

				writeresult = string.Format("[ CRASH LOG SAVED to <game root dir>/'{0}' ]", filename);;
			}
			catch (Exception writeException) {
				writeresult = string.Format(">>>> NOTE: CRASH LOG COULD NOT BE WRITTEN! <<<<\nERROR: {0} ",writeException.Message);
			}
			writeresult = vxUtil.WrapMultilineString (writeresult, MAX_NUM_OF_CHARS_PER_LINE);

			text = header + "\n" + writeresult + "\n" + text;
		}


		/// <summary>
		/// Draw the Crash Handler Screen.
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		public static void Draw(GameTime gameTime)
		{
			if (IsInitialised == true) {

					//Check if the game should exit
					if (Keyboard.GetState ().IsKeyDown (Keys.Enter))
						Engine.Game.Exit ();

					if (Keyboard.GetState ().IsKeyDown (Keys.PageUp))
						scrollpos -= (int)CrashFont.MeasureString ("A").Y;
					else if (Keyboard.GetState ().IsKeyDown (Keys.PageDown))
						scrollpos += CrashFont.MeasureString ("A").Y;
				
				scrollpos = Math.Max (scrollpos, 0);

				string txt = ">> DON'T PANIC " + vxUtil.GetTextSpinner () + text + "\n\n";

				//Get Max Scroll Travel
				int maxheight = (int)CrashFont.MeasureString (txt).Y;

				//Get Height of Screen
				int scrnHeight = Engine.GraphicsDevice.Viewport.Height;

				//Get the Max allowed travel
				int maxtravel = Math.Max(0, maxheight - scrnHeight);

				//Now set the scrollpos 
				scrollpos = Math.Min(scrollpos, maxtravel);
				int px = 5;
				int py = 5;
				rect = new Rectangle (
					Engine.Game.GraphicsDevice.Viewport.Width - 2 * px,
					py + (int)scrollpos,
					3,
					scrnHeight - maxtravel - 2 * py);
				

				//Draw Info
				Engine.Game.GraphicsDevice.Clear (Color.Purple * 0.75f);
				CrashSpriteBatch.Begin ();
				CrashSpriteBatch.DrawString (CrashFont, txt, 
					new Vector2 (2 * px, 2 * py - scrollpos), Color.White);

				CrashSpriteBatch.Draw (Engine.Assets.Textures.Blank,
					rect, Color.White);


				CrashSpriteBatch.Draw (Engine.Assets.Textures.Blank,
					new Rectangle (
						Engine.Game.GraphicsDevice.Viewport.Width - 3 * px,
						py,
						1,
						scrnHeight - 2 * py), Color.White);

				CrashSpriteBatch.End ();

			}
		}
	}
}

