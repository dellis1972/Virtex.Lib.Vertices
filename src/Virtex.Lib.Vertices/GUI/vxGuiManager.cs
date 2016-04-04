using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Core;
using Microsoft.Xna.Framework.Content;
using vxVertices.Utilities;
using Microsoft.Xna.Framework;

namespace vxVertices.GUI
{
	/// <summary>
	/// GUI Manager for a given Game Scene. This Handles all GUI Items within a given scene.
	/// </summary>
    public class vxGuiManager
    {
		/// <summary>
		/// This item is the current item with focus.
		/// </summary>
		public vxGUIBaseItem FocusedItem;


        public List<vxGUIBaseItem> List_GUIItems = new List<vxGUIBaseItem>();

		/// <summary>
		/// Does the GUI have focus.
		/// </summary>
        public bool DoesGuiHaveFocus = false;

       /// <summary>
       /// Initializes a new instance of the <see cref="vxVertices.GUI.vxGuiManager"/> class.
       /// </summary>
        public vxGuiManager()
        {

        }

        /// <summary>
		/// Adds a vxGUI Item to thie GUI Manager.
        /// </summary>
        /// <param name="xbaseGuiItem"></param>
        public void Add(vxGUIBaseItem xbaseGuiItem)
        {
            xbaseGuiItem.GUIManager = this;
            List_GUIItems.Add(xbaseGuiItem);
        }


		/// <summary>
		/// Adds a Range of vxGUI Items to thie GUI Manager.
		/// </summary>
		/// <param name="xbaseGuiItem">Xbase GUI item.</param>
        public void AddRange(IEnumerable<vxGUIBaseItem> xbaseGuiItem)
        {
            List_GUIItems.AddRange(xbaseGuiItem);
        }

        /// <summary>
        /// Tells the GUI Manager too update each of the Gui Items
        /// </summary>
        /// <param name="vxEngine"></param>
        public void Update(vxEngine vxEngine)
        {
			// The GUI Manager Draws and Updates it's items from the back forwards. It
			// only allows one item to have focus, which is the most forward item with the mouse
			// over it.


            DoesGuiHaveFocus = false;

            MouseState mouseState = Mouse.GetState();

            if (this.FocusedItem == null)
            {
                foreach (vxGUIBaseItem guiItem in List_GUIItems)
                {
                    guiItem.Update(mouseState);

                    if (guiItem.HasFocus == true)
                    {
                        DoesGuiHaveFocus = true;
                    }
                }
            }
            else
            {
                this.FocusedItem.Update(mouseState);
            }
        }

        /// <summary>
        /// Tells the GUI Manager too Draw the Gui Items
        /// </summary>
        /// <param name="vxEngine"></param>
        public void Draw(vxEngine vxEngine)
        {
			foreach (vxGUIBaseItem guiItem in List_GUIItems)
				guiItem.Draw(vxEngine);

            if (this.FocusedItem != null)
            {
                this.FocusedItem.Draw(vxEngine);
            }
        }
    }
}
