using vxVertices.Physics.BEPU.CollisionTests.CollisionAlgorithms;
using BEPUutilities.ResourceManagement;

namespace vxVertices.Physics.BEPU.CollisionTests.Manifolds
{
    public class TerrainSphereContactManifold : TerrainContactManifold
    {
        static LockingResourcePool<TriangleSpherePairTester> testerPool = new LockingResourcePool<TriangleSpherePairTester>();
        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }

        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleSpherePairTester)tester);
        }

    }
}
