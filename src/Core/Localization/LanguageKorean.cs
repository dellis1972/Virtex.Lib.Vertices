using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Localization
{
    public class LanguageKorean : vxLanguagePack
    {
        public LanguageKorean() : base("한글 (Korean)")
        {
            Graphics = "그래픽 품질";
            GraphicsSettings = "그래픽 품질";

            Resolution = "해결";
            FullScreen = "전체 화면";
            Windowed = "창";
        }
    }
}
