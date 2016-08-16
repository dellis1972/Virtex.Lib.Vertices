#if VRTC_INCLDLIB_NET 
#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Virtex.Lib.Vrtc.GUI;
#endregion

namespace Virtex.Lib.Vrtc.Network.Events
{
    /// <summary>
    /// Event Args for when the server list is recieved
    /// </summary>
    public class vxGameServerListRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxGameServerListRecievedEventArgs(string[] serverList)
        {
            this.serverList = serverList;
        }


        /// <summary>
        /// The array of server lists
        /// </summary>
        public string[] ServerList
        {
            get { return serverList; }
        }
        string[] serverList;
    }
}
#endif