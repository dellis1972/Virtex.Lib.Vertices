using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Core;

namespace Virtex.Lib.Vertices.GUI.Controls
{
	/// <summary>
	/// Tab Control which managers Tab Pages.
	/// </summary>
    public class vxTabControl : vxGUIBaseItem
    {
        /// <summary>
        /// List of Tab Pages
        /// </summary>
        public List<vxGUIBaseItem> Pages = new List<vxGUIBaseItem>();

        /// <summary>
        /// Space in between tabs
        /// </summary>
        int PageOffset = 1;

        Vector2 OffsetPosition;

        /// <summary>
        /// The Amount that the page entends out
        /// </summary>
        public int SelectionExtention
        {
            get{return this.Width;}   
        }

		/// <summary>
		/// The height of the tab stub.
		/// </summary>
        public int TabHeight = 36;

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vertices.GUI.vxTabControl"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="Width">Width.</param>
        /// <param name="Height">Height.</param>
        /// <param name="offSetPosition">Off set position.</param>
        /// <param name="GUIOrientation">GUI orientation.</param>
        public vxTabControl(vxEngine vxEngine, int Width, int Height, Vector2 offSetPosition, vxGUIItemOrientation GUIOrientation)
        {
            this.vxEngine = vxEngine;

            this.Width = Width;
            this.Height = Height;

            OffsetPosition = offSetPosition;

            ItemOreintation = GUIOrientation;
            switch (GUIOrientation)
            {
                case vxGUIItemOrientation.Left:

                    Position = offSetPosition - new Vector2(this.Width, 0);
                    gap = 0;
                    break;

                case vxGUIItemOrientation.Right:

                    Position = new Vector2(vxEngine.GraphicsDevice.Viewport.Width, 0) + offSetPosition;
                    TabHeight *= -1;
                    break;
            }
			OriginalPosition = Position;
            HoverAlphaMax = 0.75f;
            HoverAlphaMin = 0.5f;
            HoverAlphaDeltaSpeed = 10;
        }
        int gap = 35;

		/// <summary>
		/// Adds a vxTabPage.
		/// </summary>
		/// <param name="guiItem">GUI item.</param>
        public void AddItem(vxTabPage guiItem)
        {
            int tempPosition = 0;
            //First Set Position
            foreach (vxTabPage bsGuiItm in Pages)
            {
                tempPosition += bsGuiItm.TabWidth + PageOffset;
            }

            guiItem.TabOffset = new Vector2(0, tempPosition);
            guiItem.ItemOreintation = this.ItemOreintation;
            //guiItem.Position_Original = new Vector2(Position.X + gap, Position.Y + tempPosition);

            Pages.Add(guiItem);
        }

		/// <summary>
		/// Closes all tabs.
		/// </summary>
        public void CloseAllTabs()
        {
            foreach (vxTabPage tabPage in Pages)
            {
				tabPage.Close ();
            }
            this.HasFocus = false;
        }

		/// <summary>
		/// Updates the GUI Item
		/// </summary>
		/// <param name="mouseState">Mouse state.</param>
        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);

            foreach (vxGUIBaseItem bsGuiItm in Pages)
            {
                bsGuiItm.Update(vxEngine);
                if (bsGuiItm.HasFocus == true)
                    HasFocus = true;
            }
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //Viewport viewport = vxEngine.GraphicsDevice.Viewport;
            
            //
            //Update Rectangle
            //
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y), this.Width, this.Height);



            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color.Black * HoverAlpha);
            vxEngine.SpriteBatch.End();

            //vxConsole.WriteToInGameDebug(string.Format("Pos: {0}, Width: {1}", 

            //Draw Pages
            foreach (vxTabPage bsGuiItm in Pages)
            {
                bsGuiItm.Draw(vxEngine);
            }

            //Draw Tabs Ontop
            foreach (vxTabPage bsGuiItm in Pages)
            {
                bsGuiItm.DrawTab(vxEngine);
            }
            
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank,
                new Rectangle((int)(Position.X) - TabHeight, (int)(Position.Y), this.Width, this.Height),
                Color.DarkOrange);
            vxEngine.SpriteBatch.End(); 
             
        }
    }
}
