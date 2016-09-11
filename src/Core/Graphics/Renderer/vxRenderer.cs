#if VIRTICES_3D

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core.Settings;

namespace Virtex.Lib.Vrtc.Graphics
{

    /// <summary>
    /// A simple renderer. Contains methods for rendering standard XNA models, as well as
    /// instanced rendering (see class InstancedModel), and rendering of selected triangles. 
    /// The scene light position can be set with the property lightPosition.
    /// </summary>
    public partial class vxRenderer
    {

        //public static float DistortionBlurAmount = 0.0001f;
        /// <summary>
        /// Amount of Blue to perform for Distortion
        /// </summary>
        public float BlurAmount = 0.1f;

        private readonly static string[] distortionTechniqueFriendlyNames = new string[]
        {
            "Displacement-Mapped",
            "Heat-Haze",
            "Pull-In",
            "Zero Displacement",
        };

        /// <summary>
        /// Gets the name of the distortion technique.
        /// </summary>
        /// <returns>The distortion technique friendly name.</returns>
        /// <param name="technique">Technique.</param>
        public static string GetDistortionTechniqueFriendlyName(DistortionTechniques technique)
        {
            return distortionTechniqueFriendlyNames[(int)technique];
        }

        /// <summary>
        /// Gets the light position of the scene.
        /// </summary>
        /// <value>The light position.</value>
        public Vector3 lightPosition
        {
            get { return mLightPosition; }
        }
        private Vector3 mLightPosition;

        private GraphicsDevice mGraphicsDevice;

        DynamicVertexBuffer mInstanceDataStream; // secondary vertex buffer used for hardware instancing
        VertexDeclaration mInstanceVertexDeclaration;
        BlendState mAlphaBlendState;

        //Quad Renderer Vertices
        VertexPositionTexture[] quadRendererVerticesBuffer = null;
        short[] quadRendererIndexBuffer = null;


        RenderPass mCurrentPass = RenderPass.ColourPass;

        // Custom rendertargets.
        /// <summary>
        /// The Main Render Target 
        /// </summary>
        public RenderTarget2D RT_MainScene { get; internal set; }

        /// <summary>
        /// Normal Depth Render Target for use in Edge Detection.
        /// </summary>
        public RenderTarget2D RT_NormalDepth { get; internal set; }

        /// <summary>
        /// Water Reflection Render target.
        /// </summary>
        public RenderTarget2D RT_WaterReflectionMap { get; internal set; }


        /// <summary>
        /// The Render Target which holds Post Processed Scene with Edge Detection.
        /// </summary>
        public RenderTarget2D RT_EdgeDetected { get; internal set; }

        /// <summary>
        /// The Render Target which holds Post Processed Scene with Depth Of Field Post Processing.
        /// </summary>
        public RenderTarget2D RT_DepthOfField { get; internal set; }


        //**********************************************//
        //          Defferred Rendering
        //**********************************************//
        /// <summary>
        /// Colour Map Render Target.
        /// </summary>
        public RenderTarget2D RT_ColourMap { get; internal set; }

        /// <summary>
        /// Normal Render Target.
        /// </summary>
        public RenderTarget2D RT_NormalMap { get; internal set; }

        /// <summary>
        /// Depth Render Target.
        /// </summary>
        public RenderTarget2D RT_DepthMap { get; internal set; }

        /// <summary>
        /// Light Map Render Target.
        /// </summary>
        public RenderTarget2D RT_LightMap { get; internal set; }

        /// <summary>
        /// Utility Blurred Scene for use in Post Processing (i.e. Depth of Field, Menu Back ground blurring etc...).
        /// </summary>
        public RenderTarget2D RT_BlurredScene { get; internal set; }

        /// <summary>
        /// SSAO Render Target
        /// </summary>
        public RenderTarget2D RT_SSAO { get; internal set; }

        //**********************************************//
        //            Bloom Render Targets
        //**********************************************//

        /// <summary>
        /// Render target which holds the final Gaussian Blurred Scene
        /// </summary>
        public RenderTarget2D RT_BloomScene { get; internal set; }

        /// <summary>
        /// TempRender target for blurring
        /// </summary>
        public RenderTarget2D RT_BlurTempOne { get; internal set; }

        /// <summary>
        /// TempRender target for blurring
        /// </summary>
        public RenderTarget2D RT_BlurTempTwo { get; internal set; }

        /// <summary>
        /// The rectangle for the blur render target.
        /// </summary>
        public Rectangle Rect_BloomRenderTarget;

        /// <summary>
        /// Intermediate buffer. 
        /// Optionally displays one of the intermediate buffers used 
        /// by the bloom postprocess, so you can see exactly what is
        /// being drawn into each rendertarget.
        /// </summary>
        public enum IntermediateBuffer
        {
            /// <summary>
            /// The pre bloom.
            /// </summary>
            PreBloom,

            /// <summary>
            /// The blurred horizontally.
            /// </summary>
            BlurredHorizontally,

            /// <summary>
            /// The blurred both ways.
            /// </summary>
            BlurredBothWays,

            /// <summary>
            /// The final result.
            /// </summary>
            FinalResult,
        }

        /// <summary>
        /// Gets or sets the show buffer.
        /// </summary>
        /// <value>The show buffer.</value>
        public IntermediateBuffer ShowBuffer
        {
            get { return showBuffer; }
            set { showBuffer = value; }
        }
        IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;



