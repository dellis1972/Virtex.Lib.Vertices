using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI.Controls;

namespace Virtex.Lib.Vertices.GUI.GuiArtProvider
{    /// <summary>
     /// The Art Provider for Menu Screen Items. If you want to customize the draw call, then create an inherited class
     /// of this one and override this draw call. 
     /// </summary>
    public class vxMenuItemArtProvider : IGuiArtProvider
    {
        /// <summary>
        /// The Game Engine Instance.
        /// </summary>
        public vxEngine vxEngine { get; }

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

        public vxMenuItemArtProvider(vxEngine engine)
        {
            vxEngine = engine;
            ShowIcon = false;
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
                (int)(menuEntry.Position.X - vxEngine.vxGUITheme.vxMenuEntries.Padding.X / 2),
                (int)(menuEntry.Position.Y - vxEngine.vxGUITheme.vxMenuEntries.Padding.Y / 2),
                (int)(menuEntry.Font.MeasureString(menuEntry.Text).X + 2 * vxEngine.vxGUITheme.vxMenuEntries.Padding.X),
                (int)(menuEntry.Font.MeasureString(menuEntry.Text).Y + 2 * vxEngine.vxGUITheme.vxMenuEntries.Padding.Y));

            //Set Opacity from Parent Screen Transition Alpha
            menuEntry.Opacity = menuEntry.ParentScreen.TransitionAlpha;

            //Do a last second null check.
            if (menuEntry.Texture == null)
                menuEntry.Texture = vxEngine.Assets.Textures.Blank;

            //Draw Button
            if (vxEngine.vxGUITheme.vxMenuEntries.DrawBackgroungImage)
                vxEngine.SpriteBatch.Draw(menuEntry.Texture, menuEntry.BoundingRectangle, menuEntry.Colour * menuEntry.Opacity);


            //Set Text Colour Based on Focus
            menuEntry.Colour_Text = menuEntry.HasFocus ? vxEngine.vxGUITheme.vxMenuEntries.TextHover : vxEngine.vxGUITheme.vxMenuEntries.TextColour;

            //Update Left Justifications
            if (vxEngine.vxGUITheme.vxMenuEntries.TextJustification == TextJustification.Left)
            {
                vxEngine.SpriteBatch.DrawString(menuEntry.Font, menuEntry.Text,
                    new Vector2(
                        menuEntry.Position.X + menuEntry.Padding,
                        menuEntry.Position.Y + menuEntry.Padding),
                    menuEntry.Colour_Text * menuEntry.Opacity);

            }

            //Update Center Justifications
            else if (vxEngine.vxGUITheme.vxMenuEntries.TextJustification == TextJustification.Center)
            {

                vxEngine.SpriteBatch.DrawString(
                        menuEntry.Font,
                        menuEntry.Text,
                        menuEntry.Position + vxEngine.vxGUITheme.vxMenuEntries.Padding / 2,
                        menuEntry.Colour_Text * menuEntry.Opacity);
            }
        }
    }
}
