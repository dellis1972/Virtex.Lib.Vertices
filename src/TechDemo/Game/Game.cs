#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Screens;
using Virtex.Lib.Vrtc.Utilities;
#endregion

namespace Virtex.vxGame.VerticesTechDemo
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class VerticesTechDemoGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;

        //Main vxEngine for Game
        GameEngine vxEngine;
                
        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public VerticesTechDemoGame()
        {
            graphics = new GraphicsDeviceManager(this);

			#if DEBUG
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
			#endif
			graphics.SynchronizeWithVerticalRetrace = false;


            // Create the screen manager component.
            vxEngine = new GameEngine(this);
            Components.Add(vxEngine);



			string platformTag = "";
			#if TECHDEMO_PLTFRM_XNA
			//Do Nothing
			#elif TECHDEMO_PLTFRM_GL
			platformTag = "/Compiled.DesktopGL";
			#endif 

			Content.RootDirectory = "Content" + platformTag;

            vxEngine.LoadResolution = false;


            vxEngine.AddScreen(new vxTitleScreen(), null);
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            //vxEngine.Font_GUI = Content.Load<SpriteFont>("Fonts/font_gui");
            vxEngine.InputManager.ShowCursor = true;
            this.IsMouseVisible = false;
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);

//			vxEngine.SpriteBatch.Begin();
//
//			//Draw Version Information
//			int Padding = 5;
//			int TextWidth = (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.GameVersion).X;
//			int TextHeight = (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.GameVersion).Y;
//
//			//Position Of Text
//			Vector2 PosOfText = new Vector2(
//				vxEngine.GraphicsDevice.Viewport.Width - 2 * Padding - TextWidth,
//				vxEngine.GraphicsDevice.Viewport.Height - Padding - TextHeight);
//
//			//Draw Background
//			vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, 
//				new Rectangle(
//					(int)PosOfText.X - Padding,
//					(int)PosOfText.Y,
//					TextWidth + 2 * Padding, 
//					TextHeight),
//				Color.Black * 0.75f);
//
//			vxEngine.SpriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, vxEngine.GameVersion, PosOfText, 
//				Color.White * 0.85f);
//
//			vxEngine.SpriteBatch.End();
        }


        #endregion
    }
}
