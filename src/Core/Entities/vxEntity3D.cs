#if VIRTICES_3D
using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Audio;
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Core.Debug;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.Core.Entities
{
    
	/// <summary>
	/// Base Entity in the Virtex vxEngine which controls all Rendering and Provides
	/// position and world matrix updates to the other required entities.
	/// </summary>
	public class vxEntity3D : vxEntity
	{
		/// <summary>
		/// The current scene of the game
		/// </summary>
		public vxScene3D Current3DScene
		{
			get { return (vxScene3D)vxEngine.CurrentGameplayScreen; }
		}

		/// <summary>
		/// The current scenes AudioManager
		/// </summary>
		public vxAudioManager AudioManager
		{
			get { return ((vxScene3D)vxEngine.CurrentGameplayScreen).AudioManager; }
		}


		/// <summary>
		/// The current scenes Camera
		/// </summary>
		public vxCamera3D Camera
		{
			get { return ((vxScene3D)vxEngine.CurrentGameplayScreen).Camera; }
		}

		/// <summary>
		/// The vxModel model which holds are graphical, shader and vertices data to be shown.
		/// </summary>
		public vxModel vxModel { get; set; }

		/// <summary>
		/// Gets or sets the mesh set for this entity.
		/// </summary>
		/// <value>The mesh set.</value>
		public vxMeshSet MeshSet { get; set; }

		//public Color SelectionColor = Color.Black;


		/// <summary>
		/// The Owning Instance Mesh Set of this Entity if it is used in one.
		/// </summary>
		public InstanceSet InstanceSetParent { get; set; }

		/// <summary>
		/// The Colour used for certain shaders (i.e. Highliting, and Plain Color)
		/// </summary>
		public Color PlainColor = Color.White;

		private bool _isAlphaNoShadow = false;
		public bool IsAlphaNoShadow
		{
			get { return _isAlphaNoShadow; }
			set { _isAlphaNoShadow = value; }
		}

		private bool _doEdgeDetect = true;
		public bool DoEdgeDetect
		{
			get { return _doEdgeDetect; }
			set
			{
				_doEdgeDetect = value;
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["DoEdgeDetection"] != null)
						part.Effect.Parameters["DoEdgeDetection"].SetValue(_doEdgeDetect);
				}
			}
		}


        public Texture2D DiffuseMap
        {
            get { return _diffuseMap; }
            set
            {
                _diffuseMap = value;
                foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                {
                    if (part.Effect.Parameters["DiffuseMap"] != null)
                        part.Effect.Parameters["DiffuseMap"].SetValue(_diffuseMap);
                }
            }
        }
        Texture2D _diffuseMap;

        #region Entity Fog Controls

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Virtex.Lib.Vrtc.Core.Entities.vxEntity3D"/> should do fog.
        /// </summary>
        /// <value><c>true</c> if do fog; otherwise, <c>false</c>.</value>
        public bool DoFog
		{
			get { return _doFog; }
			set
			{
				_doFog = value;
				if (vxModel != null)
					foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["DoFog"] != null)
							part.Effect.Parameters["DoFog"].SetValue(_doFog);
					}
			}
		}
		bool _doFog = false;


		/// <summary>
		/// Gets or sets the fog near value.
		/// </summary>
		/// <value>The fog near.</value>
		public float FogNear
		{
			get { return _fogNear; }
			set
			{
				_fogNear = value;
				if (vxModel != null)
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["FogNear"] != null)
						part.Effect.Parameters["FogNear"].SetValue(_fogNear);
				}
			}
		}
		float _fogNear = 20;


		/// <summary>
		/// Gets or sets the fog far value.
		/// </summary>
		/// <value>The fog far.</value>
		public float FogFar
		{
			get { return _fogFar; }
			set
			{
				_fogFar = value;
				if (vxModel != null)
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["FogFar"] != null)
						part.Effect.Parameters["FogFar"].SetValue(_fogFar);
				}
			}
		}
		float _fogFar = 100;



		public Color FogColor
		{
			get { return _fogColor; }
			set
			{
				_fogColor = value;
				if (vxModel != null)
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["FogColor"] != null)
						part.Effect.Parameters["FogColor"].SetValue(_fogColor.ToVector4());
				}
			}
		}
		Color _fogColor = Color.CornflowerBlue;

		#endregion


		#region Shadow Mapping Properties
		private bool _doShading = true;
		public bool DoShading
		{
			get { return _doShading; }
			set
			{
				_doShading = value;
				if (vxModel != null)
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["DoShading"] != null)
						part.Effect.Parameters["DoShading"].SetValue(_doShading);
				}
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Virtex.Lib.Vrtc.Core.Entities.vxEntity3D"/> should do shadow map.
		/// </summary>
		/// <value><c>true</c> if do shadow map; otherwise, <c>false</c>.</value>
		public bool DoShadowMap
		{
			get { return _doShadowMap; }
			set
			{
				_doShadowMap = value;
				if (vxModel != null)
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["DoShadow"] != null)
						part.Effect.Parameters["DoShadow"].SetValue(_doShadowMap);
				}
			}
		}
		bool _doShadowMap = true;


		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Virtex.Lib.Vrtc.Core.Entities.vxEntity3D"/> render
		/// shadow split index.
		/// </summary>
		/// <value><c>true</c> if render shadow split index; otherwise, <c>false</c>.</value>
		public bool renderShadowSplitIndex
		{
			set
			{
				_renderShadowSplitIndex = value; //UpdateRenderTechnique();
				if (vxModel != null)
				{
					foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["ShadowDebug"] != null)
							part.Effect.Parameters["ShadowDebug"].SetValue(_renderShadowSplitIndex);
					}
					foreach (var part in vxModel.ModelUtility.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["ShadowDebug"] != null)
							part.Effect.Parameters["ShadowDebug"].SetValue(_renderShadowSplitIndex);
				}
				}
			}
			get { return _renderShadowSplitIndex; }
		}
		bool _renderShadowSplitIndex;



		/// <summary>
		/// Gets or sets the shadow brightness.
		/// </summary>
		/// <value>The shadow brightness.</value>
		public float ShadowBrightness
		{
			get { return _specularIntensity; }
			set
			{
				_shadowBrightness = value;
				if (vxModel != null)
				{
					foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["ShadowBrightness"] != null)
							part.Effect.Parameters["ShadowBrightness"].SetValue(_shadowBrightness);
					}
					foreach (var part in vxModel.ModelUtility.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["ShadowBrightness"] != null)
							part.Effect.Parameters["ShadowBrightness"].SetValue(_shadowBrightness);
				}
				}
			}
		}
		float _shadowBrightness = 0.25f;



		/*
                                if (effect.Parameters["TileBounds"] != null)
                                    effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
                                if (effect.Parameters["SplitColors"] != null)
                                    effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());
*/

		#endregion

		/// <summary>
		/// Off Set for Textures Used On the Model
		/// </summary>
		public Vector2 TextureUVOffset
        {
            get { return _textureUVOffset; }
            set
            {
                _textureUVOffset = value;
                if (vxModel != null)
                    foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                    {
                        if (part.Effect.Parameters["TextureUVOffset"] != null)
                            part.Effect.Parameters["TextureUVOffset"].SetValue(_textureUVOffset);
                    }
            }
        }
        Vector2 _textureUVOffset = Vector2.Zero;

        /// <summary>
        /// Model Used for Distortion
        /// </summary>
        public Model Model_Distorter
		{
			get { return _model_Distorter; }
			set { _model_Distorter = value; }
		}
		Model _model_Distorter;

		/// <summary>
		/// SpecularIntensity of the Shader
		/// </summary>
		public float SpecularIntensity
		{
			set
			{
				_specularIntensity = value;

				if (vxModel != null)
				{
					foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["SpecularIntensity"] != null)
							part.Effect.Parameters["SpecularIntensity"].SetValue(_specularIntensity);
					}
					foreach (var part in vxModel.ModelUtility.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["SpecularIntensity"] != null)
							part.Effect.Parameters["SpecularIntensity"].SetValue(_specularIntensity);
				}
				}
			}
			get { return _specularIntensity; }
		}
		float _specularIntensity = 8;



		/// <summary>
		/// SpecularIntensity of the Shader
		/// </summary>
		public float SpecularPower
		{
			set
			{
				_specularPower = value;

				if (vxModel != null)
				{
					foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["SpecularPower"] != null)
							part.Effect.Parameters["SpecularPower"].SetValue(_specularPower);
					}
					foreach (var part in vxModel.ModelUtility.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["SpecularPower"] != null)
							part.Effect.Parameters["SpecularPower"].SetValue(_specularPower);
				}
				}
			}
			get { return _specularPower; }
		}
		float _specularPower = 1;



		/// <summary>
		/// Toggles Whether or not the main diffuse texture is shown.
		/// </summary>
		public bool TextureEnabled
		{
			set
			{
				_textureEnabled = value;
				if (vxModel != null)
					if (vxModel.ModelMain != null)
						foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
						{
							if (part.Effect.Parameters["TextureEnabled"] != null)
								part.Effect.Parameters["TextureEnabled"].SetValue(_textureEnabled);
						}
			}
			get { return _textureEnabled; }
		}
		bool _textureEnabled;


		/// <summary>
		/// Emissive Colour for use in Highlighting a Model.
		/// </summary>
		public Color EmissiveColour
		{
			set
			{
				_emissiveColour = value;
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["EvissiveColour"] != null)
						part.Effect.Parameters["EvissiveColour"].SetValue(_emissiveColour.ToVector4() / 2);
				}
			}
			get { return _emissiveColour; }
		}
		Color _emissiveColour;


		/// <summary>
		/// Texture which is applied as the Reflection Map
		/// </summary>
		public Texture2D ReflectionMap
		{
			set
			{
				_reflectionMap = value;
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					part.Effect.Parameters["DoReflectionMap"].SetValue(true);
					part.Effect.Parameters["SkyboxTexture"].SetValue(_reflectionMap);
				}
			}
			get { return _reflectionMap; }
		}
		Texture2D _reflectionMap;

		/// <summary>
		/// SpecularIntensity of the Shader
		/// </summary>
		public float ReflectionAmount
		{
			set
			{
				_reflectionAmount = Math.Min(value, 1);
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					part.Effect.Parameters["ReflectionAmount"].SetValue(_reflectionAmount);
				}
			}
			get { return _reflectionAmount; }
		}
		float _reflectionAmount = 1;

		/*
        /// <summary>
        /// The Models main diffuse texture.
        /// </summary>
        public Texture DistortionMap
        {
            set
            {
                _distortionMap = value;
                foreach (var part in _model_Distorter.Meshes.SelectMany(m => m.MeshParts))
                {
                    part.Effect.Parameters["DisplacementMap"].SetValue(_modelTexture);
                }
            }
            get { return _distortionMap; }
        }
        Texture _distortionMap;
        */

		/// <summary>
		/// Model Alpha Value for Transparency
		/// </summary>
		public float AlphaValue
		{
			set
			{
				_mAlphaValue = value;
				if (vxModel != null)
				foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
				{
					if (part.Effect.Parameters["AlphaValue"] != null)
						part.Effect.Parameters["AlphaValue"].SetValue(_mAlphaValue);
				}

			}
			get { return _mAlphaValue; }
		}
		private float _mAlphaValue = 1;


		/// <summary>
		/// The Current Render Technique for thie Model
		/// </summary>
		public string RenderTechnique
		{
			set { _technique = value; /*UpdateRenderTechnique();*/ }
			get { return _technique; }
		}
		string _technique = "Lambert";

		public string mainTechnique = "Lambert";



		/// <summary>
		/// Location of Entity in world space.
		/// </summary>
		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}
		private Vector3 _position;

        /// <summary>
        /// The Bounding Sphere which is used to do frustrum culling.
        /// </summary>
        public BoundingSphere BoundingShape;

        /// <summary>
        /// Boolean value which holds whether or not the current Bounding Shape is within the Camera's View Frustrum.
        /// </summary>
        public bool IsInCameraViewFrustum = true;


        /// <summary>
        /// Direction Entity is facing.
        /// </summary>
        public Vector3 Direction;

		/// <summary>
		/// Entity's up vector.
		/// </summary>
		public Vector3 Up;

		/// <summary>
		/// Entity's right vector.
		/// </summary>
		public Vector3 Right;

		/// <summary>
		/// Current Entity velocity.
		/// </summary>
		public Vector3 Velocity;

		/// <summary>
		/// Entity world transform matrix.
		/// </summary>
		public Matrix World
		{
			get { return _world; }
			set { _world = value; }
		}
		Matrix _world = Matrix.Identity;


		/// <summary>
		/// Entity world transform matrix.
		/// </summary>
		public Matrix World_Distorter
		{
			get { return _world_Distorter; }
			set { _world_Distorter = value; }
		}
		Matrix _world_Distorter = Matrix.Identity;

		/// <summary>
		/// Item Bounding Box
		/// </summary>
		public BoundingBox BoundingBox { get; set; }


		public Texture2D Icon
		{
			get
			{
				if (_icon == null)
					return vxEngine.Assets.Textures.Gradient;
				else
					return _icon;
			}
			set
			{
				_icon = value;
			}
		}
		private Texture2D _icon;

		/// <summary>
		/// Base Entity Object for the vxEngine.
		/// <para>If a model is not specefied, this entity is used as an 'Empty' in the vxEngine.</para>
		/// </summary>
		/// <param name="vxEngine">The current instance of the vxEngine.</param>
		/// <param name="StartPosition">The Start Position of the Entity</param>
		public vxEntity3D(vxEngine vxEngine, Vector3 StartPosition) : base(vxEngine)
		{
			World = Matrix.CreateRotationX(-MathHelper.PiOver2)
				* Matrix.CreateTranslation(StartPosition);

			//Set Position Data
			Position = StartPosition;
		}

		/// <summary>
		/// Base Entity Object for the vxEngine.
		/// </summary>
		/// <param name="vxEngine">The current instance of the vxEngine.</param>
		/// <param name="EntityModel">The Entities Model to be used.</param>
		/// <param name="StartPosition">The Start Position of the Entity.</param>
		public vxEntity3D(vxEngine vxEngine, vxModel EntityModel, Vector3 StartPosition) : base(vxEngine)
		{
			World = Matrix.CreateRotationX(-MathHelper.PiOver2)
				* Matrix.CreateTranslation(StartPosition);

			//Set Model Data
			this.vxModel = EntityModel;

			//Set Position Data
			this.Position = StartPosition;

			InitShaders();
		}
        Vector3 ModelCenter;
		/// <summary>
		/// Initialise the Main Shader.
		/// <para>If a different shader is applied to this model, this method should be overridden</para>
		/// </summary>
		public virtual void InitShaders()
		{
           // BoundingShape = new BoundingSphere(this.Position, 2);

			//By Default, Don't Show Fog
			DoFog = vxEngine.Current3DSceneBase.DoFog;

			SpecularIntensity = 0;
			SpecularPower = 1;

			ShadowBrightness = 0.25f;

			AlphaValue = 1;

			// set lighting parameters for each shader
			if (vxModel != null)
			{

				//Set Texture Quality
				vxModel.SetTexturePackLevel(vxEngine.Profile.Settings.Graphics.TextureQuality);

				if (vxModel.ModelMain != null)
				{
                    BoundingShape = vxGeometryHelper.GetModelBoundingSphere(vxModel.ModelMain);
                    ModelCenter = BoundingShape.Center;

                    foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["LightDirection"] != null)
							part.Effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

						if (part.Effect.Parameters["LightColor"] != null)
							part.Effect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

						if (part.Effect.Parameters["AmbientLightColor"] != null)
							part.Effect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));

						if (part.Effect.Parameters["PoissonKernel"] != null)
							part.Effect.Parameters["PoissonKernel"].SetValue(vxEngine.Renderer.poissonKernel);

						if (part.Effect.Parameters["RandomTexture3D"] != null)
							part.Effect.Parameters["RandomTexture3D"].SetValue(vxEngine.Renderer.RandomTexture3D);
						if (part.Effect.Parameters["RandomTexture2D"] != null)
							part.Effect.Parameters["RandomTexture2D"].SetValue(vxEngine.Renderer.RandomTexture2D);

						if (part.Effect.Parameters["EvissiveColour"] != null)
							part.Effect.Parameters["EvissiveColour"].SetValue(new Vector4(0.5f));

						if (part.Effect.Parameters["FogNear"] != null)
							part.Effect.Parameters["FogNear"].SetValue(vxEngine.Current3DSceneBase.FogNear);

						if (part.Effect.Parameters["FogFar"] != null)
							part.Effect.Parameters["FogFar"].SetValue(vxEngine.Current3DSceneBase.FogFar);

                        
                        DiffuseMap = vxEngine.Assets.Textures.Blank;

                        //if (part.Effect.Parameters["FogColor"] != null)
                        //	part.Effect.Parameters["FogColor"].SetValue(vxEngine.Current3DSceneBase.FogColor.ToVector4());

                    }
				}


				//Set Shadow Data
				if (vxModel.ModelShadow != null)
				{
					foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["PoissonKernel"] != null)
							part.Effect.Parameters["PoissonKernel"].SetValue(vxEngine.Renderer.poissonKernel);

						if (part.Effect.Parameters["RandomTexture3D"] != null)
							part.Effect.Parameters["RandomTexture3D"].SetValue(vxEngine.Renderer.RandomTexture3D);
						if (part.Effect.Parameters["RandomTexture2D"] != null)
							part.Effect.Parameters["RandomTexture2D"].SetValue(vxEngine.Renderer.RandomTexture2D);


					}
				}

				if (vxModel.ModelUtility != null)
				{
					foreach (var part in vxModel.ModelUtility.Meshes.SelectMany(m => m.MeshParts))
					{
						if (part.Effect.Parameters["LightDirection"] != null)
							part.Effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

						if (part.Effect.Parameters["LightColor"] != null)
							part.Effect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

						if (part.Effect.Parameters["AmbientLightColor"] != null)
							part.Effect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));

						if (part.Effect.Parameters["PoissonKernel"] != null)
							part.Effect.Parameters["PoissonKernel"].SetValue(vxEngine.Renderer.poissonKernel);

						if (part.Effect.Parameters["RandomTexture3D"] != null)
							part.Effect.Parameters["RandomTexture3D"].SetValue(vxEngine.Renderer.RandomTexture3D);
						if (part.Effect.Parameters["RandomTexture2D"] != null)
							part.Effect.Parameters["RandomTexture2D"].SetValue(vxEngine.Renderer.RandomTexture2D);

						if (part.Effect.Parameters["TileBounds"] != null)
							part.Effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
						if (part.Effect.Parameters["SplitColors"] != null)
							part.Effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());

					}
				}
			}

		}

		/// <summary>
		/// Updates the Entity
		/// </summary>
		public override void Update(GameTime gameTime)
		{
            // Reset the Bounding Sphere's Center Position
            BoundingShape.Center = Vector3.Transform(ModelCenter, World);

            // Now Check if this entity is even in view of the camera
            IsInCameraViewFrustum = Camera.IsInViewFrustrum(BoundingShape);
        }




		/// <summary>
		/// Render's the Model using the applied shader and specefied Render Technique
		/// </summary>
		/// <param name="RenderTechnique">The Current Render Technique to Apply.</param>
		public virtual void RenderMesh(string RenderTechnique)
		{
			if (vxModel != null && vxModel.ModelMain != null && IsInCameraViewFrustum == true)
				if (vxEnviroment.GetVar(vxEnumEnvVarType.DEBUG_MESH).GetAsBool() == false || RenderEvenInDebug == true || IsAlphaNoShadow == false)
                {
                    vxEngine.RenderCount++;
                    Matrix[] transforms = new Matrix[vxModel.ModelMain.Bones.Count];
					vxModel.ModelMain.CopyAbsoluteBoneTransformsTo(transforms);

					// Draw the model. A model can have multiple meshes, so loop.
					foreach (ModelMesh mesh in vxModel.ModelMain.Meshes)
					{
						foreach (Effect effect in mesh.Effects)
						{
							if (effect.Techniques[RenderTechnique] != null)
							{
								effect.CurrentTechnique = effect.Techniques[RenderTechnique];

								effect.Parameters["World"].SetValue(World);
								effect.Parameters["View"].SetValue(Camera.View);
								effect.Parameters["Projection"].SetValue(Camera.Projection);

								if (effect.Parameters["LightDirection"] != null)
									effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(vxEngine.Renderer.lightPosition));


								if (effect.Parameters["ViewVector"] != null)
									effect.Parameters["ViewVector"].SetValue(vxEngine.Current3DSceneBase.Camera.View.Forward);


								if (effect.Parameters["ShadowMap"] != null)
									effect.Parameters["ShadowMap"].SetValue(vxEngine.Renderer.RT_ShadowMap);
								if (effect.Parameters["ShadowTransform"] != null)
									effect.Parameters["ShadowTransform"].SetValue(vxEngine.Renderer.ShadowSplitProjectionsWithTiling);


								if (effect.Parameters["CameraPos"] != null)
									effect.Parameters["CameraPos"].SetValue(vxEngine.Current3DSceneBase.Camera.WorldMatrix.Translation);


								//TODO: Move all below to get - set properties

								if (vxEngine.Profile.Settings.Graphics.ShadowQuality == Settings.vxEnumQuality.None)
									DoShadowMap = false;
								else
									DoShadowMap = true;

								if (effect.Parameters["TileBounds"] != null)
									effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
								if (effect.Parameters["SplitColors"] != null)
									effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());


								
								 if (effect.Parameters["ShadowBrightness"] != null)
									effect.Parameters["ShadowBrightness"].SetValue(0.25f);

                                if (effect.Parameters["FogNear"] != null)
                                    effect.Parameters["FogNear"].SetValue(vxEngine.Current3DSceneBase.FogNear);

                                if (effect.Parameters["FogFar"] != null)
                                    effect.Parameters["FogFar"].SetValue(vxEngine.Current3DSceneBase.FogFar);

                                if (effect.Parameters["FogColor"] != null)
                                    effect.Parameters["FogColor"].SetValue(vxEngine.Current3DSceneBase.FogColor.ToVector4());

                                if (effect.Parameters["Alpha"] != null)
									effect.Parameters["Alpha"].SetValue(AlphaValue);

							}
						}

						// Draw the mesh, using the effects set above.
						mesh.Draw();
					}
				}
            //vxDebugShapeRenderer.AddBoundingSphere(BoundingShape, Color.LimeGreen);
		}

		/// <summary>
		/// Renders to the Normal Depth Map for Edge Detection using the Utility Model in the vxModel Object.
		/// </summary>
		public virtual void RenderMeshPrepPass()
		{
			if (vxModel != null && vxModel.ModelUtility != null && IsInCameraViewFrustum == true)
			{
				// Look up the bone transform matrices.
				Matrix[] transforms = new Matrix[vxModel.ModelUtility.Bones.Count];

				vxModel.ModelUtility.CopyAbsoluteBoneTransformsTo(transforms);

				// Draw the model.
				foreach (ModelMesh mesh in vxModel.ModelUtility.Meshes)
				{
					foreach (Effect effect in mesh.Effects)
					{
						// Specify which effect technique to use.
						if (effect.Techniques["Technique_PrepPass"] != null)
						{
							effect.CurrentTechnique = effect.Techniques["Technique_PrepPass"];

							effect.Parameters["World"].SetValue(World);
							effect.Parameters["View"].SetValue(Camera.View);
							effect.Parameters["Projection"].SetValue(Camera.Projection);

							if (effect.Parameters["LightDirection"] != null)
								effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(vxEngine.Renderer.lightPosition));
						}

						mesh.Draw();
					}
				}
			}
		}
		/// <summary>
		/// Renders for the Water Reflection
		/// </summary>
		public virtual void RenderMeshForWaterReflectionPass(Plane surfacePlane)
		{
			if (vxModel!= null && vxModel.ModelUtility != null && IsInCameraViewFrustum == true)
			{
				// Look up the bone transform matrices.
				Matrix[] transforms = new Matrix[vxModel.ModelUtility.Bones.Count];

				vxModel.ModelUtility.CopyAbsoluteBoneTransformsTo(transforms);

				// Draw the model.
				foreach (ModelMesh mesh in vxModel.ModelUtility.Meshes)
				{
					foreach (Effect effect in mesh.Effects)
					{
						// Specify which effect technique to use.
						effect.CurrentTechnique = effect.Techniques["Technique_WtrRflcnPass"];

						effect.Parameters["World"].SetValue(World);
						effect.Parameters["View"].SetValue(Camera.GetReflectionView(surfacePlane));
						effect.Parameters["Projection"].SetValue(Camera.Projection);
						effect.Parameters["ClipPlane0"].SetValue(new Vector4(surfacePlane.Normal, surfacePlane.D));
						//vxConsole.WriteToInGameDebug(surfacePlane);
						//effect.Parameters["ClipPlane0"].SetValue(new Vector4(surfacePlane.Normal.X, surfacePlane.Normal.Z, surfacePlane.Normal.Y, surfacePlane.D));
					}

					mesh.Draw();
				}
			}
		}


		/// <summary>
		/// Renders to the Shadow Map
		/// </summary>
		public virtual void RenderMeshShadow()
		{

			if (vxModel != null && DoShadowMap && vxModel.ModelShadow != null && IsInCameraViewFrustum == true)
			{
				for (int i = 0; i < vxEngine.Renderer.NumberOfShadowSplits; ++i)
				{
					{
						int x = i % 2;
						int y = i / 2;
						var viewPort = new Viewport(x * vxEngine.Renderer.ShadowMapSize, y * vxEngine.Renderer.ShadowMapSize,
							vxEngine.Renderer.ShadowMapSize, vxEngine.Renderer.ShadowMapSize);

						vxEngine.GraphicsDevice.Viewport = viewPort;
					}

					// Draw the arena model first.
					foreach (ModelMesh mesh in vxModel.ModelShadow.Meshes)
					{
						foreach (var effect in mesh.Effects)
						{
							// Specify which effect technique to use.
							if (effect.Techniques["Shadow"] != null)
								effect.CurrentTechnique = effect.Techniques["Shadow"];

							if (effect.Parameters["ViewProjection_Sdw"] != null)
								effect.Parameters["ViewProjection_Sdw"].SetValue(vxEngine.Renderer.ShadowSplitProjections[i]);
							if (effect.Parameters["World"] != null)
								effect.Parameters["World"].SetValue(World);
							if (effect.Parameters["DepthBias_Sdw"] != null)
								effect.Parameters["DepthBias_Sdw"].SetValue(new Vector2(vxEngine.Renderer.ShadowDepthBias[i, 0],
								vxEngine.Renderer.ShadowDepthBias[i, 1]));

							if (effect.Parameters["TileBounds"] != null)
								effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
							if (effect.Parameters["SplitColors"] != null)
								effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());

						}

						mesh.Draw();
					}
				}
			}
		}

		/*
        /// <summary>
        /// Draws the Models to the Distortion Target
        /// </summary>
        public virtual void DrawModelDistortion(vxEngine vxEngine, GameTime gameTime)
        {
            if (Model_Distorter != null)
            {
                // draw the distorter
                Matrix worldView = World_Distorter * Camera.View;

                // make sure the depth buffering is on, so only parts of the scene
                // behind the distortion effect are affected
                vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                foreach (ModelMesh mesh in Model_Distorter.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique =
                            effect.Techniques[DistortionTechnique.ToString()];
                        effect.Parameters["WorldView"].SetValue(worldView);
                        effect.Parameters["WorldViewProjection"].SetValue(worldView *
                            Camera.Projection);

                        effect.Parameters["DisplacementMap"].SetValue(DistortionMap);

                        effect.Parameters["offset"].SetValue(0);

                        effect.Parameters["DistortionScale"].SetValue(DistortionScale);
                        effect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
                    }
                    mesh.Draw();
                }
            }
        }
        */
	}
}

#endif