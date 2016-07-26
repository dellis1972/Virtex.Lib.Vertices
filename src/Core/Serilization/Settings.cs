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

namespace Virtex.Lib.Vertices.Core.Settings
{
	public enum vxEnumQuality
	{
		None,
		Low,
		Medium,
		High,
		Ultra
	}

	/// <summary>
	/// Serializable Settings.
	/// </summary>
	public class vxSettings
	{
		//Controls
		[XmlElement ("SettingsControls")]
		public vxSettingsControls Controls;

		//Graphics
		[XmlElement ("vxSettingsGraphics")]
		public vxSettingsGraphics Graphics;

		//Audio
		[XmlElement ("vxSettingsAudio")]
		public vxSettingsAudio Audio;

		public vxSettings ()
		{
		}

		//
		//Constructor
		//
		public vxSettings (
			vxSettingsControls Controls,
			vxSettingsGraphics Graphics,
			vxSettingsAudio Audio)
		{
			//Controls
			this.Controls = Controls;

			//Graphics
			this.Graphics = Graphics;

			//Audio
			this.Audio = Audio;
		}
	}
}
