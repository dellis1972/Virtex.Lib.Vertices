using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Localization
{
    public class LanguageFrench : vxLanguagePack
    {
        public LanguageFrench() : base("Français")
        {
            Graphics = "Graphique";
            GraphicsSettings = "Paramètres Graphiques";
            Resolution = "Résolution";
            FullScreen = "Plein Écran";
            Windowed = "Fenêtré";
        }
    }
}
