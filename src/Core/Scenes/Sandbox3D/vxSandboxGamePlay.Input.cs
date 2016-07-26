#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.Core.Cameras;
using Virtex.Lib.Vertices.Core.Cameras.Controllers;
using Virtex.Lib.Vertices.Core.Input;
using Virtex.Lib.Vertices.Core.Input.Events;
using Virtex.Lib.Vertices.Core.Debug;
using Virtex.Lib.Vertices.Core.Scenes;
using Virtex.Lib.Vertices.Scenes.Sandbox.Entities;

using Virtex.Lib.Vertices.Screens.Menus;
using Virtex.Lib.Vertices.GUI;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.GUI.Events;
using Virtex.Lib.Vertices.GUI.MessageBoxs;
using Virtex.Lib.Vertices.GUI.Dialogs;
using Virtex.Lib.Vertices.Utilities;
using Virtex.Lib.Vertices.Physics.BEPU;
using Virtex.Lib.Vertices.Physics.BEPU.BroadPhaseEntries;
using Virtex.Lib.Vertices.Physics.BEPU.CollisionRuleManagement;
using Virtex.Lib.Vertices.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using Virtex.Lib.Vertices.Entities.Sandbox3D;

namespace Virtex.Lib.Vertices.Scenes.Sandbox3D
{
    public partial class vxSandboxGamePlay : vxScene3D
    {
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(vxInputManager input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.KeyboardState;// input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.GamePadState;// input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected;

            /*            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];*/
            //if (input.IsPauseGame() || gamePadDisconnected)
            if (input.IsPauseGame())
            {
                if (SandboxStartGameType != vxEnumSandboxGameType.RunGame &&
                    SandboxGameState == vxEnumSandboxGameState.Running)
                    SimulationStop();
                else
                    ShowPauseScreen();
            }
            else if (Camera.CameraType == CameraType.Freeroam)
            {
                //Update If In Edit Mode
                if (SandboxGameState == vxEnumSandboxGameState.EditMode)
                {
                    //Set the Working Plane Height
                    /****************************************************************************************/
                    if (GUIManager.DoesGuiHaveFocus == false
                        && vxEngine.InputManager.MouseState.MiddleButton == ButtonState.Released)
                    {
                        if (vxEngine.InputManager.ScrollWheelDelta > 0)
                            WrkngPln_HeightDelta += 0.5f;
                        else if (vxEngine.InputManager.ScrollWheelDelta < 0)
                            WrkngPln_HeightDelta -= 0.5f;

                        workingPlane.Position = Vector3.Up * WrkngPln_HeightDelta;
                    }
                }

                /**********************************************************/
                /*                  Left Button Press                     */
                /**********************************************************/
                if (vxEngine.InputManager.IsNewMouseButtonPress(MouseButtons.LeftButton))
                {
                    //Only do this if the GUI Manager isn't being used
                    if (GUIManager.DoesGuiHaveFocus == false && Cursor.IsMouseHovering == false)
                    {
                        //If it's in 'AddItem' Mode, then Add the Current Key Item
                        if (MouseClickState == vxEnumSanboxMouseClickState.AddItem)
                        {
                            //Set the Location of the current temp_part being added.
                            if (temp_part != null)
                                AddSandboxItem(CurrentlySelectedKey, temp_part.World);
                        }

                        else if (MouseClickState == vxEnumSanboxMouseClickState.SelectItem)
                        {
                            //Clear Addative Selection
                            if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.LeftShift) == false &&
                                Cursor.SelectionState != vxEnumSelectionState.Hover)
                            {
                                for (int ind = 0; ind < SelectedItems.Count; ind++)
                                {
                                    SelectedItems[ind].SelectionState = vxEnumSelectionState.Unseleced;
                                }
                                SelectedItems.Clear();
                                SetElementIndicies();
                            }

                            if (Index > 0 && Index < Items.Count)
                            {
                                if (SelectedItems.Contains(Items[Index]) == false)
                                {
                                    SelectedItems.Add(Items[Index]);

                                    Items[Index].SelectionState = vxEnumSelectionState.Selected;
                                    Items[Index].GetProperties(PropertiesControl);

                                    // Raise the 'Changed' event.
                                    if (ItemSelected != null)
                                        ItemSelected(this, new EventArgs());
                                }
                                else
                                {
                                    Items[Index].SelectionState = vxEnumSelectionState.Hover;
                                    SelectedItems.Remove(Items[Index]);
                                }
                            }
                        }
                    }
                }

                /**********************************************************/
                /*                Right Button Press                      */
                /**********************************************************/
                if (vxEngine.InputManager.IsNewMouseButtonPress(MouseButtons.RightButton))
                    RightClick();

                /**********************************************************/
                /*                  Process Keys                          */
                /**********************************************************/

                //Delete Selected Items
                if (vxEngine.InputManager.IsNewKeyPress(Keys.Delete))
                {
                    foreach (vxSandboxEntity sandboxEntity in SelectedItems)
                    {
                        sandboxEntity.DisposeEntity();
                    }

                    SelectedItems.Clear();

                    SetElementIndicies();
                }
            }

            HandleInputBase(input);
        }


        /// <summary>
        /// The Method Called when the Mouse is Right Clicked. The Default is to rotate the part about the
        /// 'Y-Axis'. Override this method to provide your own code.
        /// </summary>
        public virtual void RightClick()
        {
            if (temp_part != null)
                temp_part.YRotation_ModelOffset += MathHelper.PiOver2;
        }
        
    }
}

#endif