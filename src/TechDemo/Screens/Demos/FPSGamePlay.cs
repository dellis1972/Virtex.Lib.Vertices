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
using Virtex.Lib.Vrtc.Core.Cameras.Controllers;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.Screens.Menus;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using BEPUutilities;
using Virtex.Lib.Vrtc.Physics.BEPU.CollisionRuleManagement;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core.Entities;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Physics.BEPU.Entities.Prefabs;
using Virtex.Lib.Vrtc.Graphics;


namespace Virtex.vxGame.VerticesTechDemo
{
    /// <summary>
    /// This is the main class for the game. It holds the instances of the sphere simulator,
    /// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
    /// game loop, and provides keyboard and mouse input.
    /// </summary>
    public class FPSGamePlay : vxScene3D
    {

        //
        //Player
        //
        public CharacterControllerInput character;



        #region Picking

        //Motorized Grabber
        protected MotorizedGrabSpring grabber;
        protected float grabDistance;

        //Load in mesh data and create the collision mesh.
        Vector3[] staticTriangleVertices;
        int[] staticTriangleIndices;

        //The raycast filter limits the results retrieved from the Space.RayCast while grabbing.
        Func<BroadPhaseEntry, bool> rayCastFilter;
        bool RayCastFilter(BroadPhaseEntry entry)
        {
            if (character != null)
                return entry != character.CharacterController.Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;

            else
                return true;
        }

        #endregion

        public FPSGamePlay()
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

//			xEnvrio g = new xEnvrio(vxEngine, vxEngine.LoadModel("Models/tech demo/vrtc_techDemo"), Vector3.Zero);
//            g.World *= Matrix.CreateRotationZ(-MathHelper.PiOver2);
//			g.name = "tech demo";
//			g.NormalMap = vxEngine.Game.Content.Load<Texture2D>("Models/tech demo/grass_nm");


			int size = 100;
			Box baseBox = new Box (new Vector3(0, -5, 0), size, 10, size);
            BEPUPhyicsSpace.Add(baseBox);

            ///////////////////////////////////////////////////////////////////////
            //Initialise Camera Code
            ///////////////////////////////////////////////////////////////////////
            #region Set Up Camera

            base.LoadContent();
            Camera.CameraType = CameraType.CharacterFPS;

            Camera.Position = new Vector3(0,20,0);

            character = new CharacterControllerInput(BEPUPhyicsSpace, Camera, vxEngine);
            
            //Since this is the character playground, turn on the character by default.
            character.Activate();

            //Having the character body visible would be a bit distracting.
            character.CharacterController.Body.Tag = "noDisplayObject";


            //
            //Grabbers
            //
            grabber = new MotorizedGrabSpring();
            BEPUPhyicsSpace.Add(grabber);
            rayCastFilter = RayCastFilter;

			new IndexedPrimTest(vxEngine, new Vector3(0,0,0));


            #endregion
        }

        

        /// <summary>
        /// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, vxEngine.InputManager.PreviousKeyboardState,
    vxEngine.InputManager.KeyboardState, vxEngine.InputManager.PreviousGamePadState, vxEngine.InputManager.GamePadState);
            
            if (this.IsActive)
                vxEngine.InputManager.ShowCursor = false;
            else
                vxEngine.InputManager.ShowCursor = true;

            //Update grabber
            if (vxEngine.InputManager.MouseState.RightButton == ButtonState.Pressed && !grabber.IsGrabbing)
            {
                //Find the earliest ray hit
                RayCastResult raycastResult;
                if (BEPUPhyicsSpace.RayCast(new Ray(Camera.Position, Camera.WorldMatrix.Forward), 500, rayCastFilter, out raycastResult))
                {
                    var entityCollision = raycastResult.HitObject as EntityCollidable;
                    //If there's a valid ray hit, then grab the connected object!
                    if (entityCollision != null && entityCollision.Entity.IsDynamic)
                    {
                        Console.WriteLine("GRABBING ITEM: {0}", entityCollision.Entity.GetType().ToString());
                        grabber.Setup(entityCollision.Entity, raycastResult.HitData.Location);
                        //grabberGraphic.IsDrawing = true;
                        grabDistance = raycastResult.HitData.T;
                    }
                }
            }

            if (vxEngine.InputManager.MouseState.RightButton == ButtonState.Pressed && grabber.IsUpdating)
            {
                if (grabDistance < 4)
                {
                    grabDistance = 3;
                    grabber.GoalPosition = Camera.Position + Camera.WorldMatrix.Forward * grabDistance;
                }
            }

            else if (vxEngine.InputManager.MouseState.RightButton == ButtonState.Released && grabber.IsUpdating)
            {
                grabber.Release();
            }

            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
