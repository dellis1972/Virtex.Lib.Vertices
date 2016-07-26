using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vertices.Core;

namespace Virtex.Lib.Vertices.Entities.Sandbox3D
{
    public class vxSandboxEntityDescription
    {
        public Texture2D Icon;
        public string Key;
        public string Description;
        public string FilePath;

        public vxSandboxEntityDescription(string key, string description, string filePath)
        {
            this.Key = key;
            this.Description = description;
            this.FilePath = filePath;
        }

        public void Load(vxEngine vxEngine)
        {
            this.Icon = vxEngine.Game.Content.Load<Texture2D>(FilePath + "_ICON");
        }
    }
}
