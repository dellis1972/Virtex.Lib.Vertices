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
		/// Gets or sets the alpha of the GUI Manager.
		/// </summary>
		/// <value>The alpha.</value>
		public float Alpha
		{
			get { return _alpha; }
			set { _alpha = value; }
		}
		float _alpha = 1;

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
					guiItem.Alpha = this.Alpha;
                    guiItem.Update(vxEngine);

                    if (guiItem.HasFocus == true)
                    {
                        DoesGuiHaveFocus = true;
                    }
                }
            }
            else
            {
                this.FocusedItem.Update(vxEngine);
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
