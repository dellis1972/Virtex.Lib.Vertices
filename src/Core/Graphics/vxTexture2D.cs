using Virtex.Lib.Vrtc.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Virtex.Lib.Vrtc.Utilities;
using System.IO;
using System;
using Virtex.Lib.Vrtc.Core.Settings;

namespace Virtex.Lib.Vrtc.Graphics
{

	public enum vxEnumTextureQuality
	{
		Ultra,
		High,
		Medium,
		Low
	}

	/// <summary>
	/// The vxTexture2D Class holds as well as creates downscaled versions of a single Texture.
	/// </summary>
	/// <remarks>
	/// The constructor automatically scales down textures in a texture pack. For example, if
	/// a 512x512 texture is loaded initially, then the constructor will also create a 256x256, 128x128, 64x64 and 32x32 if
	/// they aren't already present. Note: The minimum scale down size would be 8x8.
	/// Although it would be quicker to do this at build time, and not run time, having this function allows for mod support.
	/// </remarks>
	public class vxTexture2D
	{
		#region Properties and Fields

		/// <summary>
		/// The engine.
		/// </summary>
		vxEngine Engine;

		/// <summary>
		/// The path to original.
		/// </summary>
		string PathToOriginal = "";

		/// <summary>
		/// Gets or sets the texture.
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture
		{
			get {
				return TexturePack[(int)MathHelper.Clamp(Convert.ToInt32(Quality), 0, 3)]; }
			set { CreateTexturePack(value); }
		}

		/// <summary>
		/// The quality of the texture.
		/// </summary>
		public vxEnumTextureQuality Quality = vxEnumTextureQuality.Ultra;


		/// <summary>
		/// The texture original.
		/// </summary>
		Texture2D TextureOriginal;

		/// <summary>
		/// The texture pack.
		/// </summary>
		public List<Texture2D> TexturePack = new List<Texture2D>();

		/// <summary>
		/// The texture pack count.
		/// </summary>
		public const int TexturePackCount = 4;

		/// <summary>
		/// The divisors.
		/// </summary>
		int[] divisors = { 1, 2, 4, 8, 16 };

		/// <summary>
		/// The minimum size of any of the textures.
		/// </summary>
		public const int MIN_SIZE = 48;

		#endregion


		public Texture2D NullDiffuseTexture
		{
			get
			{
				if (Engine.Assets != null)
				{
					return Engine.Assets.Textures.Texture_Diffuse_Null;
				}
				else
					return Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_diffuse");
			}
		}



		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.Core.Graphics.vxTexture2D"/> class using the 
		/// Game Content Manager to load all assets.
		/// </summary>
		/// <param name="engine">Engine.</param>
		/// <param name="path">Path.</param>
		public vxTexture2D(vxEngine engine, string path) :
		this(engine, engine.Game.Content.Load<Texture2D>(path), engine.Game.Content.RootDirectory +"/" + path)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.Core.Graphics.vxTexture2D"/> class using the 
		/// Specified Content Manager to load all assets.
		/// </summary>
		/// <param name="engine">Engine.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="path">Path.</param>
		public vxTexture2D(vxEngine engine, Texture2D texture, string path)
		{
			// Get the engine
			Engine = engine;

			//set the path
			PathToOriginal = path;

			// Load the original Texture
			TextureOriginal = texture;

			for (int ind = 0; ind < TexturePackCount; ind++)
			{
				try
				{
					// First get the Path name of the needed file.
					string filepath = path + "_res" + ind.ToString() + ".png";
				vxConsole.WriteVerboseLine("Looking for " + filepath + "...");

					// Next check if the file exisits
					if (File.Exists(filepath))
					{
						TexturePack.Add(vxUtil.LoadTextureFromFile(filepath, Engine.GraphicsDevice));
						vxConsole.WriteVerboseLine("Found!");
					}
					// If the file does not exist, or if this is in DEBUG mode, then create a new texture.
					else
					{
						vxConsole.WriteLine("Creating New Texture File! : " + filepath);
						// First scale down the new texture
						Texture2D newTexture = ScaleDownTexture(TextureOriginal, divisors[ind]);

						// Now add it to the texture pack
						TexturePack.Add(newTexture);

						// Finally, Save the new texture as a png file
						SaveTexture(filepath, newTexture);
					}
				}
				catch (Exception ex)
				{
					vxConsole.WriteLine("\n >> >> >> " + ex.Message);

					// Now add it to the texture pack
					TexturePack.Add(NullDiffuseTexture);
				}
			}


		}

		/// <summary>
		/// Creates the texture pack. This is used when a already loaded texture is set to the Texture property here.
		/// This will NOT create new texture *.png files.
		/// </summary>
		/// <param name="texture">Texture.</param>
		private void CreateTexturePack(Texture2D texture)
		{
			for (int i = 0; i < TexturePackCount; i++)
			{
				// First scale down the new texture
				Texture2D newTexture = ScaleDownTexture(TextureOriginal, divisors[i]);

				// Now add it to the texture pack
				TexturePack.Add(newTexture);

			}
		}

		/// <summary>
		/// Scales down texture the texture by the specified divisor.
		/// </summary>
		/// <returns>The down texture.</returns>
		/// <param name="texture">Texture.</param>
		/// <param name="divisor">Divisor.</param>
		public Texture2D ScaleDownTexture(Texture2D texture, int divisor)
		{
			int oldwidth = texture.Width;
			int oldheight = texture.Height;

			int newWidth = Math.Max(oldwidth / Math.Max(divisor, 1), MIN_SIZE);
			int newHeight = Math.Max(oldheight / Math.Max(divisor, 1), MIN_SIZE);

			//Now resize the texture
			return vxUtil.ResizeTexture2D(Engine, texture, newWidth, newHeight);
		}


		/// <summary>
		/// Saves the texture.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="texture">Texture.</param>
		public void SaveTexture(string filename, Texture2D texture)
		{
			Stream streampng = File.OpenWrite(filename);
			texture.SaveAsPng(streampng, texture.Width, texture.Height);
			streampng.Dispose();
		}
	}
}

