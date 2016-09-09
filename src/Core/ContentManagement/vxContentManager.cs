using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.XNA.ContentManagement
{
	/// <summary>
	/// Class which encorporates a number of different functions for asset loading and content management.
	/// </summary>
	public class vxContentManager
	{
		/// <summary>
		/// Corresponding Engine Instance.
		/// </summary>
		public vxEngine Engine { get; set; }


		/// <summary>
		/// A List collection of all loaded vxModel's.
		/// </summary>
		public List<vxModel> vxModelCollection = new List<vxModel>();

		public vxContentManager(vxEngine Engine)
		{
			this.Engine = Engine;
		}

		/// <summary>
		/// Check's whether or not all of the model files have been created for the vxModel class.
		/// </summary>
		private void DoModelFileCheck(string PathToModel, ContentManager Content)
		{

			//Now Check too see if the shadow model exits, if not, then create a copy in the same directory.

			string shadowModelPath = Content.RootDirectory + "/" + PathToModel + "_shdw.xnb";
			string utilModelPath = Content.RootDirectory + "/" + PathToModel + "_util.xnb";


#if DEBUG
			//Always Copy a new one during debug.
			File.Copy(Content.RootDirectory + "/" + PathToModel + ".xnb", shadowModelPath, true);
			File.Copy(Content.RootDirectory + "/" + PathToModel + ".xnb", utilModelPath, true);
#else
                
                Console.Write("\t\t\tLooking For Shadow Model: '{0}' ...", shadowModelPath);
                if (File.Exists(shadowModelPath) == false)
                {
                    Console.Write("\n\t\t\tCreating At Shadow Model: ...");
                    File.Copy(Content.RootDirectory + "/" + PathToModel + ".xnb", shadowModelPath);
                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.WriteLine("Found!");
                }

                Console.Write("\t\t\tLooking For Utility Model: '{0}' ...", utilModelPath);
                if (File.Exists(utilModelPath) == false)
                {
                    Console.Write("\n\t\t\tCreating At Utility Model: ...");
                    File.Copy(Content.RootDirectory + "/" + PathToModel + ".xnb", utilModelPath);
                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.WriteLine("Found!");
                }
#endif
		}


#if !VRTC_PLTFRM_DROID

		/// <summary>
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path">The Model File Path</param>
		/// <returns>A Model Object With the Main Shader Applied</returns>
		public vxModel LoadModel(string Path)
		{
			return LoadModel(Path, this.Engine.Game.Content);
		}

		/// <summary>
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path">The Model File Path</param>
		/// <param name="Content">The Content Manager to load the Model with</param>
		/// <returns>A Model Object With the Main Shader Applied</returns>
		public vxModel LoadModel(string Path, ContentManager Content)
		{
			return LoadModel(Path, Content, Engine.Assets.Shaders.MainShader);
		}


		/// <summary>
		/// Load a Model and apply a Custom Shader Effect to it.
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="Path">Path.</param>
		/// <param name="Content">Content.</param>
		/// <param name="EffectToSet">Effect to set.</param>
		public vxModel LoadModel(string Path, ContentManager Content, Effect EffectToSet)
		{
			return LoadModel(Path, Content, EffectToSet, Engine.Assets.Shaders.CascadeShadowShader, Engine.Assets.Shaders.UtilityShader);
		}


		/// <summary>
		/// This Loads Models at Run time performing a number of functions. See remarks for full details.
		/// </summary>
		/// <remarks>
		/// Model Loading
		/// =============================
		/// This loads a vxModel with a Specified Effect as well as applies the CascadeShadowEffect to 
		/// the vxModel's internal Shadow Model as well. XNA and potentially other back ends do not allow
		/// multiple loading of the same asset, therefore if a Shadow Model.xnb is not found, then it is created
		/// from a copy of the main model as 'mainmodelname_shdw.xnb'. 
		/// 
		/// 
		/// Texture Loading
		/// =============================
		/// Furthermore, Textures are loaded based off of the name of the model mesh name.
		/// 
		/// For Example
		/// -------------
		/// ModelMesh Name = "ship"
		/// 
		/// Then the content importer will look for textures under the following names:
		/// 
		/// Diffuse Texture:    ship_dds
		/// Normal Map:         ship_nm
		/// Specular Map:       ship_sm
		/// 
		/// The path to each of these is saved in the vxModel as well too allow for reloading of
		/// other resolution packs later on.
		/// </remarks>
		/// <returns>The model.</returns>
		/// <param name="PathToModel">Path to model.</param>
		/// <param name="Content">Content.</param>
		/// <param name="EffectToSet">Effect to set.</param>
		/// <param name="ShadowEffect">Shadow effect.</param>
		/// <param name="UtilityEffect">Utility effect.</param>
		public vxModel LoadModel(string PathToModel, ContentManager Content, Effect EffectToSet, Effect ShadowEffect, Effect UtilityEffect)
		{
			vxConsole.WriteVerboseLine("     Importing Model: " + PathToModel);

			// Create the Model Object to return
			vxModel newModel = new vxModel(Engine);


			// Next Load in the Main Model.
			newModel.ModelMain = Content.Load<Model>(PathToModel);

			DoModelFileCheck(PathToModel, Content);

			#region Main Model Loading
			/*********************************************************************************/
			//                              MAIN MODEL
			/*********************************************************************************/

			//
			// Now Apply and Initialise the New Effect on the MainModel Object using the speceified Effect.
			//


			// Table mapping the original effects to our replacement versions.
			Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

			foreach (ModelMesh mesh in newModel.ModelMain.Meshes)
			{
				vxConsole.WriteVerboseLine("          Mesh Name: " + mesh.Name);
				mesh.Tag = PathToModel;
				// Scan over all the effects currently on the mesh.
				foreach (Effect oldEffect in mesh.Effects)
				{
					// If we haven't already seen this effect...
					if (!effectMapping.ContainsKey(oldEffect))
					{
						// Make a clone of our replacement effect. We can't just use
						// it directly, because the same effect might need to be
						// applied several times to different parts of the model using
						// a different texture each time, so we need a fresh copy each
						// time we want to set a different texture into it.
						Effect newEffect = EffectToSet.Clone();

						//First Load in the Texture packs based off of the mesh name
						newModel.LoadTextures(Content, PathToModel);


						/*
						//Main Diffuse Texture
						if (newEffect.Parameters["Texture"] != null)
						{
							//First Create The Path to the Diffuse Texture
							newModel.LoadTextures(Content, PathToModel);
						}
						/*
						// Normal Map
						if (newEffect.Parameters["NormalMap"] != null &&
							File.Exists(Content.RootDirectory + "/" + vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_nm.xnb"))
						{
							newEffect.Parameters["NormalMap"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_nm"));

							vxConsole.WriteVerboseLine("\t\t\t\tNormal Map Found");

						}
						// Specular Map
						if (newEffect.Parameters["SpecularMap"] != null &&
							File.Exists(Content.RootDirectory + "/" + vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_sm.xnb"))
						{

							newEffect.Parameters["SpecularMap"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_sm"));

							vxConsole.WriteVerboseLine("\t\t\t\tSpecular Map Found");

						}
						*/

						if (newEffect.Parameters["TextureEnabled"] != null)
							newEffect.Parameters["TextureEnabled"].SetValue(true);

						if (newEffect.Parameters["IsSun"] != null)
							newEffect.Parameters["IsSun"].SetValue(false);

						Vector4[] ShadowSplitColors = new[] {
								new Vector4 (255, 0, 0, 255),
								new Vector4 (0, 255, 0, 255),
								new Vector4 (0, 0, 255, 255),
								new Vector4 (160, 32, 240, 255)
							};

						if (newEffect.Parameters["SplitColors"] != null)
							newEffect.Parameters["SplitColors"].SetValue(ShadowSplitColors);


						if (newEffect.Parameters["AlphaValue"] != null)
							newEffect.Parameters["AlphaValue"].SetValue(1);

						if (newEffect.Parameters["ShadowBrightness"] != null)
							newEffect.Parameters["ShadowBrightness"].SetValue(0.25f);


#if VRTC_PLTFRM_XNA
                            if (newEffect.Parameters["LightDirection"] != null)
                                newEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

                            if (newEffect.Parameters["LightColor"] != null)
                                newEffect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

                            if (newEffect.Parameters["AmbientLightColor"] != null)
                                newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
#endif

						effectMapping.Add(oldEffect, newEffect);
					}
				}

				// Now that we've found all the effects in use on this mesh,
				// update it to use our new replacement versions.
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = effectMapping[meshPart.Effect];
				}
			}
			#endregion

			// Now load the shadow model.
			newModel.ModelShadow = LoadShadowModel(PathToModel, Content, ShadowEffect);


			// Now load the utility model.
			newModel.ModelUtility = LoadUtilityModel(PathToModel, Content, UtilityEffect);

			// Add a tag of the Path to the model for debuging
			newModel.ModelMain.Tag = PathToModel;

			return newModel;
		}






		//////////////////////////////////////////////////////////////////////////////////
		//                              SHADOW MODEL
		//////////////////////////////////////////////////////////////////////////////////




		private Model LoadShadowModel(string PathToModel, ContentManager Content, Effect ShadowEffect)
		{
			vxConsole.WriteVerboseLine("------------------------------------------------------------");

			Model sdwmodel = Content.Load<Model>(PathToModel + "_shdw");
			//Grab the CascadeShadowShader.
			//Effect CascadeShadowShader = ShadowEffect;

			// Table mapping the original effects to our replacement versions.
			Dictionary<Effect, Effect> effectShadowMapping = new Dictionary<Effect, Effect>();

			foreach (ModelMesh mesh in sdwmodel.Meshes)
			{
				vxConsole.WriteVerboseLine("          Shadow Mesh: " + mesh.Name);

				mesh.Tag = PathToModel;
				// Scan over all the effects currently on the mesh.
				foreach (Effect oldEffect in mesh.Effects)
				{
					// If we haven't already seen this effect...
					if (!effectShadowMapping.ContainsKey(oldEffect))
					{
						// Make a clone of our replacement effect. We can't just use
						// it directly, because the same effect might need to be
						// applied several times to different parts of the model using
						// a different texture each time, so we need a fresh copy each
						// time we want to set a different texture into it.
						Effect newEffect = ShadowEffect.Clone();

						effectShadowMapping.Add(oldEffect, newEffect);
					}
				}

				// Now that we've found all the effects in use on this mesh,
				// update it to use our new replacement versions.
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = effectShadowMapping[meshPart.Effect];
				}
			}

			return sdwmodel;
		}





		/*********************************************************************************/
		//                              UTILITY MODEL
		/*********************************************************************************/


		/// <summary>
		/// Loads the utility model.
		/// </summary>
		/// <returns>The utility model.</returns>
		/// <param name="PathToModel">Path to model.</param>
		/// <param name="Content">Content.</param>
		/// <param name="UtilityEffect">Utility effect.</param>
		private Model LoadUtilityModel(string PathToModel, ContentManager Content, Effect UtilityEffect)
		{
			Model utilModel = Content.Load<Model>(PathToModel + "_util");

			// Table mapping the original effects to our replacement versions.
			Dictionary<Effect, Effect> effectUtilityMapping = new Dictionary<Effect, Effect>();

			foreach (ModelMesh mesh in utilModel.Meshes)
			{
				vxConsole.WriteVerboseLine("          Utility Mesh Name: " + mesh.Name);

				mesh.Tag = PathToModel;
				// Scan over all the effects currently on the mesh.
				foreach (Effect oldEffect in mesh.Effects)
				{
					// If we haven't already seen this effect...
					if (!effectUtilityMapping.ContainsKey(oldEffect))
					{
						// Make a clone of our replacement effect. We can't just use
						// it directly, because the same effect might need to be
						// applied several times to different parts of the model using
						// a different texture each time, so we need a fresh copy each
						// time we want to set a different texture into it.
						Effect newEffect = UtilityEffect.Clone();


						//Main Diffuse Texture
						if (newEffect.Parameters["Texture"] != null)
						{
							if (File.Exists(Content.RootDirectory + "/" + vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_dds.xnb"))
								newEffect.Parameters["Texture"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_dds"));
							else if (this.Engine.Assets != null)
							{
								newEffect.Parameters["Texture"].SetValue(this.Engine.Assets.Textures.Texture_Diffuse_Null);

								vxConsole.WriteVerboseLine("DEFAULT DEIFFUSE TEXTURE NOT FOUND FOR MODEL :" + PathToModel);

							}
							else
								newEffect.Parameters["Texture"].SetValue(this.Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_diffuse"));
						}

						// Normal Map
						if (newEffect.Parameters["NormalMap"] != null &&
							File.Exists(Content.RootDirectory + "/" + vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_nm.xnb"))
						{
							newEffect.Parameters["NormalMap"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_nm"));
							vxConsole.WriteVerboseLine("               Normal Map Found");

						}

						// Specular Map
						if (newEffect.Parameters["SpecularMap"] != null &&
							File.Exists(Content.RootDirectory + "/" + vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_sm.xnb"))
						{

							newEffect.Parameters["SpecularMap"].SetValue(Content.Load<Texture2D>(vxUtil.GetParentPathFromFilePath(PathToModel) + "/" + mesh.Name + "_sm"));
							vxConsole.WriteVerboseLine("               Specular Map Found");

						}

						if (newEffect.Parameters["TextureEnabled"] != null)
							newEffect.Parameters["TextureEnabled"].SetValue(true);

						if (newEffect.Parameters["IsSun"] != null)
							newEffect.Parameters["IsSun"].SetValue(false);



#if VRTC_PLTFRM_XNA
                        if (newEffect.Parameters["LightDirection"] != null)
                            newEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

                        if (newEffect.Parameters["LightColor"] != null)
                            newEffect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

                        if (newEffect.Parameters["AmbientLightColor"] != null)
                            newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
#endif

						effectUtilityMapping.Add(oldEffect, newEffect);
					}
				}

				// Now that we've found all the effects in use on this mesh,
				// update it to use our new replacement versions.
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = effectUtilityMapping[meshPart.Effect];
				}
			}

			return utilModel;
		}



		/// <summary>
		/// Loads the basic effect model.
		/// </summary>
		/// <returns>The basic effect model.</returns>
		/// <param name="PathToModel">Path to model.</param>
		public vxModel LoadBasicEffectModel(string PathToModel)
		{
			return LoadBasicEffectModel(PathToModel, this.Engine.Game.Content);
		}

		/// <summary>
		/// Loads the basic effect model.
		/// </summary>
		/// <returns>The basic effect model.</returns>
		/// <param name="PathToModel">Path to model.</param>
		/// <param name="Content">Content.</param>
		public vxModel LoadBasicEffectModel(string PathToModel, ContentManager Content)
		{
			return LoadBasicEffectModel(PathToModel, Content, Engine.Assets.Shaders.CascadeShadowShader, Engine.Assets.Shaders.UtilityShader);
		}

		public vxModel LoadBasicEffectModel(string PathToModel, ContentManager Content, Effect ShadowEffect, Effect UtilityEffect)
		{
			vxConsole.WriteVerboseLine("        Importing Basic Effect Model: " + PathToModel);

			// Create the Model Object to return
			vxModel newModel = new vxModel(Engine);

			// Next Load in the Main Model.
			newModel.ModelMain = Content.Load<Model>(PathToModel);

			DoModelFileCheck(PathToModel, Content);

			// Now load the shadow model.
			newModel.ModelShadow = LoadShadowModel(PathToModel, Content, ShadowEffect);


			// Now load the utility model.
			newModel.ModelUtility = LoadUtilityModel(PathToModel, Content, UtilityEffect);

			// Add a tag of the Path to the model for debuging
			newModel.ModelMain.Tag = PathToModel;

			return newModel;
		}










		/// <summary>
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path">The Model File Path</param>
		/// <param name="Content">The Content Manager to load the Model with</param>
		/// <returns>A Model Object With the Main Shader Applied</returns>
		public Model LoadModelAsWaterObject(string Path, ContentManager Content)
		{
			Model modelToReturn;
			Effect replacementEffect;

			modelToReturn = Content.Load<Model>(Path);

#if VRTC_PLTFRM_XNA
            replacementEffect = this.Engine.Assets.Shaders.WaterReflectionShader;

            try
            {
                // Table mapping the original effects to our replacement versions.
                Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

                foreach (ModelMesh mesh in modelToReturn.Meshes)
                {
                    // Scan over all the effects currently on the mesh.
                    foreach (BasicEffect oldEffect in mesh.Effects)
                    {
                        // If we haven't already seen this effect...
                        if (!effectMapping.ContainsKey(oldEffect))
                        {
                            // Make a clone of our replacement effect. We can't just use
                            // it directly, because the same effect might need to be
                            // applied several times to different parts of the model using
                            // a different texture each time, so we need a fresh copy each
                            // time we want to set a different texture into it.
                            Effect newEffect = replacementEffect.Clone();

                            effectMapping.Add(oldEffect, newEffect);
                        }
                    }

                    // Now that we've found all the effects in use on this mesh,
                    // update it to use our new replacement versions.
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = effectMapping[meshPart.Effect];
                    }
                }
            }
            catch { }
#endif

			return modelToReturn;
		}


		/// <summary>
		/// Load a Model and apply the Main Shader Effect to it.
		/// </summary>
		/// <param name="Path"></param>
		/// <returns>A Model Object With the Distortion Shader Applied</returns>
		public Model LoadDistortionModel(string Path)
		{
			Model modelToReturn;
			Effect replacementEffect = this.Engine.Assets.Shaders.DistortionShader;

			modelToReturn = this.Engine.Game.Content.Load<Model>(Path);

			try
			{
				// Table mapping the original effects to our replacement versions.
				Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

				foreach (ModelMesh mesh in modelToReturn.Meshes)
				{
					// Scan over all the effects currently on the mesh.
					foreach (BasicEffect oldEffect in mesh.Effects)
					{
						// If we haven't already seen this effect...
						if (!effectMapping.ContainsKey(oldEffect))
						{
							// Make a clone of our replacement effect. We can't just use
							// it directly, because the same effect might need to be
							// applied several times to different parts of the model using
							// a different texture each time, so we need a fresh copy each
							// time we want to set a different texture into it.
							Effect newEffect = replacementEffect.Clone();

							//// Copy across the texture from the original effect.
							//newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

							//newEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

							//newEffect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
							//newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));

							effectMapping.Add(oldEffect, newEffect);
						}
					}

					// Now that we've found all the effects in use on this mesh,
					// update it to use our new replacement versions.
					foreach (ModelMeshPart meshPart in mesh.MeshParts)
					{
						meshPart.Effect = effectMapping[meshPart.Effect];
					}
				}
			}
			catch
			{
			}

			return modelToReturn;
		}
#endif
	}
}
