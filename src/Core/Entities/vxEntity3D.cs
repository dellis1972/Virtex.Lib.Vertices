#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using vxVertices.Core;
using vxVertices.Audio;
using vxVertices.Utilities;
using vxVertices.Core.Scenes;
using vxVertices.Core.Cameras;
using vxVertices.Graphics;


namespace vxVertices.Core.Entities
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

        public Color SelectionColor = Color.Black;


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

        private bool _doFog = false;
        public bool DoFog
        {
            get { return _doFog; }
            set
            {
                _doFog = value;
                foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                {
                    if (part.Effect.Parameters["DoFog"] != null)
                        part.Effect.Parameters["DoFog"].SetValue(_doFog);
                }
            }
        }


        private bool _doShading = true;
        public bool DoShading
        {
            get { return _doShading; }
            set
            {
                _doShading = value;
                foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                {
                    part.Effect.Parameters["DoShading"].SetValue(_doShading);
                }
            }
        }

        private bool _doShadowMap = true;
        public bool DoShadowMap
        {
            get { return _doShadowMap; }
            set { _doShadowMap = value; }
        }

        /// <summary>
        /// Off Set for Textures Used On the Model
        /// </summary>
        //public Vector2 TextureOffset = Vector2.Zero;

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
                foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                {
                    if (part.Effect.Parameters["SpecularIntensity"] != null)
                        part.Effect.Parameters["SpecularIntensity"].SetValue(_specularIntensity);
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
                foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                {
                    if (part.Effect.Parameters["SpecularPower"] != null)
                        part.Effect.Parameters["SpecularPower"].SetValue(_specularPower);
                }
            }
            get { return _specularPower; }
        }
        float _specularPower = 1;

        /*
        /// <summary>
        /// The Models main diffuse texture.
        /// </summary>
        public Texture ModelTexture
        {
            set
            {
                _modelTexture = value;
                foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                {
                    part.Effect.Parameters["Texture"].SetValue(_modelTexture);
                }
            }
            get { return _modelTexture; }
        }
        Texture _modelTexture;
        */

        /// <summary>
        /// Toggles Whether or not the main diffuse texture is shown.
        /// </summary>
        public bool TextureEnabled
        {
            set
            {
                _textureEnabled = value;
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

        /*
        /// <summary>
        /// Texture which is applied as the Normal Map.
        /// </summary>
        public Texture2D NormalMap
        {
            set
            {
                _normalMap = value;
                foreach (var part in model.Meshes.SelectMany(m => m.MeshParts))
                {
                    if (part.Effect.Parameters["DoNormalMap"] != null)
                        part.Effect.Parameters["DoNormalMap"].SetValue(true);

                    if (part.Effect.Parameters["NormalMap"] != null)
                        part.Effect.Parameters["NormalMap"].SetValue(_normalMap);
                }
            }
            get { return _normalMap; }
        }
        Texture2D _normalMap;


        /// <summary>
        /// Texture which is applied as the Specular Map
        /// </summary>
        public Texture2D SpecularMap
        {
            set
            {
                _specularMap = value;
                foreach (var part in model.Meshes.SelectMany(m => m.MeshParts))
                {

                    if (part.Effect.Parameters["DoSpecularMap"] != null)
                        part.Effect.Parameters["DoSpecularMap"].SetValue(true);

                    if (part.Effect.Parameters["SpecularMap"] != null)
                        part.Effect.Parameters["SpecularMap"].SetValue(_specularMap);
                }
            }
            get { return _specularMap; }
        }
        Texture2D _specularMap;
        */

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
            set { _technique = value; UpdateRenderTechnique(); }
            get { return _technique; }
        }
        string _technique = "Lambert";

        public string mainTechnique = "Lambert";

        /// <summary>
        /// Render shadow split index
        /// </summary>
        public bool renderShadowSplitIndex
        {
            set { _renderShadowSplitIndex = value; UpdateRenderTechnique(); }
            get { return _renderShadowSplitIndex; }
        }
        private bool _renderShadowSplitIndex;

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
            this.Position = StartPosition;
        }

        /// <summary>
        /// Base Entity Object for the vxEngine.
        /// </summary>
        /// <param name="vxEngine">The current instance of the vxEngine.</param>
        /// <param name="EntityModel">The Entities Model to be used.</param>
        /// <param name="StartPosition">The Start Position of the Entity.</param>
		public vxEntity3D(vxEngine vxEngine,vxModel EntityModel, Vector3 StartPosition) : base(vxEngine)
        {
            World = Matrix.CreateRotationX(-MathHelper.PiOver2)
                * Matrix.CreateTranslation(StartPosition);

            //Set Model Data
            this.vxModel = EntityModel;

            //Set Position Data
            this.Position = StartPosition;

            InitShaders();
        }

        /// <summary>
        /// Initialise the Main Shader.
        /// <para>If a different shader is applied to this model, this method should be overridden</para>
        /// </summary>
        public virtual void InitShaders()
        {

            // set lighting parameters for each shader
            if (vxModel != null)
            {
                if (vxModel.ModelMain != null)
                {
                    foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                    {
#if VRTC_PLTFRM_XNA
                        if (part.Effect.Parameters["LightDirection"] != null)
                            part.Effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(100, 130, 0)));

                        if (part.Effect.Parameters["LightColor"] != null)
                            part.Effect.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));

                        if (part.Effect.Parameters["AmbientLightColor"] != null)
                            part.Effect.Parameters["AmbientLightColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
#endif
                        if (part.Effect.Parameters["PoissonKernel"] != null)
                            part.Effect.Parameters["PoissonKernel"].SetValue(vxEngine.Renderer.poissonKernel);

                        if (part.Effect.Parameters["RandomTexture3D"] != null)
                            part.Effect.Parameters["RandomTexture3D"].SetValue(vxEngine.Renderer.RandomTexture3D);
                        if (part.Effect.Parameters["RandomTexture2D"] != null)
                            part.Effect.Parameters["RandomTexture2D"].SetValue(vxEngine.Renderer.RandomTexture2D);

                        //By Default, Don't Show Fog
                        DoFog = vxEngine.Current3DSceneBase.DoFog;

                        SpecularIntensity = 0;
                        SpecularPower = 1;
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

                        //By Default, Don't Show Fog
                        DoFog = vxEngine.Current3DSceneBase.DoFog;

                        SpecularIntensity = 0;
                        SpecularPower = 1;
                    }
                }
            }

        }

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            UpdateRenderTechnique();
        }


        /// <summary>
        /// Updates the shaders technique according to the properties normaMapping and
        /// noTextures
        /// </summary>
        public virtual void UpdateRenderTechnique()
        {
            if (vxModel != null)
            {
                if (_renderShadowSplitIndex)
                {
                    _technique = "ShadowSplitIndex";
                }
                else
                {
                    if (_isAlphaNoShadow == true)
                        _technique = "Alpha_NoShadow";
                    else
                        _technique = mainTechnique;
                }

                if (vxModel.ModelMain != null)
                    foreach (var part in vxModel.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                    {
                        if (part.Effect.Techniques[_technique] != null)
                            part.Effect.CurrentTechnique = part.Effect.Techniques[_technique];
                    }
            }
        }

        
        public virtual void RenderMeshPlain()
        {/*
            if (vxModel.ModelMain != null)
            {
                // Look up the bone transform matrices.
                Matrix[] transforms = new Matrix[vxModel.ModelMain.Bones.Count];

                vxModel.ModelMain.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model.
                foreach (ModelMesh mesh in vxModel.ModelMain.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        // Specify which effect technique to use.
                        effect.CurrentTechnique = effect.Techniques["PlainShader"];

                        effect.Parameters["World"].SetValue(World);
                        effect.Parameters["View"].SetValue(Camera.View);
                        effect.Parameters["Projection"].SetValue(Camera.Projection);
                        //Set The Colour
                        effect.Parameters["PlainColor"].SetValue(new Vector4(PlainColor.R, PlainColor.G, PlainColor.B, AlphaValue));
                    }
                    mesh.Draw();
                }
            }*/
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void RenderWithBaseEffect()
        {
            if (vxModel.ModelMain != null)
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[vxModel.ModelMain.Bones.Count];
                vxModel.ModelMain.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in vxModel.ModelMain.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = this.World;
                        effect.View = this.Camera.View;
                        effect.Projection = this.Camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }
        
        /// <summary>
        /// Render's the Model using the applied shader and specefied Render Technique
        /// </summary>
        /// <param name="RenderTechnique">The Current Render Technique to Apply.</param>
        public virtual void RenderMesh(string RenderTechnique)
        {
            if (vxModel.ModelMain != null)
                if (vxEngine.DisplayDebugMesh == false || RenderEvenInDebug == true || IsAlphaNoShadow == false)
                {
                    //updateTechnique();

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

        //                        if (effect.Parameters["DoShadow"] != null)
        //                            effect.Parameters["DoShadow"].SetValue(vxEngine.Profile.Settings.Graphics.ShadowQuality == Settings.vxEnumQuality.None ? true : false);

        //                        if (effect.Parameters["ShadowMap"] != null)
        //                            effect.Parameters["ShadowMap"].SetValue(vxEngine.Renderer.RT_ShadowMap);
        //                        if (effect.Parameters["ShadowTransform"] != null)
        //                            effect.Parameters["ShadowTransform"].SetValue(vxEngine.Renderer.ShadowSplitProjectionsWithTiling);
        //                        if (effect.Parameters["TileBounds"] != null)
        //                            effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
        //                        if (effect.Parameters["SplitColors"] != null)
        //                            effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());

        //                        if (effect.Parameters["CameraPos"] != null)
        //                            effect.Parameters["CameraPos"].SetValue(Camera.WorldMatrix.Translation);

        //                        if (effect.Parameters["FogNear"] != null)
        //                            effect.Parameters["FogNear"].SetValue(5);

        //                        if (effect.Parameters["FogFar"] != null)
        //                            effect.Parameters["FogFar"].SetValue(vxEngine.Current3DSceneBase.Camera.FarPlane / 4);

        //                        if (effect.Parameters["FogColor"] != null)
        //                            effect.Parameters["FogColor"].SetValue(Vector4.One);

        //                        if (effect.Parameters["TextureSampler"] != null)
        //                            effect.Parameters["TextureSampler"].SetValue(vxEngine.Renderer.RT_ColourMap);

        //                        if (effect.Parameters["NormalSampler"] != null)
        //                            effect.Parameters["NormalSampler"].SetValue(vxEngine.Renderer.RT_NormalMap);
                                
        //                        if (effect.Parameters["DiffuseLight"] != null)
        //                            effect.Parameters["DiffuseLight"].SetValue(new Vector3(0.5f));

        //                        if (effect.Parameters["AmbientLight"] != null)
        //                            effect.Parameters["AmbientLight"].SetValue(new Vector3(0.5f));

        //                        effect.Parameters["World"].SetValue(World);
        //                        if (effect.Parameters["View"] != null)
        //                            effect.Parameters["View"].SetValue(this.Camera.View);

        //                        if (effect.Parameters["Projection"] != null)
        //                            effect.Parameters["Projection"].SetValue(this.Camera.Projection);


        //                        if (effect.Parameters["LightPosition"] != null)
        //                            effect.Parameters["LightPosition"].SetValue(vxEngine.Renderer.lightPosition);

        //                        if (effect.Parameters["LightDirection"] != null)
        //                            effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(vxEngine.Renderer.lightPosition));

        //                        float Factor = 125;
        //                        if (effect.Parameters["SelectionColor"] != null)
        //                            effect.Parameters["SelectionColor"].SetValue((new Vector3(SelectionColor.R / Factor, SelectionColor.G / Factor, SelectionColor.B / Factor)));
        //                        /*
        //                        if (TextureOffset != Vector2.Zero)
        //                            vxConsole.WriteToInGameDebug(TextureOffset);
        //*/
        //                        if (effect.Parameters["TextOffset"] != null)
        //                            effect.Parameters["TextOffset"].SetValue(TextureOffset);

        //                        if (effect.Parameters["Alpha"] != null)
        //                            effect.Parameters["Alpha"].SetValue(AlphaValue);



                                //effect.CurrentTechnique = effect.Techniques[technique];
                                effect.Parameters["World"].SetValue(World);
                                effect.Parameters["View"].SetValue(Camera.View);
                                effect.Parameters["Projection"].SetValue(Camera.Projection);
                                effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(vxEngine.Renderer.lightPosition));

                                if (vxEngine.Profile.Settings.Graphics.ShadowQuality == Settings.vxEnumQuality.None)
                                    DoShadowMap = false;
                                else
                                    DoShadowMap = true;

                                if (effect.Parameters["DoShadow"] != null)
                                    effect.Parameters["DoShadow"].SetValue(DoShadowMap);

                                if (effect.Parameters["ViewVector"] != null)
                                    effect.Parameters["ViewVector"].SetValue(vxEngine.Current3DSceneBase.Camera.View.Forward);

                                if (effect.Parameters["ShadowMap"] != null)
                                    effect.Parameters["ShadowMap"].SetValue(vxEngine.Renderer.RT_ShadowMap);
                                if (effect.Parameters["ShadowTransform"] != null)
                                    effect.Parameters["ShadowTransform"].SetValue(vxEngine.Renderer.ShadowSplitProjectionsWithTiling);
                                if (effect.Parameters["TileBounds"] != null)
                                    effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
                                if (effect.Parameters["SplitColors"] != null)
                                    effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());

                                if (effect.Parameters["CameraPos"] != null)
                                    effect.Parameters["CameraPos"].SetValue(vxEngine.Current3DSceneBase.Camera.WorldMatrix.Translation);

                                if (effect.Parameters["FogNear"] != null)
                                    effect.Parameters["FogNear"].SetValue(5);

                                if (effect.Parameters["FogFar"] != null)
                                    effect.Parameters["FogFar"].SetValue(vxEngine.Current3DSceneBase.Camera.FarPlane / 4);

                                if (effect.Parameters["FogColor"] != null)
                                    effect.Parameters["FogColor"].SetValue(Vector4.One);

                                if (effect.Parameters["Alpha"] != null)
                                    effect.Parameters["Alpha"].SetValue(AlphaValue);
                            }
                        }

                        // Draw the mesh, using the effects set above.
                        mesh.Draw();
                    }
                }
        }

        /// <summary>
        /// Renders to the Normal Depth Map for Edge Detection
        /// </summary>
        public virtual void RenderMeshPrepPass()
        {
            if (vxModel != null &&  vxModel.ModelMain != null)
            {
                // Look up the bone transform matrices.
                Matrix[] transforms = new Matrix[vxModel.ModelMain.Bones.Count];

                vxModel.ModelMain.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model.
                foreach (ModelMesh mesh in vxModel.ModelMain.Meshes)
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
                            // vxConsole.WriteToInGameDebug(renderShadowSplitIndex);

                            if (effect.Parameters["CameraPos"] != null)
                                effect.Parameters["CameraPos"].SetValue(Camera.WorldMatrix.Translation);

                            if (effect.Parameters["FogNear"] != null)
                                effect.Parameters["FogNear"].SetValue(5);

                            if (effect.Parameters["FogFar"] != null)
                                effect.Parameters["FogFar"].SetValue(Camera.FarPlane / 2);

                            if (effect.Parameters["FogColor"] != null)
                                effect.Parameters["FogColor"].SetValue(Vector4.One);

#if VRTC_PLTFRM_XNA
                            if (effect.Parameters["ShadowDebug"] != null)
                                effect.Parameters["ShadowDebug"].SetValue(renderShadowSplitIndex);

                            if (effect.Parameters["ShadowMap"] != null)
                                effect.Parameters["ShadowMap"].SetValue(vxEngine.Renderer.RT_ShadowMap);
                            if (effect.Parameters["ShadowTransform"] != null)
                                effect.Parameters["ShadowTransform"].SetValue(vxEngine.Renderer.ShadowSplitProjectionsWithTiling);
                            if (effect.Parameters["TileBounds"] != null)
                                effect.Parameters["TileBounds"].SetValue(vxEngine.Renderer.ShadowSplitTileBounds);
                            if (effect.Parameters["SplitColors"] != null)
                                effect.Parameters["SplitColors"].SetValue(vxEngine.Renderer.ShadowSplitColors.Select(c => c.ToVector4()).ToArray());
#endif
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
            if (vxModel.ModelMain != null)
            {
                // Look up the bone transform matrices.
                Matrix[] transforms = new Matrix[vxModel.ModelMain.Bones.Count];

                vxModel.ModelMain.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model.
                foreach (ModelMesh mesh in vxModel.ModelMain.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        // Specify which effect technique to use.
                        effect.CurrentTechnique = effect.Techniques["Technique_WtrRflcnPass"];

                        effect.Parameters["World"].SetValue(World);
                        effect.Parameters["View"].SetValue(Camera.GetReflectionView(surfacePlane));
                        effect.Parameters["Projection"].SetValue(Camera.Projection);
                        effect.Parameters["ClipPlane0"].SetValue(new Vector4(surfacePlane.Normal, surfacePlane.D));
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

            if (vxModel != null && DoShadowMap && vxModel.ModelShadow != null)
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