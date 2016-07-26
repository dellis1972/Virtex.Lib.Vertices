using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vertices.GUI.Dialogs;

namespace Virtex.Lib.Vertices.GUI.Events
{
    public class vxGuiItemClickEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxGuiItemClickEventArgs(vxGUIBaseItem vxbaseGuiItem)
        {
            this.vxbaseGuiItem = vxbaseGuiItem;
        }


        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public vxGUIBaseItem GUIitem
        {
            get { return vxbaseGuiItem; }
        }
        vxGUIBaseItem vxbaseGuiItem;
    }


	public class vxFileDialogItemClickEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public vxFileDialogItemClickEventArgs(vxFileDialogItem vxbaseGuiItem)
		{
			this.fileDialogItem = vxbaseGuiItem;
		}


		/// <summary>
		/// Gets the index of the player who triggered this event.
		/// </summary>
		public vxFileDialogItem FileDialogItem
		{
			get { return fileDialogItem; }
		}
		vxFileDialogItem fileDialogItem;
	}



}
