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
    /// <summary>
    /// This is the main class for the game. It holds the instances of the sphere simulator,
    /// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
    /// game loop, and provides keyboard and mouse input.
    /// </summary>
    public partial class vxSandboxGamePlay : vxScene3D
    {
        /// <summary>
        /// An in game Open File Dialog too access files from specific directories.
        /// </summary>
        public vxOpenFileDialog OpenFileDialog;

        /// <summary>
        /// Save As Message Box.
        /// </summary>
        vxMessageBoxSaveAs SaveAsMsgBox;

        public vxEnumSanboxMouseClickState MouseClickState { get; set; }

        /// <summary>
        /// Main Tab Control Which Holds All Choices
        /// </summary>
        public vxTabControl tabControl { get; set; }


        /// <summary>
        /// Main Tab Control Which Holds All Properties
        /// </summary>
        public vxTabControl propertiesTabControl { get; set; }

        /// <summary>
        /// Top Toolbar which holds all of the New File, Open File, Start Simulation etc... buttons.
        /// </summary>
        public vxToolbar toolbar { get; set; }


        /// <summary>
        /// Player
        /// </summary>
        public CharacterControllerInput character;

        /// <summary>
        /// File Format
        /// </summary>
		vxSandBoxFileStructure sandBoxFile { get; set; }

        /// <summary>
        /// List of Current Selected Items
        /// </summary>
        public List<vxSandboxEntity> SelectedItems { get; set; }

        /// <summary>
        /// List of Current Track Parts
        /// </summary>
        public List<vxSandboxEntity> Items { get; set; }

        /// <summary>
        /// The Currently Selected Type of Entity to be added in the Sandbox
        /// </summary>
        public vxSandboxEntity temp_part { get; set; }

        /// <summary>
        /// Is the Sandbox In Testing Mode
        /// </summary>
        public vxEnumSandboxGameState SandboxGameState { get; set; }

        /// <summary>
        /// The Selected Index, x > 0 means the index of the selected item, -1 means nothing is select, -2 means a snapped object
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// A variable which only incrememnts to always give unique ID's
        /// </summary>
        public int IncrementalItemCount { get; set; }


        /// <summary>
        /// The Matrix of the Snapped Object
        /// </summary>
        public Matrix ConnectedMatrix { get; set; }

        /// <summary>
        /// Working Plane
        /// </summary>
        public vxWorkingPlane workingPlane { get; set; }

        /// <summary>
        /// Working Plane Intersection Point
        /// </summary>
        public Vector3 int_intersc = new Vector3();

        /// <summary>
        /// Previous Working Plane Intersection Point
        /// </summary>
        public Vector3 int_intersc_previous = new Vector3();

        /// <summary>
        /// "Out Of Sight" Position
        /// </summary>
        public Vector3 OutofSight = new Vector3();

        /// <summary>
        /// Working Plane Height
        /// </summary>
        public float WrkngPln_HeightDelta = 0;

        public vxEnumSandboxGameType SandboxStartGameType;

        //List<vxSandboxEntity> List_Selection = new List<vxSandboxEntity>();

        vxCursor3D Cursor;
        
        /****************************************************************************/
        /*                               EVENTS
        /****************************************************************************/
        /// <summary>
        /// The Event Fired when a New Item is Selected
        /// </summary>
        public event EventHandler<EventArgs> ItemSelected;

        vxToolbarButton AddItemModeToolbarItem;
        vxToolbarButton SelectItemModeToolbarItem;

        /// <summary>
        /// The vxScrollPanel control which is used too store Entity Properties. See the GetProperties Method for examples.
        /// </summary>
        public vxScrollPanel PropertiesControl { get; set; }

        /// <summary>
        /// Place holder for the texture for taking a fullscreenshot.
        /// </summary>
        public Texture2D ThumbnailImage;


        vxSandboxEntity ParentEntityPlaceHolder;
        bool IsHoveringOverItem = false;

        #region Picking

        //Motorized Grabber
        protected MotorizedGrabSpring grabber;
        protected float grabDistance;

        //The raycast filter limits the results retrieved from the Space.RayCast while grabbing.
        public Func<BroadPhaseEntry, bool> rayCastFilter;
        public virtual bool RayCastFilter(BroadPhaseEntry entry)
        {
            if (character != null)
                return entry != character.CharacterController.Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;
            else
                return true;
        }

        #endregion

        public vxSandboxGamePlay()
        {
            sandBoxFile = new vxSandBoxFileStructure();

            //SandBoxItemList = new Dictionary<string, vxSandboxEntity>();
            Items = new List<vxSandboxEntity>();
            SelectedItems = new List<vxSandboxEntity>();

            Index = -1;

            if (SandboxGameState == null)
                SandboxGameState = vxEnumSandboxGameState.EditMode;

            ConnectedMatrix = Matrix.Identity;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MouseClickState = vxEnumSanboxMouseClickState.SelectItem;
        }


        public vxSandboxGamePlay(vxEnumSandboxGameType SandboxStartGameType, string FileToOpen)
            : this()
        {
            this.SandboxStartGameType = SandboxStartGameType;

            if (SandboxStartGameType == vxEnumSandboxGameType.RunGame)
                SandboxGameState = vxEnumSandboxGameState.Running;

            //Set File name
            sandBoxFile.Name = FileToOpen;
        }

        public override void InitialiseLevel()
        {
            IncrementalItemCount = 0;

            base.InitialiseLevel();

            //Set up the GUI For the Sandbox

            //Add Tab Pages
            tabControl = new vxTabControl(vxEngine,
                vxEngine.GraphicsDevice.Viewport.Width - 100,
                vxEngine.GraphicsDevice.Viewport.Height - 50,
                new Vector2(50, 48),
                vxGUIItemOrientation.Left);
            GUIManager.Add(tabControl);

            propertiesTabControl = new vxTabControl(vxEngine,
                300,
                vxEngine.GraphicsDevice.Viewport.Height - 50,
                new Vector2(-50, 48),
                vxGUIItemOrientation.Right);
            GUIManager.Add(propertiesTabControl);

            vxTabPage properties = new vxTabPage(vxEngine, propertiesTabControl, "Properties");
            propertiesTabControl.AddItem(properties);


            //Set up The Scroll Panel which will hold all Properties
            PropertiesControl = new vxScrollPanel(new Vector2(0, 0),
                properties.Width - 50, properties.Height - 10);
            properties.AddItem(PropertiesControl);

            
            //Sets The Outof sifht Position
            OutofSight = Vector3.One * 100000;


            #region Setup Top Toolbar

            toolbar = new vxToolbar(new Vector2(0, 0))
            {
                Height = 48,
                PaddingX = 0,
                PaddingY = 0,
            };

            //Texture2D Texture_Splitter = vxEngine.EngineContentManager.Load<Texture2D>("Textures/sandbox/toolbar_icons/Toolbar_Seperator");

            GUIManager.Add(toolbar);

            //Test Buttons
            vxToolbarButton RunGameToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/test/test_run");
            //vxToolbarButton StopGameToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/test/test_stop");

            //Create Toolbar Items
            vxToolbarButton NewFileToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/file/file_new");
            vxToolbarButton OpenFileToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/file/file_open");
            vxToolbarButton SaveFileToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/file/file_save");
            vxToolbarButton SaveAsFileToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/file/file_saveas");

            vxToolbarButton ImportToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/io/io_import");
            vxToolbarButton ExportFileToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/io/io_export");


            AddItemModeToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/sel/sel_addItem")
            {
                IsTogglable = true,
            };
            SelectItemModeToolbarItem = new vxToolbarButton(vxEngine, vxEngine.EngineContentManager, "Textures/sandbox/tlbr/sel/sel_selItem")
            {
                IsTogglable = true,
            };

            //Setup Events
            NewFileToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Event_NewFileToolbarItem_Clicked);
            OpenFileToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Event_OpenFileToolbarItem_Clicked);
            SaveFileToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Event_SaveFileToolbarItem_Clicked);
            SaveAsFileToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Event_SaveAsFileToolbarItem_Clicked);

            ExportFileToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(ExportFileToolbarItem_Clicked);

            RunGameToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(RunGameToolbarItem_Clicked);
            //StopGameToolbarItem.Clicked += new EventHandler<vxGuiItemClickEventArgs>(StopGameToolbarItem_Clicked);

            AddItemModeToolbarItem.Clicked += AddItemModeToolbarItem_Clicked;
            SelectItemModeToolbarItem.Clicked += SelectItemModeToolbarItem_Clicked;
            //Add Toolbar Items
            toolbar.AddItem(NewFileToolbarItem);
            toolbar.AddItem(OpenFileToolbarItem);
            toolbar.AddItem(SaveFileToolbarItem);
            toolbar.AddItem(SaveAsFileToolbarItem);

            toolbar.AddItem(new vxToolbarSpliter(vxEngine, 5));

            toolbar.AddItem(ImportToolbarItem);
            toolbar.AddItem(ExportFileToolbarItem);

            toolbar.AddItem(new vxToolbarSpliter(vxEngine, 5));

            toolbar.AddItem(RunGameToolbarItem);
            //toolbar.AddItem(StopGameToolbarItem);

            toolbar.AddItem(new vxToolbarSpliter(vxEngine, 5));

            toolbar.AddItem(AddItemModeToolbarItem);
            toolbar.AddItem(SelectItemModeToolbarItem);
            #endregion

            //Set Initial State
            SelectItemModeToolbarItem.ToggleState = ToggleState.On;
            MouseClickState = vxEnumSanboxMouseClickState.SelectItem;
        }


        public override void LoadContent()
        {
            vxScaleCube center = new vxScaleCube(vxEngine, Vector3.One * 1000);

            base.LoadContent();

            Cursor = new vxCursor3D(vxEngine);

            //
            //Grabbers
            //
            grabber = new MotorizedGrabSpring();
            BEPUPhyicsSpace.Add(grabber);
            rayCastFilter = RayCastFilter;

            workingPlane = new vxWorkingPlane(vxEngine, vxEngine.Model_Sandbox_WorkingPlane, new Vector3(0, WrkngPln_HeightDelta, 0));

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Index = i;
            }
            ConnectedMatrix = Matrix.Identity;
            CurrentlySelectedKey = "";
        }

        public virtual void InitialiseCamera()
        {

        }



        /// <summary>
        /// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
            ParentEntityPlaceHolder = null;

            if (vxEngine.InputManager.IsNewMouseButtonPress(MouseButtons.MiddleButton))
            {
                vxEngine.Mouse_ClickPos = new Vector2(vxEngine.InputManager.MouseState.X, vxEngine.InputManager.MouseState.Y);
            }

            if (IsActive)
            {
                //If it's in Testing Mode, Update the Vehicle and Chase Camera
                if (SandboxGameState == vxEnumSandboxGameState.Running)
                {
                    //UpdateCameraChaseTarget();

                    //drop it out of view
                    workingPlane.Position = OutofSight;
                }
                
                //Update If In Edit Mode
                if (SandboxGameState == vxEnumSandboxGameState.EditMode)
                {
                    //Reset to Negative One each loop
                    Index = -1;

                    Ray Ray_Mouse = vxGeometryHelper.CalculateCursorRay(vxEngine, Camera.Projection, Camera.View);
                    if (!grabber.IsGrabbing)
                    {
                        Cursor.Update(gameTime, Ray_Mouse);
                        
                        if (Cursor.IsMouseHovering == false)
                        {
                            //Next Find the earliest ray hit
                            RayCastResult raycastResult;
                            if (BEPUPhyicsSpace.RayCast(Ray_Mouse, 5000, rayCastFilter, out raycastResult))
                            {
                                var entityCollision = raycastResult.HitObject as EntityCollidable;

                                //Set the Index of the Highlited Item
                                if (raycastResult.HitObject.Tag != null)
                                {
                                    if (raycastResult.HitObject.Tag.GetType() == typeof(int))
                                    {
                                        //Unselect The Previous Selection
                                        if (Index > -1)
                                            if (Items[Index] != null)
                                                Items[Index].SelectionState = vxEnumSelectionState.Unseleced;

                                        //Get Index of Currently Selected Item
                                        Index = Convert.ToInt32(raycastResult.HitObject.Tag);
                                        if (Index < Items.Count)
                                        {
                                            Items[Index].SelectionState = vxEnumSelectionState.Hover;

                                            if (Items[Index].GetType() == typeof(vxSnapBox))
                                            {
                                                if (temp_part != null && entityCollision != null)
                                                {
                                                    temp_part.SetMesh(entityCollision.WorldTransform.Matrix, false, false);
                                                    ParentEntityPlaceHolder = Items[Index];
                                                    Index = -2;
                                                    vxDebugShapeRenderer.AddBoundingBox(raycastResult.HitObject.BoundingBox, Color.HotPink);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //If Index still equals -1, then it isn't over any elements, and a new element can be added.
                    int_intersc_previous = int_intersc;

                    if (SandboxGameState == vxEnumSandboxGameState.EditMode)
                    {
                        if (Ray_Mouse.Intersects(workingPlane.WrknPlane) != null)
                        {
                            Vector3 intersection = (float)Ray_Mouse.Intersects(workingPlane.WrknPlane) * Ray_Mouse.Direction + Ray_Mouse.Position;
                            int_intersc = new Vector3((int)intersection.X, (int)intersection.Y, (int)intersection.Z);
                        }
                    }

                    else
                    {
                        //Get it WAYYYY out of the scene
                        int_intersc = OutofSight;
                    }

                    if (temp_part != null && Index > -2)
                    {
                        temp_part.Position = int_intersc;
                        temp_part.SetMesh(false, false);
                    }


                    /**********************************************************/
                    /*                   Update Cursor                        */
                    /**********************************************************/

                    Vector3 CursorAverage = Vector3.Zero;
                    for (int ind = 0; ind < SelectedItems.Count; ind++)
                    {
                        SelectedItems[ind].SelectionState = vxEnumSelectionState.Selected;
                        CursorAverage += SelectedItems[ind].World.Translation;
                    }

                    //Only Set the Cursor Position if the Mouse is Up, otherwise, the cursor sets the entity positions
                    if (SelectedItems.Count > 0 && vxEngine.InputManager.MouseState.LeftButton == ButtonState.Released)
                    {
                        CursorAverage /= SelectedItems.Count;
                        Cursor.Position = CursorAverage;
                    }
                }

                if (vxEngine.InputManager.MouseState.MiddleButton == ButtonState.Pressed)
                    Mouse.SetPosition((int)vxEngine.Mouse_ClickPos.X, (int)vxEngine.Mouse_ClickPos.Y);
            }
            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void DrawOverlayItems()
        {
            base.DrawOverlayItems();
            vxEngine.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Cursor.RenderMeshPlain();
        }

        public override void DrawGameplayScreen(GameTime gameTime)
        {
            base.DrawGameplayScreen(gameTime);
            
            if (SandboxGameState == vxEnumSandboxGameState.EditMode)
                ShowGUI = true;
            else
                ShowGUI = false;
        }        
    }
}
#endif