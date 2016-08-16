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

namespace Virtex.Lib.Vrtc.Core.Settings
{
    public class vxSettingsControls
    {
        //Controls
        [XmlElement("SettingsControlsMouse")]
        public vxSettingsControlsMouse MouseSettings;

        [XmlElement("SettingsControlsKeyboard")]
        public vxSettingsControlsKeyboard KeyboardSettings;

        [XmlElement("vxSettingsControlsGamePad")]
        public vxSettingsControlsGamePad GamePadSettings;
        

        public vxSettingsControls() { }
        //
        //Constructor
        //
        public vxSettingsControls(
            vxSettingsControlsMouse MouseSettings,
            vxSettingsControlsKeyboard KeyboardSettings,
            vxSettingsControlsGamePad GamePadSettings)
        {
            //Controls
            this.MouseSettings = MouseSettings;
            this.KeyboardSettings = KeyboardSettings;
            this.GamePadSettings = GamePadSettings;
        }
    }

    public class vxSettingsControlsMouse
    {
        //Controls
        [XmlElement("Double_Mouse_Sensitivity")]
        public float Double_Mouse_Sensitivity;
        [XmlElement("Int_Mouse_Inverted")]
        public int Int_Mouse_Inverted;

        public vxSettingsControlsMouse() { }
        //
        //Constructor
        //
        public vxSettingsControlsMouse(
            float Double_Mouse_Sensitivity,
            int Int_Mouse_Inverted)
        {
            //Controls
            this.Double_Mouse_Sensitivity = Double_Mouse_Sensitivity;
            this.Int_Mouse_Inverted = Int_Mouse_Inverted;
        }
    }

    public class vxSettingsControlsKeyboard
    {
        [XmlElement("Key_Movement_Forwards")]
        public Keys Key_Movement_Forwards;

        [XmlElement("Key_Movement_Backwards")]
        public Keys Key_Movement_Backwards;

        [XmlElement("Key_Movement_Left")]
        public Keys Key_Movement_Left;

        [XmlElement("Key_Movement_Right")]
        public Keys Key_Movement_Right;

        [XmlElement("Key_Movement_Jump")]
        public Keys Key_Movement_Jump;

        [XmlElement("Key_Movement_Crouch")]
        public Keys Key_Movement_Crouch;

        public vxSettingsControlsKeyboard() { }
        //
        //Constructor
        //
        public vxSettingsControlsKeyboard(
            Keys Key_Movement_Forwards,
            Keys Key_Movement_Backwards,
            Keys Key_Movement_Left,
            Keys Key_Movement_Right,
            Keys Key_Movement_Jump,
            Keys Key_Movement_Crouch)
        {
            //Controls
            this.Key_Movement_Forwards = Key_Movement_Forwards;
            this.Key_Movement_Backwards = Key_Movement_Backwards;
            this.Key_Movement_Left = Key_Movement_Left;
            this.Key_Movement_Right = Key_Movement_Right;
            this.Key_Movement_Jump = Key_Movement_Jump;
            this.Key_Movement_Jump = Key_Movement_Jump;
        }
    }
    
    public class vxSettingsControlsGamePad
    {
        //Controls
        [XmlElement("Double_GamePad_Sensitivity")]
        public float Double_GamePad_Sensitivity;
        [XmlElement("Int_GamePad_Inverted")]
        public int Int_GamePad_Inverted;

        public vxSettingsControlsGamePad() { }
        //
        //Constructor
        //
        public vxSettingsControlsGamePad(
            float Double_GamePad_Sensitivity,
            int Int_GamePad_Inverted)
        {
            //Controls
            this.Double_GamePad_Sensitivity = Double_GamePad_Sensitivity;
            this.Int_GamePad_Inverted = Int_GamePad_Inverted;
        }
    }
}
