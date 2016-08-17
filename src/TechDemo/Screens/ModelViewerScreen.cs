using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using System.Linq;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core.Input;

namespace VerticeEnginePort.Base
{
    /// <summary>
    /// This is the main class for the game. It holds the instances of the sphere simulator,
    /// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
    /// game loop, and provides keyboard and mouse input.
    /// </summary>
    public class ModelViewerScreen : vxScene3D
    {        
        //xPlane plane;

        public ModelViewerScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            InitialiseLevel();

            // Create the chase camera
            //plane = new xPlane(vxEngine, ((GameEngine)vxEngine).Model_Sandbox_WorkingPlane, Vector3.Zero);
            base.LoadContent();
            Camera.CameraType = CameraType.Orbit;
//
//            ship = new Ship(vxEngine, ((GameEngine)vxEngine).Model_Ship, new Vector3(0, 30, 0), ShipController.Player);
//            camera.OrbitTarget = ship.Position;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        float angle = 0;
        /// <summary>
        /// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            angle += 0.01f;
            Vector3 position = new Vector3(0, 7.5f, 0);
            
            //camera.View = Matrix.CreateLookAt(new Vector3(0, 15, 15), position, new Vector3(0, 1, 0));
            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
//            ship.Deactivate();
//            ship.World = Matrix.CreateScale(1.0f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(position);
//            plane.World = Matrix.CreateTranslation(new Vector3(0, -10, 0));

            Camera.OrbitTarget = position;
        }

        /*
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.KeyboardState;// input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.GamePadState;// input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected;

            /*            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame() || gamePadDisconnected)
            {
                vxEngine.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {

            }
        }
            */
    }
}
