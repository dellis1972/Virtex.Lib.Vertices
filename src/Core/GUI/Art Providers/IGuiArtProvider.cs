using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.GUI
{
    /// <summary>
    /// The GUI Art provider acts as the "Renderer" for GUI Elements. This provides an easy way to override
    /// graphical style for a given GUI Element.
    /// </summary>
    public interface IGuiArtProvider : ICloneable
    {
        /// <summary>
        /// The object past through can be anything. Often times what you're wanting to draw.
        /// </summary>
        /// <param name="guiItem"></param>
        void Draw(object guiItem);
    }
}
