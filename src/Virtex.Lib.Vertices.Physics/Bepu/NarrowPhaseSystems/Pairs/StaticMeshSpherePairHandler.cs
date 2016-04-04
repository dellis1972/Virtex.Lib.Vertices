using vxVertices.Physics.BEPU.CollisionTests.Manifolds;

namespace vxVertices.Physics.BEPU.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Handles a static mesh-sphere collision pair.
    ///</summary>
    public class StaticMeshSpherePairHandler : StaticMeshPairHandler
    {

        StaticMeshSphereContactManifold contactManifold = new StaticMeshSphereContactManifold();
        protected override StaticMeshContactManifold MeshManifold
        {
            get { return contactManifold; }
        }


    }

}
