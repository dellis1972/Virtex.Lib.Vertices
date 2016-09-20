using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vrtc.Graphics
{
    public interface vxPostProcessorInterface
    {
        void SetResoultion();

        void LoadContent();

        void Apply();
    }
}
