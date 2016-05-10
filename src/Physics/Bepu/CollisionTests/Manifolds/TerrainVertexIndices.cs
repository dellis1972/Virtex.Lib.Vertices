using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vxVertices.Physics.BEPU.CollisionTests.Manifolds
{
    internal struct TerrainVertexIndices
    {
        public int ColumnIndex;
        public int RowIndex;

        public int ToSequentialIndex(int terrainWidth)
        {
            return RowIndex * terrainWidth + ColumnIndex;
        }
    }
}
