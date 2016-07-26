using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virtex.Lib.Vertices.Core.Cameras
{
	public class vxVirtualCamera : vxCamera3D
    {
        Matrix mView;
        float mRotationY = 0.0f;

		public vxVirtualCamera()
        {
            mView = new Matrix(
                0.999263f, 0.00147375371f, -0.038360253f, 0.0f,
                0.0000000126671047f, 0.999262869f, 0.03839077f, 0.0f,
                 0.0383885577f, -0.038362477f, 0.9985262f, 0.0f,
                -37.09832f, -76.10451f, -291.163971f, 1.0f
             );

            View = mView;
        }

        public override void Update(GameTime time)
        {
            mRotationY += time.ElapsedGameTime.Milliseconds * 0.001f;
            View = Matrix.CreateRotationY(mRotationY) * mView;
        }
    }
}
