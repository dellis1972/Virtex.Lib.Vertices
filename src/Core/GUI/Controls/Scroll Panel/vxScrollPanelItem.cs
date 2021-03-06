﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Events;
using Microsoft.Xna.Framework.Audio;

namespace Virtex.Lib.Vrtc.GUI.Dialogs
{
    /// <summary>
    /// File Chooser Dialor Item.
    /// </summary>
    public class vxScrollPanelItem : vxGUIBaseItem
    {
        /// <summary>
        /// The button image.
        /// </summary>
        public Texture2D ButtonImage;
        Rectangle ImageRectangle;

        /// <summary>
        /// Should this Scroll Panel Item Show the Icon.
        /// </summary>
        public bool ShowIcon = true;

        /// <summary>
        /// The width of the button.
        /// </summary>
        public int ButtonWidth = 512;

        /// <summary>
        /// The Button Height
        /// </summary>
        public int ButtonHeight = 64;

		/// <summary>
		/// The is item selected.
		/// </summary>
		public bool IsItemSelected = false;

        /// <summary>
        /// Returns a Harcoded Type
        /// </summary>
        /// <returns></returns>
        public override Type GetBaseGuiType()
        {
            return typeof(vxScrollPanelItem);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.xFileDialogItem"/> class.
        /// </summary>
        /// <param name="vxEngine">Vx engine.</param>
        /// <param name="FilePath">File path.</param>
        /// <param name="Position">Position.</param>
        /// <param name="buttonImage">Button image.</param>
        /// <param name="ElementIndex">Element index.</param>
        /// <param name="function">Function.</param>
        public vxScrollPanelItem(vxEngine vxEngine,
            string Text,
            Vector2 Position,
            Texture2D buttonImage,
            int ElementIndex)
        {
            Padding = 4;
            this.vxEngine = vxEngine;
            
            //Set Text
            this.Text = Text;

            //Set Position
            this.Position = Position;
            OriginalPosition = Position;

            //Set Index
            Index = ElementIndex;

            //Set Button Image
            ButtonImage = buttonImage;
            BoundingRectangle = new Rectangle(0, 0, 64, 64);

            Width = 3000;

            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            Color_Highlight = Color.DarkOrange;
			Colour_Text = Color.LightGray;
			this.OnInitialHover += this_OnInitialHover;
			this.Clicked += this_Clicked;
		}

		private void this_OnInitialHover(object sender, EventArgs e)
		{
			//If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
			#if !NO_DRIVER_OPENAL
			SoundEffectInstance MenuHighlight = vxEngine.vxGUITheme.SE_Menu_Hover.CreateInstance();
			MenuHighlight.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume / 6;
			MenuHighlight.Play();

			#endif
		}

		void this_Clicked (object sender, Virtex.Lib.Vrtc.GUI.Events.vxGuiItemClickEventArgs e)
		{
			#if !NO_DRIVER_OPENAL
			SoundEffectInstance equipInstance = vxEngine.vxGUITheme.SE_Menu_Confirm.CreateInstance();
			equipInstance.Volume = vxEngine.Profile.Settings.Audio.Double_SFX_Volume;
			equipInstance.Play();
			#endif
		}

        public void UnSelect()
        {
            Color_Normal = new Color(0.15f, 0.15f, 0.15f, 0.5f);
			Colour_Text = Color.LightGray;

			IsItemSelected = false;
        }
        public void ThisSelect()
        {
            Color_Normal = Color.DarkOrange;
            Colour_Text = Color.Black;

			IsItemSelected = true;
        }

        public override void Draw(vxEngine vxEngine)
        {
            vxEngine.SpriteBatch.Begin();
            DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }



        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);

            //
            //Update Rectangle
            //
            BoundingRectangle = new Rectangle((int)(Position.X), (int)(Position.Y),
                ButtonWidth, ButtonHeight);

            //
            //Draw Button
            //
            float i = 1;
            if (HasFocus)
            {
                i = 1.250f;
            }

            //Draw Button Background
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Color_Normal * i);

            //Draw Icon
			if (ButtonImage != null) {
				vxEngine.SpriteBatch.Draw (ButtonImage, new Rectangle ((int)(Position.X + 2), (int)(Position.Y + 2),
					ButtonHeight - 4, ButtonHeight - 4), Color.LightGray * i);
			}

            //Draw Text String
            vxEngine.SpriteBatch.DrawString(vxEngine.vxGUITheme.Font, Text,
                new Vector2((int)(Position.X + ButtonHeight + Padding * 2), (int)(Position.Y + 8)),
                Colour_Text);
        }
    }
}
