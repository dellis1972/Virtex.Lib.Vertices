#if VIRTICES_3D
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core.Cameras.Controllers;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.Core.Input.Events;
using Virtex.Lib.Vrtc.Core.Debug;
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Scenes.Sandbox.Entities;

using Virtex.Lib.Vrtc.Screens.Menus;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.GUI.MessageBoxs;
using Virtex.Lib.Vrtc.GUI.Dialogs;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries;
using Virtex.Lib.Vrtc.Physics.BEPU.CollisionRuleManagement;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using Virtex.Lib.Vrtc.Entities.Sandbox3D;

namespace Virtex.Lib.Vrtc.Scenes.Sandbox3D
{
    public partial class vxSandboxGamePlay : vxScene3D
    {

        public virtual void LoadFile()
        {

        }

        /// <summary>
        /// Start a New File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_NewFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            vxMessageBox NewFile = new vxMessageBox("Are you sure you want to Start a New File,\nAll Unsaved Work will be Lost", "quit?");
            vxEngine.AddScreen(NewFile, ControllingPlayer);
            NewFile.Accepted += new EventHandler<PlayerIndexEventArgs>(Event_NewFile_Accepted);
        }

        public virtual void Event_NewFile_Accepted(object sender, PlayerIndexEventArgs e) { }





        /// <summary>
        /// Event for Opening a File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_OpenFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            OpenFileDialog = new vxOpenFileDialog(vxEngine, vxEngine.Path_Sandbox, "*.sbx");
            vxEngine.AddScreen(OpenFileDialog, ControllingPlayer);
            OpenFileDialog.Accepted += new EventHandler<PlayerIndexEventArgs>(Event_OpenFileDialog_Accepted);
        }

        public virtual void Event_OpenFileDialog_Accepted(object sender, PlayerIndexEventArgs e)
        {
            vxMessageBoxSaveBeforeQuit saveBeforeCloseCheck = new vxMessageBoxSaveBeforeQuit("Are you sure you want to close without Saving?\nAll un-saved work will be lost", "Close Without Saving?");
            saveBeforeCloseCheck.Save += new EventHandler<PlayerIndexEventArgs>(Event_SaveBeforeCloseCheck_Save);
            saveBeforeCloseCheck.DontSave += new EventHandler<PlayerIndexEventArgs>(Event_SaveBeforeCloseCheck_DontSave);
            vxEngine.AddScreen(saveBeforeCloseCheck, ControllingPlayer);
        }

        public virtual void Event_SaveBeforeCloseCheck_DontSave(object sender, PlayerIndexEventArgs e) { }

        public virtual void Event_SaveBeforeCloseCheck_Save(object sender, PlayerIndexEventArgs e) { SaveFile(true); }




        public virtual void SaveFile(bool takeScreenshot)
        {
            vxConsole.WriteLine("============================================");
            vxConsole.WriteLine("Saving File : '" + sandBoxFile.Name + "'");

            //First Take the Temp_Part out of the Items List
            try
            {
                Items.Remove(temp_part);
            }
            catch (Exception ex)
            {
                vxConsole.WriteError(this.ToString(), ex.Message);
            }
            sandBoxFile.items.Clear();

            foreach (vxSandboxEntity part in Items)
            {
                sandBoxFile.items.Add(new vxSandboxItemStruct(part.Index,
                    part.ToString(),
                    part.World,
                    part.UserDefinedData01,
                    part.UserDefinedData02,
                    part.UserDefinedData03,
                    part.UserDefinedData04,
                    part.UserDefinedData05));
                Console.Write(".");
            }


            int Width = vxEngine.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int Height = vxEngine.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Texture2D thumbnail = vxUtil.ResizeTexture2D(vxEngine, ThumbnailImage, 48, 48);

            byte[] b = new byte[thumbnail.Width * thumbnail.Height * 4];
            thumbnail.GetData<byte>(b);
            sandBoxFile.texture = b;

            sandBoxFile.textureWidth = thumbnail.Width;
            sandBoxFile.textureHeight = thumbnail.Height;

            string path = vxEngine.Path_Sandbox;
            //string path = "Temp/Sandbox/" + LevelFile.Name;

            //First Check, if the Items Directory Doesn't Exist, Create It
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            //Write The Sandbox File
            XmlSerializer serializer = new XmlSerializer(typeof(vxSandBoxFileStructure));
            using (TextWriter writer = new StreamWriter(path + "/" + sandBoxFile.Name + ".sbx"))
            {
                serializer.Serialize(writer, sandBoxFile);
            }
            sandBoxFile.items.Clear();
            Items.Add(temp_part);

            vxConsole.WriteLine("Finished Save!");
            vxConsole.WriteLine("============================================");
        }

        /// <summary>
        /// Event for Saving the Current File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_SaveFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SaveFile(true);
        }


        /// <summary>
        /// Event for Saving As the Current File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_SaveAsFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            ShowGUI = false;
            ThumbnailImage = vxUtil.TakeScreenshot(vxEngine);
            ShowGUI = true;

            SaveAsMsgBox = new vxMessageBoxSaveAs("Save the current file as...", "Save As", sandBoxFile.Name);
            vxEngine.AddScreen(SaveAsMsgBox, ControllingPlayer);
            SaveAsMsgBox.Accepted += new EventHandler<PlayerIndexEventArgs>(Event_SaveAsMsg_Accepted);
        }

        public virtual void Event_SaveAsMsg_Accepted(object sender, PlayerIndexEventArgs e)
        {
            sandBoxFile.Name = SaveAsMsgBox.Textbox.Text;
            SaveFile(false);
        }








        /// <summary>
        /// Exports the Current File too an STL file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void ExportFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            //string path = vxEngine.Path_Sandbox + "\\" + sandBoxFile.Name;
            string path = vxEngine.Path_Sandbox + "\\Export\\";

            //First Check, if the Items Directory Doesn't Exist, Create It
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            Console.Write("Exporting File...");
            StreamWriter writer = new StreamWriter(path + sandBoxFile.Name + "_export.stl");
            writer.WriteLine("solid Exported from Vertices vxEngine");
            foreach (vxSandboxEntity entity in Items)
            {
                if (entity.MeshIndices != null)
                {
                    for (int i = 0; i < entity.MeshIndices.Length - 3; i += 3)
                    {
                        int indice = entity.MeshIndices[i];
                        Matrix correctionMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
                        Vector3 Pt1 = vxGeometryHelper.RotatePoint(entity.World * correctionMatrix, entity.MeshVertices[indice]);
                        Vector3 Pt2 = vxGeometryHelper.RotatePoint(entity.World * correctionMatrix, entity.MeshVertices[indice + 1]);
                        Vector3 Pt3 = vxGeometryHelper.RotatePoint(entity.World * correctionMatrix, entity.MeshVertices[indice + 2]);

                        Vector3 Normal = Vector3.Cross(Pt2, Pt1);
                        Normal.Normalize();
                        writer.WriteLine(string.Format("facet normal {0} {1} {2}", Normal.X, Normal.Y, Normal.Z));
                        writer.WriteLine("outer loop");
                        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt1.X, Pt1.Y, Pt1.Z));
                        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt2.X, Pt2.Y, Pt2.Z));
                        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt3.X, Pt3.Y, Pt3.Z));
                        writer.WriteLine("endloop");
                        writer.WriteLine("endfacet");

                    }
                }
            }
            writer.WriteLine("endsolid");
            writer.Close();
            Console.WriteLine("Done!");
        }
    }
}

#endif