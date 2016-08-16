using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    public class vxToolbarSpliter : vxGUIBaseItem
    {
		public Texture2D Texture { get; set; }

        /// <summary>
        /// Splitter for vxGUI Toolbar
        /// </summary>
        /// <param name="vxEngine"></param>
        /// <param name="buttonImage"></param>
        /// <param name="width"></param>
        public vxToolbarSpliter(vxEngine vxEngine, Texture2D buttonImage, int width)
        {
            //Get the current Game vxEngine
            vxEngine = vxEngine;

            //Position is Set by Toolbar
            Position = Vector2.Zero;

            //Set Button Images
			Texture = buttonImage;

            //Set Initial Bounding Rectangle
            Width = width;
            Height = 48;
			BoundingRectangle = new Rectangle(0, 0, Texture.Width, BoundingRectangle.Height);

            Color_Normal = Color.Black;
            Color_Highlight = Color.Black;
        }

        /// <summary>
        /// Splitter for vxGUI Toolbar
        /// </summary>
        /// <param name="vxEngine"></param>
        /// <param name="width"></param>
        public vxToolbarSpliter(vxEngine vxEngine, int width)
            : this(vxEngine, vxEngine.Assets.Textures.Blank, width)
        {

        }

        int clickCapture() { return 0; }

        public override void Update(vxEngine vxEngine)
        { }

        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //Update Rectangle
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), Width, Height + Padding / 2);

            HoverAlpha = 0.75f;

            //Draw Button
            vxEngine.SpriteBatch.Begin();
			vxEngine.SpriteBatch.Draw(Texture, BoundingRectangle, Color_Normal * HoverAlpha);
            vxEngine.SpriteBatch.End();
        }
    }
}
