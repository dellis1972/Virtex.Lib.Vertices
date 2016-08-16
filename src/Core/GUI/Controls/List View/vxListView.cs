using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    public enum vxListViewLayout
    {
        Grid,
        Details,
        List
    }
    public class vxListView : vxGUIBaseItem
    {
		public float Alpha = 0.5f;
        
		public int ScrollBarWidth = 20;

        Viewport viewport_Original;

        public PanelLayout PanelLayout = PanelLayout.List;

        List<vxListViewItem> Items = new List<vxListViewItem>();

        vxListViewScrollBar scrollBar;

        Vector2 PaddingVector = new Vector2(0, 0);

        /// <summary>
        /// Scroll Panael to Hold a Grid or Detail list of Items
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public vxListView(Vector2 Position, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            this.Position = Position;
            this.OriginalPosition = Position;

            BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            Padding = 5;

            scrollBar = new vxListViewScrollBar(this,
                new Vector2(
                    BoundingRectangle.X + BoundingRectangle.Width - Padding - 20,
                    BoundingRectangle.Y + Padding), 
                BoundingRectangle.Height, 
                BoundingRectangle.Height)
			{
				BarWidth = ScrollBarWidth,
			};
        }

        public override void Select()
        {
            Colour = Color_Selected;

            HasFocus = true;
        }

        //Add an Item to the Scroll Panel
        public void AddItem(vxListViewItem guiItem)
        {
            int temp_height = 0;

            if (Items.Count > 0)
            {
                vxListViewItem LastGuiItem = Items[Items.Count - 1];

                //First Set the Poition
                guiItem.Position = LastGuiItem.Position + new Vector2(LastGuiItem.BoundingRectangle.Width + Padding, 0);
            }
            else
            {
                //First Set the Poition
                guiItem.Position = new Vector2(Padding);
            }

            //Check if it is inside the bounding rectangle, if not, move it down one row.
            if (guiItem.Position.X + guiItem.BoundingRectangle.Width > this.Position.X + BoundingRectangle.Width - Padding * 2 - ScrollBarWidth)
            {
                if (Items.Count > 0)
                {
                    vxListViewItem LastGuiItem = Items[Items.Count - 1];
                    guiItem.Position = new Vector2(Padding, LastGuiItem.Position.Y + LastGuiItem.BoundingRectangle.Height + Padding);
                }

                //There's a chance that This item is the width of the scroll panel, so it 
                //should be set as the minimum between it's width, and the scroll panels width
                guiItem.Width = Math.Min(guiItem.Width, BoundingRectangle.Width - Padding * 2 - ScrollBarWidth);
             }

            guiItem.OriginalPosition = guiItem.Position;

            Items.Add(guiItem);



            /////////////////////////////////////////
            //        SET SCROLL BAR LENGTH
            /////////////////////////////////////////
            float totalHeight = this.Height;
            float tempPos_min = this.Height - 1;
            float tempPos_max = this.Height;

            //Get The Max and Min Positions of Items to get the overall Height
            foreach (vxGUIBaseItem bsGuiItm in Items)
            {
                tempPos_min = Math.Min(bsGuiItm.Position.Y, tempPos_min);
                tempPos_max = Math.Max(bsGuiItm.Position.Y + bsGuiItm.BoundingRectangle.Height, tempPos_max);
            }

            scrollBar.ScrollLength = (int)((tempPos_max - tempPos_min));
        }

        /// <summary>
        /// Adds a Range of Values for to the Scroll Panel
        /// </summary>
        /// <param name="xbaseGuiItem"></param>
        public void AddRange(IEnumerable<vxListViewItem> xbaseGuiItem)
        {
            foreach (vxListViewItem guiItem in xbaseGuiItem)
            {
                AddItem(guiItem);
            }
        }

        /// <summary>
        /// Sets the layout of the 
        /// </summary>
        void SetLayout()
        {

        }

        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);

            //Recalculate the Bounding rectangle each loop
            BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            Vector2 offset = new Vector2(viewport_Original.X - BoundingRectangle.X,
                viewport_Original.Y - BoundingRectangle.Y);

            scrollBar.Position = new Vector2(
                BoundingRectangle.X + BoundingRectangle.Width - Padding - scrollBar.BarWidth,
                BoundingRectangle.Y + Padding);

            scrollBar.Update(vxEngine);
            MouseState ms = Mouse.GetState();
            //Only Update Stuff if it has Focus
            MouseState dummyMouseState = new MouseState(
                ms.X - (int)this.Position.X,
                ms.Y - (int)this.Position.Y,
                ms.ScrollWheelValue,
                ms.LeftButton,
                ms.MiddleButton,
                ms.RightButton,
                ms.XButton1,
                ms.XButton2);

                foreach (vxGUIBaseItem bsGuiItm in Items)
                {
                    bsGuiItm.Position = PaddingVector + bsGuiItm.OriginalPosition
                        - new Vector2(0, (scrollBar.Percentage * (scrollBar.ScrollLength - this.Height + 2 * Padding)));

                if (this.HasFocus)
                {
                    MouseState prevMS = vxEngine.InputManager.MouseState;
                    vxEngine.InputManager.MouseState = dummyMouseState;
                    bsGuiItm.Update(vxEngine);
                    vxEngine.InputManager.MouseState = prevMS;
                }
                else
                    bsGuiItm.NotHover();
                }

        }

        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //Save the original Viewport
            viewport_Original = vxEngine.GraphicsDevice.Viewport;

            //Draw Background
            vxEngine.SpriteBatch.Begin();
			vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color.Black * Alpha);
            vxEngine.SpriteBatch.End();

            scrollBar.Draw(vxEngine);

            //Set Viewport
            try
            {
                int Width = BoundingRectangle.Width;
                if (BoundingRectangle.X + BoundingRectangle.Width > vxEngine.GraphicsDevice.Viewport.Width)
                    Width = vxEngine.GraphicsDevice.Viewport.Width - BoundingRectangle.X;

                int Height = BoundingRectangle.Height;
                if (BoundingRectangle.Y + BoundingRectangle.Height > vxEngine.GraphicsDevice.Viewport.Height)
                    Height = vxEngine.GraphicsDevice.Viewport.Height - BoundingRectangle.Y;

                vxEngine.GraphicsDevice.Viewport = new Viewport(
                    new Rectangle(
                        Math.Min(Math.Max(BoundingRectangle.X, 0), vxEngine.GraphicsDevice.Viewport.Width - 1),
                        Math.Min(Math.Max(BoundingRectangle.Y, 0), vxEngine.GraphicsDevice.Viewport.Height - 1),
                        Width, 
                        Height));
            }
            catch (Exception ex) { vxConsole.WriteLine(ex.Message); }

            //Draw Items
            foreach (vxGUIBaseItem bsGuiItm in Items)
            {
                bsGuiItm.Draw(vxEngine);
            }

            //Reset the original Viewport
            vxEngine.GraphicsDevice.Viewport = viewport_Original;
        }
    }
}
