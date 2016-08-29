#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using Virtex.Lib.Vrtc.Core.Settings;


#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Screens.Async;
#endregion

namespace Virtex.vxGame.VerticesTechDemo
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : vxGameBaseScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
			string platformTag = "";
			#if TECHDEMO_PLTFRM_XNA
				//Do Nothing
			#elif TECHDEMO_PLTFRM_GL
				platformTag = "/Compiled.WindowsGL";
			#endif  

            if (content == null)
				content = new ContentManager(vxEngine.Game.Services, "Content" + platformTag);

			backgroundTexture = content.Load<Texture2D>("Textures/menu/background");


            //
            //Loads Global Content
            //
            if (vxEngine.HasContentBeenLoaded == false)
                vxEngine.LoadGlobalContent(vxEngine.Game.Content);
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {            
            content.Unload();
            //backgrounMusic.Dispose();
        }


        #endregion

        #region Update and Draw

        bool firstloop = true;
        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            //if (firstloop)
            //{
            //    firstloop = false;
            //    LoadingScreen.Load(vxEngine, true, PlayerIndex.One, new TechDemoSponza());
            //}
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            #region Set Up
            SpriteBatch spriteBatch = vxEngine.SpriteBatch;
            Viewport viewport = vxEngine.GraphicsDevice.Viewport;

            vxEngine.GraphicsDevice.Clear(ClearOptions.Target, Color.GhostWhite, 0, 0);

            //Draw SpriteBatch shit
            vxEngine.Game.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
            spriteBatch.Begin();

            //Draw Version Information
            int Padding = 5;
            int TextWidth = (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.GameVersion).X;
            int TextHeight = (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.GameVersion).Y;

            //Position Of Text
            Vector2 PosOfText = new Vector2(
                vxEngine.GraphicsDevice.Viewport.Width - 2 * Padding - TextWidth,
                vxEngine.GraphicsDevice.Viewport.Height - Padding - TextHeight);

			spriteBatch.Draw(backgroundTexture, 
				viewport.Bounds,
				Color.White*TransitionAlpha);

            //Draw Background
            spriteBatch.Draw(vxEngine.Assets.Textures.Blank, 
                new Rectangle(
                    (int)PosOfText.X - Padding,
                    (int)PosOfText.Y,
                    TextWidth + 2 * Padding, 
                    TextHeight),
                Color.Black * TransitionAlpha * 0.75f);

            spriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, vxEngine.GameVersion, PosOfText, 
                Color.White * TransitionAlpha * 0.85f);
            
            spriteBatch.End();

			#endregion
		}        
    }
	#endregion
}
