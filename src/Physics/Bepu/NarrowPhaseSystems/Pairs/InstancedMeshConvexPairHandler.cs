using Virtex.Lib.Vrtc.Physics.BEPU.CollisionTests.Manifolds;

namespace Virtex.Lib.Vrtc.Physics.BEPU.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a instanced mesh-convex collision pair.
    ///</summary>
    public class InstancedMeshConvexPairHandler : InstancedMeshPairHandler
    {

        InstancedMeshConvexContactManifold contactManifold = new InstancedMeshConvexContactManifold();
        protected override InstancedMeshContactManifold MeshManifold
        {
            get { return contactManifold; }
        }
        

    }

}
