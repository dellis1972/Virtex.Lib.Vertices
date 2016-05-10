using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace vxVertices.Core.Cameras.Controllers
{
    /// <summary>
    /// Superclass of implementations which control the behavior of a camera.
    /// </summary>
    public abstract class CameraControlScheme
    {
        /// <summary>
        /// Gets the game associated with the camera.
        /// </summary>
        public vxEngine vxEngine { get; private set; }

        /// <summary>
        /// Gets the camera controlled by this control scheme.
        /// </summary>
        public vxCamera3D Camera { get; private set; }

		protected CameraControlScheme(vxCamera3D camera, vxEngine vxEngine)
        {
            Camera = camera;
            this.vxEngine = vxEngine;
        }

        /// <summary>
        /// Updates the camera state according to the control scheme.
        /// </summary>
        /// <param name="dt">Time elapsed since previous frame.</param>
        public virtual void Update(float dt)
        {
            /*
            //Only turn if the mouse is controlled by the game.
            if (vxEngine._input.ShowCursor)
            {
                Camera.Yaw += ((int)vxEngine.Mouse_ClickPos.X - vxEngine.CurrentGameplayScreen.mouseInput.X) * dt * .12f;
                Camera.Pitch += ((int)vxEngine.Mouse_ClickPos.Y - vxEngine.CurrentGameplayScreen.mouseInput.Y) * dt * .12f;
            }
             */
        }
    }
}
