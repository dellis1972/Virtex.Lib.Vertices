using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Virtex.Lib.Vrtc.Core.Debug;

namespace Virtex.Lib.Vrtc.Geometry
{
    public class Quad
    {
        List<Vector3> ListPoints = new List<Vector3>();

        //The Faces Geometric Center
        public Vector3 Center { get; set; }

        //The Faces Normal
        public Vector3 Normal { get; set; }
        
        //A Four Point Face
        public Quad(Vector3 Pt1,
            Vector3 Pt2,
            Vector3 Pt3,
            Vector3 Pt4)
        {
            ListPoints.Add(Pt1);
            ListPoints.Add(Pt2);
            ListPoints.Add(Pt3);
            ListPoints.Add(Pt4);

            //Calcaulate the Center
            foreach (Vector3 pt in ListPoints)
            {
                Center += pt;
            }
            Center /= 4;
        }

        public void Draw()
        {
            for (int i = 0; i < ListPoints.Count-1; i++)
                vxDebugShapeRenderer.AddLine(ListPoints[i], ListPoints[i+1], Color.Red);

            for (int i = 0; i < ListPoints.Count - 1; i++)
                vxDebugShapeRenderer.AddBoundingSphere(new BoundingSphere(Center, 0.5f), Color.White); 

        }
    }
}
