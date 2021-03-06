using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Themes
{
	public class vxThemeDialog : vxBaseItemTheme {

        public int Header_Width { get; set; }
        public int Header_Height { get; set; }

        public Color Header_TextColour { get; set; }
        public Color Header_TextHover { get; set; }

        public Color Header_BackgroundColour { get; set; }
        public Color Header_BackgroundHoverColour { get; set; }

        public Vector2 Header_Padding { get; set; }
        public Vector2 Header_FineTune { get; set; }

        public Texture2D Header_BackgroundImage { get; set; }

        public int Header_BorderWidth { get; set; }
        public bool Header_DoBorder { get; set; }

        public TextJustification TextJustification;

        public vxThemeDialog(vxEngine Engine) :base(Engine)
		{
            Width = 150;
            Height = 24;

            TextJustification = TextJustification.Center;

            BackgroundColour = Color.LightGray;
            BackgroundHoverColour = Color.White;

            TextColour = Color.White;
            TextHover = Color.White;


            Header_BackgroundImage = Engine.Assets.Textures.Blank;

            Header_Padding = new Vector2(10, 10);
            Header_FineTune = new Vector2(0);

            int s = 35;
            Header_TextColour = new Color(s, s, s, 255);
            Header_TextHover = Color.Black;

            Header_BackgroundColour = Color.Gray;
            Header_BackgroundHoverColour = Color.DarkOrange;

            Header_BorderWidth = 1;
            Header_DoBorder = false;
        }
    }
}

