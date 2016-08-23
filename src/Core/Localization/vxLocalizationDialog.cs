using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Core.Settings;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Localization;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxLocalizationDialog : vxDialogBase
    {

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxLocalizationDialog()
			: base("Localization", vxEnumButtonTypes.OkApplyCancel)
        {

        }

        public override void LoadContent()
        {
            vxEngine.LoadResolution = true;
            base.LoadContent();


			//All Items below are stored in this column as it's the longest word
			
            float Margin = 25;
            float MarginTwo = 400;

            int horiz = 75;
            int horizTwo = 75;
            

            //Full Screen
            /*****************************************************************************************************/
            vxGraphicSettingsItem LanguageSelectSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, "Language", vxEngine.Language.LanguageName, 
                new Vector2(Margin, horiz));
            horiz += 45;
            foreach (vxLanguagePackBase language in vxEngine.Languages)
            {
                LanguageSelectSettingsItem.ValueComboBox.AddItem(language.LanguageName);
            }

            LanguageSelectSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

                vxEngine.Language = vxEngine.Languages[e.SelectedIndex];

                vxConsole.WriteLine("Setting Language to: " + vxEngine.Language.LanguageName);
            };
                        
            Btn_Apply.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
        }

        void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
            ExitScreen();
        }

        void SetSettings()
        {
            Console.WriteLine("Setting Items");
            //Save Settings
            vxEngine.Profile.SaveSettings(vxEngine);
        }
    }
}
