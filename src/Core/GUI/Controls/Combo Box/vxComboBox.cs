using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
	/// <summary>
	/// Provides a ComboBox Control which can be populated be vxComboBoxItems.
	/// </summary>
    public class vxComboBox : vxGUIBaseItem
    {
        //List of Items
        List<vxComboBoxItem> Items = new List<vxComboBoxItem>();

        //Padding for Item Seperation
        int Padding_ItemSeperation = 6;

        /// <summary>
		/// Selected Index
        /// </summary>
        public int SelectedIndex = 0;

        //Decides Whether or not too Display List
        bool DisplayList = false;

        //int length = 150;

		/// <summary>
		/// [Obsolete("Use the SelectionChanged Event when possible. This is kept for compatibility.")]
		/// </summary>
		public string fncn_string = "";

		/// <summary>
		/// [Obsolete("Use the SelectionChanged Event when possible. This is kept for compatibility.")]
		/// </summary>
        public Func<int> Event_OnSelectionChanged;

        /// <summary>
        /// Event Raised when the selected item is changed.
        /// </summary>
        public event EventHandler<vxComboBoxSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Combo Box for vxGui
        /// </summary>
        /// <param name="vxEngine"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
		public vxComboBox(vxEngine vxEngine, string text, Vector2 position):base(position)
        {
            Text = text;

			this.Font = vxEngine.vxGUITheme.Font;

            //Function = ComboBoxSelected;

			Width = 150;

            this.vxEngine = vxEngine;
			this.Font = this.vxEngine.Assets.Fonts.MenuFont;
			this.PositionChanged+= VxComboBox_PositionChanged;
			this.Clicked += VxComboBox_Clicked;

            Padding = 4;
        }

		void VxComboBox_Clicked (object sender, vxGuiItemClickEventArgs e)
		{
			DisplayList = !DisplayList;
            CaptureInput = DisplayList;
            int h = 0;
			if (PositionInvalided == true) {

				foreach (vxComboBoxItem btn in Items) {
					btn.Position = new Vector2 ((int)(Position.X+1),
						Position.Y + (h + 1) * ((int)this.Font .MeasureString (Text).Y + Padding_ItemSeperation));
					h++;
				}

				PositionInvalided = false;
			}
		}
		bool PositionInvalided = false;
        void VxComboBox_PositionChanged (object sender, EventArgs e)
        {
			PositionInvalided = true;
        }

        /// <summary>
        /// Add's a String to the Combo Box
        /// </summary>
        /// <param name="Item"></param>
        public void AddItem(string Item)
        {
            vxComboBoxItem item = new vxComboBoxItem(
                vxEngine, 
                Item,
                Items.Count,
                new Vector2(
                    (int)(Position.X+1),
					Position.Y + (Items.Count + 1) * ((int)this.Font .MeasureString(Text).Y + Padding_ItemSeperation))
                 );

            item.Clicked +=  item_Clicked;

            item.Color_Normal = Color.DarkGray * 0.75f;
            item.Colour = Color.Black;
            Items.Add(item);
        }

        void item_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            // Raise the Clicked event.
            if (SelectionChanged != null)
                SelectionChanged(this, new vxComboBoxSelectionChangedEventArgs((vxComboBoxItem)e.GUIitem));


            SelectedIndex = e.GUIitem.Index;
            Text = e.GUIitem.Text;

            //Close the list
            DisplayList = false;
            CaptureInput = DisplayList;
        }

        //Displays the Combo box
        int ComboBoxSelected()
        {
            DisplayList = !DisplayList;
			int h = 0;
			if (PositionInvalided == true) {

				foreach (vxComboBoxItem btn in Items) {
					btn.Position = new Vector2 ((int)(Position.X - Padding / 2),
						Position.Y + (h + 1) * ((int)this.Font .MeasureString (Text).Y + Padding_ItemSeperation));
					h++;
				}

				PositionInvalided = false;
			}



            return 0;
        }

		/// <summary>
		/// Updates the GUI Item
		/// </summary>
		/// <param name="mouseState">Mouse state.</param>
        public override void Update(vxEngine vxEngine)
        {
            //Update Each Button
            if (DisplayList)
                foreach (vxComboBoxItem btn in Items)
                    btn.Update(vxEngine);

            base.Update(vxEngine);

        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);
            Padding = 4;
            //
            //Update Rectangle
            //                       
			BoundingRectangle = new Rectangle((int)(Position.X-Padding), (int)(Position.Y-Padding/2), Width, (int)this.Font .MeasureString(Text).Y + Padding / 2);

            
            Rectangle BackRectangle = new Rectangle(
                    (int)(BoundingRectangle.X - Padding / 2),
                    (int)(BoundingRectangle.Y - Padding / 2),
                    (int)(BoundingRectangle.Width + Padding),
                    (int)(BoundingRectangle.Height + Padding));

		
            //Draw Each Button
            if (DisplayList)
            {
                //First Draw the Backing for the drop down items but only if there is at least one item.
                if (Items.Count > 0)
                {
                    Rectangle backing = new Rectangle(
                        (int)(BoundingRectangle.X - Padding / 2),
                        (int)(BoundingRectangle.Y - Padding / 2),
                        (int)(BoundingRectangle.Width + Padding),
                        (int)(Items[Items.Count - 1].BoundingRectangle.Bottom - BoundingRectangle.Top + Padding));

                    vxEngine.SpriteBatch.Begin();
                    vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, backing, Color.Black * 0.75f);
                    vxEngine.SpriteBatch.End();
                }
                foreach (vxComboBoxItem btn in Items)
                {
					btn.Font = this.Font;
					btn.Opacity = this.Opacity;
					btn.Width = Width;
                    btn.Draw(vxEngine);
                }
            }

            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BackRectangle, Color.Black);
			vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour * Opacity);
			vxEngine.SpriteBatch.DrawString(this.Font, Text, new Vector2(Position.X + Width / 2 - this.Font.MeasureString(Text).X / 2 - Padding, Position.Y), Colour_Text * Opacity);
            vxEngine.SpriteBatch.End();
        }
    }
}
