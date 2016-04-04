using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;

//Virtex vxEngine Declaration
using vxVertices.Core;

namespace vxVertices.Scenes.Sandbox
{
	/// <summary>
	/// Sand box file structure.
	/// </summary>
	public class vxSandBoxFileStructure
    {
        //File Save Name
        [XmlAttribute("Name")]
        public string Name = "sanboxfile1";

        //Importer Version
        [XmlElement("ioVersion")]
        public int ioVersion = 1;
        
        //Importer Version
        [XmlElement("texture")]
        public byte[] texture;

        //Importer Version
        [XmlElement("textureWidth")]
        public int textureWidth;

        //Importer Version
        [XmlElement("textureHeight")]
        public int textureHeight;
        
        [XmlElement("SandboxItems")]
        public List<vxSandboxItemStruct> items { get; set; }

		public vxSandBoxFileStructure()
        {
            items = new List<vxSandboxItemStruct>();
        }
    }

    public class vxSandboxItemStruct
    {
        [XmlAttribute("id")]
        public int id = 0;

        [XmlAttribute("Type")]
        public string Type;

        [XmlElement("Orientation")]
        public Matrix Orientation;

        [XmlAttribute("FilePath")]
        public string FilePath;

        [XmlAttribute("UserDefinedData01")]
        public string UserDefinedData01;

        [XmlAttribute("UserDefinedData02")]
        public string UserDefinedData02;

        [XmlAttribute("UserDefinedData03")]
        public string UserDefinedData03;

        [XmlAttribute("UserDefinedData04")]
        public string UserDefinedData04;

        [XmlAttribute("UserDefinedData05")]
        public string UserDefinedData05;

		public vxSandboxItemStruct()
        {
            id = 0;
            Type = "null";
            Orientation = Matrix.Identity;
            UserDefinedData01 = "-- insert user defined data here --";
            UserDefinedData02 = "-- insert user defined data here --";
            UserDefinedData03 = "-- insert user defined data here --";
            UserDefinedData04 = "-- insert user defined data here --";
            UserDefinedData05 = "-- insert user defined data here --";
        }

		public vxSandboxItemStruct(
            int ID,
            string type,
            Matrix orientation,
            string userDefinedData01,
            string userDefinedData02,
            string userDefinedData03,
            string userDefinedData04,
            string userDefinedData05)
        {
            id = ID;
            Type = type;
            Orientation = orientation;
            FilePath = "NA";
            UserDefinedData01 = userDefinedData01;
            UserDefinedData02 = userDefinedData02;
            UserDefinedData03 = userDefinedData03;
            UserDefinedData04 = userDefinedData04;
            UserDefinedData05 = userDefinedData05;
        }
    }
}
