#region File Description
//-----------------------------------------------------------------------------
// Animation.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;


#endregion

using System;
using Microsoft.Xna.Framework.Graphics;

namespace vxVertices.Graphics.Animation
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
	/// Provides the information of a 2 dimension Animation Sprite Texture.
    /// </remarks>
    public class vxAnimationSprite2D
    {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

		/// <summary>
		/// Gets the number of rows in the sprite sheet.
		/// </summary>
		/// <value>The number of rows.</value>
		public int NumOfRows
		{
			get { return numOfRows; }
		}
		int numOfRows;

		/// <summary>
		/// Gets the number of columns in the sprite sheet.
		/// </summary>
		/// <value>The number of cols.</value>
		public int NumOfCols
		{
			get { return numOfCols; }
		}
		int numOfCols;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
        {
			get { return frameCount; }
        }
		int frameCount;

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Width/NumOfCols; }
        }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height/NumOfRows; }
        }

		/// <summary>
		/// Texture Offset from Owning Entity Position.
		/// </summary>
		/// <value>The offset.</value>
		public Vector2 Offset
		{
			get { return offset; }
		}
		Vector2 offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="vxVertices.Graphics.Animation.vxAnimationSprite2D"/> class.
        /// </summary>
        /// <param name="texture">The main Sprite Texture.</param>
        /// <param name="frameCount">Number of frames in the Sprite Texture.</param>
		/// <param name="numOfRows">Number of rows in the Sprite Texture.</param>
		/// <param name="numOfCols">Number of columns in the Sprite Texture.</param>
		/// <param name="offset">Texture Offset from Owning Entity Position.</param>
        /// <param name="frameTime">Frame time.</param>
        /// <param name="isLooping">If set to <c>true</c> is looping.</param>
		public vxAnimationSprite2D(Texture2D texture, int frameCount, int numOfRows, int numOfCols, Vector2 offset, float frameTime, bool isLooping)
        {
			this.texture = texture;

			this.frameCount = frameCount;

			this.numOfRows = numOfRows;
			this.numOfCols = numOfCols;

			this.offset = offset;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }
}