        /// <summary>
        /// Gets or sets the blur settings.
        /// </summary>
        /// <value>The blur settings.</value>
        public vxBloomSettings BloomSettings
        {
            get { return _bloomSettings; }
            set { _bloomSettings = value; }
        }

        //Bloom Settings
        vxBloomSettings _bloomSettings = vxBloomSettings.PresetSettings[5];


        //**********************************************//
        //            Crepuscular Rays Code
        //**********************************************//
        /// <summary>
        /// Render target which holds the mask map for Crepuscular Rays
        /// </summary>
        public RenderTarget2D RT_SunMap { get; internal set; }

        /// <summary>
        /// Render target which holds the mask map for Crepuscular Rays
        /// </summary>
        public RenderTarget2D RT_MaskMap { get; internal set; }

        /// <summary>
		/// Render target which holds the mask map blurred for Crepuscular Rays
        /// </summary>
        public RenderTarget2D RT_GodRaysScene { get; internal set; }

        /// <summary>
		/// This is the Blurred Map Mask Applied to the Main Scene for Crepuscular Rays 
        /// </summary>
        public RenderTarget2D RT_FinalScene { get; internal set; }

        //**********************************************//
        //            Distortion Code
        //**********************************************//

        /// <summary>
        /// The distorted scene Render Target.
        /// </summary>
        public RenderTarget2D RT_DistortionScene { get; internal set; }

        /// <summary>
        /// The distortion map Render Target.
        /// </summary>
        public RenderTarget2D RT_DistortionMap { get; internal set; }

        // Choose what display settings to use.
        NonPhotoRealisticSettings Settings
        {
            get { return NonPhotoRealisticSettings.PresetSettings[settingsIndex]; }
        }
        int settingsIndex = 0;




        //**********************************************//
        //            Shadow Mapping Code
        //**********************************************//

        /// <summary>
        /// Shadow Map Render Target.
        /// </summary>
        public RenderTarget2D RT_ShadowMap { get; internal set; }

        /// <summary>
        /// The block texture for use in shadow map debugging.
        /// </summary>
        public RenderTarget2D RT_BlockTexture { get; internal set; }


        /// <summary>
        /// Gets the number of shadow splits.
        /// </summary>
        /// <value>The number of shadow splits.</value>
        public int NumberOfShadowSplits
        {
            get { return (int) MathHelper.Clamp((int)vxEngine.Profile.Settings.Graphics.ShadowQuality, 1, 4); }
        }
        //const int _numShadowSplits = 2;

        /// <summary>
        /// The m snap shadow maps.
        /// </summary>
        public bool mSnapShadowMaps = true;


        //Shadow Mapping Area's Dimensions
        BoundingBox bbDim;
        const int MaxSupportedPrimitivesPerDraw = 1048575;

        /// <summary>
        /// Gets the shadow view.
        /// </summary>
        /// <value>The shadow view.</value>
        public Matrix ShadowView
        {
            get { return _shadowView; }
        }
        private Matrix _shadowView;

        /// <summary>
        /// Gets the shadow projection.
        /// </summary>
        /// <value>The shadow projection.</value>
        public Matrix ShadowProjection
        {
            get { return _shadowProjection; }
        }
        private Matrix _shadowProjection;

        /// <summary>
        /// The shadow projections.
        /// </summary>
        public Matrix[] ShadowProjections;

        /// <summary>
        /// The shadow split projections.
        /// </summary>
        public Matrix[] ShadowSplitProjections;

        /// <summary>
        /// The shadow split projections with tiling.
        /// </summary>
        public Matrix[] ShadowSplitProjectionsWithTiling;

		/// <summary>
		/// The shadow split tile bounds.
		/// </summary>
		public Vector4[] ShadowSplitTileBounds;

        /// <summary>
        /// The view frustum splits.
        /// </summary>
        public Vector3[][] ViewFrustumSplits;

        /// <summary>
        /// The color of the view frustum.
        /// </summary>
        public Color ViewFrustumColor = new Color(0, 255, 255, 32);

        /// <summary>
        /// The shadow split colors.
        /// </summary>
        public Color[] ShadowSplitColors = new[]
        {
            new Color(255, 0, 0, 255),
            new Color(0, 255, 0, 255),
            new Color(0, 0, 255, 255),
            new Color(160, 32, 240, 255)
        };

        /// <summary>
        /// The shadow depth bias.
        /// </summary>
        public float[,] ShadowDepthBias =
        {
            { 2.5f, 0.000009f },
            { 2.5f, 0.00009f },
            { 2.5f, 0.0009f },
            { 2.5f, 0.0009f }
        };


