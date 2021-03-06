﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Screens.Async;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.vxGame.VerticesTechDemo
{
    
    public class GameEngine : vxEngine
    {
        #region Fields

        public vxModel Model_Items_WoodenCrate { get; set; }
        public vxModel Model_Items_ModelObjs { get; set; }
        public vxModel Model_Items_WaterCrate { get; set; }
        public vxModel Model_Items_Concrete { get; set; }
        public vxModel Model_Items_Teapot { get; set; }


        #endregion

        #region Initialization

        /// <summary>
        /// vxEngine Interface on Game Side, Add in all Global Assets specific to the game here.
        /// </summary>
		public GameEngine(Game game) : base(game, "VerticesEngineTechDemo") { 
			this.GameVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
		}


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }



        /// <summary>
        /// Screens to Load at Start
        /// </summary>
        public override void vxEngineMainEntryPoint()
        {
			vxLoadAssetsScreen.Load(this, true, null, new IntroBackground(),
                    new MainvxMenuBaseScreen());
        }
        
        /// <summary>
        /// Load Global Content
        /// </summary>
        /// <param name="content"></param>
        public override void LoadGlobalContent(ContentManager content)
        {
            HasContentBeenLoaded = true;

            GameVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
			vxConsole.WriteLine("Loading Global Content...");

            ////////////////////////////////////////////////////////////////////////////////////////////
            //Fonts
            ////////////////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////////////////
            //Images
            ////////////////////////////////////////////////////////////////////////////////////////////



            ////////////////////////////////////////////////////////////////////////////////////////////
            //Shaders
            ////////////////////////////////////////////////////////////////////////////////////////////





            ////////////////////////////////////////////////////////////////////////////////////////////
            //MODELS
            ////////////////////////////////////////////////////////////////////////////////////////////


            Model_Items_ModelObjs = vxContentManager.LoadModel("Models/modelobjs/modelobjs");

            Model_Items_WoodenCrate = vxContentManager.LoadModel("Models/items/wooden crate/wooden crate");
            
			Model_Items_Concrete = vxContentManager.LoadModel("Models/concrete_cube/concrete_cube");
            
			Model_Items_Teapot = vxContentManager.LoadModel("Models/teapot/teapot");



            ////////////////////////////////////////////////////////////////////////////////////////////
            //Sounde Effects
            ////////////////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////////////////
            // Music
            ////////////////////////////////////////////////////////////////////////////////////////////

        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            this.InputManager.CursorRotation += (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Draw(gameTime);
        }
    }
}
