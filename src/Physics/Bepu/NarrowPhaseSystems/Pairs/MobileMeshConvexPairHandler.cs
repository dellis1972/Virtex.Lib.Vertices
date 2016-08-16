using Virtex.Lib.Vrtc.Physics.BEPU.CollisionTests.Manifolds;

namespace Virtex.Lib.Vrtc.Physics.BEPU.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a mobile mesh-convex collision pair.
    ///</summary>
    public class MobileMeshConvexPairHandler : MobileMeshPairHandler
    {
        MobileMeshConvexContactManifold contactManifold = new MobileMeshConvexContactManifold();
        protected internal override MobileMeshContactManifold MeshManifold
        {
            get { return contactManifold; }
        }



    }

}
