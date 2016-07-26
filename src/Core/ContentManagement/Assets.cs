using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Virtex.Lib.Vertices.Graphics;

namespace Virtex.Lib.Vertices.Core.ContentManagement
{
    /// <summary>
    /// Class for holding all vxEngine Assets
    /// </summary>
    public class Assets
    {
        vxEngine vxEngine;

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
            public Model UnitArrow { get; set; }
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
            public Effect EdgeDetection { get; set; }
            public Effect CartoonEdgeDetection { get; set; }
            public Effect BloomExtractEffect { get; set; }
            public Effect BloomCombineEffect { get; set; }
            public Effect GaussianBlurEffect { get; set; }
            public Effect DepthOfFieldEffect { get; set; }

            //God Rays
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
        public Assets(vxEngine vxEngine)
        {
            //Game vxEngine
            this.vxEngine = vxEngine;

			/********************************************************************************************/
			/*										Fonts												*/
			/********************************************************************************************/
            fonts.MenuTitleFont = vxEngine.EngineContentManager.Load<SpriteFont>("Fonts/font_gui_title");
            fonts.MenuFont = vxEngine.EngineContentManager.Load<SpriteFont>("Fonts/font_gui");
            fonts.DebugFont = vxEngine.EngineContentManager.Load<SpriteFont>("Fonts/font_debug");



			/********************************************************************************************/
			/*										Textures											*/
			/********************************************************************************************/

            textures.Blank = vxEngine.EngineContentManager.Load<Texture2D>("Textures/xGUI/blank");
            textures.Gradient = vxEngine.EngineContentManager.Load<Texture2D>("Textures/xGUI/gradient");

            //Arrows
            textures.Arrow_Left = vxEngine.EngineContentManager.Load<Texture2D>("Textures/Menu/Slider/Arrow_Left");
            textures.Arrow_Right = vxEngine.EngineContentManager.Load<Texture2D>("Textures/Menu/Slider/Arrow_Right");

            textures.Texture_Diffuse_Null = vxEngine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_diffuse");
            textures.Texture_NormalMap_Null = vxEngine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_normal");
            textures.Texture_SpecularMap_Null = vxEngine.EngineContentManager.Load<Texture2D>("Textures/nullTextures/null_specular");

            //textures.Texture_Cube_Null = vxEngine.EngineContentManager.Load<Texture2D>("Textures/null_cube");

            //Glow
            textures.Texture_Sun_Glow = vxEngine.EngineContentManager.Load<Texture2D>("Textures/Rays/Rays");




#if !VRTC_PLTFRM_DROID
            //Water
            textures.Texture_WaterWaves = vxEngine.EngineContentManager.Load<Texture2D>("Shaders/Water/waterbump");
            textures.Texture_WaterDistort = vxEngine.EngineContentManager.Load<Texture2D>("Shaders/Water/waterdistort");


            /********************************************************************************************/
            /*										Shaders												*/
			/********************************************************************************************/

			string prefixtag = "";

            //Model Shaders
#if VRTC_PLTFRM_XNA
            //shaders.MainShader = vxEngine.EngineContentManager.Load<Effect>("Shaders/MainModelShader");

            //Water
            shaders.WaterReflectionShader = vxEngine.EngineContentManager.Load<Effect>("Shaders/Water/vxWater");

			//Distorter
			shaders.DistortionShader = vxEngine.EngineContentManager.Load<Effect>("Shaders/Distorter/Distorters");
			postProcessShaders.distortEffect = vxEngine.EngineContentManager.Load<Effect>("Shaders/Distorter/DistortScene");
			postProcessShaders.distortTechnique = postProcessShaders.distortEffect.Techniques["Distort"];
			postProcessShaders.distortBlurTechnique = postProcessShaders.distortEffect.Techniques["DistortBlur"];

            postProcessShaders.EdgeDetection = vxEngine.EngineContentManager.Load<Effect>("Shaders/EdgeDetection");
#else

			prefixtag = "MonoGame/";


#endif

            shaders.MainShader = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/MainModelShader");
            shaders.CascadeShadowShader  = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/CascadeShadowShader");
            shaders.UtilityShader = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/UtilityShader");

            //Bloom
            postProcessShaders.BloomExtractEffect = vxEngine.EngineContentManager.Load<Effect>("Shaders/Bloom/BloomExtract");
			postProcessShaders.BloomCombineEffect = vxEngine.EngineContentManager.Load<Effect>("Shaders/Bloom/BloomCombine");
			postProcessShaders.GaussianBlurEffect = vxEngine.EngineContentManager.Load<Effect>("Shaders/Bloom/GaussianBlur");

			//Depth Of Field
			postProcessShaders.DepthOfFieldEffect = vxEngine.EngineContentManager.Load<Effect>("Shaders/DepthOfField/DepthOfField");

			//Post Processing Shaders            
			
			postProcessShaders.CartoonEdgeDetection = vxEngine.EngineContentManager.Load<Effect>("Shaders/CartoonEdgeDetection");




			//Defferred Shading
			shaders.DrfrdRndrClearGBuffer = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Lighting/ClearGBuffer");
			shaders.DrfrdRndrCombineFinal = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Lighting/CombineFinal");
			shaders.DrfrdRndrDirectionalLight = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Lighting/DirectionalLight");
			shaders.DrfrdRndrPointLight = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/Lighting/PointLight");

            //Crepuscular Rays
            postProcessShaders.MaskedSunEffect = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/God Rays/MaskedSun");
            postProcessShaders.GodRaysCombineEffect = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/God Rays/GodRaysCombine");
			postProcessShaders.LightRaysEffect = vxEngine.EngineContentManager.Load<Effect>(prefixtag + "Shaders/God Rays/LightRays");
            /********************************************************************************************/
            /*										Models												*/
            /********************************************************************************************/
            string tag = "";

#if VRTC_PLTFRM_GL
				tag = "_mg";

#else
            //Unit Models
            models.UnitArrow = vxEngine.EngineContentManager.Load<Model>("Models/utils/unit_arrow/unit_arrow");
#endif
            models.UnitBox = vxEngine.vxContentManager.LoadModel("Models/utils/unit_box/unit_box" + tag, vxEngine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.UnitPlane = vxEngine.vxContentManager.LoadModel("Models/utils/unit_plane/unit_plane"+ tag, vxEngine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.UnitSphere = vxEngine.vxContentManager.LoadModel("Models/utils/unit_sphere/unit_sphere"+ tag, vxEngine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
			models.Sun_Mask = vxEngine.vxContentManager.LoadModel("Models/sun/sun_mask", vxEngine.EngineContentManager, shaders.MainShader, shaders.CascadeShadowShader, shaders.UtilityShader);
            models.WaterPlane = models.Sun_Mask;// vxEngine.LoadModel("Models/sun/sun_mask", vxEngine.EngineContentManager);
#endif
        }
    }
}
