using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Localization
{
    public class vxLanguagePackKoreanBase : vxLanguagePackBase
    {
        public vxLanguagePackKoreanBase() : base("한글 (Korean)")
        {
            Add(vxLocalization.Settings_Graphics, "그래픽 품질");
            Add(vxLocalization.Graphics_GraphicsSettings, "그래픽 품질");
            Add(vxLocalization.Graphics_Resolution, "해결");
            Add(vxLocalization.Graphics_FullScreen, "전체 화면");
            Add(vxLocalization.Graphics_Windowed, "창");
        }
    }
}
