
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Graphics;

namespace Virtex.Lib.Vrtc.Core.ContentManagement
{
    /// <summary>
    /// Class for holding all vxEngine Assets
    /// </summary>
    public class Assets
    {
        vxEngine Engine;

        public struct AssetFonts
        {
            public SpriteFont MenuTitleFont { get; set; }
            public SpriteFont MenuFont { get; set; }
            public SpriteFont DebugFont { get; set; }
        };


        /// <summary>
        /// Fonts For use within the vxEngine.
        /// </summary>
        public AssetFonts Fonts { 
            get {return fonts;}
            set { fonts = value; }
        }
        private AssetFonts fonts = new AssetFonts();



        public struct AssetTextures
        {
            public Texture2D Blank;
            public Texture2D Gradient;
			public Texture2D RandomValues;
            public Texture2D Arrow_Left, Arrow_Right;

            public Texture2D Texture_WaterEnvr;
            public Texture2D Texture_WaterWaves;
            public Texture2D Texture_WaterDistort;

            public Texture2D Texture_Cube_Null;

            public Texture2D Texture_Diffuse_Null;
            public Texture2D Texture_NormalMap_Null;
            public Texture2D Texture_SpecularMap_Null;

            public Texture2D Texture_Sun_Glow;
        };

        /// <summary>
        /// vxEngine Base Textures
        /// </summary>
        public AssetTextures Textures
        {
            get { return textures; }
            set { textures = value; }
        }
        private AssetTextures textures = new AssetTextures();



#if !VRTC_PLTFRM_DROID

        public struct AssetModels
        {
            public vxModel UnitArrow { get; set; }
            public vxModel UnitBox { get; set; }
            public vxModel UnitSphere { get; set; }
            public vxModel UnitPlane { get; set; }
            public vxModel WaterPlane { get; set; }

            public vxModel Sun_Mask { get; set; }
        };

        /// <summary>
        /// vxEngine Base Models
        /// </summary>
        public AssetModels Models
        {
            get { return models; }
            set { models = value; }
        }
        private AssetModels models = new AssetModels();




        public struct AssetShaders
        {
            public Effect MainShader { get; set; }
            public Effect CascadeShadowShader { get; set; }
            public Effect UtilityShader { get; set; }
            public Effect CartoonShader { get; set; }
            public Effect DistortionShader { get; set; }
            public Effect WaterReflectionShader { get; set; }

			//Deffered Rendering
			public Effect DrfrdRndrClearGBuffer { get; set; }
			public Effect DrfrdRndrCombineFinal { get; set; }
			public Effect DrfrdRndrDirectionalLight { get; set; }
			public Effect DrfrdRndrPointLight { get; set; }
        };

        /// <summary>
        /// Model Shaders.
        /// </summary>
        public AssetShaders Shaders
        {
            get { return shaders; }
            set { shaders = value; }
        }
        private AssetShaders shaders = new AssetShaders();







        public struct AssetPostProcessShaders
        {
            public Effect CartoonEdgeDetection { get; set; }
            public Effect BloomExtractEffect { get; set; }
            public Effect BloomCombineEffect { get; set; }
            public Effect GaussianBlurEffect { get; set; }
            public Effect DepthOfFieldEffect { get; set; }

            //God Rays
            public Effect SSAOEffect { get; set; }
            public Effect MaskedSunEffect { get; set; }
            public Effect GodRaysCombineEffect { get; set; }
            public Effect LightRaysEffect { get; set; }

            public Effect distortEffect { get; set; }
            public EffectTechnique distortTechnique { get; set; }
            public EffectTechnique distortBlurTechnique { get; set; }
        };

        /// <summary>
        /// Postprocessing Shaders.
        /// </summary>
        public AssetPostProcessShaders PostProcessShaders
        {
            get { return postProcessShaders; }
            set { postProcessShaders = value; }
        }
        private AssetPostProcessShaders postProcessShaders = new AssetPostProcessShaders();
#endif




