//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Virtex.Lib.Vrtc.Core;
//
//namespace Virtex.Lib.Vrtc.GUI.Themes
//{
//	public class vxMenuEntryTheme : vxBaseItemTheme {
//
//		public int vxMenuItemWidth { get; set; }
//		public int vxMenuItemHeight { get; set; }
//
//		public TextJustification TextJustification;
//		public Texture2D vxMenuItemBackground { get; set; }
//		public Texture2D vxMenuSplitterTexture { get; set; }
//
//
//        //Title Code
//		public Vector2 TitlePosition { get; set; }
//		public Vector2 TitlePadding { get; set; }
//		public Texture2D TitleBackground { get; set; }
//		public Color TitleBackgroundColor { get; set; }
//		public Color TitleColor { get; set; }
//
//		public Vector2 BoundingRectangleOffset { get; set; }
//
//		public vxMenuEntryTheme(vxEngine Engine) : base(Engine)
//		{
//			TextJustification = TextJustification.Center;
//
//			FineTune = new Vector2 (0, 5);
//
//			vxMenuItemWidth = 100;
//			vxMenuItemHeight = 34;
//
//			TitleColor = Color.White;
//			TitleBackgroundColor = Color.White;
//			TitlePadding = new Vector2 (10, 10);
//			TitlePosition = new Vector2(Engine.GraphicsDevice.Viewport.Width / 2, 80);
//
//			BoundingRectangleOffset = new Vector2(0,0);
//
//            DrawBackgroungImage = true;
//        } 
//	}
//}
//
