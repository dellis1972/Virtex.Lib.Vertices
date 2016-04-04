using vxVertices.Physics.BEPU.CollisionTests.CollisionAlgorithms;
using BEPUutilities.ResourceManagement;

namespace vxVertices.Physics.BEPU.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a convex and an instanced mesh.
    ///</summary>
    public class MobileMeshTriangleContactManifold : MobileMeshContactManifold
    {

        UnsafeResourcePool<TriangleTrianglePairTester> testerPool = new UnsafeResourcePool<TriangleTrianglePairTester>();
        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleTrianglePairTester)tester);
        }

        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }

    }
}