        /// <summary>
        /// Assets For the vxEngine
        /// </summary>
        public Assets(vxEngine engine)
        {
            //Game vxEngine
            Engine = engine;

			/********************************************************************************************/
			/*										Fonts												*/
			/********************************************************************************************/
            fonts.MenuTitleFont = Engine.EngineContentManager.Load<SpriteFont>("Fonts/font_gui_title");
            fonts.MenuFont = Engine.EngineContentManager.Load<SpriteFont>("Fonts/font_gui");
            fonts.DebugFont = Engine.EngineContentManager.Load<SpriteFont>("Fonts/font_debug");



			/********************************************************************************************/
			/*										Textures											*/
			/********************************************************************************************/

            textures.Blank = Engine.EngineContentManager.Load<Texture2D>("Textures/xGUI/blank");
            textures.Gradient = Engine.EngineContentManager.Load<Texture2D>("Textures/xGUI/gradient");
			textures.RandomValues = Engine.EngineContentManager.Load<Texture2D>("Textures/rndm");

            //Arrows
            textures.Arrow_Left = Engine.EngineContentManager.Load<Texture2D>("Textures/Menu/Slider/Arrow_Left");
            textures.Arrow_Right = Engine.EngineContentManager.Load<Texture2D>("Textures/Menu/Slider/Arrow_Right");

            textures.Texture_Diffuse_Null = Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_diffuse");
            textures.Texture_NormalMap_Null = Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_normal");
            textures.Texture_SpecularMap_Null = Engine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_specular");

            //textures.Texture_Cube_Null = vxEngine.EngineContentManager.Load<Texture2D>("Textures/null_cube");

            //Glow
            textures.Texture_Sun_Glow = Engine.EngineContentManager.Load<Texture2D>("Textures/Rays/Rays");




#if !VRTC_PLTFRM_DROID
            //Water
            textures.Texture_WaterWaves = Engine.EngineContentManager.Load<Texture2D>("Shaders/Water/waterbump");
            textures.Texture_WaterDistort = Engine.EngineContentManager.Load<Texture2D>("Shaders/Water/waterdistort");


            /********************************************************************************************/
            /*										Shaders												*/
			/********************************************************************************************/

			string prefixtag = "";

            //Model Shaders
#if VRTC_PLTFRM_XNA
            
#else
			prefixtag = "MonoGame/";
#endif

            shaders.MainShader = Engine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Model Shaders/MainModelShader");
            shaders.CascadeShadowShader  = Engine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Shadows/CascadeShadowShader");
            shaders.UtilityShader = Engine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Utility/UtilityShader");

            //Shader Collection
            shaders.CartoonShader = Engine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Model Shaders/CellModelShader");

            //Water Shader
            shaders.WaterReflectionShader = Engine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Water/vxWater");

            //Bloom
            postProcessShaders.BloomExtractEffect = Engine.EngineContentManager.Load<Effect>("Shaders/Bloom/BloomExtract");
			postProcessShaders.BloomCombineEffect = Engine.EngineContentManager.Load<Effect>("Shaders/Bloom/BloomCombine");
			postProcessShaders.GaussianBlurEffect = Engine.EngineContentManager.Load<Effect>("Shaders/Bloom/GaussianBlur");

			//Depth Of Field
			postProcessShaders.DepthOfFieldEffect = Engine.EngineContentManager.Load<Effect>("Shaders/DepthOfField/DepthOfField");

			//Post Processing Shaders 
			postProcessShaders.CartoonEdgeDetection = Engine.EngineContentManager.Load<Effect>("Shaders/Edge Detection/CartoonEdgeDetection");

			//Distortion Shaders
			shaders.DistortionShader = Engine.EngineContentManager.Load<Effect>("Shaders/Distorter/Distorters");
			postProcessShaders.distortEffect = Engine.EngineContentManager.Load<Effect>("Shaders/Distorter/DistortScene");
			postProcessShaders.distortTechnique = postProcessShaders.distortEffect.Techniques["Distort"];
			postProcessShaders.distortBlurTechnique = postProcessShaders.distortEffect.Techniques["DistortBlur"];



			//Defferred Shading
			shaders.DrfrdRndrClearGBuffer = Engine.EngineContentManager.Load<Effect>("Shaders/Lighting/ClearGBuffer");
			shaders.DrfrdRndrCombineFinal = Engine.EngineContentManager.Load<Effect>("Shaders/Lighting/CombineFinal");
			shaders.DrfrdRndrDirectionalLight = Engine.EngineContentManager.Load<Effect>("Shaders/Lighting/DirectionalLight");
			shaders.DrfrdRndrPointLight = Engine.EngineContentManager.Load<Effect>("Shaders/Lighting/PointLight");

            //Crepuscular Rays
            postProcessShaders.SSAOEffect = Engine.EngineContentManager.Load<Effect>("Shaders/SSAO/SSAO");
            postProcessShaders.MaskedSunEffect = Engine.EngineContentManager.Load<Effect>("Shaders/God Rays/MaskedSun");
            postProcessShaders.GodRaysCombineEffect = Engine.EngineContentManager.Load<Effect>("Shaders/God Rays/GodRaysCombine");
			postProcessShaders.LightRaysEffect = Engine.EngineContentManager.Load<Effect>("Shaders/God Rays/LightRays");
            /********************************************************************************************/
            /*										Models												*/
            /********************************************************************************************/
            string tag = "";

#if VRTC_PLTFRM_GL
				tag = "_mg";

#else
#endif

			//Unit Models
			models.UnitArrow = Engine.vxContentManager.LoadBasicEffectModel("Models/utils/unit_arrow/unit_arrow", Engine.EngineContentManager, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.UnitBox = Engine.vxContentManager.LoadModel("Models/utils/unit_box/unit_box" + tag, Engine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.UnitPlane = Engine.vxContentManager.LoadModel("Models/utils/unit_plane/unit_plane"+ tag, Engine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.UnitSphere = Engine.vxContentManager.LoadModel("Models/utils/unit_sphere/unit_sphere"+ tag, Engine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.Sun_Mask = Engine.vxContentManager.LoadModel("Models/sun/sun_mask", Engine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
            models.WaterPlane = Engine.vxContentManager.LoadModel("Models/water_plane/water_plane_mg", Engine.EngineContentManager, shaders.WaterReflectionShader, shaders.CascadeShadowShader, shaders.UtilityShader);
#endif
        }
    }
}
