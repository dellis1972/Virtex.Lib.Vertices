using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vrtc.Localization
{
    public class vxLanguagePackFrenchBase : vxLanguagePackBase
    {
        public vxLanguagePackFrenchBase() : base("Français")
        {
            //Main Menu
            Add(vxLocalization.Main_Multiplayer, "Mode Multijoueur");
            Add(vxLocalization.Main_Sandbox, "Sandbox");
            Add(vxLocalization.Main_Settings, "Paramètres");
            Add(vxLocalization.Main_Exit, "Sortie");

            //Multiplayer
            Add(vxLocalization.Net_Local, "Locale");
            Add(vxLocalization.Net_Online, "En Ligne");

            //Pause
            Add(vxLocalization.Pause, "Pause");
            Add(vxLocalization.Pause_Resume, "Continuer");
            Add(vxLocalization.Pause_AreYouSureYouWantToQuit, "Êtes-vous sûr de vouloir quitter ce jeu?");


            //Settings Page
            Add(vxLocalization.Settings_Controls, "Contrôles");
            Add(vxLocalization.Settings_Graphics, "Graphique");
            Add(vxLocalization.Settings_Localization, "Localisation");
            Add(vxLocalization.Settings_Audio, "Audio");

            //Graphics Page
            Add(vxLocalization.Graphics_GraphicsSettings, "Paramètres Graphiques");
            Add(vxLocalization.Graphics_Resolution, "Résolution");
            Add(vxLocalization.Graphics_FullScreen, "Plein Écran");
            Add(vxLocalization.Graphics_Windowed, "Fenêtré");

            //Misc
            Add(vxLocalization.Misc_Yes, "Oui");
            Add(vxLocalization.Misc_No, "Non");
            Add(vxLocalization.Misc_OK, "OK");
            Add(vxLocalization.Misc_Cancel, "Annuler");
            Add(vxLocalization.Misc_New, "Nouveau");
            Add(vxLocalization.Misc_Open, "Ouvrir");
            Add(vxLocalization.Misc_Back, "Arrière");
        }
    }
}
