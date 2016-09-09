using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.Utilities
{
	/// <summary>
	/// Collection of static utility methods.
	/// </summary>
    public static class vxUtil
    {
		/// <summary>
		/// Wraps the multiline string. Useful for displaying Stack Traces which sometimes go off the screen.
		/// </summary>
		/// <returns>The wrapped multiline string.</returns>
		/// <param name="inputstring">Inputstring.</param>
		/// <param name="Width">Width.</param>
		public static string WrapMultilineString(string inputstring, int MaxCharsPerLine)
		{
			string returnstring = "";
			using (StringReader reader = new StringReader(inputstring))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					// Do something with the line
					toolongloop:
					//If the line is longer than the width of the string, then split it
					if (line.Length > MaxCharsPerLine) {
						returnstring += line.Substring (0, MaxCharsPerLine) + "\n";

						//Now set the line to the remainder and loop back up
						line = line.Substring (MaxCharsPerLine);
						goto toolongloop;
					} else {
						returnstring += line + "\n";
					}
				}
			}
			return returnstring;
		}


		static int spinner_index_i = 0;
		static int spinner_index_j = 0;
		static string spinner_text = "|";
		public static string GetTextSpinner()
		{
			spinner_index_i++;

			if (spinner_index_i % 10 == 0) {
				spinner_index_j++;

				switch(spinner_index_j%4)
				{
				case 0:
					spinner_text = "|";
					break;
				case 1:
					spinner_text = "/";
					break;
				case 2:
					spinner_text = "-";
					break;
				case 3:
					spinner_text = "\\";
					break;
				}
			}
			return spinner_text;
		}

		/// <summary>
		/// Takes in a string and parses based off of the 'XML Tag'. If Tag is not found, No text is returned.
		/// </summary>
		/// <param name="Text">Text to be Parsed</param>
		/// <param name="XMLTag">XML Tag to Parse By</param>
		/// <returns>Return Text parsed by XML Tags</returns>
		public static string ReadXML(string Text, string XMLTag)
		{
			string value = "";

			try
			{
				//Start and End tags of XML Tag
				string StartTag = "<" + XMLTag + ">";
				string EndTag = "</" + XMLTag + ">";

				value = Text.Substring(Text.IndexOf(StartTag) + StartTag.Length, Text.IndexOf(EndTag) - Text.IndexOf(StartTag) - EndTag.Length + 1);
			}
			catch
			{
				value = "ERROR PARSING XML TAG!";
			}
			return value;
		}

		/// <summary>
		/// Combines Text and XML Tag into String XML Tag line
		/// </summary>
		/// <param name="Text">Text to Combine into XML Tag</param>
		/// <param name="XMLTag">XML Tag to use</param>
		/// <returns>String that is XML Tagged</returns>
		public static string WriteXML(string Text, string XMLTag)
		{
			string value = "";

			//Start and End tags of XML Tag
			string StartTag = "<" + XMLTag + ">";
			string EndTag = "</" + XMLTag + ">";

			value = StartTag + Text + EndTag;

			return value;
		}

        public static string GetShortenedTimespanString(TimeSpan timespan)
        {
            string time = timespan.Duration().ToString();
                        
            int length = time.Length;

            if(time.LastIndexOf('.') != -1)
                length = time.LastIndexOf('.')+4;

            return timespan.Duration().ToString().Substring(0, length);
        }

        /// <summary>
        /// Parses a File URL and returns the File Name without the Extention
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GetFileNameFromPath(string FilePath)
        {
			char c = '/';

            return FilePath.Substring(FilePath.LastIndexOf(c) + 1,
                    FilePath.LastIndexOf('.') - FilePath.LastIndexOf(c) - 1);
        }

        public static string GetParentPathFromFilePath(string FilePath)
        {
            char c = '/';

            return FilePath.Substring(0,FilePath.LastIndexOf(c));
        }

        /// <summary>
        /// Loads a Texture at Runtime from a File URL
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public static Texture2D LoadTextureFromFile(string FilePath, GraphicsDevice graphicsDevice)
        {
			using (FileStream fileStream = new FileStream(FilePath, FileMode.Open))
			{
				return Texture2D.FromStream(graphicsDevice, fileStream);
			}
        }



		/// <summary>
		/// Checks Whether or not Point is inside rectangle
		/// </summary>
		/// <param name="Point"></param>
		/// <param name="Rectangle"></param>
		public static bool IsPointInsideRectangle(Vector2 Point, Rectangle Rectangle)
		{
			//Checks if Point is within Rectangle X Coordinates
			if (Point.X > Rectangle.Left && Point.X < Rectangle.Right)
			{
				//Checks if point is within Rectangle Y Coordinates
				if (Point.Y > Rectangle.Top && Point.Y < Rectangle.Bottom) {
					return true;
				} 
			}

			return false;
		}




        /// <summary>
        /// Takes a Screenshot and returns it as a Texture 2D
        /// </summary>
        /// <returns></returns>
		public static Texture2D TakeScreenshot(vxEngine vxEngine)
        {
			int w = vxEngine.GraphicsDevice.PresentationParameters.BackBufferWidth;
			int h = vxEngine.GraphicsDevice.PresentationParameters.BackBufferHeight;
#if VRTC_PLTFRM_XNA

			//force a frame to be drawn (otherwise back buffer is empty) 
            vxEngine.Draw(new GameTime());

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            vxEngine.GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(vxEngine.GraphicsDevice, w, h, false, vxEngine.GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);
            return texture;

            /*
            //save to disk 
            Stream streampng = File.OpenWrite(Path + ".png");
            texture.SaveAsPng(streampng, w, h);
            streampng.Dispose();
            texture.Dispose();
            */
#else
			RenderTarget2D screenshot = new RenderTarget2D (vxEngine.GraphicsDevice, w, h);
			vxEngine.GraphicsDevice.SetRenderTarget (screenshot);
			//force a frame to be drawn (otherwise back buffer is empty) 
			vxEngine.Draw(new GameTime());
			vxEngine.GraphicsDevice.SetRenderTarget (null);
			return screenshot;// new Texture2D(Game.GraphicsDevice, 1,1);
#endif
        }

		/// <summary>
		/// Resizes the Texture2D to the Specified Width and Height
		/// </summary>
		/// <returns>The texture2 d.</returns>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="NewWidth">New width.</param>
		/// <param name="NewHeight">New height.</param>
		public static Texture2D ResizeTexture2D(vxEngine vxEngine, Texture2D texture, int NewWidth, int NewHeight)
		{
			//Create a New Render Target to take the resized texture.
			RenderTarget2D NewTexture = new RenderTarget2D (vxEngine.GraphicsDevice, NewWidth, NewHeight);

			//Set the New Render Target.
			vxEngine.GraphicsDevice.SetRenderTarget (NewTexture);

			//Draw the original texture resized.
			vxEngine.SpriteBatch.Begin ();
			vxEngine.SpriteBatch.Draw (texture, 
				new Rectangle(0,0,NewWidth, NewHeight),
				new Rectangle(0,0,texture.Width, texture.Height),
				Color.White);
			vxEngine.SpriteBatch.End ();

			//Reset the Rendertarget to null
			vxEngine.GraphicsDevice.SetRenderTarget (null);

			return NewTexture;
		}

        #region File Compression

        public delegate void ProgressDelegate(string sMessage);

        public static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
        {
            //Compress file name
            char[] chars = sRelativePath.ToCharArray();
            zipStream.Write(BitConverter.GetBytes(chars.Length), 0, sizeof(int));
            foreach (char c in chars)
                zipStream.Write(BitConverter.GetBytes(c), 0, sizeof(char));

            //Compress file content
            byte[] bytes = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));
            zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
            zipStream.Write(bytes, 0, bytes.Length);
        }

        public static bool DecompressFile(string sDir, GZipStream zipStream, ProgressDelegate progress)
        {
            //Decompress file name
            byte[] bytes = new byte[sizeof(int)];
            int Readed = zipStream.Read(bytes, 0, sizeof(int));
            if (Readed < sizeof(int))
                return false;

            int iNameLen = BitConverter.ToInt32(bytes, 0);
            bytes = new byte[sizeof(char)];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < iNameLen; i++)
            {
                zipStream.Read(bytes, 0, sizeof(char));
                char c = BitConverter.ToChar(bytes, 0);
                sb.Append(c);
            }
            string sFileName = sb.ToString();
            if (progress != null)
                progress(sFileName);

            //Decompress file content
            bytes = new byte[sizeof(int)];
            zipStream.Read(bytes, 0, sizeof(int));
            int iFileLen = BitConverter.ToInt32(bytes, 0);

            bytes = new byte[iFileLen];
            zipStream.Read(bytes, 0, bytes.Length);

            string sFilePath = Path.Combine(sDir, sFileName);
            string sFinalDir = Path.GetDirectoryName(sFilePath);
            if (!Directory.Exists(sFinalDir))
                Directory.CreateDirectory(sFinalDir);
            TryAgain:

            try
            {
                using (FileStream outFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    outFile.Write(bytes, 0, iFileLen);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                goto TryAgain;
            }
            return true;
        }

        public static void CompressDirectory(string sInDir, string sOutFile, ProgressDelegate progress)
        {
            string[] sFiles = Directory.GetFiles(sInDir, "*.*", SearchOption.AllDirectories);
            int iDirLen = sInDir[sInDir.Length - 1] == Path.DirectorySeparatorChar ? sInDir.Length : sInDir.Length + 1;

            using (FileStream outFile = new FileStream(sOutFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (GZipStream str = new GZipStream(outFile, CompressionMode.Compress))
                foreach (string sFilePath in sFiles)
                {
                    string sRelativePath = sFilePath.Substring(iDirLen);
                    if (progress != null)
                        progress(sRelativePath);
                    CompressFile(sInDir, sRelativePath, str);
                }
        }

        public static void DecompressToDirectory(string sCompressedFile, string sDir, ProgressDelegate progress)
        {
            using (FileStream inFile = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                while (DecompressFile(sDir, zipStream, progress)) ;
        }

        #endregion
    }
}
