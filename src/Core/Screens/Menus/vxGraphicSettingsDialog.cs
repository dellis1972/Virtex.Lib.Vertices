using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vertices.Core;
using Virtex.Lib.Vertices.GUI;
using Virtex.Lib.Vertices.GUI.Events;
using Virtex.Lib.Vertices.GUI.Controls;
using Virtex.Lib.Vertices.Core.Settings;
using Virtex.Lib.Vertices.Utilities;

namespace Virtex.Lib.Vertices.GUI.Dialogs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxGraphicSettingsDialog : vxDialogBase
    {

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxGraphicSettingsDialog()
            : base("Graphics Settings", ButtonTypes.OkApplyCancel)
        {

        }

        public override void LoadContent()
        {
            vxEngine.LoadResolution = true;
            base.LoadContent();

            this.Title = vxEngine.Language.GraphicsSettings;


			//All Items below are stored in this column as it's the longest word
			
            float Margin = 25;
            float MarginTwo = 400;

            int horiz = 75;
            int horizTwo = 75;


            //Resolutions
            /*****************************************************************************************************/
            //Get Current Resolution
            PresentationParameters pp = vxEngine.GraphicsDevice.PresentationParameters;
            string currentRes = string.Format("{0}x{1}", pp.BackBufferWidth, pp.BackBufferHeight);

            vxGraphicSettingsItem ResolutionSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, vxEngine.Language.Resolution, currentRes, new Vector2(Margin, horiz));
            horiz += 45;

            bool AddItem = true;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                AddItem = true;

                //Don't Show All Resolutions
                if (mode.Width > 599 || mode.Height > 479)
                {
                    string menuItemText = string.Format("{0}x{1}", mode.Width, mode.Height);

                    //If Good Resolution and Not being repeated, Add Item
                    if (AddItem)
                        ResolutionSettingsItem.ValueComboBox.AddItem(menuItemText);
                }
            }

            ResolutionSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

                vxEngine.Profile.Settings.Graphics.Int_Resolution = e.SelectedIndex;
                vxConsole.WriteLine("Setting Resolution Index to: " + vxEngine.Profile.Settings.Graphics.Int_Resolution);
            };


            //Full Screen
            /*****************************************************************************************************/
            vxGraphicSettingsItem FullScreenSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, vxEngine.Language.FullScreen,
                vxEngine.Profile.Settings.Graphics.Bool_FullScreen ? vxEngine.Language.FullScreen : vxEngine.Language.Windowed, 
                new Vector2(Margin, horiz));
            horiz += 45;
            FullScreenSettingsItem.ValueComboBox.AddItem(vxEngine.Language.FullScreen);
            FullScreenSettingsItem.ValueComboBox.AddItem(vxEngine.Language.Windowed);
            FullScreenSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

                if (e.SelectedItem.Text == "Full Screen")
                    vxEngine.Profile.Settings.Graphics.Bool_FullScreen = true;
                else
                    vxEngine.Profile.Settings.Graphics.Bool_FullScreen = false;

                vxConsole.WriteLine("Setting Full Screen to: " + vxEngine.Profile.Settings.Graphics.Bool_FullScreen);
            };


            //VSync
            /*****************************************************************************************************/
            vxGraphicSettingsItem VSyncSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, "VSync",
                vxEngine.Profile.Settings.Graphics.Bool_VSync ? "On" : "Off",
                new Vector2(Margin, horiz));
            horiz += 45;

            VSyncSettingsItem.ValueComboBox.AddItem("Off");
            VSyncSettingsItem.ValueComboBox.AddItem("On");
            VSyncSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

                if (e.SelectedItem.Text == "On")
                    vxEngine.Profile.Settings.Graphics.Bool_VSync = true;
                else
                    vxEngine.Profile.Settings.Graphics.Bool_VSync = false;

                vxConsole.WriteLine("Setting VSync to: " + vxEngine.Profile.Settings.Graphics.Bool_VSync);
            };

			#if VIRTICES_3D

            //Shadows
            /*****************************************************************************************************/
            vxGraphicSettingsItem ShadowsSettingsItem = new vxGraphicSettingsItem(
               vxEngine, InternalvxGUIManager, "Shadows",
               vxEngine.Profile.Settings.Graphics.ShadowQuality.ToString(), new Vector2(MarginTwo, horizTwo));
            horizTwo += 45;

            ShadowsSettingsItem.ValueComboBox.AddItem(vxEnumQuality.None.ToString());
            ShadowsSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Low.ToString());
            ShadowsSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Medium.ToString());
            ShadowsSettingsItem.ValueComboBox.AddItem(vxEnumQuality.High.ToString());
            ShadowsSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Ultra.ToString());

            ShadowsSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {
                vxEngine.Profile.Settings.Graphics.ShadowQuality = (vxEnumQuality)(e.SelectedIndex);
                vxConsole.WriteLine("Setting Bloom to: " + vxEngine.Profile.Settings.Graphics.ShadowQuality);
            };


            //Bloom
            /*****************************************************************************************************/
            vxGraphicSettingsItem BloomSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, "Bloom",
                vxEngine.Profile.Settings.Graphics.Bloom.ToString(), new Vector2(MarginTwo, horizTwo));
            horizTwo += 45;
            BloomSettingsItem.ValueComboBox.AddItem(vxEnumQuality.None.ToString());
            BloomSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Low.ToString());
            BloomSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Medium.ToString());
            BloomSettingsItem.ValueComboBox.AddItem(vxEnumQuality.High.ToString());
            BloomSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Ultra.ToString());

            BloomSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {
                vxEngine.Profile.Settings.Graphics.Bloom = (vxEnumQuality)(e.SelectedIndex);
                vxConsole.WriteLine("Setting Bloom to: " + vxEngine.Profile.Settings.Graphics.Bloom);
                Console.WriteLine((int)vxEngine.Profile.Settings.Graphics.Bloom);
            };


            //Edge Detect
            /*****************************************************************************************************/
            vxGraphicSettingsItem EdgeDetectSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, "Edge Detect",
                vxEngine.Profile.Settings.Graphics.Bool_DoEdgeDetection ? "On" : "Off",
                new Vector2(MarginTwo, horizTwo));
            horizTwo += 45;

            EdgeDetectSettingsItem.ValueComboBox.AddItem("Off");
            EdgeDetectSettingsItem.ValueComboBox.AddItem("On");
            EdgeDetectSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

                if (e.SelectedItem.Text == "On")
                    vxEngine.Profile.Settings.Graphics.Bool_DoEdgeDetection = true;
                else
                    vxEngine.Profile.Settings.Graphics.Bool_DoEdgeDetection = false;

                vxConsole.WriteLine("Setting Edge Detect to: " + vxEngine.Profile.Settings.Graphics.Bool_DoEdgeDetection);
            };


            //Depth of Field
            /*****************************************************************************************************/
            vxGraphicSettingsItem DOFSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, "Depth of Field",
                vxEngine.Profile.Settings.Graphics.DepthOfField.ToString(), new Vector2(MarginTwo, horizTwo));
            horizTwo += 45;

            DOFSettingsItem.ValueComboBox.AddItem(vxEnumQuality.None.ToString());
            DOFSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Low.ToString());
            DOFSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Medium.ToString());
            DOFSettingsItem.ValueComboBox.AddItem(vxEnumQuality.High.ToString());
            DOFSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Ultra.ToString());

            DOFSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {
                vxEngine.Profile.Settings.Graphics.DepthOfField = (vxEnumQuality)(e.SelectedIndex);
                vxConsole.WriteLine("Setting DepthOfField to: " + vxEngine.Profile.Settings.Graphics.DepthOfField);
            };


            //Crepuscular Rays
            /*****************************************************************************************************/
            vxGraphicSettingsItem GodRaySettingsItem = new vxGraphicSettingsItem(
               vxEngine, InternalvxGUIManager, "Crepuscular Rays",
               vxEngine.Profile.Settings.Graphics.GodRays.ToString(), new Vector2(MarginTwo, horizTwo));
            horizTwo += 45;

            GodRaySettingsItem.ValueComboBox.AddItem(vxEnumQuality.None.ToString());
            GodRaySettingsItem.ValueComboBox.AddItem(vxEnumQuality.Low.ToString());
            GodRaySettingsItem.ValueComboBox.AddItem(vxEnumQuality.Medium.ToString());
            GodRaySettingsItem.ValueComboBox.AddItem(vxEnumQuality.High.ToString());
            GodRaySettingsItem.ValueComboBox.AddItem(vxEnumQuality.Ultra.ToString());

            GodRaySettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {
                vxEngine.Profile.Settings.Graphics.GodRays = (vxEnumQuality)(e.SelectedIndex);
                vxConsole.WriteLine("Setting GodRays to: " + vxEngine.Profile.Settings.Graphics.GodRays);
            };


            //SSAO
            /*****************************************************************************************************/
            vxGraphicSettingsItem SSAOSettingsItem = new vxGraphicSettingsItem(
                vxEngine, InternalvxGUIManager, "SSAO",
                vxEngine.Profile.Settings.Graphics.SSAO.ToString(), new Vector2(MarginTwo, horizTwo));
            horizTwo += 45;

            SSAOSettingsItem.ValueComboBox.AddItem(vxEnumQuality.None.ToString());
            SSAOSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Low.ToString());
            SSAOSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Medium.ToString());
            SSAOSettingsItem.ValueComboBox.AddItem(vxEnumQuality.High.ToString());
            SSAOSettingsItem.ValueComboBox.AddItem(vxEnumQuality.Ultra.ToString());

            SSAOSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {
                vxEngine.Profile.Settings.Graphics.SSAO = (vxEnumQuality)(e.SelectedIndex);
                vxConsole.WriteLine("Setting SSAO to: " + vxEngine.Profile.Settings.Graphics.SSAO);
            };

			#endif


            Btn_Apply.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
        }

        void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
            ExitScreen();
        }

        void SetSettings()
        {
            Console.WriteLine("Setting Items");
            //Save Settings
            vxEngine.Profile.SaveSettings(vxEngine);

            //Set Graphics
            vxEngine.SetGraphicsSettings();
        }
    }
}
