using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.Core.Settings
{
    public class PlayerProfile
    {
        //public UserGraphicsProfile UserGraphicsProfile = new UserGraphicsProfile();

        public string String_ProfileName = "v.1";

        //Default Settings
		public vxSettingsFileStructure Settings = new vxSettingsFileStructure(
            //Controls
            new vxSettingsControls(
                new vxSettingsControlsMouse(1, 1), 
                new vxSettingsControlsKeyboard(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.LeftControl),
                new vxSettingsControlsGamePad(1, 1)
                ),
            //Graphics
            new vxGraphicsSettingsFileStructure(1280,720, false, false, vxEnumQuality.Medium, true, vxEnumQuality.Medium, vxEnumQuality.None, vxEnumQuality.None, vxEnumQuality.Medium),
            //Audio
            new vxSettingsAudio(1, 1, 1));



        public PlayerProfile()
        {

        }
        
        public void LoadSettings(vxEngine vxEngine)
        {
            try
            {
				vxConsole.WriteLine(string.Format("Loading Settings at: {0}", 
					vxEnviroment.GetVar(vxEnumEnvVarType.PATH_SETTINGS).Value.ToString() + "settings.set"));
				XmlSerializer deserializer = new XmlSerializer(typeof(vxSettingsFileStructure));
				TextReader reader = new StreamReader(vxEnviroment.GetVar(vxEnumEnvVarType.PATH_SETTINGS).Value.ToString() + "settings.set");
                object obj = deserializer.Deserialize(reader);
				Settings = (vxSettingsFileStructure)obj;
                reader.Close();
            }

            catch (Exception exception)
            {
				vxConsole.WriteLine("Error Loading Settings! " + exception.Message);
                
				// Create our menu entries.
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if (mode.Width > 599 || mode.Height > 479)
                    {
						//Set the Largest Resolution as default
						if (Settings.Graphics.Int_Resolution_X < mode.Width && 
							Settings.Graphics.Int_Resolution_Y < mode.Height) {

							//Set the New Resolution
							Settings.Graphics.Int_Resolution_X = mode.Width;
							Settings.Graphics.Int_Resolution_Y = mode.Height;
							Console.WriteLine ("res: {0}x{1}",mode.Width, mode.Height);
						}
                    }
                }

                SaveSettings(vxEngine);
            }
        }
        
        public void SaveSettings(vxEngine vxEngine)
        {
            try
			{
				vxConsole.WriteLine("Saving New Settings File.");
                //Write The Sandbox File
				XmlSerializer serializer = new XmlSerializer(typeof(vxSettingsFileStructure));
				using (TextWriter writer = new StreamWriter(vxEnviroment.GetVar(vxEnumEnvVarType.PATH_SETTINGS).Value.ToString() + "settings.set"))
                {
                    serializer.Serialize(writer, Settings);
				}
            }
            catch (Exception exception)
            {
                vxConsole.WriteLine("Error Saving Settings : " + exception.Message);
            }
        }
    }
}
