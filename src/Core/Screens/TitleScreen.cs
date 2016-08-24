#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Virtex.Lib.Vrtc.Core;

#endregion

namespace Virtex.Lib.Vrtc.Screens
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    public class TitleScreen : vxGameBaseScreen
    {
        #region Fields

        ContentManager content;

        SpriteFont TitleFont;

        float pauseAlpha;

        float UpdateCount = 0;

        KeyboardState CurrentKeyboardState = new KeyboardState();
        KeyboardState PreviousKeyboardState = new KeyboardState();

        #endregion

        #region Initialization

#if DEBUG
        float UpdateTime = 0.5f;
#else
        float UpdateTime = 5;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Screens.TitleScreen"/> class.
        /// </summary>
        public TitleScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Screens.TitleScreen"/> class.
		/// </summary>
		/// <param name="UpdateTime">Update time.</param>
        public TitleScreen(int UpdateTime):this()
        {
            this.UpdateTime = UpdateTime;
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
            if (content == null)
                content = new ContentManager(vxEngine.Game.Services, "Content");

            TitleFont = vxEngine.EngineContentManager.Load<SpriteFont>("Fonts/font_splash");
			try
			{
				Console.WriteLine ("Loading Splash Screen");
				vxEngine.SplashScreen = vxEngine.Game.Content.Load<Texture2D> ("SplashScreen");
				Console.WriteLine ("Success");
			}
			catch {
				Console.WriteLine ("Error Loading Splash Screen");
			}
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw



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
            #region Fade and Base Update

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            #endregion           

            CurrentKeyboardState = Keyboard.GetState();

			UpdateCount += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (UpdateCount > UpdateTime || CurrentKeyboardState.IsKeyDown(Keys.Enter) || vxEngine.HasContentBeenLoaded)
            {
				vxEngine.vxEngineMainEntryPoint();
            }

            PreviousKeyboardState = CurrentKeyboardState;

            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        /// 
        public override void Draw(GameTime gameTime)
        {            
            SpriteBatch spriteBatch = vxEngine.SpriteBatch;
            Viewport viewport = vxEngine.GraphicsDevice.Viewport;
            
            vxEngine.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            
            vxEngine.GraphicsDevice.Clear(ClearOptions.Target, Color.GhostWhite, 0, 0);
            GraphicsDevice device = vxEngine.Game.GraphicsDevice;


            //Draw SpriteBatch shit
            vxEngine.Game.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

                spriteBatch.Begin();
                //Draw Version Information
                spriteBatch.Draw(vxEngine.Assets.Textures.Blank, new Rectangle(20, vxEngine.GraphicsDevice.Viewport.Height - 20,
                    (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.EngineVersion).X + 10, (int)vxEngine.Assets.Fonts.DebugFont.MeasureString(vxEngine.EngineVersion).Y),
                    Color.Black * TransitionAlpha * 0.75f);
                spriteBatch.DrawString(vxEngine.Assets.Fonts.DebugFont, vxEngine.EngineVersion, new Vector2(25, vxEngine.GraphicsDevice.Viewport.Height - 20), Color.White * TransitionAlpha * 0.85f);

                string Title = "VIRTEX EDGE DESIGN";
                spriteBatch.DrawString(TitleFont, Title,
                    new Vector2(viewport.Width / 2 - TitleFont.MeasureString(Title).X / 2, viewport.Height / 2), Color.Black * TransitionAlpha);

                spriteBatch.End();

            #region Transition Code
            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                
                vxEngine.FadeBackBufferToBlack(alpha);
            }
            #endregion
        }

        #endregion
    }
}
