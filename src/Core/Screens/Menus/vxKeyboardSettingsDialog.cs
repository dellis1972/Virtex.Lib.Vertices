using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Core.Settings;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Localization;
using System.Collections.Generic;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Core.Input;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// The graphic settings dialog.
    /// </summary>
    public class vxKeyboardSettingsDialog : vxDialogBase
    {
		KeyBindings QWERTYPresetKeyBindings;
		KeyBindings AZERTYPresetKeyBindings;

		List<vxKeyBindingSettingsGUIItem> KeyBindingGUIItems = new List<vxKeyBindingSettingsGUIItem>();

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxKeyboardSettingsDialog()
			: base("Control Settings", vxEnumButtonTypes.OkApplyCancel)
        {

        }

		/// <summary>
		/// Load graphics content for the screen.
		/// </summary>
        public override void LoadContent()
        {
            vxEngine.LoadResolution = true;
            base.LoadContent();

            this.Title = vxEngine.Language.Get(vxLocalization.Keyboard_TitleSettings);

			QWERTYPresetKeyBindings = new KeyBindings(vxEngine, KeyboardTypes.QWERTY);
			AZERTYPresetKeyBindings = new KeyBindings(vxEngine, KeyboardTypes.AZERTY);


			//All Items below are stored in this column as it's the longest word
			
            float Margin = vxEngine.GraphicsDevice.Viewport.Width/2 - this.viewportSize.X/2 + 25;
            float MarginTwo = Margin + 450;

            int horiz = 0;
			int horizTwo = 0;




			// Keyboard Preset Settings Item
			/*****************************************************************************************************/
			var KeyboardPresetSettingsItem = new vxSettingsGUIItem(
                vxEngine, InternalvxGUIManager, "Preset",
                "QWERTY", 
				new Vector2(this.ArtProvider.BoundingGUIRectangle.X, this.ArtProvider.BoundingGUIRectangle.Y + horiz));
            horiz += 45;
			KeyboardPresetSettingsItem.ValueComboBox.AddItem(KeyboardTypes.QWERTY.ToString());
            KeyboardPresetSettingsItem.ValueComboBox.AddItem(KeyboardTypes.AZERTY.ToString());
			KeyboardPresetSettingsItem.ValueComboBox.AddItem(KeyboardTypes.CUSTOM.ToString());
            KeyboardPresetSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

				KeyBindings presetKeyBindings = new KeyBindings(vxEngine);

				bool continueOn = true;

				switch ((KeyboardTypes)e.SelectedIndex)
				{
					case KeyboardTypes.QWERTY:
						presetKeyBindings = QWERTYPresetKeyBindings;
						break;
						case KeyboardTypes.AZERTY:
						presetKeyBindings = AZERTYPresetKeyBindings;
						break;
					default:
						continueOn = false;
					break;
				}

				if(continueOn)
				foreach (vxKeyBindingSettingsGUIItem guiitem in KeyBindingGUIItems)
				{
						guiitem.KeyBinding = presetKeyBindings.Bindings[guiitem.BindingID];
					guiitem.Button.Text = presetKeyBindings.Bindings[guiitem.BindingID].Key.ToString();
				}
                
            };






			// Key Bindings
			/*****************************************************************************************************/

			foreach (KeyValuePair<object, KeyBinding> binding in vxEngine.InputManager.KeyBindings.Bindings)
			{
				var keyBinding = new vxKeyBindingSettingsGUIItem(
					vxEngine, InternalvxGUIManager,
					binding.Value.Name, 
					binding.Value,
					binding.Key,
					new Vector2(this.ArtProvider.BoundingGUIRectangle.X + MarginTwo, this.ArtProvider.BoundingGUIRectangle.Y + horizTwo));

				KeyBindingGUIItems.Add(keyBinding);

				horizTwo += 45;
			}



            Btn_Apply.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
            Btn_Ok.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
        }

        public override void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
            ExitScreen();
        }

        void SetSettings()
        {
            //Save Settings
            //vxEngine.Profile.SaveSettings(vxEngine);

            //Set Graphics
            //vxEngine.GraphicsSettingsManager.Apply();
			foreach (vxKeyBindingSettingsGUIItem guiitem in KeyBindingGUIItems)
			{
				vxEngine.InputManager.KeyBindings.Bindings[guiitem.BindingID] = guiitem.KeyBinding;
			}
        }
    }
}
