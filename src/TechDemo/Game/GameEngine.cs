using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

using vxVertices.Core;
using vxVertices.Screens.Async;
using vxVertices.Graphics;

namespace VerticeEnginePort.Base
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
        public GameEngine(Game game) : base(game, "vertices_techDemo") { }


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
			LoadAssetsScreen.Load(this, true, null, new IntroBackground(),
                    new MainMenuScreen());
        }
        
        /// <summary>
        /// Load Global Content
        /// </summary>
        /// <param name="content"></param>
        public override void LoadGlobalContent(ContentManager content)
        {
            HasContentBeenLoaded = true;

            GameVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            WriteLine_White("Loading Global Content...");

            ////////////////////////////////////////////////////////////////////////////////////////////
            //Fonts
            ////////////////////////////////////////////////////////////////////////////////////////////
            Console.Write("\tLoading Fonts...");

            WriteLine_Green("\t\tDone!");

            ////////////////////////////////////////////////////////////////////////////////////////////
            //Images
            ////////////////////////////////////////////////////////////////////////////////////////////
            Console.Write("\tLoading Textures...");


            WriteLine_Green("\t\tDone!");


            ////////////////////////////////////////////////////////////////////////////////////////////
            //Shaders
            ////////////////////////////////////////////////////////////////////////////////////////////
            Console.Write("\tLoading Shaders...");

            WriteLine_Green("\t\tDone!");




            ////////////////////////////////////////////////////////////////////////////////////////////
            //MODELS
            ////////////////////////////////////////////////////////////////////////////////////////////

            Console.Write("\tLoading Models...\n");

            Model_Items_ModelObjs = ContentManager.LoadModel("Models/modelobjs/modelobjs");

            Model_Items_WoodenCrate = ContentManager.LoadModel("Models/items/wooden crate/wooden crate");
            Model_Items_Concrete = ContentManager.LoadModel("Models/concrete_cube/concrete_cube");
            Model_Items_Teapot = ContentManager.LoadModel("Models/teapot/teapot");

            //Model_Items_WaterCrate = ContentManager.LoadModelAsWaterObject("Models/items/wooden crate/wooden crate", content);
            WriteLine_Green("\t\tDone!");


            ////////////////////////////////////////////////////////////////////////////////////////////
            //Sounde Effects
            ////////////////////////////////////////////////////////////////////////////////////////////
            Console.Write("\tLoading Sound Effects...");


            WriteLine_Green("\tDone!");

            ////////////////////////////////////////////////////////////////////////////////////////////
            // Music
            ////////////////////////////////////////////////////////////////////////////////////////////
            Console.Write("\tLoading Music...");
            

            WriteLine_Green("\t\tDone!");
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            this.InputManager.CursorRotation += (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Draw(gameTime);
        }
    }
}
