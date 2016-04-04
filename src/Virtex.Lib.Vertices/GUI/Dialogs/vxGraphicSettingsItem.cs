using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Utilities;
using vxVertices.Core;

namespace vxVertices.GUI.Controls
{
    /// <summary>
    /// Toolbar control that holds <see cref="vxVertices.GUI.Controls.vxGraphicSettingsItem"/> 
    /// </summary>
    public class vxGraphicSettingsItem : vxGUIBaseItem
    {
        public vxLabel Label;
        public vxComboBox ValueComboBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxGraphicSettingsItem"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        public vxGraphicSettingsItem(vxEngine Engine, vxGuiManager GUIManager, string Title, string Value, Vector2 position): base(position)
        {
            GUIManager.Add(this);

            Label = new vxLabel(Engine, Title, position + new Vector2(10, 5));
            GUIManager.Add(Label);

            ValueComboBox = new vxComboBox(Engine, Value, position + new Vector2(200, 10));
            GUIManager.Add(ValueComboBox);

            Height = 40;
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

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
