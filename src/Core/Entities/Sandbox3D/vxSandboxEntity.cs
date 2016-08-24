#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Entities;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries;
using BEPUutilities;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.Scenes.Sandbox3D;
using Virtex.Lib.Vrtc.Graphics;

namespace Virtex.Lib.Vrtc.Entities.Sandbox3D
{  
    /// <summary>
	/// Sandbox Entity for use in the vxSandboxGamePlay class.
    /// </summary>
    public class vxSandboxEntity : vxEntity3D
    {
        public vxSandboxEntity Parent { get; set; }

        public List<vxSandboxEntityProperty> Properties { get; set; }

        public bool Saveable
        {
            get { return _saveable; }
            set { _saveable = value; }
        }
        bool _saveable = true;

        /// <summary>
        /// Entity world transform matrix.
        /// </summary>
        public Matrix EndOrientation
        {
            get { return _endOrientation; }
            set { _endOrientation = value; }
        }
        Matrix _endOrientation = Matrix.Identity;

        public vxSandboxGamePlay CurrentSandboxLevel
        {
            get { return ((vxSandboxGamePlay)vxEngine.Current3DSceneBase); }
        }

        public List<vxSandboxEntity> ChildEntities = new List<vxSandboxEntity>();

        public vxEnumSelectionState SelectionState = vxEnumSelectionState.Unseleced;
        vxEnumSelectionState PreviousSelectionState = vxEnumSelectionState.Unseleced;

        /// <summary>
        /// Event Fired when the Items Selection stat Changes too Hovered
        /// </summary>
        public event EventHandler<EventArgs> SelectionStateSelected;
        
        /// <summary>
        /// Event Fired when the Items Selection stat Changes too Hovered
        /// </summary>
        public event EventHandler<EventArgs> SelectionStateHovered;
        
        /// <summary>
        /// Event Fired when the Items Selection stat Changes too unselected (or unhovered)
        /// </summary>
        public event EventHandler<EventArgs> SelectionStateUnSelected;


        /// <summary>
        /// State of the Entity which is triggered by the simulation.
        /// </summary>
        public vxEnumSandboxGameState SandboxState = vxEnumSandboxGameState.EditMode;

        public bool HasEndSnapbox = true;

        public string UserDefinedData01 = "no-data";
        public string UserDefinedData02 = "no-data";
        public string UserDefinedData03 = "no-data";
        public string UserDefinedData04 = "no-data";
        public string UserDefinedData05 = "no-data";

        /// <summary>
        /// Thumbnail for Sandbox
        /// </summary>
        public Texture2D Thumbnail;

        /// <summary>
        /// Description of the Entity
        /// </summary>
        public string Description { get; set; }

        public StaticMesh PhysicsSkin_Main { get; set; }

