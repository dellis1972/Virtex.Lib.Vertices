using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Input;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    /// <summary>
    /// Key binding settings GUII tem.
    /// </summary>
	public class vxKeyBindingSettingsGUIItem : vxGUIBaseItem
    {
        public vxLabel Label;
        public vxButton Button;

		// Is this gui item waiting to take in the next key press.
		bool TakingInput = false;

		public KeyBinding KeyBinding;

		public object BindingID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.GUI.Controls.vxKeyBindingSettingsGUIItem"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="GUIManager">GUI Manager.</param>
        /// <param name="Title">Title.</param>
        /// <param name="KeyBinding">Key binding.</param>
        /// <param name="position">Position.</param>
        public vxKeyBindingSettingsGUIItem(vxEngine Engine, vxGuiManager GUIManager, string Title, KeyBinding KeyBinding, object id, Vector2 position)
			: base(position)
        {
            GUIManager.Add(this);

			this.KeyBinding = KeyBinding;

			BindingID = id;

            Label = new vxLabel(Engine, Title, position + new Vector2(10, 5));
            GUIManager.Add(Label);

			Button = new vxButton(Engine, KeyBinding.Key.ToString(), position + new Vector2(200, 10));
			Button.Clicked += delegate {
				TakingInput = true;
				Button.Text = "Press Any Key...";
				Button.Colour = Color.CornflowerBlue;
				Button.Color_Normal = Color.CornflowerBlue;
				Button.Color_Highlight = Color.CornflowerBlue;
			};
            GUIManager.Add(Button);

            Height = 40;
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

			if (TakingInput)
			{
				if (Keyboard.GetState().GetPressedKeys().Length > 0)
				{
					Keys newKey = Keyboard.GetState().GetPressedKeys()[0];

					if(newKey != Keys.Escape && newKey != Keys.OemTilde)
						KeyBinding.Key = newKey;

					TakingInput = false;

					Button.Text = KeyBinding.Key.ToString();
					Button.Color_Highlight = Color.DarkOrange;
				}
			}


            //Update Rectangle
            int length = 354;
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y), length, Height);
          
            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color.Black * 0.5f);
            vxEngine.SpriteBatch.End();
            
        }
    }
}
