using Microsoft.Xna.Framework;

using BEPUutilities;

namespace Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseSystems
{
    ///<summary>
    /// Requires that a class have a BoundingBox.
    ///</summary>
    public interface IBoundingBoxOwner
    {
        ///<summary>
        /// Gets the bounding box of the object.
        ///</summary>
        BoundingBox BoundingBox { get; }
    }
}
