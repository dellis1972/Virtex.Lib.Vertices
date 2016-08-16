using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;
using Microsoft.Xna.Framework.Content;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    
	/// <summary>
	/// Toolbar Button Controls for the vxToolbar Class.
	/// </summary>
    public class vxToolbarButton : vxGUIBaseItem
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
        Texture2D HoverButtonImage { get; set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxToolbarButton"/> class. Note the texutres
		/// are loaded by the Games Content manager.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="TexturesPath">Path to the textures, note a 'hover texture' must be present with a '_hover' suffix</param>
		public vxToolbarButton(vxEngine vxEngine, string TexturesPath):this(vxEngine, vxEngine.Game.Content, TexturesPath)
		{

		}

        /// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxToolbarButton"/> class.
        /// </summary>
        /// <param name="vxEngine">The current Vertices vxEngine Instance</param>
		/// <param name="Content">Content Manager too load the Textures with.</param>
        /// <param name="TexturesPath">Path to the textures, note a 'hover texture' must be present with a '_hover' suffix</param>
		public vxToolbarButton(vxEngine vxEngine, ContentManager Content, string TexturesPath)
        {
            //Get the current Game vxEngine
            this.vxEngine = vxEngine;

            //Position is Set by Toolbar
            Position = Vector2.Zero;

            //Set Button Images
			ButtonImage = Content.Load<Texture2D>(TexturesPath);
			HoverButtonImage = Content.Load<Texture2D>(TexturesPath + "_hover");

            //Set Initial Bounding Rectangle
            Width = ButtonImage.Width;
            Height = ButtonImage.Height;
            BoundingRectangle = new Rectangle(0, 0, Width, Height);

            //Set Default Colours
            Color_Normal = Color.White;
            Color_Highlight = Color.DarkOrange;
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //Update Rectangle
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), Width, Height + Padding / 2);
            
            //Draw Button            
            vxEngine.SpriteBatch.Begin();

            //Draw Regular Image
            vxEngine.SpriteBatch.Draw(ButtonImage, BoundingRectangle, Enabled ? Color_Normal : Color.Gray);

			if (Enabled) {

				//Draw Hover Items
				vxEngine.SpriteBatch.Draw (vxEngine.Assets.Textures.Blank, BoundingRectangle, Color_Highlight * HoverAlpha);

				if (HoverButtonImage != null)
					vxEngine.SpriteBatch.Draw (HoverButtonImage, BoundingRectangle, Color_Normal * HoverAlpha);
			}
                
            vxEngine.SpriteBatch.End();
        }
    }
}
