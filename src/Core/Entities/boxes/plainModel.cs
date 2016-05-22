#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using vxVertices.Core;
using vxVertices.Core.Entities;
using vxVertices.Entities.Sandbox3D;
using vxVertices.Utilities;

namespace vxVertices.Core.Entities
{
    public class plainModel : vxEntity3D
    {
        Model Newmodel;

        public plainModel(vxEngine vxEngine, Model Newmodel, Vector3 StartPosition) :
            base(vxEngine, StartPosition)
        {
            this.Newmodel = Newmodel;// LoadModel(path, vxEngine.EngineContentManager, effectReplacement);
        }        
        
        public void Draw()
        {
            if (Newmodel != null)
            {
                // Look up the bone transform matrices.
                Matrix[] transforms = new Matrix[Newmodel.Bones.Count];

                Newmodel.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model.
                foreach (ModelMesh mesh in Newmodel.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        // Specify which effect technique to use.
                        effect.CurrentTechnique = effect.Techniques["Technique1"];

                        effect.Parameters["World"].SetValue(World);
                        effect.Parameters["View"].SetValue(Camera.View);
                        effect.Parameters["Projection"].SetValue(Camera.Projection);
                        //Set The Colour
                        //effect.Parameters["PlainColor"].SetValue(new Vector4(PlainColor.R, PlainColor.G, PlainColor.B, AlphaValue));
                    }
                    mesh.Draw();
                }
            }
        }

        public static Model LoadModel(string Path, ContentManager Content, Effect replacementEffect)
        {
            Model modelToReturn;
            
            modelToReturn = Content.Load<Model>(Path);
            
            
            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\t\tImporting Model: " + Path);
                // Table mapping the original effects to our replacement versions.
                Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

                foreach (ModelMesh mesh in modelToReturn.Meshes)
                {

                    Console.WriteLine("\t\t\tMesh Mesh: " + mesh.Name);
                    mesh.Tag = Path;
                    // Scan over all the effects currently on the mesh.
                    foreach (Effect oldEffect in mesh.Effects)
                    {
                        // If we haven't already seen this effect...
                        if (!effectMapping.ContainsKey(oldEffect))
                        {
                            // Make a clone of our replacement effect. We can't just use
                            // it directly, because the same effect might need to be
                            // applied several times to different parts of the model using
                            // a different texture each time, so we need a fresh copy each
                            // time we want to set a different texture into it.
                            Effect newEffect = replacementEffect.Clone();
                            
                            effectMapping.Add(oldEffect, newEffect);
                        }
                    }

                    // Now that we've found all the effects in use on this mesh,
                    // update it to use our new replacement versions.
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = effectMapping[meshPart.Effect];
                    }
                }
            }
            catch (Exception ex)
            {
                vxConsole.WriteError("Plain Model.cs","ERROR IMPORTING FILE: " + Path + "\n" + ex.Message);
            }
            //#endif
            modelToReturn.Tag = Path;
            return modelToReturn;
        }
    }


}
#endif