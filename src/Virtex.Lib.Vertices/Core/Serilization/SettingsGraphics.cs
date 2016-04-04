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
    public class vxSettingsGraphics
    {
        //Graphics
        [XmlElement("Int_Resolution")]
        public int Int_Resolution;

        [XmlElement("Bool_FullScreen")]
        public bool Bool_FullScreen;

        [XmlElement("Bool_VSync")]
        public bool Bool_VSync;

        [XmlElement("ShadowQuality")]
        public vxEnumQuality ShadowQuality;

        [XmlElement("Bool_DoEdgeDetection")]
        public bool Bool_DoEdgeDetection;

        [XmlElement("GodRays")]
        public vxEnumQuality GodRays;

        [XmlElement("Bloom")]
        public vxEnumQuality Bloom;

        [XmlElement("DepthOfField")]
        public vxEnumQuality DepthOfField;

        [XmlElement(";")]
        public vxEnumQuality SSAO;

        public vxSettingsGraphics() { }
        //
        //Constructor
        //
        public vxSettingsGraphics(
            int Int_Resolution,
            bool Bool_FullScreen,
            bool Bool_VSync,
            vxEnumQuality ShadowQuality,
            bool Bool_DoEdgeDetection,
            vxEnumQuality GodRays,
            vxEnumQuality DoBloom,
            vxEnumQuality DoDepthOfField,
            vxEnumQuality DoSSAO)
        {
            
            //Graphics
            this.Int_Resolution = Int_Resolution;
            this.Bool_FullScreen = Bool_FullScreen;
            this.Bool_VSync = Bool_VSync;
            
            this.ShadowQuality = ShadowQuality;
            this.Bool_DoEdgeDetection = Bool_DoEdgeDetection;
            this.GodRays = GodRays;
            this.Bloom = DoBloom;
            this.DepthOfField = DoDepthOfField;
            this.SSAO = DoSSAO;
        }   
    }
}
