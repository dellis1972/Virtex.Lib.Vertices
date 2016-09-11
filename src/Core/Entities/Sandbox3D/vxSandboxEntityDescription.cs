using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.Entities.Sandbox3D
{
    public delegate vxSandboxEntity NewEntityDelegate(vxEngine Engine);

    /// <summary>
    /// This class holds all information to register a vxSandboxEntity with the vxSandbox enviroment. It also handles creating
    /// a new entity when the corresponding button is pressed.
    /// </summary>
    public class vxSandboxEntityRegistrationInfo
    {
        vxEngine Engine;
        public Texture2D Icon;
        public readonly string Key;
        public string Description;
        public string FilePath;
        NewEntityDelegate NewEntityDelegate;

        bool isEngineEntity;

        public vxSandboxEntityRegistrationInfo(string Key, string Description, string FilePath, NewEntityDelegate NewEntityDelegate) :
            this(false, Key, Description, FilePath, NewEntityDelegate)
        {
        }

        public vxSandboxEntityRegistrationInfo(bool isEngineEntity, string Key, string Description, string FilePath, NewEntityDelegate NewEntityDelegate)
        {
            this.Key = Key;
            this.Description = Description;
            this.FilePath = FilePath;
            this.NewEntityDelegate = NewEntityDelegate;
            this.isEngineEntity = isEngineEntity;
        }

        public void Load(vxEngine Engine)
        {
            this.Engine = Engine;

            // If it's an Engine Entity, then it should use the Engine's Content Manager to load the icon.
            if(isEngineEntity)
                this.Icon = Engine.EngineContentManager.Load<Texture2D>(FilePath + "_ICON");
            else
                this.Icon = Engine.Game.Content.Load<Texture2D>(FilePath + "_ICON");
        }

        public vxSandboxEntity CreateNewEntity()
        {
            if (NewEntityDelegate != null && Engine != null)
                return NewEntityDelegate(Engine);
            else
                return null;
        }
    }
}
