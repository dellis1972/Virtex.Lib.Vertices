﻿using Virtex.Lib.Vrtc.Physics.BEPU.CollisionTests.CollisionAlgorithms;
using BEPUutilities.ResourceManagement;

namespace Virtex.Lib.Vrtc.Physics.BEPU.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a convex and an instanced mesh.
    ///</summary>
    public class MobileMeshSphereContactManifold : MobileMeshContactManifold
    {

        UnsafeResourcePool<TriangleSpherePairTester> testerPool = new UnsafeResourcePool<TriangleSpherePairTester>();
        protected override void GiveBackTester(TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleSpherePairTester)tester);
        }

        protected override TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }

    }
}
