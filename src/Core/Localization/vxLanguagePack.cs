using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Localization
{
    /// <summary>
    /// A base class which can be inherited too hold all text for a specific language pack.
    /// </summary>
    public class vxLanguagePack
    {
        /// <summary>
        /// Get's the Language Name of this Pack
        /// </summary>
        public string LanguageName
        {
            get { return _languageName; }
        }
        private string _languageName;

        public string Graphics = "Graphics";
        public string GraphicsSettings = "Graphics Settings";
        public string Resolution = "Resolution";
        public string FullScreen = "Full Screen";
        public string Windowed = "Windowed";

        /// <summary>
        /// Constructor for the vxLanguagePack base class
        /// </summary>
        /// <param name="LanguageName"></param>
        public vxLanguagePack(string LanguageName)
        {
            _languageName = LanguageName;
        }
    }
}
