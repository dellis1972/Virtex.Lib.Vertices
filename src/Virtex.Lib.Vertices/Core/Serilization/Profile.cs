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
using vxVertices.Core;

namespace vxVertices.Core.Settings
{
    public class PlayerProfile
    {
        //public UserGraphicsProfile UserGraphicsProfile = new UserGraphicsProfile();

        public string String_ProfileName = "v.1";

        //Default Settings
        public vxSettings Settings = new vxSettings(
            //Controls
            new vxSettingsControls(
                new vxSettingsControlsMouse(1, 1), 
                new vxSettingsControlsKeyboard(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.LeftControl),
                new vxSettingsControlsGamePad(1, 1)
                ),
            //Graphics
            new vxSettingsGraphics(4, false, false, vxEnumQuality.Medium, true, vxEnumQuality.Medium, vxEnumQuality.Medium, vxEnumQuality.Medium, vxEnumQuality.Medium),
            //Audio
            new vxSettingsAudio(1, 1, 1));



        public PlayerProfile()
        {

        }
        
        public void LoadSettings(vxEngine vxEngine)
        {
            try
            {
                //Console.Write("Loading Settings...");
                XmlSerializer deserializer = new XmlSerializer(typeof(vxSettings));
                TextReader reader = new StreamReader(vxEngine.Path_Profiles + "settings.set");
                object obj = deserializer.Deserialize(reader);
                Settings = (vxSettings)obj;
                reader.Close();
                //Console.WriteLine("Done!");
            }

            catch (Exception exception)
            {
                Console.WriteLine("Error!");
                vxEngine.WriteError("Profile.cs", "LoadSettings", exception.Message);
            
                // Create our menu entries.
                int LoopCount = 0;
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if (mode.Width > 599 || mode.Height > 479)
                    {
                        LoopCount++;
                    }
                }
                vxEngine.Profile.Settings.Graphics.Int_Resolution = LoopCount-1;
                Settings.Graphics.Int_Resolution = LoopCount - 1;
                Console.Write("Saving New Settings File...");
                SaveSettings(vxEngine);
                vxEngine.WriteLine_Green("Done!");
            }
        }
        
        public void SaveSettings(vxEngine vxEngine)
        {
            try
            {
                //Write The Sandbox File
                XmlSerializer serializer = new XmlSerializer(typeof(vxSettings));
                using (TextWriter writer = new StreamWriter(vxEngine.Path_Profiles + "settings.set"))
                {
                    serializer.Serialize(writer, Settings);
                }
            }
            catch (Exception exception)
            {
                vxEngine.WriteError("Profile.cs", "SaveSettings", exception.Message);
            }
        }
    }
}
