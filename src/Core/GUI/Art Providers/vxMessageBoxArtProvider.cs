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
using Virtex.Lib.Vrtc.GUI.MessageBoxs;

namespace Virtex.Lib.Vrtc.GUI.GuiArtProvider
{    /// <summary>
     /// The Art Provider for Menu Screen Items. If you want to customize the draw call, then create an inherited class
     /// of this one and override this draw call. 
     /// </summary>
	public class vxMessageBoxArtProvider : vxArtProviderBase, IGuiArtProvider
    {
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
		/// Gets or sets the form bounding rectangle.
		/// </summary>
		/// <value>The form bounding rectangle.</value>
		public Rectangle FormBoundingRectangle {get; set;}


		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.GuiArtProvider.vxMenuItemArtProvider"/> class.
		/// </summary>
		/// <param name="engine">Engine.</param>
		public vxMessageBoxArtProvider(vxEngine engine):base(engine)
        {
            vxEngine = engine;

			TitleAlpha = 1;
			Alpha = 1;

			TitlePadding = new Vector2 (10, 10);
			Padding = new Vector2 (10, 10);

			BackgroundColour = Color.Black * 0.75f;
			TitleBackgroundColour = Color.DarkOrange;
			BackgroundHoverColour = Color.DarkOrange;

            TitleTextColour = Color.Black;
            TextColour = Color.White;
			TextHoverColour = Color.Black;

			TitleBackgroundImage = vxEngine.Assets.Textures.Blank;
			BackgroundImage = vxEngine.Assets.Textures.Blank;// vxEngine.EngineContentManager.Load<Texture2D>("Gui/DfltThm/vxGUITheme/vxMenuEntry/Bckgrnd_Nrml");

        }

		/// <summary>
		/// Clone this instance.
		/// </summary>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

		/// <summary>
		/// Gets the size of the text.
		/// </summary>
		/// <returns>The text size.</returns>
		/// <param name="text">Text.</param>
		public Vector2 GetTextSize(string text)
		{
			if (Font != null) {
				return Font.MeasureString (text);
			} else
				return new Vector2 (10, 10);
		}

        /// <summary>
        /// The Draw Method for the Menu Screen Art Provider. If you want to customize the draw call, then create an inherited class
        /// of this one and override this draw call. 
        /// </summary>
        /// <param name="guiItem"></param>
        public virtual void Draw(object guiItem)
        {
			//Set Font
			Font = vxEngine.vxGUITheme.Font;

			//First Cast the GUI Item to be a Menu Entry
			vxMessageBox msgBox = (vxMessageBox)guiItem;

			TitlePosition = msgBox.textTitlePosition;

			Vector2 TitleSize = GetTextSize(msgBox.Title);

			FormBoundingRectangle = new Rectangle (
				(int)(msgBox.backgroundRectangle.X - Padding.X),
				(int)(msgBox.backgroundRectangle.Y - Padding.Y),
				(int)(msgBox.backgroundRectangle.Width + 2 * Padding.X),
				(int)(msgBox.backgroundRectangle.Height + 2 * Padding.Y));

			TitleBoundingRectangle = new Rectangle (
				(int)(TitlePosition.X - TitlePadding.X),
				(int)(TitlePosition.Y - TitlePadding.Y),
				(int)(TitleSize.X + 2 * TitlePadding.X),
				(int)(TitleSize.Y + 2 * TitlePadding.Y));
			

			vxEngine.SpriteBatch.Begin();


			// Draw the message box text.
			vxEngine.SpriteBatch.Draw(BackgroundImage, FormBoundingRectangle, BackgroundColour * Alpha * msgBox.TransitionAlpha);
			vxEngine.SpriteBatch.DrawString(Font, msgBox.message, msgBox.textPosition, TextColour* Alpha * msgBox.TransitionAlpha);


			// Draw the Title
			vxEngine.SpriteBatch.Draw(TitleBackgroundImage, TitleBoundingRectangle, TitleBackgroundColour * TitleAlpha* msgBox.TransitionAlpha);
			vxEngine.SpriteBatch.DrawString(Font, msgBox.Title, TitlePosition, TitleTextColour * TitleAlpha* msgBox.TransitionAlpha);

			vxEngine.SpriteBatch.End();
        }
    }
}
