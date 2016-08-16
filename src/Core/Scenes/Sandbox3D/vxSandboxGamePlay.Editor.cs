#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core.Cameras.Controllers;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Core.Debug;
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Scenes.Sandbox.Entities;

using Virtex.Lib.Vrtc.Screens.Menus;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.GUI.MessageBoxs;
using Virtex.Lib.Vrtc.GUI.Dialogs;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries;
using Virtex.Lib.Vrtc.Physics.BEPU.CollisionRuleManagement;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries.MobileCollidables;

namespace Virtex.Lib.Vrtc.Scenes.Sandbox3D
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