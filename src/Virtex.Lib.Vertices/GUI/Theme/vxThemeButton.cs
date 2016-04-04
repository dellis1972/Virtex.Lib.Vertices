using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core;

namespace vxVertices.GUI.Themes
{
	public class vxThemeButton : vxBaseItemTheme {

		public TextJustification TextJustification;

        public vxThemeButton(vxEngine Engine) :base(Engine)
		{
            Width = 150;
            Height = 24;

            TextJustification = TextJustification.Center;

            BackgroundColour = Color.LightGray;
            BackgroundHoverColour = Color.White;

            TextColour = Color.White;
            TextHover = Color.White;
        }
    }
}

