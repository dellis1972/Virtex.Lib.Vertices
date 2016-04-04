using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core;

namespace vxVertices.GUI.Controls
{
	/// <summary>
	/// Button which has no text, only an Image.
	/// </summary>
	public class vxButtonImage : vxGUIBaseItem
    {
		/// <summary>
		/// Gets or sets the button image.
		/// </summary>
		/// <value>The button image.</value>
        public Texture2D ButtonImage { get; set; }

        /// <summary>
        /// Gets or sets the button image.
        /// </summary>
        /// <value>The button image.</value>
        public Texture2D HoverButtonImage { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxButtonImage"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="position">Position.</param>
		/// <param name="buttonImage">Button image.</param>
		/// <param name="hoverImage">Hover image.</param>
		public vxButtonImage(vxEngine vxEngine, 
			Vector2 position, 
			Texture2D buttonImage, 
			Texture2D hoverImage):
		this(vxEngine, 
				position,
				buttonImage,
				hoverImage,
				buttonImage.Width,
				buttonImage.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxButtonImage"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="position">Position.</param>
		/// <param name="buttonImage">Button image.</param>
		/// <param name="hoverImage">Hover image.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public vxButtonImage(vxEngine vxEngine, 
			Vector2 position, 
			Texture2D buttonImage, 
			Texture2D hoverImage,
			int width,
			int height)
        {
            //Get the current Game vxEngine
            this.vxEngine = vxEngine;

            //Set Position
			Position = position;
			OriginalPosition = position;

            //Set Button Images
			ButtonImage = buttonImage;
			HoverButtonImage = hoverImage;

            //Set Initial Bounding Rectangle
			Width = width;
            Height = height;
            BoundingRectangle = new Rectangle(0, 0, Width, Height);

            //Set Default Colours
            Color_Normal = Color.White;
            Color_Highlight = Color.DarkOrange;
        }
        public override void Update(MouseState mouseState)
        {
            base.Update(mouseState);

            //Update Rectangle
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), Width, Height + Padding / 2);

        }
        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            //Draw Button            
            vxEngine.SpriteBatch.Begin();
            this.DrawByOwner(vxEngine);                
            vxEngine.SpriteBatch.End();
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            //Draw Regular Image
            vxEngine.SpriteBatch.Draw(ButtonImage, BoundingRectangle, Color_Normal);

            //Draw Hover Items
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color_Highlight * HoverAlpha);


            if (HoverButtonImage != null)
                vxEngine.SpriteBatch.Draw(HoverButtonImage, BoundingRectangle, Color_Normal * HoverAlpha);

        }
    }
}
