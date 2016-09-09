using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Dialogs;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    /// <summary>
    /// Panel layout.
    /// </summary>
    public enum PanelLayout
    {
        /// <summary>
        /// Set the layout as a grid.
        /// </summary>
        Grid,

        /// <summary>
        /// Set the layout as a list.
        /// </summary>
        List
    }

    /// <summary>
    /// Scroll Panel Control which allows for any item too be added to it.
    /// </summary>
    public class vxScrollPanel : vxGUIBaseItem
    {
        /// <summary>
        /// The alpha.
        /// </summary>
        //public float Alpha = 0.5f;

        /// <summary>
        /// The width of the scroll bar.
        /// </summary>
        public int ScrollBarWidth
        {
            get { return scrollBar.BarWidth; }
            set
            {
                if(scrollBar != null)
                    scrollBar.BarWidth = value;
            }
        }
        
        /// <summary>
        /// The panel layout.
        /// </summary>
        public PanelLayout PanelLayout = PanelLayout.List;

        /// <summary>
        /// Collection of the vxGUIBaseItem's in this Panel.
        /// </summary>
        public List<vxGUIBaseItem> Items = new List<vxGUIBaseItem>();

        /// <summary>
        /// The Scroll Bar Control.
        /// </summary>
        vxScrollBar scrollBar;

        Vector2 PaddingVector = new Vector2(0, 0);

		/// <summary>
		/// The panel background texture.
		/// </summary>
		public Texture2D PanelBackground;

		/// <summary>
		/// A Specific Rasterizer State to preform Rectangle Sizzoring.
		/// </summary>
		RasterizerState _rasterizerState;

		/// <summary>
		/// The background colour.
		/// </summary>
		public Color BackgroundColour = Color.Black;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.GUI.Controls.vxScrollPanel"/> class which
		/// can hold a Grid or Detail list of Items.
		/// </summary>
		/// <param name="Position">Position.</param>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		public vxScrollPanel(Vector2 Position, int Width, int Height) :
		this(Position, Width, Height, 64)
        {
            
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.GUI.Controls.vxScrollPanel"/> class which
		/// can hold a Grid or Detail list of Items.
		/// </summary>
		/// <param name="Position">Position.</param>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		/// <param name="ScrollBarWidth">Scroll bar width.</param>
		public vxScrollPanel(Vector2 Position, int Width, int Height, int ScrollBarWidth)
		{

			this.Position = Position;
			this.OriginalPosition = Position;
			this.Width = Width;
			this.Height = Height;
			this.ScrollBarWidth = ScrollBarWidth;

			BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
			Padding = 10;

			scrollBar = new vxScrollBar(this,
				new Vector2(
					BoundingRectangle.X + BoundingRectangle.Width - Padding - 20,
					BoundingRectangle.Y + Padding),
				BoundingRectangle.Height,
				BoundingRectangle.Height)
			{
				BarWidth = ScrollBarWidth,
			};

			this.EnabledStateChanged += new EventHandler<EventArgs>(xScrollPanel_EnabledStateChanged);

			_rasterizerState = new RasterizerState () { ScissorTestEnable = true };
		}

        void xScrollPanel_EnabledStateChanged(object sender, EventArgs e)
        {
            //Get The Max and Min Positions of Items to get the overall Height
            foreach (vxGUIBaseItem bsGuiItm in Items)
            {
                bsGuiItm.Enabled = this.Enabled;
            }
        }

        /// <summary>
        /// When the GUIItem is Selected
        /// </summary>
        public override void Select()
        {
            Colour = Color_Selected;

            HasFocus = true;
        }

        /// <summary>
        /// Clear the scrollbar of all items.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
		/// Add an Item to the Scroll Panel
        /// </summary>
        /// <param name="guiItem">GUI item.</param>
        public void AddItem(vxGUIBaseItem guiItem)
        {
            //vxConsole.WriteLine ("Adding Item: " + guiItem.GetType ().ToString ());
            if (guiItem.GetType() == typeof(vxFileDialogItem) ||
                guiItem.GetBaseGuiType() == typeof(vxScrollPanelItem))
                Padding = 5;

            //int temp_height = 0;


            //First, Check that the Width of the item is not wider than the panel it's self;
            /****************************************************************************************************/
            if (guiItem.Width > this.BoundingRectangle.Width)
            {
                guiItem.Width = this.BoundingRectangle.Width - this.Padding * 3 - this.ScrollBarWidth;
            }

            //First set position relative too last item
            /****************************************************************************************************/
            if (Items.Count > 0)
            {
                //If there's more than one item in teh scroll panel, get it.
                vxGUIBaseItem LastGuiItem = Items[Items.Count - 1];

                //Now Set the Position of the new item relative too the previous one.
                guiItem.Position = LastGuiItem.Position + new Vector2(LastGuiItem.BoundingRectangle.Width + Padding, 0);
            }
            else
            {
                //If this is the first item, stick it in the top left corner.
                guiItem.Position = new Vector2(Padding);
            }


            //Check if it is inside the bounding rectangle, if not, move it down one row.
            /****************************************************************************************************/
            //if (guiItem.Position.X + guiItem.BoundingRectangle.Width > this.Position.X + BoundingRectangle.Width - Padding * 2 - ScrollBarWidth ||
            if (guiItem.Position.X + guiItem.BoundingRectangle.Width > BoundingRectangle.Width - Padding * 2 - ScrollBarWidth ||
                guiItem.GetType() == typeof(vxFileDialogItem) ||
                guiItem.GetBaseGuiType() == typeof(vxScrollPanelItem))
            {
                if (Items.Count > 0)
                {
                    vxGUIBaseItem LastGuiItem = Items[Items.Count - 1];
                    guiItem.Position = new Vector2(Padding, LastGuiItem.Position.Y + LastGuiItem.BoundingRectangle.Height + Padding);
                }

                //There's a chance that This item is the width of the scroll panel, so it 
                //should be set as the minimum between it's width, and the scroll panels width
                //guiItem.Width = Math.Min(guiItem.Width, BoundingRectangle.Width - Padding * 2 - ScrollBarWidth);
            }
            //Reset the Original Position
            guiItem.OriginalPosition = guiItem.Position;

            //scrollBar.ViewHeight = (int)((float)(BoundingRectangle.Height - 2*Padding) * Math.Min(1,((float)BoundingRectangle.Height / (float)temp_height)));

            //TODO: Put Item Spefic code in the items classes them selves, not here.
            if (guiItem.GetType() == typeof(vxFileDialogItem))
                ((vxFileDialogItem)guiItem).ButtonWidth = BoundingRectangle.Width - Padding * 4 - ScrollBarWidth;

            if (guiItem.GetBaseGuiType() == typeof(vxScrollPanelItem))
                ((vxScrollPanelItem)guiItem).ButtonWidth = BoundingRectangle.Width - Padding * 4 - ScrollBarWidth;

            //Finally Add the newly Positioned and Sized Item.
            Items.Add(guiItem);


            //Finally, set the Scrollbar scroll length.
            /****************************************************************************************************/
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
        public void AddRange(IEnumerable<vxGUIBaseItem> xbaseGuiItem)
        {
            foreach (vxGUIBaseItem guiItem in xbaseGuiItem)
            {
                AddItem(guiItem);
            }
        }
        Vector2 PositionPreviousFrame = Vector2.Zero;

        /// <summary>
        /// Updates the GUI Item
        /// </summary>
        /// <param name="mouseState">Mouse state.</param>
        public override void Update(vxEngine vxEngine)
        {
            base.Update(vxEngine);

            //Recalculate the Bounding rectangle each loop
            BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            scrollBar.Position = new Vector2(
                BoundingRectangle.X + BoundingRectangle.Width - Padding - scrollBar.BarWidth,
                BoundingRectangle.Y + Padding);

            scrollBar.Update(vxEngine);

            //vxConsole.WriteToInGameDebug ("=> " + );
            float dif = Vector2.Subtract(Position, PositionPreviousFrame).Length();

            //foreach (vxGUIBaseItem bsGuiItm in Items)
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Position = this.Position + PaddingVector + Items[i].OriginalPosition
                        - new Vector2(0, (scrollBar.Percentage * (scrollBar.ScrollLength - this.Height + 2 * Padding)));

                if (this.HasFocus && dif < 0.05f)
                    Items[i].Update(vxEngine);
                else
                    Items[i].NotHover();
            }

            PositionPreviousFrame = this.Position;

        }


        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            Rectangle rec = BoundingRectangle;
            //Set Viewport

            try
            {
                //Set up Minimum 
                int x = Math.Min(Math.Max(BoundingRectangle.X, 0), vxEngine.GraphicsDevice.Viewport.Width - 1);
                int y = Math.Min(Math.Max(BoundingRectangle.Y, 0), vxEngine.GraphicsDevice.Viewport.Height - 1)+5;

                int Width = BoundingRectangle.Width;
                if (x + BoundingRectangle.Width > vxEngine.GraphicsDevice.Viewport.Width)
                    Width = vxEngine.GraphicsDevice.Viewport.Width - x;
                Width = Math.Max(Width, 1);

                int Height = BoundingRectangle.Height-10;
                if (y + BoundingRectangle.Height > vxEngine.GraphicsDevice.Viewport.Height)
                    Height = BoundingRectangle.Height - y;

                if (BoundingRectangle.Y < 0)
                    Height = BoundingRectangle.Height + BoundingRectangle.Y;

                Height = Math.Max(Height, 1);

                rec =
                    new Rectangle(
                        x,
                        y,
                        Width,
                        Height);			
            }
            catch (Exception ex) { vxConsole.WriteError(ex); }

			//Only draw if the rectangle is larger than 2 pixels. This is to avoid
			//artifacts where the minimum size is 1 pixel, which is pointless.
			if (rec.Height > 2) {

				//Draw Background
				vxEngine.SpriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend,
					null, null, _rasterizerState);

				//First Draw The Background
				vxEngine.SpriteBatch.Draw (vxEngine.Assets.Textures.Blank, BoundingRectangle, BackgroundColour * Alpha);


				//Copy the current scissor rect so we can restore it after
				Rectangle currentRect = vxEngine.SpriteBatch.GraphicsDevice.ScissorRectangle;

				//Set the current scissor rectangle
				vxEngine.SpriteBatch.GraphicsDevice.ScissorRectangle = rec;

				//Then draw the scroll bar
				scrollBar.DrawByOwner (vxEngine);

				//use for loops, items can be removed while rendereing through the
				//loop. This is generally an issue during networking games when a
				//signal is recieved to remove an item while it's already rendering.
				for (int i = 0; i < Items.Count; i++) {
					Items [i].DrawByOwner (vxEngine);
				}

				//Console.WriteLine(rec);
				//Reset scissor rectangle to the saved value
				vxEngine.SpriteBatch.GraphicsDevice.ScissorRectangle = currentRect;

				vxEngine.SpriteBatch.End();
			}
        }
    }
}
