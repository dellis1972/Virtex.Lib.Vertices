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
using Virtex.Lib.Vrtc.Graphics;

namespace Virtex.Lib.Vrtc.Core.Settings
{
	/// <summary>
	/// Serializable Settings.
	/// </summary>
	public class vxGraphicsSettingsFileStructure
    {
        //Graphics
        [XmlElement("Int_Resolution_X")]
		public int Int_Resolution_X
		{
			get { return vxEnviroment.GetVar (vxEnumEnvVarType.RES_X).GetAsInt (); }
			set { vxEnviroment.GetVar (vxEnumEnvVarType.RES_X).Value = value; }
		}

		[XmlElement("Int_Resolution_Y")]
		public int Int_Resolution_Y
		{
			get { return vxEnviroment.GetVar (vxEnumEnvVarType.RES_Y).GetAsInt (); }
			set { vxEnviroment.GetVar (vxEnumEnvVarType.RES_Y).Value = value; }
		}

        [XmlElement("Bool_FullScreen")]
		public bool Bool_FullScreen
		{
			get { return vxEnviroment.GetVar (vxEnumEnvVarType.FLSCRN).GetAsBool (); }
			set { vxEnviroment.GetVar (vxEnumEnvVarType.FLSCRN).Value = value; }
		}

        [XmlElement("Bool_VSync")]
		public bool Bool_VSync
		{
			get { return vxEnviroment.GetVar (vxEnumEnvVarType.VSYNC).GetAsBool (); }
			set { vxEnviroment.GetVar (vxEnumEnvVarType.VSYNC).Value = value; }
		}

		[XmlElement("TextureQuality")]
		public vxEnumTextureQuality TextureQuality;

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

		[XmlElement("SSAO")]
        public vxEnumQuality SSAO;

		public vxGraphicsSettingsFileStructure() { }
        //
        //Constructor
        //
		public vxGraphicsSettingsFileStructure(
			int Resolution_X,
			int Resolution_Y,
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
			try
			{
			this.Int_Resolution_X = Resolution_X;
			this.Int_Resolution_Y = Resolution_Y;
            this.Bool_FullScreen = Bool_FullScreen;
            this.Bool_VSync = Bool_VSync;
            
            this.ShadowQuality = ShadowQuality;
            this.Bool_DoEdgeDetection = Bool_DoEdgeDetection;
            this.GodRays = GodRays;
            this.Bloom = DoBloom;
            this.DepthOfField = DoDepthOfField;
            this.SSAO = DoSSAO;
			}
			catch(Exception ex){
				//Console.WriteLine (ex.Message);
			}
        }   
    }
}
