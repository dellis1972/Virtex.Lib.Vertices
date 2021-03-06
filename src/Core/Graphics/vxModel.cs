﻿using System;
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

		/// <summary>
		/// This List holds all Normal Map Texture Packs for this model.
		/// </summary>
		public List<vxTexture2D> NormalMaps = new List<vxTexture2D>();

		/// <summary>
		/// This List holds all Specular Map Texture Packs for this model.
		/// </summary>
		public List<vxTexture2D> SpecularMaps = new List<vxTexture2D>();

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

		public Texture2D NullNormalMap
		{
			get
			{
				if (Engine.Assets != null)
					return Engine.Assets.Textures.Texture_NormalMap_Null;
				else
					return Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_normal");
			}
		}

		public Texture2D NullSpecularMap
		{
			get
			{
				if (Engine.Assets != null)
					return Engine.Assets.Textures.Texture_SpecularMap_Null;
				else
					return Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_specular");
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
		/// <param name="Content">Content Manager to use if different than Game's Content Manager.</param>
		/// <param name="PathToModel">Path to model.</param>
		public void LoadTextures(ContentManager Content, string PathToModel)
		{
			// Load the Textures for hte Model Main.
			foreach (ModelMesh mesh in ModelMain.Meshes)
			{
				//First Create The Path to the Diffuse Texture
				string pathToDiffTexture = vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_dds";
				string pathToNrmMpTexture = vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_nm";
				string pathToSpecMpTexture = vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_sm";

				Texture2D diftexture;
				Texture2D nrmtexture;
				Texture2D spctexture;


				// Load/Create Diffuse Texture Pack
				//**************************************************************************************************************

				//First try to find the corresponding diffuse texture for this mesh, 
				//if it isn't found, then set the null texture as a fall back
				if (File.Exists(Content.RootDirectory + "/" + pathToDiffTexture + ".xnb"))
					diftexture = Content.Load<Texture2D>(pathToDiffTexture);
				else
					diftexture = NullDiffuseTexture;


				//Now Set the Diffuse Texture Pack
				DiffuseTextureCollection.Add(new vxTexture2D(Engine, diftexture, Content.RootDirectory + "/" + pathToDiffTexture));





				// Load/Create Normal Map Texture Pack
				//**************************************************************************************************************

				//First try to find the corresponding normal map texture for this mesh, 
				//if it isn't found, then set the null texture as a fall back
				if (File.Exists(Content.RootDirectory + "/" + pathToNrmMpTexture + ".xnb"))
					nrmtexture = Content.Load<Texture2D>(pathToNrmMpTexture);
				else
					nrmtexture = NullNormalMap;

				// Now Load/Create the Texture pack based off the path
				NormalMaps.Add(new vxTexture2D(Engine, nrmtexture, Content.RootDirectory + "/" + pathToNrmMpTexture));





				// Load/Create Specular Map Texture Pack
				//**************************************************************************************************************

				//First try to find the corresponding normal map texture for this mesh, 
				//if it isn't found, then set the null texture as a fall back
				if (File.Exists(Content.RootDirectory + "/" + pathToSpecMpTexture + ".xnb"))
					spctexture = Content.Load<Texture2D>(pathToSpecMpTexture);
				else
					spctexture = NullSpecularMap;

				// Now Load/Create the Texture pack based off the path
				SpecularMaps.Add(new vxTexture2D(Engine, spctexture, Content.RootDirectory + "/" + pathToSpecMpTexture));

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
						//Set Texture Pack Qualities
						DiffuseTextureCollection[index].Quality = quality;
						NormalMaps[index].Quality = quality;
						SpecularMaps[index].Quality = quality;


						// Set Texture with Specified Quality
						if (effect.Parameters["Texture"] != null)
							effect.Parameters["Texture"].SetValue(DiffuseTextureCollection[index].Texture);

						// Set Normal Map with Specified Quality
						if (effect.Parameters["NormalMap"] != null)
							effect.Parameters["NormalMap"].SetValue(NormalMaps[index].Texture);

						// Set Specular Map with Specified Quality
						if (effect.Parameters["SpecularMap"] != null)
							effect.Parameters["SpecularMap"].SetValue(SpecularMaps[index].Texture);


                    }
				}
				catch (Exception ex)
				{

					Console.WriteLine("-----------------------------------------------");
					Console.WriteLine("Error Setting Texture for Main Model");
					Console.WriteLine(ModelMain.Tag);
					Console.WriteLine("Index: " + index);
					Console.WriteLine(ex.Message);
					Console.WriteLine("-----------------------------------------------");
				}

				index++;
			}

			index = 0;
			foreach (ModelMesh mesh in ModelUtility.Meshes)
			{
				try
				{
					foreach (Effect effect in mesh.Effects)
					{
						//Set Texture Pack Qualities
						DiffuseTextureCollection[index].Quality = quality;
						NormalMaps[index].Quality = quality;
						SpecularMaps[index].Quality = quality;


						// Set Texture with Specified Quality
						if (effect.Parameters["Texture"] != null)
							effect.Parameters["Texture"].SetValue(DiffuseTextureCollection[index].Texture);

						// Set Normal Map with Specified Quality
						if (effect.Parameters["NormalMap"] != null)
							effect.Parameters["NormalMap"].SetValue(NormalMaps[index].Texture);

						// Set Specular Map with Specified Quality
						if (effect.Parameters["SpecularMap"] != null)
							effect.Parameters["SpecularMap"].SetValue(SpecularMaps[index].Texture);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("-----------------------------------------------");
					Console.WriteLine("Error Setting Texture for Utility Model");
					Console.WriteLine(ModelUtility.Tag);
					Console.WriteLine("Index: " + index);
					Console.WriteLine(ex.Message);
					Console.WriteLine("-----------------------------------------------");
				}

				index++;
			}
		}
	}
}

