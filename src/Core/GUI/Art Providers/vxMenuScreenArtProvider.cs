using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.GUI.GuiArtProvider;
using Virtex.Lib.Vertices.Screens.Menus;

namespace Virtex.Lib.Vertices.GUI.GuiArtProvider
{
    /// <summary>
    /// The Art Provider for Menu Screens. If you want to customize the draw call, then create an inherited class
    /// of this one and override this draw call. 
    /// </summary>
    public class vxMenuScreenArtProvider : IGuiArtProvider
    {
        /// <summary>
        /// The Game Engine Instance.
        /// </summary>
        public vxEngine vxEngine { get; }

        /// <summary>
        /// The Owning Menu Screen.
        /// </summary>
        public vxMenuBaseScreen MenuScreen { get; set; }


        /// <summary>
        /// Gets or sets the menu start position.
        /// </summary>
        /// <value>The menu start position.</value>
        public Vector2 MenuStartPosition
        {
            get { return menuStartPosition; }
            set { menuStartPosition = value; }
        }
        Vector2 menuStartPosition = new Vector2(200, 200);
        Vector2 position = new Vector2(0, 0);

        /// <summary>
        /// Gets or sets the offset between Menu Item
        /// </summary>
        /// <value>The offset for the next menu item.</value>
        public Vector2 NextMenuItemOffset
        {
            get { return nextMenuItemOffset; }
            set { nextMenuItemOffset = value; }
        }
        Vector2 nextMenuItemOffset = new Vector2(0, 0);


        /// <summary>
        /// Title Position.
        /// </summary>
        public Vector2 TitlePosition
        {
            get { return titlePosition; }
            set { titlePosition = value; }
        }
        public Vector2 titlePosition = new Vector2(0, 0);

        /// <summary>
        /// Is there a background image on the title
        /// </summary>
        public bool DoTitleBackground
        {
            get { return doTitleBackground; }
            set { doTitleBackground = value; }
        }
        public bool doTitleBackground = true;



        /// <summary>
        /// Constructor for the Menu Screen Art Provider.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="MenuScreen"></param>
        public vxMenuScreenArtProvider(vxEngine engine)
        {
            vxEngine = engine;
            this.MenuScreen = MenuScreen;

            //Set up default values
            titlePosition = new Vector2(vxEngine.GraphicsDevice.Viewport.Width / 2, 80);            
            menuStartPosition = new Vector2(200, 200);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// The Draw Method for the Menu Item Art Provider. If you want to customize the draw call, then create an inherited class
        /// of this one and override this draw call. 
        /// </summary>
        /// <param name="guiItem"></param>
        public virtual void Draw(object menuScreen)
        {
            MenuScreen = (vxMenuBaseScreen)menuScreen;

            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = vxEngine.GraphicsDevice;
            SpriteBatch spriteBatch = vxEngine.SpriteBatch;
            SpriteFont font = vxEngine.vxGUITheme.Font;

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(MenuScreen.TransitionPosition, 2);
            
            float titleScale = 1;

            Rectangle BoundingRectangle = new Rectangle(
                (int)(TitlePosition.X - font.MeasureString(MenuScreen.MenuTitle).X / 2 - vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.X),
                (int)(TitlePosition.Y - font.MeasureString(MenuScreen.MenuTitle).Y / 2 - vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.Y),
                (int)(font.MeasureString(MenuScreen.MenuTitle).X + vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.X),
                (int)(font.MeasureString(MenuScreen.MenuTitle).Y + vxEngine.vxGUITheme.vxMenuEntries.TitlePadding.Y));

            if (DoTitleBackground)
            {
                vxEngine.SpriteBatch.Draw(vxEngine.vxGUITheme.vxMenuEntries.TitleBackground,
                    BoundingRectangle, vxEngine.vxGUITheme.vxMenuEntries.TitleBackgroundColor * MenuScreen.TransitionAlpha);
            }

            spriteBatch.DrawString(font, MenuScreen.MenuTitle, TitlePosition,
                vxEngine.vxGUITheme.vxMenuEntries.TitleColor * MenuScreen.TransitionAlpha,
                0, Vector2.Zero, titleScale, SpriteEffects.None, 0);
        }



        /// <summary>
        /// Updates the Position of all Menu Entries. For custom layout, override this method.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(MenuScreen.TransitionPosition, 2);

            //Set the Top Menu Start Position
            position = menuStartPosition;

            // update each menu entry's location in turn
            for (int i = 0; i < MenuScreen.MenuEntries.Count; i++)
            {
                vxMenuEntry vxMenuEntry = MenuScreen.MenuEntries[i];
                NextMenuItemOffset = new Vector2(0, vxMenuEntry.Height + vxEngine.vxGUITheme.vxMenuEntries.Margin.Y);

                //Set Menu Item Location
                if (vxEngine.vxGUITheme.vxMenuEntries.TextJustification == Virtex.Lib.Vertices.GUI.TextJustification.Left)
                    position.X = menuStartPosition.X;// -vxMenuEntry.Width / 2;
                else if (vxEngine.vxGUITheme.vxMenuEntries.TextJustification == Virtex.Lib.Vertices.GUI.TextJustification.Center)
                    position.X = menuStartPosition.X - vxMenuEntry.Width / 2;

                if (MenuScreen.ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                vxMenuEntry.Position = position;

                // move down for the next entry the size of this entry
                position += NextMenuItemOffset;


                this.vxEngine.InputManager.ShowCursor = true;
            }
        }
    }
}
