using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Core.Entities;

namespace Virtex.Lib.Vertices.Network
{
    public class vxNetPlayerManager
    {
        vxEngine Engine { get; }

        /// <summary>
        /// An entity collection containing all Network Players
        /// </summary>
        public Dictionary<long, vxNetPlayerInfo> Players = new Dictionary<long, vxNetPlayerInfo>();

        public vxNetPlayerManager(vxEngine engine)
        {
            this.Engine = engine;
        }

        public void Add(vxNetPlayerInfo entity)
        {
            Players.Add(entity.ID, entity);
        }

        public bool Contains(vxNetPlayerInfo entity)
        {
            return Players.ContainsKey(entity.ID);
        }
    }
}