        /// <summary>
        /// Gets the poisson kernel.
        /// </summary>
        /// <value>An array of Two Dimensional Vectors (Vector2's) that define the poisson kernel.</value>
        /// <example>
        /// It returns the following values.
        /// <code>
        ///     public static IEnumerable<Vector2> poissonKernel()
        ///         {
        ///             return new[]
        ///             {
        ///             new Vector2(-0.326212f, -0.405810f),
        ///             new Vector2(-0.840144f, -0.073580f),
        ///             new Vector2(-0.695914f,  0.457137f),
        ///             new Vector2(-0.203345f,  0.620716f),
        ///             new Vector2( 0.962340f, -0.194983f), 
        ///             new Vector2( 0.473434f, -0.480026f),
        ///             new Vector2( 0.519456f,  0.767022f), 
        ///             new Vector2( 0.185461f, -0.893124f),
        ///             new Vector2( 0.507431f,  0.064425f), 
        ///             new Vector2( 0.896420f,  0.412458f),
        ///             new Vector2(-0.321940f, -0.932615f),
        ///             new Vector2(-0.791559f, -0.597710f)
        ///             };
        ///     }
        /// </code></example>
        public Vector2[] poissonKernel
        {
            get
            {
                return vxGeometryHelper.poissonKernel()
                    .Select(v => v / (float)ShadowMapSize)
                    .OrderBy(v => v.Length())
                    .ToArray();
            }
        }

        /// <summary>
        /// Shadow Map Render Target Size.
        /// </summary>
        /// <value>A higher value will give sharper looking shaows. 
        /// The Default Value is 512, but it can go up too 2096.</value>
        public int ShadowMapSize
        {
            get { return _shadowMapSize; }
            set { _shadowMapSize = value; }
        }
        private int _shadowMapSize = 512;

        float size = 750;



        /// <summary>
        /// Gets the random texture3 d.
        /// </summary>
        /// <value>The random texture3 d.</value>
        public Texture3D RandomTexture3D
        {
            get { return _randomTexture3D; }
        }
        Texture3D _randomTexture3D;


        /// <summary>
        /// Gets the random texture2 d.
        /// </summary>
        /// <value>The random texture2 d.</value>
        public Texture2D RandomTexture2D
        {
            get { return _randomTexture2D; }
        }
        Texture2D _randomTexture2D;

        vxEngine vxEngine;

        //TODO: Add More Input parameters (i.e. Size, Shadow Map Size, etc...).
        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Graphics.vxRenderer"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public vxRenderer(vxEngine vxEngine)
        {
			vxConsole.WriteLine ("Starting 3D Rendering Engine");
            this.vxEngine = vxEngine;
            bbDim = new BoundingBox(new Vector3(-size, -size, -size), new Vector3(size, size, size));
			this.LoadContent ();
        }

        public vxRenderer(vxEngine vxEngine, BoundingBox ShadowMapBB)
		{
			vxConsole.WriteLine ("Starting 3D Rendering Engine");
            this.vxEngine = vxEngine;
            bbDim = ShadowMapBB;
			this.LoadContent ();
        }

        /// <summary>
        /// Sets the light position.
        /// </summary>
        /// <param name="lightPosition">Light position.</param>
        public void setLightPosition(Vector3 lightPosition)
        {
            mLightPosition = lightPosition;

            var look = Vector3.Normalize(-mLightPosition);
            var up = Vector3.Cross(look, Vector3.Right);

            // Remember: XNA uses a right handed coordinate system, i.e. -Z goes into the screen
            _shadowView = Matrix.Invert(
                new Matrix(
                    1, 0, 0, 0,
                    0, 0, -1, 0,
                    -look.X, -look.Y, -look.Z, 0,
                    mLightPosition.X, mLightPosition.Y, mLightPosition.Z, 1
                )
            );

            // bounding box
            {
                var bb = vxGeometryHelper.transformBoundingBox(bbDim, _shadowView);
                _shadowProjection = Matrix.CreateOrthographicOffCenter(bb.Min.X, bb.Max.X, bb.Min.Y, bb.Max.Y, -bb.Max.Z, -bb.Min.Z);
            }
        }


        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="graphicsManager">Graphics manager.</param>
        public void LoadContent()
        {
			mGraphicsDevice = vxEngine.GraphicsDevice;// graphicsManager.GraphicsDevice;

            mInstanceVertexDeclaration = new VertexDeclaration(new[]
            {
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5)
            });

