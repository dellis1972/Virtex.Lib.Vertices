using Virtex.Lib.Vertices.Physics.BEPU.CollisionTests.Manifolds;

namespace Virtex.Lib.Vertices.Physics.BEPU.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a instanced mesh-convex collision pair.
    ///</summary>
    public class InstancedMeshSpherePairHandler : InstancedMeshPairHandler
    {
        InstancedMeshSphereContactManifold contactManifold = new InstancedMeshSphereContactManifold();
        protected override InstancedMeshContactManifold MeshManifold
        {
            get { return contactManifold; }
        }
        
    }

}
