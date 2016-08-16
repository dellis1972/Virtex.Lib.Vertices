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

namespace Virtex.Lib.Vrtc.GUI.GuiArtProvider
{    /// <summary>
     /// The Art Provider for Menu Screen Items. If you want to customize the draw call, then create an inherited class
     /// of this one and override this draw call. 
     /// </summary>
	public class vxMenuItemArtProvider : vxArtProviderBase, IGuiArtProvider
    {
        /// <summary>
        /// Defines whether or not the icon should be shown. The default is false.
        /// </summary>
        public bool ShowIcon { get; set; }

        /// <summary>
        /// Icon Padding.
        /// </summary>
        public Vector2 IconPadding
        {
            get { return _iconPadding;  }
            set { _iconPadding = value; }
        }
        private Vector2 _iconPadding = new Vector2(4);


		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.GuiArtProvider.vxMenuItemArtProvider"/> class.
		/// </summary>
		/// <param name="engine">Engine.</param>
		public vxMenuItemArtProvider(vxEngine engine):base(engine)
        {
            vxEngine = engine;
			ShowIcon = false;
			Padding = new Vector2 (10, 4);

			BackgroundColour = Color.White;
			BackgroundHoverColour = Color.DarkOrange;

			TextColour = Color.Black;
			TextHoverColour = Color.Black;

			DrawBackgroungImage = true;

			BackgroundImage = vxEngine.EngineContentManager.Load<Texture2D>("Gui/DfltThm/vxGUITheme/vxMenuEntry/Bckgrnd_Nrml");
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }


        /// <summary>
        /// The Draw Method for the Menu Screen Art Provider. If you want to customize the draw call, then create an inherited class
        /// of this one and override this draw call. 
        /// </summary>
        /// <param name="guiItem"></param>
        public virtual void Draw(object guiItem)
        {
            //First Cast the GUI Item to be a Menu Entry
            vxMenuEntry menuEntry = (vxMenuEntry)guiItem;

            //Update Rectangle
            menuEntry.BoundingRectangle = new Rectangle(
                (int)(menuEntry.Position.X - Padding.X / 2),
                (int)(menuEntry.Position.Y - Padding.Y / 2),
                (int)(menuEntry.Font.MeasureString(menuEntry.Text).X + 2 * Padding.X),
                (int)(menuEntry.Font.MeasureString(menuEntry.Text).Y + 2 * Padding.Y));

            //Set Opacity from Parent Screen Transition Alpha
            menuEntry.Opacity = menuEntry.ParentScreen.TransitionAlpha;

            //Do a last second null check.
            if (menuEntry.Texture == null)
                menuEntry.Texture = vxEngine.Assets.Textures.Blank;

            //Draw Button
            if (DrawBackgroungImage)
                vxEngine.SpriteBatch.Draw(BackgroundImage, menuEntry.BoundingRectangle, menuEntry.Colour * menuEntry.Opacity);


			vxEngine.SpriteBatch.DrawString(
				menuEntry.Font,
				menuEntry.Text,
				menuEntry.Position + Padding / 2,
				(menuEntry.HasFocus ? TextHoverColour : TextColour) * menuEntry.Opacity);
        }
    }
}
