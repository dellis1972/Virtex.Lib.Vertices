using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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

        /// <summary>
        /// Basic Constructor. Note: All Items must be instantiated outside of this function.
        /// </summary>
        public vxModel()
        {

        }

        /// <summary>
        /// Sets Up a New vxModel with two corresponding Models.
        /// </summary>
        /// <param name="modelMain"></param>
        /// <param name="modelShadow"></param>
        public vxModel (Model modelMain, Model modelShadow)
		{
						
		}
	}
}

