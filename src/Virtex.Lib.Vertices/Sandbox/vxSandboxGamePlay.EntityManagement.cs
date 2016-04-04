#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using vxVertices.Core;
using vxVertices.Core.Cameras;
using vxVertices.Core.Cameras.Controllers;
using vxVertices.Core.Input;
using vxVertices.Core.Input.Events;
using vxVertices.Core.Debug;
using vxVertices.Core.Scenes;
using vxVertices.Scenes.Sandbox.Entities;
using vxVertices.Scenes.Sandbox.MessageBoxs;
using vxVertices.Screens.Menus;
using vxVertices.GUI;
using vxVertices.GUI.Controls;
using vxVertices.GUI.Events;
using vxVertices.GUI.MessageBoxs;
using vxVertices.GUI.Dialogs;
using vxVertices.Utilities;
using vxVertices.Physics.BEPU;
using vxVertices.Physics.BEPU.BroadPhaseEntries;
using vxVertices.Physics.BEPU.CollisionRuleManagement;
using vxVertices.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using vxVertices.Core.Entities;

namespace vxVertices.Scenes.Sandbox
{
    public partial class vxSandboxGamePlay : vxScene3D
    {
        /// <summary>
        /// This Collection stores all the items which are in the editor at start time, therefore
        /// any items which are added during the simulation (particles, entitie, etc...) can be caught when
        /// the stop method is run.
        /// </summary>
        public List<vxSandboxEntity> EditorItems = new List<vxSandboxEntity>();


        /// <summary>
        /// Current Key being used too add new entities.
        /// </summary>
        public string CurrentlySelectedKey = "";


        /// <summary>
        /// Starts the Sandbox.
        /// </summary>
        public virtual void SimulationStart()
        {
            //Clear out the Edtor Items
            EditorItems.Clear();

            foreach (vxSandboxEntity entity in Items)
            {
                if (entity != null)
                {
                    EditorItems.Add(entity);
                    entity.ToggleSimulation(true);
                }
            }
        }
        

        /// <summary>
        /// Stops the Sandbox.
        /// </summary>
        public virtual void SimulationStop()
        {
            CurrentlySelectedKey = "";
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null)
                {
                    Items[i].ToggleSimulation(false);

                    if(EditorItems.Contains(Items[i]) == false)
                    {
                        Items[i].DisposeEntity();
                        i--;
                    }
                }
            }
        }


        /// <summary>
        /// Creates a New InstanceSet in the Instances Collection using the speciefied model. The Instance Set can be
        /// retrieved by the Specified key.
        /// </summary>
        /// <param name="Key">Unique Key to use for this InstanceSet.</param>
        /// <param name="model">The Model to be used in the InstanceSet.</param>
        public virtual void CreateNewInstanceCollection(object Key, Model model)
        {
            InstanceSet instanceSet = new InstanceSet(vxEngine);
            instanceSet.InstancedModel = model;
            Instances.Add(Key, instanceSet);
        }

        /// <summary>
        /// Register New Sandbox Item 
        /// </summary>
        /// <param name="Entity"></param>
        public vxSandboxItemButton RegisterNewSandboxItem(vxSandboxEntityDescription EntityDescription)
        {
            return RegisterNewSandboxItem(EntityDescription, Vector2.Zero, 150, 150);
        }

        public vxSandboxItemButton RegisterNewSandboxItem(vxSandboxEntityDescription EntityDescription, Vector2 ButtonPosition, int Width, int Height)
        {
            //First Ensure the Entity Description Is Loaded.
            EntityDescription.Load(vxEngine);


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\tRegistering: \t'{0}' to Dictionary", EntityDescription.Key);
            Console.ResetColor();

            vxSandboxItemButton button = new vxSandboxItemButton(vxEngine,
                EntityDescription.Icon,
                EntityDescription.Description,
                EntityDescription.Key,
                ButtonPosition, Width, Height);

            button.Clicked += AddItemModeToolbarItem_Clicked;
            button.Clicked += Button_Clicked;

            return button;
        }

        private void Button_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            NewSandboxItemClicked(((vxSandboxItemButton)e.GUIitem).Key);
        }

        public virtual string NewSandboxItemClicked(string key)
        {
            //First Dispose of the Temp Part
            DisposeOfTempPart();

            //Tell the GUI it doesn't have focus.
            GUIManager.DoesGuiHaveFocus = false;

            //Close the Tab Control.
            tabControl.CloseAllTabs();

            return AddSandboxItem(key, Matrix.Identity);
        }


        /// <summary>
        /// Disposes of the Currently Created Temp Part.
        /// </summary>
        public virtual void DisposeOfTempPart()
        {
            if (temp_part != null)
            {
                temp_part.DisposeEntity();
            }

            if (Items.Contains(temp_part))
                Items.Remove(temp_part);

            temp_part = null;
        }

        public virtual string AddSandboxItem(string key, Matrix World)
        {
            //Set Currently Selected Key
            CurrentlySelectedKey = key;

            //Create the new Entity as per the Key and let the temp_part access it.
            temp_part = GetNewEntity(key);

            // If the Key is not found, temp_part is set too null. So before any further 
            // operations are preformed, check first to make sure it's a valid entity.
            if (temp_part != null)
            {
                //Add too Item Collection.
                Items.Add(temp_part);

                //Process the New Entity.
                ProcessEntity(World);

            }
            //Finally Reset All Element Indicies.
            SetElementIndicies();

            this.Entities.Remove(workingPlane);
            this.Entities.Add(workingPlane);

            return key;
        }

        /// <summary>
        /// Do any Processing Required.
        /// </summary>
        /// <param name="trackPart"></param>
        public virtual void ProcessEntity(Matrix World)
        {

        }

        /// <summary>
        /// Returns a new instance based off of the returned key. This must be overridden by an inherited class.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual vxSandboxEntity GetNewEntity(string key)
        {
            switch (key)
            {
                //Cubes
                case "vxVertices.Core.Entities.vxWaterEntity":
                    return AddWaterVolume(Vector3.Zero);
                    break;

                default:
                    vxConsole.WriteError(this.ToString(), string.Format("'{0}' Key Not Found!", key));
                    return null;
                    break;
            }
        }


        public vxWaterEntity AddWaterVolume(Vector3 Position)
        {
#if VIRTICES_XNA

            vxWaterEntity water = new vxWaterEntity(vxEngine, Position, new Vector3(5, 5, 8));
            waterItems.Add(water);
            return water;
#endif
			return null;
        }

        /// <summary>
        /// Resets the Layout of Element Indecies for Selection and Element Management
        /// </summary>
        public virtual void SetElementIndicies()
        {
            for (int i = 0; i < Items.Count; i++) {
                if (Items[i] != null)
                    Items[i].SetIndex(i);
                else
                    vxConsole.WriteError(this.ToString(), "ITEM ENTITY IS NULL");
            }
        }

    }
}

#endif