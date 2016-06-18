using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core;

namespace vxVertices.GUI.Themes
{
	public class vxLoadingScreen : vxBaseItemTheme {

		public Texture2D SplashScreen { get; set; }

		public float PercentageComplete { get; set; }

		public Vector2 Position { get; set; }

		public vxLoadingScreen(vxEngine Engine) :base(Engine)
		{
			BackgroundColour = Color.LightGray;
			TextColour = Color.Black;

			Position = new Vector2 (0, 0);
		}
	}
}