using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Virtex.Lib.Vrtc.Core.Settings;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Virtex.Lib.Vrtc.Graphics
{
	/// <summary>
	/// A Model Class which loads and processes all data at runtime. Although this add's to load times,
	/// it allows for more control as well as modding for any and all models which are used in the game.
	/// Using three different models to handle different types of rendering does add too over all installation
	/// size, it is necessary to allow the shaders to be compiled for cross platform use.
	/// </summary>
	public class vxModel
	{
		vxEngine Engine;

		#region Models
		/// <summary>
		/// This is the Main Model which is drawn to the screen
		/// using which ever main Shader is in the model.
		/// </summary>
		public Model ModelMain
		{
			get { return _model; }
			set { _model = value; }
		}
		private Model _model;


		/// <summary>
		/// This secondary model holds the shadow model which corresponds to the 
		/// 'ModelMain'. This allows for a lower poly version of the main model to be
		/// used for an optimized approach at shadow mapping. 
		/// </summary>
		public Model ModelShadow
		{
			get { return _modelShadow; }
			set { _modelShadow = value; }
		}
		private Model _modelShadow;

		/// <summary>
		/// This third model holds a utility model which corresponds to the 
		/// 'ModelMain'. This model is used for Defferred Rendering and setting up
		/// Depth/Normal buffers.
		/// </summary>
		public Model ModelUtility
		{
			get { return _modelutility; }
			set { _modelutility = value; }
		}
		private Model _modelutility;
		#endregion

		#region Texture Packs


		/// <summary>
		/// The diffuse texture collection.
		/// </summary>
		public List<vxTexture2D> DiffuseTextureCollection = new List<vxTexture2D>();

		#endregion

		#region Null Texture References

		public Texture2D NullDiffuseTexture
		{
			get
			{
				if (Engine.Assets != null)
					return Engine.Assets.Textures.Texture_Diffuse_Null;
				else
					return Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_diffuse");
			}
		}
		#endregion

		/// <summary>
		/// Basic Constructor. Note: All Items must be instantiated outside of this function.
		/// </summary>
		public vxModel(vxEngine Engine)
		{
			this.Engine = Engine;
			this.Engine.vxContentManager.vxModelCollection.Add(this);
		}


		/// <summary>
		/// Loads the texture packs for each mesh.
		/// </summary>
		/// <param name="PathToModel">Path to original model file.</param>
		public void LoadTextures(ContentManager Content, string PathToModel)
		{
			// Load the Textures for hte Model Main.
			foreach (ModelMesh mesh in ModelMain.Meshes)
			{
				//First Create The Path to the Diffuse Texture
				string pathToDiffTexture = vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_dds";

				Texture2D diftexture;

				//First try to find the corresponding diffuse texture for this mesh
				if (File.Exists(Content.RootDirectory + "/" + pathToDiffTexture + ".xnb"))
				{
					diftexture = Content.Load<Texture2D>(pathToDiffTexture);
				}

				// If the mesh diffuse texture isn't found, then set the null texture as a fall back
				else
				{
					vxConsole.WriteWarning(this.ToString(), "DEFAULT DEIFFUSE TEXTURE NOT FOUND FOR MODEL :" + PathToModel);
					diftexture = NullDiffuseTexture;
				}


				//Now Set the Diffuse Texture Pack
				DiffuseTextureCollection.Add(new vxTexture2D(this.Engine, diftexture, Content.RootDirectory + "/" + pathToDiffTexture));

			}
		}

		public void SetTexturePackLevel(vxEnumTextureQuality quality)
		{

			int index = 0;

			//Loop through Main Model and set set texture from pack
			foreach (ModelMesh mesh in ModelMain.Meshes)
			{
				try
				{
					foreach (Effect effect in mesh.Effects)
					{
						//Set Texture Qualities
						DiffuseTextureCollection[index].Quality = quality;

						// Set Texture with Specified Quality
						if (effect.Parameters["Texture"] != null)
							effect.Parameters["Texture"].SetValue(DiffuseTextureCollection[index].Texture);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(this.ModelMain.Tag);
					Console.WriteLine(ex.Message);
					Console.WriteLine("-------");
				}

				index++;
			}
		}
	}
}

