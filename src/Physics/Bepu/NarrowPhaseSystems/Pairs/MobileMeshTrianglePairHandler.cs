using vxVertices.Physics.BEPU.CollisionTests.Manifolds;

namespace vxVertices.Physics.BEPU.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a mobile mesh-convex collision pair.
    ///</summary>
    public class MobileMeshTrianglePairHandler : MobileMeshPairHandler
    {
        MobileMeshTriangleContactManifold contactManifold = new MobileMeshTriangleContactManifold();
        protected internal override MobileMeshContactManifold MeshManifold
        {
            get { return contactManifold; }
        }



    }

}
