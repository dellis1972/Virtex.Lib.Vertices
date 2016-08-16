using Virtex.Lib.Vrtc.Physics.BEPU.CollisionTests.CollisionAlgorithms;
using BEPUutilities.ResourceManagement;

namespace Virtex.Lib.Vrtc.Physics.BEPU.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a convex and an instanced mesh.
    ///</summary>
    public class InstancedMeshConvexContactManifold : InstancedMeshContactManifold
    {

        static LockingResourcePool<TriangleConvexPairTester> testerPool = new LockingResourcePool<TriangleConvexPairTester>();
        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleConvexPairTester)tester);
        }

        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }

    }
}
