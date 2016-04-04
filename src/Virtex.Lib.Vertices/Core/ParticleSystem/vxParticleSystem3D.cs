#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace vxVertices.Core.Particles
{
    public class vxParticleSystem3D
    {
		public List<vxParticle3D> List_Particle = new List<vxParticle3D>();

        //For keeping track of which items to get rid of
        public List<int> intList = new List<int>();

		public vxParticleSystem3D()
        {

        }

		public void Add(vxParticle3D particle)
        {
            List_Particle.Add(particle);
        }

        public void Clear()
        {
            List_Particle.Clear();
        }

        public void Update(GameTime gameTime)
        {
            int removeIndex = 0;
			foreach (vxParticle3D particle in List_Particle)
            {
                if (particle.Alive == true)
                {
                    //particle.UpdateParticle(gameTime);
                }
                else
                    intList.Add(removeIndex);

                removeIndex++;
            }
            //Remove Used Up Particls to keep memory low
            foreach (int i in intList)
                List_Particle.RemoveAt(i);

            intList.Clear();
        }

        public void Draw()
        {
			foreach (vxParticle3D particle in List_Particle)
            {
                if (particle.Alive == true)
                {
                    //particle.DrawParticle();
                }
            }

        }
    }
}
#endif