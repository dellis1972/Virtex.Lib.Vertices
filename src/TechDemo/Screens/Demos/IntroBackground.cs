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
using vxVertices.Core.Entities;
using vxVertices.Utilities;
using vxVertices.Physics.BEPU.Entities.Prefabs;
using vxVertices.Graphics;


namespace VerticeEnginePort.Base
{
	/// <summary>
	/// This is the main class for the game. It holds the instances of the sphere simulator,
	/// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
	/// game loop, and provides keyboard and mouse input.
	/// </summary>
	public class IntroBackground : vxScene3D
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

		public IntroBackground()
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

		}

		xEnvrio d;
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public override void LoadContent()
		{
			//Load Global Content First
			if (vxEngine.HasContentBeenLoaded == false)
				vxEngine.LoadGlobalContent(vxEngine.Game.Content);

			InitialiseLevel();

			d = new xEnvrio(vxEngine, vxEngine.LoadModel("Models/courtyard/td_courtyard"), Vector3.Zero);
			//d.NormalMap = vxEngine.Game.Content.Load<Texture2D>("Models/courtyard/crtyrd_bricks_nm");
			//d.SpecularMap = vxEngine.Game.Content.Load<Texture2D>("Models/courtyard/crtyrd_bricks_sm");
			d.SpecularIntensity = 1;


			int size = 100;
			Box baseBox = new Box (new Vector3(0, -5, 0), size, 10, size);
			BEPUPhyicsSpace.Add(baseBox);

			///////////////////////////////////////////////////////////////////////
			//Initialise Camera Code
			///////////////////////////////////////////////////////////////////////
			#region Set Up Camera

			base.LoadContent();
//			Camera.CameraType = CameraType.CharacterFPS;
//
//			Camera.Position = new Vector3(0,0,0);
//
//			character = new CharacterControllerInput(BEPUPhyicsSpace, Camera, vxEngine);
//
//			//Since this is the character playground, turn on the character by default.
//			character.Activate();
//
//			//Having the character body visible would be a bit distracting.
//			character.CharacterController.Body.Tag = "noDisplayObject";


			Camera.CameraType = CameraType.Orbit;
			Camera.OrbitTarget = new Vector3(0,1.5f,0);
			//Camera.OrbitZoom = -375;

			//
			//Grabbers
			//
			grabber = new MotorizedGrabSpring();
			BEPUPhyicsSpace.Add(grabber);
			rayCastFilter = RayCastFilter;

			IsPausable = false;

			ConcreteCube cc = new ConcreteCube((GameEngine)vxEngine, new Vector3(0, 5, 0));
			cc.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, 0)), true, true);

			cc = new ConcreteCube((GameEngine)vxEngine, new Vector3(0, 5, 4f));
			cc.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, 5f)), true, true);

			cc = new ConcreteCube((GameEngine)vxEngine, new Vector3(0, 5, 4f));
			cc.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, -5f)), true, true);

			#endregion
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
//			character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, vxEngine.InputManager.PreviousKeyboardState,
//				vxEngine.InputManager.KeyboardState, vxEngine.InputManager.PreviousGamePadState, vxEngine.InputManager.GamePadState);
			if (vxEngine.InputManager.MouseState.MiddleButton == ButtonState.Pressed) {
				angle = Camera.Yaw;
			} else {
				angle += 0.001f;
				//d.World = Matrix.CreateScale(1.0f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(angle);
				Camera.Yaw = angle;
			}

            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}
	}
}
