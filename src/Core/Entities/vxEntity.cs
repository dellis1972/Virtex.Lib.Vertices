using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Graphics;


namespace Virtex.Lib.Vrtc.Core.Entities
{
    /// <summary>
    /// Base Entity in the Virtex vxEngine which controls all Rendering and Provides
    /// position and world matrix updates to the other required entities.
    /// </summary>
    public class vxEntity : ICloneable
    {
		/// <summary>
		/// This base name which is set internally in the vxEntity to keep track of unique items in the engine.
		/// </summary>
        public string name = "base";

        /// <summary>
        /// Gives reference to the vxEngine from the entity
        /// </summary>
        public vxEngine vxEngine { get; set; }

        

        /// <summary>
        /// The current scene of the game
        /// </summary>
		public vxSceneBase CurrentScene
        {
			get { return vxEngine.CurrentGameplayScreen; }
        }



        /// <summary>
        /// Returns the Current Keyboard State.
        /// </summary>
        public KeyboardState keyboardState { get { return vxEngine.InputManager.KeyboardState; } }

        /// <summary>
        /// Returns the Previous Updates Keyboard State.
        /// </summary>
        public KeyboardState keyboardState_previous { get { return  vxEngine.InputManager.PreviousKeyboardState; } }

        /// <summary>
        /// Current Gamepad state. Fetched at the most recent update() call.
        /// </summary>
        public GamePadState gamePadState { get { return  vxEngine.InputManager.GamePadState; } }

        /// <summary>
        /// The Gamepad state of the previous game loop iteration
        /// </summary>
        public GamePadState lastGamePadState { get { return vxEngine.InputManager.PreviousGamePadState; } }

		/// <summary>
		/// Whether or not too keep Updating the current Entity
		/// </summary>
        public bool KeepUpdating = true;


		/// <summary>
		/// The distortion scale.
		/// </summary>
        public float DistortionScale = 0.05f;
        
		/// <summary>
		/// The distortion technique.
		/// </summary>
		public DistortionTechniques DistortionTechnique = DistortionTechniques.PullIn;
        
		/// <summary>
		/// The distortion blur.
		/// </summary>
		public bool DistortionBlur = true;

		/// <summary>
		/// The texture offset.
		/// </summary>
        public Vector2 TextureOffset = Vector2.Zero;
        
        /// <summary>
        /// Model Used for Distortion
        /// </summary>
		public Texture2D Texture2D_Distorter
        {
			get { return texture2D_Distorter; }
			set { texture2D_Distorter = value; }
        }
		Texture2D texture2D_Distorter;

		/// <summary>
		/// Should it be Rendered in Debug
		/// </summary>
        public bool RenderEvenInDebug = false;

        /// <summary>
        /// A reference to the graphics device used to access the viewport for touch input.
        /// </summary>
        private GraphicsDevice graphicsDevice;

		/// <summary>
		/// Is there something to render, or is this just
		/// an empty placeholder.
		/// </summary>
        public bool ShouldDraw = true;


        /// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Core.Entities.vxEntity"/> class. The Base Entity Object for the vxEngine.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public vxEntity(vxEngine vxEngine)
        {
            //Give Access to the vxEngine
            this.vxEngine = vxEngine;

            //Add the Entity to the Main Loop List
            vxEngine.CurrentGameplayScreen.Entities.Add(this);
        }


        /// <summary>
        /// Clones this Entity.
        /// </summary>
        /// <returns>A Clone copy of this object</returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }


        /// <summary>
        /// Update the Entity.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public virtual void Update(GameTime gameTime)
        {
           
        }

		/// <summary>
		/// Disposes the entity.
		/// </summary>
        public virtual void DisposeEntity()
        {
            //this.vxEngine.CurrentGameplayScreen.List_Entities.Remove(this);
        }

        /// <summary>
        /// Updates the position technique.
        /// </summary>
        public virtual void UpdatePositionTechnique()
        {

        }
    }
}

