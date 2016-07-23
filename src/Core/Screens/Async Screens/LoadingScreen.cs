#region File Description
//-----------------------------------------------------------------------------
// LoadingScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.Diagnostics;
using vxVertices.Core;
#endregion

namespace vxVertices.Screens.Async
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    public class LoadingScreen : vxGameBaseScreen
    {
        #region Fields

        bool loadingIsSlow;
        bool otherScreensAreGone;

        static string messageToDraw = "";

        vxGameBaseScreen[] screensToLoad;

        Thread backgroundThread;
        EventWaitHandle backgroundThreadExit;

        GraphicsDevice graphicsDevice;
        //IMessageDisplay messageDisplay;

        GameTime loadStartTime;
        TimeSpan loadAnimationTimer;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>

        private LoadingScreen(vxEngine screenManager, bool loadingIsSlow,
                              vxGameBaseScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);

            messageToDraw = "";

            // If this is going to be a slow load operation, create a background
            // thread that will update the network session and draw the load screen
            // animation while the load is taking place.
            if (loadingIsSlow)
            {
                backgroundThread = new Thread(BackgroundWorkerThread);
                backgroundThreadExit = new ManualResetEvent(false);

                graphicsDevice = screenManager.GraphicsDevice;
            }
        }


        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(vxEngine screenManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params vxGameBaseScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (vxGameBaseScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);


            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }

        //
        //Takes in a center message
        //
        public static void Load(string message, vxEngine screenManager, bool loadingIsSlow,
                PlayerIndex? controllingPlayer,
                params vxGameBaseScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (vxGameBaseScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);

            messageToDraw = message;

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone)
            {
                // Start up the background thread, which will update the network
                // session and draw the animation while we are loading.
                if (backgroundThread != null)
                {
                    loadStartTime = gameTime;
                    backgroundThread.Start();
                }

                // Perform the load operation.
                vxEngine.RemoveScreen(this);

                foreach (vxGameBaseScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        vxEngine.AddScreen(screen, ControllingPlayer);
                    }
                }

                // Signal the background thread to exit, then wait for it to do so.
                if (backgroundThread != null)
                {
                    backgroundThreadExit.Set();
                    backgroundThread.Join();
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                vxEngine.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            this.vxEngine.InputManager.ShowCursor = false;
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (vxEngine.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (loadingIsSlow)
            {

                SpriteBatch spriteBatch = vxEngine.SpriteBatch;
				SpriteFont font = vxEngine.vxGUITheme.Font;

                string message = "loading";

                // Center the text in the viewport.
                Viewport viewport = vxEngine.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) * 95 / 100;

                Color color = vxEngine.LoadingScreenTextColor * TransitionAlpha;

                // Animate the number of dots after our "Loading..." message.
                loadAnimationTimer += gameTime.ElapsedGameTime;

                int dotCount = (int)(loadAnimationTimer.TotalSeconds * 5) % 5;

                message += new string('.', dotCount);

                Vector2 messageToDrawPos = new Vector2(vxEngine.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(messageToDraw).X / 2,
    vxEngine.GraphicsDevice.Viewport.Height / 2 - font.MeasureString(messageToDraw).Y / 2);

                // Draw the text.
                spriteBatch.Begin();
                if (messageToDraw != "")
                    Thread.Sleep(20);
                spriteBatch.DrawString(font, messageToDraw, messageToDrawPos, color);
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }
        }


        #endregion

        #region Background Thread


        /// <summary>
        /// Worker thread draws the loading animation and updates the network
        /// session while the load is taking place.
        /// </summary>
        void BackgroundWorkerThread()
        {
            long lastTime = Stopwatch.GetTimestamp();

            // EventWaitHandle.WaitOne will return true if the exit signal has
            // been triggered, or false if the timeout has expired. We use the
            // timeout to update at regular intervals, then break out of the
            // loop when we are signalled to exit.
            while (!backgroundThreadExit.WaitOne(1000 / 30))
            {
                GameTime gameTime = GetGameTime(ref lastTime);

                DrawLoadAnimation(gameTime);
            }
        }


        /// <summary>
        /// Works out how long it has been since the last background thread update.
        /// </summary>
        GameTime GetGameTime(ref long lastTime)
        {
            long currentTime = Stopwatch.GetTimestamp();
            long elapsedTicks = currentTime - lastTime;
            lastTime = currentTime;

            TimeSpan elapsedTime = TimeSpan.FromTicks(elapsedTicks *
                                                      TimeSpan.TicksPerSecond /
                                                      Stopwatch.Frequency);

            return new GameTime(loadStartTime.TotalGameTime + elapsedTime, elapsedTime);
        }


        /// <summary>
        /// Calls directly into our Draw method from the background worker thread,
        /// so as to update the load animation in parallel with the actual loading.
        /// </summary>
        void DrawLoadAnimation(GameTime gameTime)
        {
            if ((graphicsDevice == null) || graphicsDevice.IsDisposed)
                return;

            try
            {
                graphicsDevice.Clear(vxEngine.LoadingScreenBackColor);

                // Draw the loading screen.
                Draw(gameTime);

                // If we have a message display component, we want to display
                // that over the top of the loading screen, too.
                //if (messageDisplay != null)
                //{
                //    messageDisplay.Update(gameTime);
                //    messageDisplay.Draw(gameTime);
                //}

                graphicsDevice.Present();
            }
            catch
            {
                // If anything went wrong (for instance the graphics device was lost
                // or reset) we don't have any good way to recover while running on a
                // background thread. Setting the device to null will stop us from
                // rendering, so the main game can deal with the problem later on.
                graphicsDevice = null;
            }
        }
        #endregion
    }
}
