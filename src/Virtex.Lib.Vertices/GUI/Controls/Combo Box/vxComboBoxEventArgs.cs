#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using vxVertices.GUI.Controls;


#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using vxVertices.GUI;
#endregion

namespace vxVertices.GUI.Events
{
    /// <summary>
    /// Event Args for Combo Box Selection Change
    /// </summary>
    public class vxComboBoxSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxComboBoxSelectionChangedEventArgs(vxComboBoxItem vxcomboBoxItem)
        {
            this.vxcomboBoxItem = vxcomboBoxItem;
        }


        /// <summary>
        /// Gets the Currently Selected vxComboBoxItem Item that is associated with the Selection change.
        /// </summary>
        public vxComboBoxItem SelectedItem
        {
            get { return vxcomboBoxItem; }
        }
        vxComboBoxItem vxcomboBoxItem;

        /// <summary>
        /// Gets the Currently Selected vxComboBoxItem Index that is associated with the Selection change.
        /// </summary>
        public int SelectedIndex
        {
            get { return vxcomboBoxItem.Index; }
        }
    }
}
