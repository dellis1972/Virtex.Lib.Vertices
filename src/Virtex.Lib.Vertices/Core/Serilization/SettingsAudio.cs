using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace vxVertices.Core.Settings
{
	/// <summary>
	/// Serializable Settings.
	/// </summary>
    public class vxSettingsAudio
    {
        //Audio
        [XmlElement("Double_Main_Volume")]
        public float Double_Main_Volume;

        [XmlElement("Double_SFX_Volume")]
        public float Double_SFX_Volume;

        [XmlElement("Double_MUSIC_Volume")]
        public float Double_MUSIC_Volume;
        
        public vxSettingsAudio() { }
        //
        //Constructor
        //
        public vxSettingsAudio(
            float Double_Main_Volume, 
            float Double_MUSIC_Volume,
            float Double_SFX_Volume)
        {
            //Audio
            this.Double_Main_Volume = Double_Main_Volume;
            this.Double_SFX_Volume = Double_SFX_Volume;
            this.Double_MUSIC_Volume = Double_MUSIC_Volume;
        }   
    }
}
