using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc.GUI.GuiArtProvider
{
	public class vxButtonArtProvider : vxArtProviderBase, IGuiArtProvider
	{
		public vxButtonArtProvider (vxEngine engine):base(engine)
		{
			DefaultWidth = 150;
			DefaultHeight = 24;

			DoBorder = true;
			BorderWidth = 2;
			TextColour = Color.Black;
			BackgroundColour = Color.DarkOrange;
			BackgroundHoverColour = Color.DarkOrange * 1.2f;

			BackgroundImage = vxEngine.EngineContentManager.Load<Texture2D>("Gui/DfltThm/vxGUITheme/vxButton/Bckgrnd_Nrml");
		}


		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public virtual void Draw(object guiItem)
		{
			vxButton button = (vxButton)guiItem;

			this.Font = vxEngine.vxGUITheme.Font;

			//Set Width and Height
			button.Width = Math.Max(this.DefaultWidth, (int)(this.Font.MeasureString(button.Text).X + Padding.X * 2));
			button.Height = Math.Max(this.DefaultHeight, (int)(this.Font.MeasureString(button.Text).Y + Padding.Y * 2));


			//Update Rectangle
			button.BoundingRectangle = new Rectangle(
				(int)(button.Position.X - Padding.X),
				(int)(button.Position.Y - Padding.Y / 2),
				button.Width, button.Height);


			Rectangle BorderRectangle = new Rectangle(
				(int)(button.Position.X - Padding.X) - BorderWidth,
				(int)(button.Position.Y - Padding.Y / 2) - BorderWidth,
				button.Width + BorderWidth * 2, 
				button.Height + BorderWidth * 2);

			//Draw Button
			if(DoBorder)
				vxEngine.SpriteBatch.Draw(BackgroundImage, BorderRectangle, Color.Black * Opacity);


			vxEngine.SpriteBatch.Draw(BackgroundImage, button.BoundingRectangle, (button.HasFocus ? this.BackgroundHoverColour : this.BackgroundColour) * Opacity);

			vxEngine.SpriteBatch.DrawString(this.Font, button.Text,
				new Vector2(
					button.Position.X + button.Width / 2 - this.Font.MeasureString(button.Text).X / 2 - Padding.X,
					button.Position.Y + button.Height / 2 - this.Font.MeasureString(button.Text).Y / 2),
				TextColour * Opacity);
		}
	}
}

