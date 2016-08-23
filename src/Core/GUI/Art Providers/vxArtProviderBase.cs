using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virtex.Lib.Vrtc.GUI
{
	/// <summary>
	/// Art provider base class which holds common elements such as Padding, Highlight colour etc...
	/// </summary>
	public class vxArtProviderBase
	{
		/// <summary>
		/// The Game Engine Instance.
		/// </summary>
		public vxEngine vxEngine { get; set; }

		/// <summary>
		/// Gets or sets the padding.
		/// </summary>
		/// <value>The padding.</value>
		public Vector2 Padding { get; set; }

		/// <summary>
		/// Gets or sets the margin.
		/// </summary>
		/// <value>The margin.</value>
		public Vector4 Margin { get; set; }

		public float Alpha { get; set; }

		/// <summary>
		/// Gets or sets the text colour.
		/// </summary>
		/// <value>The text colour.</value>
		public Color TextColour { get; set; }

		/// <summary>
		/// Gets or sets the text hover colour.
		/// </summary>
		/// <value>The text hover colour.</value>
		public Color TextHoverColour { get; set; }

		/// <summary>
		/// Gets or sets the background colour.
		/// </summary>
		/// <value>The background colour.</value>
		public Color BackgroundColour { get; set; }

		/// <summary>
		/// Gets or sets the background hover colour.
		/// </summary>
		/// <value>The background hover colour.</value>
		public Color BackgroundHoverColour { get; set; }

		/// <summary>
		/// Gets or sets the opacity of the current GUI Item.
		/// </summary>
		/// <value>The opacity.</value>
		public float Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}
		float opacity = 1;

		/// <summary>
		/// Text Of GUI Item
		/// </summary>
		public SpriteFont Font{get;set;}

		/// <summary>
		/// Gets or sets the background image.
		/// </summary>
		/// <value>The background image.</value>
		public Texture2D BackgroundImage{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="Virtex.Lib.Vrtc.GUI.GuiArtProvider.vxMenuItemArtProvider"/> draw backgroung image.
		/// </summary>
		/// <value><c>true</c> if draw backgroung image; otherwise, <c>false</c>.</value>
		public bool DrawBackgroungImage { get; set; }

		/// <summary>
		/// Gets or sets the width of the border.
		/// </summary>
		/// <value>The width of the border.</value>
		public int BorderWidth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Virtex.Lib.Vrtc.GUI.vxArtProviderBase"/> do border.
		/// </summary>
		/// <value><c>true</c> if do border; otherwise, <c>false</c>.</value>
		public bool DoBorder { get; set; }

		/// <summary>
		/// Gets or sets the default width.
		/// </summary>
		/// <value>The default width.</value>
		public int DefaultWidth { get; set; }

		/// <summary>
		/// Gets or sets the default height.
		/// </summary>
		/// <value>The default height.</value>
		public int DefaultHeight { get; set; }

		public vxArtProviderBase(vxEngine engine)
		{
			this.vxEngine = engine;

			DefaultWidth = 150;
			DefaultHeight = 24;

			Alpha = 1;
		}
	}
}