            quadRendererVerticesBuffer = new VertexPositionTexture[]
            {
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,0)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,0))
            };

            quadRendererIndexBuffer = new short[] { 0, 1, 2, 2, 3, 0 };

            InitialiseRenderTargetsAll();
        }

        public void InitialiseRenderTargetsAll()
        {
            // Create two custom rendertargets.
            mGraphicsDevice = vxEngine.GraphicsDevice;
            PresentationParameters pp = mGraphicsDevice.PresentationParameters;
            
            InitialiseRenderTargetsForMain(pp);
            InitialiseRenderTargetsForBloom(pp);
            InitialiseRenderTargetsForDistortion(pp);
            InitialiseRenderTargetsForWaterReflection(pp);

            mAlphaBlendState = new BlendState();
            mAlphaBlendState.ColorSourceBlend = Blend.SourceAlpha;
            mAlphaBlendState.AlphaSourceBlend = Blend.SourceAlpha;
            mAlphaBlendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
            mAlphaBlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;

            InitialiseRenderTargetsForShadowMaps();
        }

        public void InitialiseRenderTargetsForMain(PresentationParameters pp)
        {
            RT_MainScene = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_EdgeDetected = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_DepthOfField = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_ColourMap = new RenderTarget2D(mGraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_NormalMap = new RenderTarget2D(mGraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, false,
                SurfaceFormat.Color, DepthFormat.None);

            RT_DepthMap = new RenderTarget2D(mGraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, false,
                SurfaceFormat.Single, DepthFormat.None);

            RT_LightMap = new RenderTarget2D(mGraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_BlurredScene = new RenderTarget2D(mGraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_SSAO = new RenderTarget2D(mGraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_SunMap = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_MaskMap = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_FinalScene = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_GodRaysScene = new RenderTarget2D(mGraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            RT_NormalDepth = new RenderTarget2D(mGraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);
        }

        public void InitialiseRenderTargetsForWaterReflection(PresentationParameters pp)
        {
            //TODO: Tie into Graphics Settings
            int res = 4;
            RT_WaterReflectionMap = new RenderTarget2D(mGraphicsDevice,
                                                         pp.BackBufferWidth / res, pp.BackBufferHeight / res, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);
        }

        public void InitialiseRenderTargetsForDistortion(PresentationParameters pp)
        {
            //
            //Distortion
            //
            // create textures for reading back the backbuffer contents
            RT_DistortionScene = new RenderTarget2D(mGraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            RT_DistortionMap = new RenderTarget2D(mGraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            // set the blur parameters for the current viewport
            SetBlurEffectParameters(1f / (float)pp.BackBufferWidth, 1f / (float)pp.BackBufferHeight);
        }

        public void InitialiseRenderTargetsForBloom(PresentationParameters pp)
        {
            //
            //Blur Render Targets
            //
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            // Create a texture for rendering the main scene, prior to applying bloom.
            RT_BloomScene = new RenderTarget2D(mGraphicsDevice, width, height, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                                                   RenderTargetUsage.DiscardContents);

            // Create two rendertargets for the bloom processing. These are half the
            // size of the backbuffer, in order to minimize fillrate costs. Reducing
            // the resolution in this way doesn't hurt quality, because we are going
            // to be blurring the bloom images in any case.
            width /= (5 - (int)vxEngine.Profile.Settings.Graphics.Bloom);
            height /= (5 - (int)vxEngine.Profile.Settings.Graphics.Bloom);
            
            RT_BlurTempOne = new RenderTarget2D(mGraphicsDevice, width, height, false,
                pp.BackBufferFormat, DepthFormat.None);
            RT_BlurTempTwo = new RenderTarget2D(mGraphicsDevice, width, height, false,
                pp.BackBufferFormat, DepthFormat.None);

            Rect_BloomRenderTarget = new Rectangle(0, 0, RT_BlurTempOne.Width, RT_BlurTempOne.Height);
        }
        

#region Instancing Code

        //************************************************************************************//
        //                          MESH INSTANCING METHODS
        //************************************************************************************//

        /// <summary>
        /// Sets the instancing data.
        /// </summary>
        /// <param name="model2worldTransformations">Model2world transformations.</param>
        /// <param name="numInstances">Number instances.</param>
        public void SetInstancingData(Matrix[] model2worldTransformations, int numInstances)
        {
            if (numInstances > 0)
            {
                // Make sure our instance data vertex buffer is big enough. (4x4 float matrix)
                int instanceDataSize = 16 * sizeof(float) * numInstances;

                if ((mInstanceDataStream == null) ||
                    (mInstanceDataStream.VertexCount < numInstances))
                {
                    if (mInstanceDataStream != null)
                        mInstanceDataStream.Dispose();

                    mInstanceDataStream = new DynamicVertexBuffer(mGraphicsDevice, mInstanceVertexDeclaration, numInstances, BufferUsage.WriteOnly);
                }

                // Upload transform matrices to the instance data vertex buffer.
                mInstanceDataStream.SetData(model2worldTransformations, 0, numInstances, SetDataOptions.Discard);
            }
        }

        /// <summary>
        /// Renders the instanced shadow.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="camera">Camera.</param>
        /// <param name="numInstances">Number instances.</param>
        public virtual void RenderInstancedShadow(Model model, vxCamera3D camera, int numInstances)
        {
            //Only XNA Supports Instance Drawing Currently
#if VRTC_PLTFRM_XNA
            for (int i = 0; i < NumberOfShadowSplits; ++i)
            {
                {
                    int x = i % 2;
                    int y = i / 2;
                    var viewPort = new Viewport(x * ShadowMapSize, y * ShadowMapSize, ShadowMapSize, ShadowMapSize);

                    mGraphicsDevice.Viewport = viewPort;
                }

                // now render the spheres
                if (numInstances > 0)
                {
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (var part in mesh.MeshParts)
                        {
                            part.Effect.CurrentTechnique = part.Effect.Techniques["ShadowInstanced"];
                            part.Effect.Parameters["ViewProjection_Sdw"].SetValue(ShadowSplitProjections[i]);
                            part.Effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
                            part.Effect.Parameters["DepthBias_Sdw"].SetValue(new Vector2(ShadowDepthBias[i, 0], ShadowDepthBias[i, 1]));
                            part.Effect.CurrentTechnique.Passes[0].Apply();

                            // set vertex buffer
                            mGraphicsDevice.SetVertexBuffers(new[]
                            {
                                part.VertexBuffer,
                                new VertexBufferBinding(mInstanceDataStream, 0, 1 )
                            });

                            // set index buffer and draw
                            mGraphicsDevice.Indices = part.IndexBuffer;
                            mGraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount, numInstances);
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
		/// Instanced rendering. Draws instancedModel numInstances times. This demo uses hardware
		/// instancing: a secondary vertex stream is created, where the transform matrices of the
		/// individual instances are passed down to the shader. Note that in order to be efficient,
		/// the model should contain as little meshes and meshparts as possible.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="camera">Camera.</param>
        /// <param name="numInstances">Number instances.</param>
        /// <param name="technique">Technique.</param>
        public virtual void RenderInstanced(Model model, vxCamera3D camera, int numInstances, string technique)
        {
            //Only XNA Supports Instance Drawing Currently
#if VRTC_PLTFRM_XNA
            if (numInstances > 0)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // loop through meshes
                foreach (ModelMesh mesh in model.Meshes)
                {

                    foreach (var shadowedEffect in mesh.Effects.Where(e => e.Parameters.Any(p => p.Name == "ShadowMap")))
                    {
                        shadowedEffect.Parameters["ShadowMap"].SetValue(RT_ShadowMap);
                        shadowedEffect.Parameters["ShadowTransform"].SetValue(ShadowSplitProjectionsWithTiling);
                        shadowedEffect.Parameters["TileBounds"].SetValue(ShadowSplitTileBounds);
                    }

                    // get bone matrix
                    Matrix boneMatrix = transforms[mesh.ParentBone.Index];
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect.CurrentTechnique = part.Effect.Techniques[technique];
                        part.Effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
                        part.Effect.Parameters["View"].SetValue(camera.View);
                        part.Effect.Parameters["Projection"].SetValue(camera.Projection);
                        part.Effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(mLightPosition));

                        part.Effect.Parameters["ShadowMap"].SetValue(RT_ShadowMap);
                        part.Effect.Parameters["ShadowTransform"].SetValue(ShadowSplitProjectionsWithTiling);
                        part.Effect.Parameters["TileBounds"].SetValue(ShadowSplitTileBounds);

                        part.Effect.CurrentTechnique.Passes[0].Apply();

                        // set vertex buffer
                        mGraphicsDevice.SetVertexBuffers(new[]
                        {
                            part.VertexBuffer,
                            new VertexBufferBinding(mInstanceDataStream, 0, 1 )
                        });

                        // draw primitives 
                        mGraphicsDevice.Indices = part.IndexBuffer;
                        mGraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount, numInstances);
                    }
                }
            }
#endif
        }

#endregion

#region Post Processing Methods


        public void CreateBluredScreen(vxEngine vxEngine)
        {
            //Set First Temp Blur Rendertarget 
            vxEngine.GraphicsDevice.SetRenderTarget(RT_BlurTempOne);

            //Now Draw the the Final Scene too the res-downed TempBlur Target.
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(RT_FinalScene,
                new Rectangle(0, 0, RT_BlurTempOne.Width, RT_BlurTempOne.Height),
                Color.White);
            vxEngine.SpriteBatch.End();

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            BloomComponent.SetBloomEffectParameters(vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                1.0f / (float)RT_BlurTempOne.Width, 0, BloomSettings.BlurAmount);

            DrawRenderTargetIntoOther(vxEngine, RT_BlurTempOne, RT_BlurTempTwo,
                               vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                               IntermediateBuffer.BlurredHorizontally);

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            BloomComponent.SetBloomEffectParameters(vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                0, 1.0f / (float)RT_BlurTempOne.Height, BloomSettings.BlurAmount);

            DrawRenderTargetIntoOther(vxEngine, RT_BlurTempTwo, RT_BlurTempOne,
                               vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                               IntermediateBuffer.BlurredBothWays);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.

            vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BloomIntensity"].SetValue(BloomSettings.BloomIntensity);
            vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BaseIntensity"].SetValue(BloomSettings.BaseIntensity);
            vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BloomSaturation"].SetValue(BloomSettings.BloomSaturation);
            vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BaseSaturation"].SetValue(BloomSettings.BaseSaturation);

            vxEngine.GraphicsDevice.SetRenderTarget(RT_BlurredScene);

            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(RT_BlurTempOne, vxEngine.GraphicsDevice.Viewport.Bounds, Color.White);
            vxEngine.SpriteBatch.End();
        }

        /// <summary>
        /// Helper applies the edge detection and pencil sketch postprocess effect.
        /// </summary>
        public void CreateSSAO(vxEngine vxEngine)
        {
            //Set Render Target
            vxEngine.GraphicsDevice.SetRenderTarget(RT_SSAO);

            //Clear
            vxEngine.GraphicsDevice.Clear(Color.Black);
            //vxEngine.GraphicsDevice.BlendState = BlendState.Opaque;
            //vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            EffectParameterCollection parameters = vxEngine.Assets.PostProcessShaders.SSAOEffect.Parameters;
            
            //Calculate Frustum Corner of the Camera
            Vector3 cornerFrustum = Vector3.Zero;
            cornerFrustum.Y = (float)Math.Tan(Math.PI / 3.0 / 2.0) * vxEngine.Current3DSceneBase.Camera.FarPlane;
            cornerFrustum.X = cornerFrustum.Y * vxEngine.Current3DSceneBase.Camera.AspectRatio;
            cornerFrustum.Z = vxEngine.Current3DSceneBase.Camera.FarPlane;


            //Set SSAO parameters

            //parameters["depthTexture"].SetValue(RT_DepthMap);

            //sAVE sATATES
            SamplerState s1 = new SamplerState();
            s1 = vxEngine.GraphicsDevice.SamplerStates[1];

            vxEngine.GraphicsDevice.Textures[1] = RT_DepthMap;
            vxEngine.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            
         float _randomTile = 100;
         float _radius = 0.05f;
         float _maxRadius = 0.5f;
         float _bias = 0.00001f;
         int _blurCount = 1;
         float _intensity = 1.75f;

        parameters["NormalBuffer"].SetValue(RT_NormalMap);
            parameters["DepthBuffer"].SetValue(RT_DepthMap);
            parameters["RandomMap"].SetValue(RT_NormalMap);

            //parameters["Projection"].SetValue(vxEngine.Current3DSceneBase.Camera.Projection);
            parameters["Radius"].SetValue(new Vector2(_radius, _maxRadius));
            parameters["RandomTile"].SetValue(_randomTile);
            parameters["Bias"].SetValue(_bias);
            parameters["FarClip"].SetValue(vxEngine.Current3DSceneBase.Camera.FarPlane);

            Matrix orthoproj = Matrix.CreateOrthographicOffCenter(0,
           vxEngine.GraphicsDevice.Viewport.Width,
           vxEngine.GraphicsDevice.Viewport.Height,
           0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            parameters["MatrixTransform"].SetValue(halfPixelOffset * orthoproj);

            // Activate the appropriate effect technique.
            vxEngine.Assets.PostProcessShaders.SSAOEffect.CurrentTechnique = vxEngine.Assets.PostProcessShaders.SSAOEffect.Techniques["Technique1"];
            //vxEngine.Assets.PostProcessShaders.SSAOEffect.CurrentTechnique = vxEngine.Assets.PostProcessShaders.SSAOEffect.Techniques["SSAO"];

            vxEngine.Assets.PostProcessShaders.SSAOEffect.CurrentTechnique.Passes[0].Apply();
            
            vxEngine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, vxEngine.Assets.PostProcessShaders.SSAOEffect);
            vxEngine.SpriteBatch.Draw(RT_FinalScene, Vector2.Zero, Color.White);
            vxEngine.SpriteBatch.End();
            
            //Apply
            //vxEngine.Assets.PostProcessShaders.SSAOEffect.CurrentTechnique.Passes[0].Apply();
            //vxEngine.Renderer.RenderQuad(Vector2.One * -1, Vector2.One);

            if (s1 != null)
            vxEngine.GraphicsDevice.SamplerStates[1] = s1;

        }

        /// <summary>
        /// Helper applies the edge detection and pencil sketch postprocess effect.
        /// </summary>
        public void ApplyEdgeDetect(vxEngine vxEngine)
        {
            //Set Render Target
            mGraphicsDevice.SetRenderTarget(RT_EdgeDetected);

            if (vxEngine.Profile.Settings.Graphics.Bool_DoEdgeDetection)
            {
                EffectParameterCollection parameters = vxEngine.Assets.PostProcessShaders.CartoonEdgeDetection.Parameters;

                Vector2 resolution = new Vector2(RT_MainScene.Width,
                                                 RT_MainScene.Height);

                // Pass in the current screen resolution.
                parameters["ScreenResolution"].SetValue(resolution);
                parameters["NormalTexture"].SetValue(RT_NormalMap);
                parameters["DepthTexture"].SetValue(RT_DepthMap);


                // Settings controlling the edge detection filter.
                parameters["EdgeWidth"].SetValue(Settings.EdgeWidth);
                parameters["EdgeIntensity"].SetValue(Settings.EdgeIntensity);

                // How sensitive should the edge detection be to tiny variations in the input data?
                // Smaller settings will make it pick up more subtle edges, while larger values get
                // rid of unwanted noise.
                parameters["NormalThreshold"].SetValue(0.5f);
                parameters["DepthThreshold"].SetValue(0.001f);

                // How dark should the edges get in response to changes in the input data?
                parameters["NormalSensitivity"].SetValue(1.0f);
                parameters["DepthSensitivity"].SetValue(10000.0f);

                Matrix orthoproj = Matrix.CreateOrthographicOffCenter(0,
                           vxEngine.GraphicsDevice.Viewport.Width,
                           vxEngine.GraphicsDevice.Viewport.Height,
                           0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                parameters["MatrixTransform"].SetValue(halfPixelOffset * orthoproj);



                // Activate the appropriate effect technique.
                vxEngine.Assets.PostProcessShaders.CartoonEdgeDetection.CurrentTechnique = vxEngine.Assets.PostProcessShaders.CartoonEdgeDetection.Techniques["EdgeDetect"];


                // Draw a fullscreen sprite to apply the postprocessing effect.
                vxEngine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, vxEngine.Assets.PostProcessShaders.CartoonEdgeDetection);
                vxEngine.SpriteBatch.Draw(RT_FinalScene, Vector2.Zero, Color.White);
                vxEngine.SpriteBatch.End();
            }
            else
            {
                //If the user elects to not use the effect, simply draw the previous scene into the current 
                //active render target.
                vxEngine.SpriteBatch.Begin();
                vxEngine.SpriteBatch.Draw(RT_FinalScene, Vector2.Zero, Color.White);
                vxEngine.SpriteBatch.End();
            }
        }

        public float DepthOfFieldAlpha = 1;
        public void ApplyDepthOfField()
        {
            //Set Render Target
            mGraphicsDevice.SetRenderTarget(RT_DepthOfField);

            if (vxEngine.Profile.Settings.Graphics.DepthOfField != vxEnumQuality.None)
            {
                EffectParameterCollection parameters = vxEngine.Assets.PostProcessShaders.DepthOfFieldEffect.Parameters;

                parameters["SceneTexture"].SetValue(RT_EdgeDetected);
                parameters["DepthTexture"].SetValue(RT_DepthMap);
                parameters["BlurTexture"].SetValue(RT_BlurredScene);

                Matrix orthoproj = Matrix.CreateOrthographicOffCenter(0,
                           vxEngine.GraphicsDevice.Viewport.Width,
                           vxEngine.GraphicsDevice.Viewport.Height,
                           0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                parameters["MatrixTransform"].SetValue(halfPixelOffset * orthoproj);
#if VIRTICES_3D
                parameters["FarClip"].SetValue(vxEngine.Current3DSceneBase.Camera.FarPlane);
                parameters["FocalDistance"].SetValue(vxEngine.Current3DSceneBase.Camera.FocalDistance);
                parameters["FocalWidth"].SetValue(vxEngine.Current3DSceneBase.Camera.FocalWidth);
#endif

                // Draw a fullscreen sprite to apply the postprocessing effect.
                vxEngine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, vxEngine.Assets.PostProcessShaders.DepthOfFieldEffect);
                vxEngine.SpriteBatch.Draw(RT_EdgeDetected, Vector2.Zero, Color.White);
                vxEngine.SpriteBatch.End();
            }
            else
            {
                //If the user elects to not use the effect, simply draw the previous scene into the current 
                //active render target.
                vxEngine.SpriteBatch.Begin();
                vxEngine.SpriteBatch.Draw(RT_EdgeDetected, Vector2.Zero, Color.White);
                vxEngine.SpriteBatch.End();
            }
        }

#endregion

#region Guassian Blur Code

        /// <summary>
        /// Applies the guassian bloom.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public void ApplyGuassianBloom(vxEngine vxEngine)
        {
            //Set Render Target
            if (vxEngine.Profile.Settings.Graphics.Bloom != vxEnumQuality.None)
            {
                // Pass 1: draw the main scene (without edge detection) into rendertarget 1, using a
                // shader that extracts only the brightest parts of the image.
                vxEngine.Assets.PostProcessShaders.BloomExtractEffect.Parameters["BloomThreshold"].SetValue(BloomSettings.BloomThreshold);

                DrawRenderTargetIntoOther(vxEngine, RT_FinalScene, RT_BlurTempOne,
                                   vxEngine.Assets.PostProcessShaders.BloomExtractEffect,
                                   IntermediateBuffer.PreBloom);

                // Pass 2: draw from rendertarget 1 into rendertarget 2,
                // using a shader to apply a horizontal gaussian blur filter.
                BloomComponent.SetBloomEffectParameters(vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                    1.0f / (float)RT_BlurTempOne.Width, 0, BloomSettings.BlurAmount);

                DrawRenderTargetIntoOther(vxEngine, RT_BlurTempOne, RT_BlurTempTwo,
                                   vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                                   IntermediateBuffer.BlurredHorizontally);

                // Pass 3: draw from rendertarget 2 back into rendertarget 1,
                // using a shader to apply a vertical gaussian blur filter.
                BloomComponent.SetBloomEffectParameters(vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                    0, 1.0f / (float)RT_BlurTempOne.Height, BloomSettings.BlurAmount);

                DrawRenderTargetIntoOther(vxEngine, RT_BlurTempTwo, RT_BlurTempOne,
                                   vxEngine.Assets.PostProcessShaders.GaussianBlurEffect,
                                   IntermediateBuffer.BlurredBothWays);

                // Pass 4: draw both rendertarget 1 and the original scene
                // image back into the main backbuffer, using a shader that
                // combines them to produce the final bloomed result.

                vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BloomIntensity"].SetValue(BloomSettings.BloomIntensity);
                vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BaseIntensity"].SetValue(BloomSettings.BaseIntensity);
                vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BloomSaturation"].SetValue(BloomSettings.BloomSaturation);
                vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BaseSaturation"].SetValue(BloomSettings.BaseSaturation);

                //No Apply the bloom to the current post-processed scene.
                //vxEngine.GraphicsDevice.Textures[1] = RT_DepthOfField;
                vxEngine.Assets.PostProcessShaders.BloomCombineEffect.Parameters["BaseTexture"].SetValue(RT_DepthOfField);

                vxEngine.GraphicsDevice.SetRenderTarget(RT_FinalScene);
                DrawFullscreenQuad(RT_BlurTempOne,
                                   vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height,
                                   vxEngine.Assets.PostProcessShaders.BloomCombineEffect);
            }
            else
            {
                vxEngine.GraphicsDevice.SetRenderTarget(RT_FinalScene);
                vxEngine.SpriteBatch.Begin();
                vxEngine.SpriteBatch.Draw(RT_DepthOfField, Vector2.Zero, Color.White);
                vxEngine.SpriteBatch.End();
            }
        }

        /// <summary>
        /// Applies the crepuscular rays.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public void ApplyCrepuscularRays(vxEngine vxEngine)
        {

#if VIRTICES_3D
            Vector2 lighScreenSourcePos = new Vector2(
                vxEngine.Current3DSceneBase.SunEmitter.screenPos.X / vxEngine.GraphicsDevice.Viewport.Width,
                vxEngine.Current3DSceneBase.SunEmitter.screenPos.Y / vxEngine.GraphicsDevice.Viewport.Height);

            Vector2 HalfPixel = -new Vector2(.5f / (float)vxEngine.GraphicsDevice.Viewport.Width,
                                        .5f / (float)vxEngine.GraphicsDevice.Viewport.Height);


            if (vxEngine.Profile.Settings.Graphics.GodRays != vxEnumQuality.None && lighScreenSourcePos.X > 0 && lighScreenSourcePos.Y > 0 &&
                lighScreenSourcePos.X < vxEngine.GraphicsDevice.Viewport.Width && lighScreenSourcePos.Y < vxEngine.GraphicsDevice.Viewport.Height
            && vxEngine.Current3DSceneBase.SunEmitter.IsOnScreen)
            {

                float Density = .55f;
                float Decay = .9f;
                float Weight = 1.0f;
                float Exposure = .15f;

                Matrix orthoproj = Matrix.CreateOrthographicOffCenter(0,
                           vxEngine.GraphicsDevice.Viewport.Width,
                           vxEngine.GraphicsDevice.Viewport.Height,
                           0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);



                vxEngine.Assets.PostProcessShaders.LightRaysEffect.CurrentTechnique = vxEngine.Assets.PostProcessShaders.LightRaysEffect.Techniques["LightRayFX"];

                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * orthoproj);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["halfPixel"].SetValue(HalfPixel);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["Density"].SetValue(Density);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["Decay"].SetValue(Decay);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["Weight"].SetValue(Weight);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["Exposure"].SetValue(Exposure);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["Texture"].SetValue(RT_MaskMap);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["lightScreenPosition"].SetValue(lighScreenSourcePos);
                //vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["DepthMap"].SetValue(vxEngine.Renderer.RT_DepthMap);

                vxEngine.GraphicsDevice.SetRenderTarget(RT_GodRaysScene);

                DrawFullscreenQuad(RT_MaskMap,
                                   vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height,
                                   vxEngine.Assets.PostProcessShaders.LightRaysEffect);

                vxEngine.GraphicsDevice.SetRenderTarget(null);
                vxEngine.Assets.PostProcessShaders.GodRaysCombineEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * orthoproj);

                vxEngine.Assets.PostProcessShaders.GodRaysCombineEffect.Parameters["TextureSampler"].SetValue(RT_FinalScene);
                vxEngine.Assets.PostProcessShaders.LightRaysEffect.Parameters["Weight"].SetValue(Weight);

                DrawFullscreenQuad(RT_GodRaysScene,
                                   vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height,
                                    vxEngine.Assets.PostProcessShaders.GodRaysCombineEffect);

            }

            else
            {
                vxEngine.GraphicsDevice.SetRenderTarget(null);

                DrawFullscreenQuad(RT_FinalScene,
                                   vxEngine.GraphicsDevice.Viewport.Width, vxEngine.GraphicsDevice.Viewport.Height);
            }


#endif

        }

#endregion

#region Drawing Rendertarget Code

        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawRenderTargetIntoOther(vxEngine vxEngine, Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            mGraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect);
        }


        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="effect"></param>
        public void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect)
        {
            vxEngine.SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            vxEngine.SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            vxEngine.SpriteBatch.End();
        }

        /// <summary>
        /// Draws With No Effect Used
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void DrawFullscreenQuad(Texture2D texture, int width, int height)
        {
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            vxEngine.SpriteBatch.End();
        }



        /// <summary>
        /// Render the specified v1 and v2.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public void RenderQuad(Vector2 v1, Vector2 v2)
        {
            quadRendererVerticesBuffer[0].Position.X = v2.X;
            quadRendererVerticesBuffer[0].Position.Y = v1.Y;

            quadRendererVerticesBuffer[1].Position.X = v1.X;
            quadRendererVerticesBuffer[1].Position.Y = v1.Y;

            quadRendererVerticesBuffer[2].Position.X = v1.X;
            quadRendererVerticesBuffer[2].Position.Y = v2.Y;

            quadRendererVerticesBuffer[3].Position.X = v2.X;
            quadRendererVerticesBuffer[3].Position.Y = v2.Y;

            vxEngine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
            (PrimitiveType.TriangleList, quadRendererVerticesBuffer, 0, 4, quadRendererIndexBuffer, 0, 2);
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        /// <remarks>
        /// This function was originally provided in the BloomComponent class in the 
        /// Bloom Postprocess sample.
        /// </remarks>
        public void SetBlurEffectParameters(float dx, float dy)
        {
#if VRTC_PLTFRM_XNA
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = vxEngine.Assets.PostProcessShaders.distortEffect.Parameters["SampleWeights"];
            offsetsParameter = vxEngine.Assets.PostProcessShaders.distortEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
#endif
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        /// <remarks>
        /// This function was originally provided in the BloomComponent class in the 
        /// Bloom Postprocess sample.
        /// </remarks>
        float ComputeGaussian(float n)
        {
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * BlurAmount)) *
                           Math.Exp(-(n * n) / (2 * BlurAmount * BlurAmount)));
        }

#endregion
    }
}

#endif