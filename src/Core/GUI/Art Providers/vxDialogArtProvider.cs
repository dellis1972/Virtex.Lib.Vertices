using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.GUI.Dialogs;

namespace Virtex.Lib.Vrtc.GUI.GuiArtProvider
{
	public class vxDialogArtProvider : vxArtProviderBase, IGuiArtProvider
	{

		#region Title Properties
		/// <summary>
		/// Gets or sets the title background image.
		/// </summary>
		/// <value>The title background image.</value>
		public Texture2D TitleBackgroundImage {get; set;}

		/// <summary>
		/// Gets or sets the title text colour.
		/// </summary>
		/// <value>The title text colour.</value>
		public Color TitleTextColour { get; set; }

		/// <summary>
		/// Gets or sets the color of the title background.
		/// </summary>
		/// <value>The color of the title background.</value>
		public Color TitleBackgroundColour { get; set; }

		/// <summary>
		/// Gets or sets the title alpha.
		/// </summary>
		/// <value>The title alpha.</value>
		public float TitleAlpha { get; set; }

		/// <summary>
		/// Gets or sets the form bounding rectangle.
		/// </summary>
		/// <value>The form bounding rectangle.</value>
		public Rectangle TitleBoundingRectangle {get; set;}

		/// <summary>
		/// Gets or sets the title padding.
		/// </summary>
		/// <value>The title padding.</value>
		public Vector2 TitlePadding { get; set; }

		/// <summary>
		/// Gets or sets the title position.
		/// </summary>
		/// <value>The title position.</value>
		public Vector2 TitlePosition { get; set; }

		/// <summary>
		/// Gets or sets the border of the title rectangle.
		/// </summary>
		/// <value>The brdr title rectangle.</value>
		public Rectangle brdr_TitleRectangle { get; set; }
		#endregion


		#region Main Form Properties


		/// <summary>
		/// This is the bounding rectangle which all controls should be kept within.
		/// </summary>
		public Rectangle BoundingGUIRectangle { get; set; }

		/// <summary>
		/// Gets or sets the border of that background rectangle.
		/// </summary>
		/// <value>The brdr background rectangle.</value>
		public Rectangle brdr_backgroundRectangle { get; set; }

		/// <summary>
		/// The background margin.
		/// </summary>
		/// <remarks>X, Y, Z, W = Left, Top, Right, Bottom</remarks>
		public Rectangle FormBackground { get; set; }

		/// <summary>
		/// The position offset for the gui panel.
		/// </summary>
		public Vector2 PosOffset = new Vector2 (0);

		#endregion

		public Viewport viewport;
		public Vector2 textTitleSize;

		public Vector2 textTitlePosition;

		public vxDialogArtProvider (vxEngine engine):base(engine)
		{
			DefaultWidth = 150;
			DefaultHeight = 24;

			Alpha = 1;

			TitleTextColour = Color.Black;
			TitleAlpha = 1;
			TitlePadding = new Vector2 (10, 10);
			TitleBackgroundColour = Color.DarkOrange;

			Padding = new Vector2 (10, 10);

			Margin = new Vector4 (0);

			DoBorder = true;
			BorderWidth = 2;
			TextColour = Color.Black;
			BackgroundColour = Color.Black * 0.75f;

			BackgroundImage = vxEngine.Assets.Textures.Blank;
			TitleBackgroundImage = vxEngine.Assets.Textures.Blank;



			textTitleSize = engine.Font.MeasureString("A");

			SetBounds();
		}

		public override void SetBounds()
		{
			base.SetBounds();

			viewport = vxEngine.GraphicsDevice.Viewport;

			TitleBoundingRectangle = new Rectangle(
				(int)Padding.X,
				(int)Padding.Y,
				(int)(viewport.Width - Padding.X * 2),
				(int)(textTitleSize.Y + Padding.Y * 2));

			brdr_TitleRectangle = new Rectangle(
				(int)Padding.X - BorderWidth,
				(int)Padding.Y - BorderWidth,
				TitleBoundingRectangle.Width + BorderWidth * 2,
				TitleBoundingRectangle.Height + BorderWidth * 2);

			textTitlePosition = new Vector2(
				TitleBoundingRectangle.X,
				TitleBoundingRectangle.Y + Padding.Y);

			BoundingGUIRectangle = new Rectangle(
				(int)(textTitlePosition.X),
				(int)(textTitlePosition.Y + TitleBoundingRectangle.Height),
				(int)(viewport.Width - Padding.X * 2),
				(int)(viewport.Height - Padding.Y - TitleBoundingRectangle.Height - textTitlePosition.Y));

			FormBackground = new Rectangle(
				(int)(BoundingGUIRectangle.X - Margin.X),
				(int)(BoundingGUIRectangle.Y - Margin.Y),
				(int)(BoundingGUIRectangle.Width + Margin.X + Margin.Z),
				(int)(BoundingGUIRectangle.Height + Margin.Y + Margin.W));
		}


		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public virtual void Draw(object guiItem)
		{
			vxDialogBase dialog = (vxDialogBase)guiItem;

			this.Font = vxEngine.vxGUITheme.Font;

			viewport = vxEngine.GraphicsDevice.Viewport;

			// Center the message text in the viewport.
			//viewport = vxEngine.GraphicsDevice.Viewport;
			textTitleSize = Font.MeasureString(dialog.Title);


			// Darken down any other screens that were drawn beneath the popup.
			vxEngine.FadeBackBufferToBlack(dialog.TransitionAlpha * 2 / 3);


			vxEngine.SpriteBatch.Begin();

			// Draw the message box text.
			vxEngine.SpriteBatch.Draw(BackgroundImage, FormBackground, BackgroundColour);

			// Draw the Title
			vxEngine.SpriteBatch.Draw(TitleBackgroundImage, brdr_TitleRectangle, Color.Black);
			vxEngine.SpriteBatch.Draw(TitleBackgroundImage, TitleBoundingRectangle, TitleBackgroundColour);
			vxEngine.SpriteBatch.DrawString(Font, dialog.Title, textTitlePosition, TitleTextColour);

			vxEngine.SpriteBatch.End();
		}
	}
}

