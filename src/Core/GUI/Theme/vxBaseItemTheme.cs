using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Themes
{
	public class vxBaseItemTheme
	{
        public vxEngine Engine;

        public int Width { get; set; }
        public int Height { get; set; }

        public Color TextColour { get; set; }
		public Color TextHover { get; set; }

		public Color BackgroundColour { get; set; }
		public Color BackgroundHoverColour { get; set; }


		public Vector2 Margin { get; set; }
		public Vector2 Padding { get; set; }
		public Vector2 FineTune { get; set; }

        public Texture2D BackgroundImage { get; set; }

        public int BorderWidth { get; set; }
        public bool DoBorder { get; set; }

        public vxBaseItemTheme(vxEngine Engine)
		{
            this.Engine = Engine;

            BackgroundImage = Engine.Assets.Textures.Blank;

			Margin = new Vector2 (10, 10);
            Padding = new Vector2 (10, 10);
			FineTune = new Vector2 (0);

            int s = 35;
			TextColour = new Color(s, s, s, 255);
			TextHover = Color.Black;

			BackgroundColour = Color.Gray;
			BackgroundHoverColour = Color.DarkOrange;

            BorderWidth = 1;
            DoBorder = false;
        }        
	}
}

