using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.Localization
{
    public enum vxLocalization
    {
        //Main Menu
        Main_Multiplayer,
        Main_Sandbox,
        Main_Settings,
        Main_Exit,

        //Multiplayer
        Net_Local,
        Net_Online,
        Net_Join,

        //Pause Screen
        Pause,
        Pause_Resume,
        Pause_AreYouSureYouWantToQuit,

        //Settings Page
        Settings_Controls,
        Settings_Graphics,
        Settings_Localization,
        Settings_Audio,


        //Graphics Settings
        Graphics_GraphicsSettings,
        Graphics_Resolution,
        Graphics_FullScreen,
        Graphics_Windowed,

        //Misc
        Misc_Yes,
        Misc_No,
        Misc_OK,
        Misc_Cancel,
        Misc_New,
        Misc_Open,
        Misc_Back
    }

    /// <summary>
    /// A base class which can be inherited too hold all text for a specific language pack.
    /// </summary>
    public partial class vxLanguagePackBase
    {
        /// <summary>
        /// The collection holding all of the language keys and text.
        /// </summary>
        Dictionary<object, string> Collection = new Dictionary<object, string>();

        /// <summary>
        /// Get's the Language Name of this Pack
        /// </summary>
        public string LanguageName
        {
            get { return _languageName; }
        }
        private string _languageName;


        /// <summary>
        /// Constructor for the vxLanguagePack base class. Add all of your required language keys here.
        /// </summary>
        /// <param name="LanguageName"></param>
        public vxLanguagePackBase(string LanguageName)
        {
            _languageName = LanguageName;

            //Main Menu
            Add(vxLocalization.Main_Multiplayer, "Multiplayer");
            Add(vxLocalization.Main_Sandbox, "Sandbox");
            Add(vxLocalization.Main_Settings, "Settings");
            Add(vxLocalization.Main_Exit, "Exit");

            //Multiplayer
            Add(vxLocalization.Net_Local, "Local");
            Add(vxLocalization.Net_Online, "Online");

            //Pause
            Add(vxLocalization.Pause, "Pause");
            Add(vxLocalization.Pause_Resume, "Resume");
            Add(vxLocalization.Pause_AreYouSureYouWantToQuit, "Are you sure you want to quit this game?");

            //Settings Page
            Add(vxLocalization.Settings_Controls, "Controls");
            Add(vxLocalization.Settings_Graphics, "Graphics");
            Add(vxLocalization.Settings_Localization, "Localization");
            Add(vxLocalization.Settings_Audio, "Audio");

            //Graphics Settings
            Add(vxLocalization.Graphics_GraphicsSettings, "Graphics Settings");
            Add(vxLocalization.Graphics_Resolution, "Resolution");
            Add(vxLocalization.Graphics_FullScreen, "Full Screen");
            Add(vxLocalization.Graphics_Windowed, "Windowed");

            //Misc
            Add(vxLocalization.Misc_Yes, "Yes");
            Add(vxLocalization.Misc_No, "No");
            Add(vxLocalization.Misc_OK, "OK");
            Add(vxLocalization.Misc_Cancel, "Cancel");
            Add(vxLocalization.Misc_New, "New");
            Add(vxLocalization.Misc_Open, "Open");
            Add(vxLocalization.Misc_Back, "Back");

        }

        /// <summary>
        /// Add's a line of text to the language pack based off of a supplied key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="Text"></param>
        public void Add(object key, string Text)
        {
            //First Check if the Key exists, if so, then overwrite the old values
            if(Collection.ContainsKey(key))
            {
                Collection[key] = Text;
            }
            //If not, Add a new Key
            else
            {
                Collection.Add(key, Text);
            }
        }

        /// <summary>
        /// Returns the Text Specified by the given key. If the key is not present, an warning is fired.
        /// </summary>
        /// <param name="key">The key which is associated with a given line of text for this language.</param>
        /// <returns>The Text associated with the supplied key for this language.</returns>
        public string Get(object key)
        {
            if (Collection.ContainsKey(key))
                return Collection[key];
            else
            {
                string outputWarning = string.Format("KEY '{0}' IN LANGUAGE DICTIONARY NOT FOUND!", key);
                vxConsole.WriteWarning(this.ToString(), outputWarning);
                return outputWarning;
            }
        }
    }
}
