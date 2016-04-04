#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using vxVertices.Core;
using vxVertices.Core.Cameras;
using vxVertices.Core.Cameras.Controllers;
using vxVertices.Core.Input;
using vxVertices.Core.Input.Events;
using vxVertices.Core.Debug;
using vxVertices.Core.Scenes;
using vxVertices.Scenes.Sandbox.Entities;
using vxVertices.Scenes.Sandbox.MessageBoxs;
using vxVertices.Screens.Menus;
using vxVertices.GUI;
using vxVertices.GUI.Controls;
using vxVertices.GUI.Events;
using vxVertices.GUI.MessageBoxs;
using vxVertices.GUI.Dialogs;
using vxVertices.Utilities;
using vxVertices.Physics.BEPU;
using vxVertices.Physics.BEPU.BroadPhaseEntries;
using vxVertices.Physics.BEPU.CollisionRuleManagement;
using vxVertices.Physics.BEPU.BroadPhaseEntries.MobileCollidables;

namespace vxVertices.Scenes.Sandbox
{
    public partial class vxSandboxGamePlay : vxScene3D
    {

    }
}

#endif