using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
//using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;

namespace Virtex.Lib.Vertices.Network.Messages
{
    /// <summary>
    /// This message is used during the discovery phase to glean basic server information.
    /// </summary>
    public class vxNetmsgUpdatePlayerEntityState : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public vxNetPlayerInfo PlayerInfo { get; set; }


        /// <summary>
        /// Gets or sets MessageTime to help with interpolating the actual position after lag.
        /// </summary>
        public double MessageTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetmsgUpdatePlayerEntityState(vxNetPlayerInfo playerinfo)
        {
            this.PlayerInfo = playerinfo;
            this.MessageTime = NetTime.Now;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgUpdatePlayerEntityState(NetIncomingMessage im)
        {
            PlayerInfo = new vxNetPlayerInfo(1, "any", vxEnumNetPlayerStatus.ReadyToPlay);
            this.DecodeMsg(im);
        }

        /// <summary>
        /// The Message Type
        /// </summary>
        public vxNetworkMessageTypes MessageType
        {
            get
            {
                return vxNetworkMessageTypes.UpdatePlayerEntityState;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            //player ID
            PlayerInfo.ID = im.ReadInt64();
            PlayerInfo.UserName = im.ReadString();
            this.MessageTime = im.ReadDouble();

            //get the current physical orientations and locations
			PlayerInfo.EntityState.Position = new Vector3(im.ReadFloat(), im.ReadFloat(), im.ReadFloat());
			PlayerInfo.EntityState.Velocity = new Vector3(im.ReadFloat(), im.ReadFloat(), im.ReadFloat());
            PlayerInfo.EntityState.Rotation = im.ReadFloat();
            
            //What is the current control status
            PlayerInfo.EntityState.IsBackDown = im.ReadBoolean();
            PlayerInfo.EntityState.IsForwardDown = im.ReadBoolean();
            PlayerInfo.EntityState.IsLeftDown = im.ReadBoolean();
            PlayerInfo.EntityState.IsRightDown = im.ReadBoolean();
            PlayerInfo.EntityState.IsThrustDown = im.ReadBoolean();

            //how much should these controls be applied
            PlayerInfo.EntityState.ThrustAmount = im.ReadFloat();
            PlayerInfo.EntityState.TurnAmount = im.ReadFloat();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            //player ID
            om.Write(PlayerInfo.ID);
            om.Write(PlayerInfo.UserName);
            om.Write(this.MessageTime);            

            //get the current physical orientations and locations
			om.Write(PlayerInfo.EntityState.Position.X);
			om.Write(PlayerInfo.EntityState.Position.Y);
			om.Write(PlayerInfo.EntityState.Position.Z);

			om.Write(PlayerInfo.EntityState.Velocity.X);
			om.Write(PlayerInfo.EntityState.Velocity.Y);
			om.Write(PlayerInfo.EntityState.Velocity.Z);

            om.Write(PlayerInfo.EntityState.Rotation);

            //What is the current control status
            om.Write(PlayerInfo.EntityState.IsBackDown);
            om.Write(PlayerInfo.EntityState.IsForwardDown);
            om.Write(PlayerInfo.EntityState.IsLeftDown);
            om.Write(PlayerInfo.EntityState.IsRightDown);
            om.Write(PlayerInfo.EntityState.IsThrustDown);

            //how much should these controls be applied
            om.Write(PlayerInfo.EntityState.ThrustAmount);
            om.Write(PlayerInfo.EntityState.TurnAmount);
        }
    }
}
