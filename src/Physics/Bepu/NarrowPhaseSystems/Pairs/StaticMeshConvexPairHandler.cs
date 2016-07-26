using Virtex.Lib.Vertices.Physics.BEPU.CollisionTests.Manifolds;

namespace Virtex.Lib.Vertices.Physics.BEPU.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a static mesh-convex collision pair.
    ///</summary>
    public class StaticMeshConvexPairHandler : StaticMeshPairHandler
    {

        StaticMeshConvexContactManifold contactManifold = new StaticMeshConvexContactManifold();
        protected override StaticMeshContactManifold MeshManifold
        {
            get { return contactManifold; }
        }


    }

}
