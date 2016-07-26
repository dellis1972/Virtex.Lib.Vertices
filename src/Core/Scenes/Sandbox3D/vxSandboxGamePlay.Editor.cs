#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Core.Cameras;
using Virtex.Lib.Vertices.Core.Cameras.Controllers;
using Virtex.Lib.Vertices.Core.Input;
using Virtex.Lib.Vertices.Core.Input.Events;
using Virtex.Lib.Vertices.Core.Debug;
using Virtex.Lib.Vertices.Core.Scenes;
using Virtex.Lib.Vertices.Scenes.Sandbox.Entities;

using Virtex.Lib.Vertices.Screens.Menus;
using Virtex.Lib.Vertices.GUI;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.GUI.Events;
using Virtex.Lib.Vertices.GUI.MessageBoxs;
using Virtex.Lib.Vertices.GUI.Dialogs;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Physics.BEPU;
using Virtex.Lib.Vertices.Physics.BEPU.BroadPhaseEntries;
using Virtex.Lib.Vertices.Physics.BEPU.CollisionRuleManagement;
using Virtex.Lib.Vertices.Physics.BEPU.BroadPhaseEntries.MobileCollidables;

namespace Virtex.Lib.Vertices.Scenes.Sandbox3D
{
    public partial class vxSandboxGamePlay : vxScene3D
    {
        #region TOP TOOLBAR

        private void AddItemModeToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            AddItemModeToolbarItem.ToggleState = ToggleState.On;
            SelectItemModeToolbarItem.ToggleState = ToggleState.Off;
            MouseClickState = vxEnumSanboxMouseClickState.AddItem;
        }

        private void SelectItemModeToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            AddItemModeToolbarItem.ToggleState = ToggleState.Off;
            MouseClickState = vxEnumSanboxMouseClickState.SelectItem;
            DisposeOfTempPart();
        }

        /// <summary>
        /// Event Fired too test the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RunGameToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SimulationStart();
        }

        /// <summary>
        /// Event Fired too stop the test of the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StopGameToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SimulationStop();
        }

        #endregion

    }
}

#endif