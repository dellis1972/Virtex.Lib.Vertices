#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Virtex.Lib.Vertices.Physics.BEPU.Entities.Prefabs;
using Virtex.Lib.Vertices.Core.Debug;

namespace Virtex.Lib.Vertices.Geometry
{
    public class Tri
    {
        public List<Vector3> ListPoints = new List<Vector3>();

        //The Faces Geometric Center
        public Vector3 Center { get; set; }

        //The Faces Normal
        public Vector3 Normal { get; set; }

        public float d = 0;

        public Triangle face;

        public Plane plane;

        //A Four Point Face
        public Tri(Vector3 Pt1,
            Vector3 Pt2,
            Vector3 Pt3)
        {
            ListPoints.Add(Pt1);
            ListPoints.Add(Pt2);
            ListPoints.Add(Pt3);

            //Calcaulate the Center
            foreach (Vector3 pt in ListPoints)
                Center += pt;

            Center /= 3;
            
            face = new Triangle(Pt1, Pt2, Pt3);
            face.CollisionInformation.Tag= "TRI";
            
            Normal = Vector3.Cross(Vector3.Subtract(Pt3, Pt1), Vector3.Subtract(Pt2, Pt1));
            Normal.Normalize();

            plane = new Plane(Pt1, Pt2, Pt3);
        }

        public void DrawTri()
        {
            for (int i = 0; i < ListPoints.Count; i++)
                vxDebugShapeRenderer.AddLine(ListPoints[i % 3], ListPoints[(i + 1) % 3], Color.Red);
        }

        public void Draw()
        {

            /*
            for (int i = 0; i < ListPoints.Count; i++)
                DebugShapeRenderer.AddLine(ListPoints[i%3], ListPoints[(i+1)%3], Color.Red);

            DebugShapeRenderer.AddBoundingSphere(new BoundingSphere(Center, 0.5f), Color.White);
            DebugShapeRenderer.AddLine(Center, Center + Normal/10, Color.LightGreen);
            */
        }
    }
}
#endif