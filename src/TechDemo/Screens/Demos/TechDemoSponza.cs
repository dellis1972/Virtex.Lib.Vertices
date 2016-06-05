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
using vxVertices.Core.Scenes;
using vxVertices.Core.Cameras.Controllers;
using vxVertices.Core.Input;
using vxVertices.Screens.Menus;
using vxVertices.Physics.BEPU;
using vxVertices.Physics.BEPU.BroadPhaseEntries;
using vxVertices.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using BEPUutilities;
using vxVertices.Physics.BEPU.CollisionRuleManagement;
using vxVertices.Core.Cameras;
using vxVertices.Core;
using vxVertices.GUI.Controls;
using vxVertices.Utilities;
using vxVertices.Physics.BEPU.Entities.Prefabs;
using vxVertices.Scenes.Sandbox3D;
using vxVertices.Entities.Sandbox3D;

namespace VerticeEnginePort.Base
{
    /// <summary>
    /// This is the main class for the game. It holds the instances of the sphere simulator,
    /// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
    /// game loop, and provides keyboard and mouse input.
    /// </summary>
    public class TechDemoSponza : vxSandboxGamePlay
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

        public TechDemoSponza()
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


            ///////////////////////////////////////////////////////////////////////
            //Initialise Camera Code
            ///////////////////////////////////////////////////////////////////////
            #region Set Up Camera

            base.LoadContent();
            Camera.CameraType = CameraType.CharacterFPS;

            character = new CharacterControllerInput(BEPUPhyicsSpace, Camera, vxEngine);

            //Since this is the character playground, turn on the character by default.
            character.Activate();

            //Having the character body visible would be a bit distracting.
            character.CharacterController.Body.Tag = "noDisplayObject";

            SimulationStart();
            SimulationStop();

            //
            //Grabbers
            //
            grabber = new MotorizedGrabSpring();
            BEPUPhyicsSpace.Add(grabber);
            rayCastFilter = RayCastFilter;


            #endregion

            DoFog = true;
            /*
			xEnvrio g = new xEnvrio(vxEngine, vxEngine.LoadModel("Models/sponza/sponza"), Vector3.Zero);
            g.NormalMap = vxEngine.Game.Content.Load<Texture2D>("Models/sponza/spnza_bricks_nm");
            g.SpecularMap = vxEngine.Game.Content.Load<Texture2D>("Models/sponza/spnza_bricks_sm");
            */

            xEnvrio envr = new xEnvrio(vxEngine, vxEngine.ContentManager.LoadModel("Models/courtyard/td_courtyard"), Vector3.Zero);
            //g.NormalMap = vxEngine.Game.Content.Load<Texture2D>("Models/courtyard/structure_nm");
            //g.SpecularMap = vxEngine.Game.Content.Load<Texture2D>("Models/courtyard/structure_sm");
            envr.SpecularIntensity = 1;

            //This is a little convenience method used to extract vertices and indices from a model.
            //It doesn't do anything special; any approach that gets valid vertices and indices will work.
            
			#if !TECHDEMO_PLTFRM_GL
			ModelDataExtractor.GetVerticesAndIndicesFromModel(envr.vxModel.ModelMain, out staticTriangleVertices, out staticTriangleIndices);

            //var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices, new AffineTransform(Matrix3X3.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), new Vector3(0, -10, 0)));
            var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices,
                new AffineTransform(new Vector3(1),
                    Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(0)),
                    new Vector3(0)));
            staticMesh.Sidedness = TriangleSidedness.Counterclockwise;

            BEPUPhyicsSpace.Add(staticMesh);
            BEPUDebugDrawer.Add(staticMesh);
			#endif

            int size = 100;
			BEPUPhyicsSpace.Add(new Box (new Vector3(0, -5, 0), size, 10, size));

            vxTabPage Straights = new vxTabPage(vxEngine, tabControl, "Items");
            tabControl.AddItem(Straights);

            vxScrollPanel ScrollPanel_Straights = new vxScrollPanel(new Vector2(0, 0),
                vxEngine.GraphicsDevice.Viewport.Width - 150, vxEngine.GraphicsDevice.Viewport.Height - 75);

            //Cubes
            ScrollPanel_Straights.AddItem(new vxScrollPanelSpliter(vxEngine, "Items"));
            ScrollPanel_Straights.AddItem(RegisterNewSandboxItem(WoodenCrate.EntityDescription));
            //Add the scrollpanel to the slider tab page.
            Straights.AddItem(ScrollPanel_Straights);

            IndexedCubeTest cube = new IndexedCubeTest(vxEngine, new Vector3(4, 4, 0));

            /*
            Teapot t = new Teapot((GameEngine)vxEngine, new Vector3(4, 4, 0));
            t.SetMesh(Matrix.CreateTranslation(new Vector3(4, 2, 0)), true, true);
            
            ConcreteCube cc = new ConcreteCube((GameEngine)vxEngine, new Vector3(0, 5, 0));
            cc.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, 0)), true, true);
            */

            ModelObjs mo = new ModelObjs((GameEngine)vxEngine, new Vector3(-4, 4, 0));
            mo.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, 8)), true, true);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override vxSandboxEntity GetNewEntity(string key)
        {
            switch (key)
            {
                //Cubes
                case "VerticeEnginePort.Base.WoodenCrate":
                    return new WoodenCrate((GameEngine)vxEngine, Vector3.Zero);
                    break;

                default:
                    vxConsole.WriteError(this.ToString(), string.Format("'{0}' Key Not Found!", key));
                    return base.GetNewEntity(key);
                    break;
            }
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

            //if (vxEngine.InputManager.MouseState.MiddleButton == ButtonState.Pressed)
            //    Mouse.SetPosition((int)vxEngine.Mouse_ClickPos.X, (int)vxEngine.Mouse_ClickPos.Y);

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
                //grabberGraphic.IsDrawing = false;
            }
			vxConsole.WriteToInGameDebug ("Update");
            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

		public override void DrawGameplayScreen (GameTime gameTime)
		{
			base.DrawGameplayScreen (gameTime);

			vxConsole.WriteToInGameDebug ("Draw");
		}
        //Testing
        public override void SimulationStart()
        {
            vxEngine.InputManager.ShowCursor = false;

            if (SandboxGameState == vxEnumSandboxGameState.EditMode)
            {
                SandboxGameState = vxEnumSandboxGameState.Running;

                // Set the Camera type to chase Camera
                character.Activate();
                Camera.CameraType = CameraType.CharacterFPS;
            }
            base.SimulationStart();
        }
        

        public override void SimulationStop()
        {
            vxEngine.InputManager.ShowCursor = true;
            if (SandboxGameState == vxEnumSandboxGameState.Running)
            {
                //Set Working Plane in its original Position
                workingPlane.Position = Vector3.Up * WrkngPln_HeightDelta;

                SandboxGameState = vxEnumSandboxGameState.EditMode;

                character.Deactivate();
                Camera.CameraType = CameraType.Freeroam;
            }
            base.SimulationStop();
        }
    }
}