        //Load in mesh data and create the collision mesh.
        public Vector3[] MeshVertices;
        public int[] MeshIndices;

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                if (PhysicsSkin_Main != null)
                    PhysicsSkin_Main.Tag = value;
            }
        }
        int _index = 0;

        public bool AddToPhysicsLibrary = true;

        /// <summary>
        /// The Path to the content file for this entity
        /// </summary>
        public string PathToFile { get; set; }


        /***********************************************/
        //             MODEL ROTATIONS
        /***********************************************/

        /// <summary>
        /// Provides the needed X rotation for the model
        /// </summary>
        public float XRotation_ModelOffset
        {
            get { return _xRotation_ModelOffset; }
            set { _xRotation_ModelOffset = value; }
        }
        float _xRotation_ModelOffset = 0;

        /// <summary>
        /// Provides the needed Y rotation for the model
        /// </summary>
        public float YRotation_ModelOffset
        {
            get { return _yRotation_ModelOffset; }
            set { _yRotation_ModelOffset = value; }
        }
        float _yRotation_ModelOffset = 0;

        /// <summary>
        /// Provides the needed Z rotation for the model
        /// </summary>
        public float ZRotation_ModelOffset
        {
            get { return _zRotation_ModelOffset; }
            set { _zRotation_ModelOffset = value; }
        }
        float _zRotation_ModelOffset = 0;


        vxTextbox nameBox;
        vxTextbox indexProp;


        public Matrix PreSelectionWorld = Matrix.Identity;


        public vxSandboxEntity(vxEngine vxEngine, vxModel EntityModel, Vector3 StartPosition)
            : base(vxEngine, EntityModel, StartPosition)
        {
            Properties = new List<vxSandboxEntityProperty>();
        }

        public virtual void SetMesh(bool AddToPhysics, bool ResetWholeMesh)
        {
           //New Matrix
            World = Matrix.Identity;

            //Correct for Blender Coordinate System Difference
            World *= Matrix.CreateRotationX(-MathHelper.PiOver2);
            World *= Matrix.CreateRotationX(XRotation_ModelOffset);
            World *= Matrix.CreateRotationY(YRotation_ModelOffset);
            World *= Matrix.CreateRotationZ(ZRotation_ModelOffset);
            World *= Matrix.CreateTranslation(Position);
           
            SetMesh(World, AddToPhysics, ResetWholeMesh);
        }

        /// <summary>
        /// A method which allows for certain opperations to be preformed just before the entity is saved to a file.
        /// </summary>
        public virtual void PreSave()
        {

        }


        /// <summary>
        /// A method which allows for certain opperations to be preformed after the entity is loaded from a file.
        /// </summary>
        public virtual void PostLoad()
        {

        }

        public virtual void SetMesh(Matrix NewWorld, bool AddToPhysics, bool ResetWholeMesh)
        {
            /*
            AddToPhysicsLibrary = AddToPhysics;
            this.Position = NewWorld.Translation;
            World = NewWorld;

            if (AddToPhysicsLibrary)
            {
                if (PhysicsSkin_Main != null)
                {
                    if (PhysicsSkin_Main.Space != null)
                    {
                        Current3DScene.BEPUDebugDrawer.Remove(PhysicsSkin_Main);
                        Current3DScene.BEPUPhyicsSpace.Remove(PhysicsSkin_Main);
                    }
                }

                //This is a little convenience method used to extract vertices and indices from a model.
                //It doesn't do anything special; any approach that gets valid vertices and indices will work.
                ModelDataExtractor.GetVerticesAndIndicesFromModel(vxModel.ModelMain, out MeshVertices, out MeshIndices);

                PhysicsSkin_Main = new StaticMesh(MeshVertices, MeshIndices,
                    new AffineTransform(new Vector3(1),
                        Quaternion.CreateFromRotationMatrix(World),
                        World.Translation));

                Current3DScene.BEPUPhyicsSpace.Add(PhysicsSkin_Main);
                Current3DScene.BEPUDebugDrawer.Add(PhysicsSkin_Main);
            }
            */
        }

        /// <summary>
        /// Disposes the Entity
        /// </summary>
        public override void DisposeEntity()
        {
            try
            {
                vxConsole.WriteLine(((vxSandboxGamePlay)CurrentScene).Items.Count);
                //First Remove From Entities List
                CurrentScene.Entities.Remove(this);
                
                //Now Remove from the Items List in the Sandbox Screen                
                CurrentSandboxLevel.Items.Remove(this);

                //Now Dispose all Child Entities
                foreach (vxSandboxEntity entity in ChildEntities)
                    entity.DisposeEntity();

                //Now dispose of the Physics Skin if it's being used.
                if(PhysicsSkin_Main != null) { 
                    PhysicsSkin_Main.Space.Remove(PhysicsSkin_Main);
                    Current3DScene.BEPUDebugDrawer.Remove(PhysicsSkin_Main);
                }
            }
            catch (Exception ex)
            {
                vxConsole.WriteError(ex);
            }

            base.DisposeEntity();
        }


        public virtual void GetProperties(vxScrollPanel owningItem)
        {
            owningItem.Clear();

            nameBox = new vxTextbox(vxEngine, this.name, Vector2.Zero);            
            owningItem.AddItem(nameBox);

            indexProp = new vxTextbox(vxEngine, this.Index.ToString(), new Vector2(0,20));
            owningItem.AddItem(indexProp);
        }

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (PhysicsSkin_Main != null)
                BoundingBox = PhysicsSkin_Main.BoundingBox;

            if (SelectionState == vxEnumSelectionState.Hover && vxEngine.InputManager.IsNewMouseButtonPress(MouseButtons.LeftButton))
                SelectionState = vxEnumSelectionState.Selected;


            if (SelectionState == vxEnumSelectionState.Selected &&
                PreviousSelectionState != vxEnumSelectionState.Selected)
            {
                // Raise the 'SelectionStateSelected' event.
                if (SelectionStateSelected != null)
                    SelectionStateSelected(this, new EventArgs());
            }

            if (SelectionState == vxEnumSelectionState.Hover && 
                PreviousSelectionState == vxEnumSelectionState.Unseleced)
            {
                // Raise the 'SelectionStateHovered' event.
                if (SelectionStateHovered != null)
                    SelectionStateHovered(this, new EventArgs());
            }

            if (PreviousSelectionState == vxEnumSelectionState.Hover && 
                SelectionState == vxEnumSelectionState.Unseleced)
            {
                // Raise the 'SelectionStateUnSelected' event.
                if (SelectionStateUnSelected != null)
                    SelectionStateUnSelected(this, new EventArgs());
            }

            PreviousSelectionState = SelectionState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Render's the mesh with the specified render technique
        /// </summary>
        /// <param name="RenderTechnique"></param>
        public override void RenderMesh(string RenderTechnique)
        {
            //Set the Selection Colour based off of Selection State
            switch (SelectionState)
            {
                case vxEnumSelectionState.Selected:
                    EmissiveColour = Color.DarkOrange;
                    break;
                case vxEnumSelectionState.Hover:
                    EmissiveColour = Color.DeepSkyBlue;
                    break;
                case vxEnumSelectionState.Unseleced:
                    EmissiveColour = Color.Black;
                    break;
            }

            //Reset Selection state if it's only hovered
            if (SelectionState == vxEnumSelectionState.Hover)
                SelectionState = vxEnumSelectionState.Unseleced;

            //Render through the base class
            base.RenderMesh(RenderTechnique);
        }

        public virtual void AddChild(vxSandboxEntity Entity, Matrix OffSetOrientation)
        {
            Entity.Parent = this;
            Entity.SetMesh(OffSetOrientation * this.World, false, false);
            ChildEntities.Add(Entity);
            SetChildOrientation();
        }

        public virtual void AddChildrenEntities()
        {
            
        }

        public virtual void SetChildOrientation()
        {
            foreach (vxSandboxEntity entity in ChildEntities)
                entity.SetOffetOrientation();
        }

        public virtual void SetOffetOrientation()
        {
            World = this.Parent.EndOrientation * this.Parent.World;
        }

        public virtual void ToggleSimulation(bool IsRunning)
        {
            if(IsRunning == true)
                SandboxState = vxEnumSandboxGameState.Running;
            else
                SandboxState = vxEnumSandboxGameState.EditMode;
        }

        public virtual void SetIndex(int NewIndex)
        {
            Index = NewIndex;
        }
    }
}
#endif