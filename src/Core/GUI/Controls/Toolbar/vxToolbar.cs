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
	/// Toolbar control that holds <see cref="Virtex.Lib.Vertices.GUI.Controls.vxToolbarButton"/> 
	/// </summary>
    public class vxToolbar : vxGUIBaseItem
    {
        public List<vxGUIBaseItem> ToolbarItems = new List<vxGUIBaseItem>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vertices.GUI.Controls.vxToolbar"/> class.
		/// </summary>
		/// <param name="position">Position.</param>
		public vxToolbar(Vector2 position): base(position)
        {
			Height = 32+2;

            Color_Normal = Color.Black;
            Color_Highlight = Color.Black;
            Color_Selected = Color.Black;
            HoverAlphaMax = 0.75f;
            HoverAlphaMin = 0.5f;
            HoverAlphaDeltaSpeed = 10;

        }

		/// <summary>
		/// When the GUIItem is Selected
		/// </summary>
        public override void Select()
        {
            HasFocus = true;
        }

		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <param name="guiItem">GUI item.</param>
        public void AddItem(vxGUIBaseItem guiItem)
        {
            int tempPosition = PaddingX;
            //First Set Position
            foreach (vxGUIBaseItem bsGuiItm in ToolbarItems)
            {
                tempPosition += bsGuiItm.BoundingRectangle.Width + PaddingX;
                //Console.WriteLine("{0} : {1}", bsGuiItm.ToString() ,bsGuiItm.Width);
            }

            guiItem.Position = new Vector2(tempPosition + PaddingX / 2, Position.Y - guiItem.BoundingRectangle.Height + PaddingX);

            ToolbarItems.Add(guiItem);
        }

		/// <summary>
		/// Updates the GUI Item
		/// </summary>
		/// <param name="mouseState">Mouse state.</param>
        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);

			Vector2 tempPosition = this.Position + new Vector2(PaddingX, PaddingY);
            foreach (vxGUIBaseItem bsGuiItm in ToolbarItems)
			{
				//Set Position
				bsGuiItm.Position = tempPosition;
				bsGuiItm.Update(vxEngine);


				//Incrememnet Up the Position
				tempPosition += new Vector2(bsGuiItm.Width + PaddingX, 0);
            }
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            Viewport viewport = vxEngine.GraphicsDevice.Viewport;
            
            //Update Rectangle
            int length = viewport.Width;
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y), length, Height + PaddingY);
          
            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour * HoverAlpha);
            vxEngine.SpriteBatch.End();

            foreach (vxGUIBaseItem bsGuiItm in ToolbarItems)
            {
                bsGuiItm.Draw(vxEngine);
            }
        }
    }
}
