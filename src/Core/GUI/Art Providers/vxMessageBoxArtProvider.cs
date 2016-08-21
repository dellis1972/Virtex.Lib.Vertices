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
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.GuiArtProvider.vxMenuItemArtProvider"/> class.
		/// </summary>
		/// <param name="engine">Engine.</param>
		public vxMessageBoxArtProvider(vxEngine engine):base(engine)
        {
            vxEngine = engine;

			Padding = new Vector2 (10, 4);

			BackgroundColour = Color.White;
			BackgroundHoverColour = Color.DarkOrange;

			TextColour = Color.Black;
			TextHoverColour = Color.Black;

			BackgroundImage = vxEngine.Assets.Textures.Blank;// vxEngine.EngineContentManager.Load<Texture2D>("Gui/DfltThm/vxGUITheme/vxMenuEntry/Bckgrnd_Nrml");

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

			Font = vxEngine.vxGUITheme.Font;

            //First Cast the GUI Item to be a Menu Entry
			vxMessageBox msgBox = (vxMessageBox)guiItem;


			// Fade the popup alpha during transitions.
			Color color = Color.White * msgBox.TransitionAlpha;

			vxEngine.SpriteBatch.Begin();

			// Draw the Title
			vxEngine.SpriteBatch.Draw(BackgroundImage,msgBox.TitleRectangle, color * 0.75f);
			vxEngine.SpriteBatch.DrawString(Font, msgBox.Title, msgBox.textTitlePosition, Color.White);

			// Draw the message box text.
			vxEngine.SpriteBatch.Draw(BackgroundImage, msgBox.backgroundRectangle, color * 0.55f);
			vxEngine.SpriteBatch.DrawString(Font, msgBox.message, msgBox.textPosition, Color.White);

			vxEngine.SpriteBatch.End();
        }
    }
}
