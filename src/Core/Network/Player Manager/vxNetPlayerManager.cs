﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Entities;

namespace Virtex.Lib.Vrtc.Network
{
    public class vxNetPlayerManager
    {
		vxEngine Engine { get; set; }

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
