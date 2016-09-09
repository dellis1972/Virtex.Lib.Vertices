using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Mathematics;
using Virtex.Lib.Vrtc.Core.Debug;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    enum InputJustification
    {
        All,
        Letters,
        Numerical
    }

	/// <summary>
	/// Textbox Control for us in the vxGUI System.
	/// </summary>
    public class vxTextbox : vxGUIBaseItem
    {
        InputJustification InputJustification;
        public int Textbox_Length;
        public int Textbox_height = 31;
        
        float Caret_Blink = 0;
        float alpha = 0;
        float Alpha_Actual = 0;

        /// <summary>
        /// Occurs when enabled state changed.
        /// </summary>
        public event EventHandler<EventArgs> TextChanged;


        private int cursorIndex = 0;

        string DisplayText;

		/// <summary>
		/// Cursor character.
		/// </summary>
		const string Cursor = "|";


		// Key that pressed last frame.
		private Keys pressedKey;

		// Timer for key repeating.
		private float keyRepeatTimer;

		// Key repeat duration in seconds for the first key press.
		private const float keyRepeatStartDuration = 0.3f;

		// Key repeat duration in seconds after the first key press.
		private const float keyRepeatDuration = 0.03f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxTextbox"/> class.
        /// </summary>
		/// <param name="vxEngine">Engine.</param>
        /// <param name="text">Text.</param>
        /// <param name="position">Position.</param>
		public vxTextbox(vxEngine vxEngine, string text, Vector2 position) : this(vxEngine, text, position, 200)
        {
        }


		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.Controls.vxTextbox"/> class.
		/// </summary>
		/// <param name="vxEngine">Engine.</param>
		/// <param name="text">Text.</param>
		/// <param name="position">Position.</param>
		/// <param name="length">Length.</param>
		public vxTextbox(vxEngine vxEngine, string text, Vector2 position, int length)
        {
			//Set the Engine
			this.vxEngine = vxEngine;

			//Set Text
            Text = text;

			//Set Textbox Length
            Textbox_Length = length;
            Width = length;

			//Set Position
            Position = position;

			//Set Justification
            InputJustification = InputJustification.All;

			//Set Colours
			Color_Normal = vxEngine.vxGUITheme.vxTextboxes.BackgroundColour;
            Color_Highlight = vxEngine.vxGUITheme.vxTextboxes.BackgroundHoverColour;

            Colour_Text = vxEngine.vxGUITheme.vxTextboxes.TextColour;

            this.Font = vxEngine.vxGUITheme.Font;

            //Get Text Height
            Textbox_height = Math.Max(Textbox_height, (int)this.Font.MeasureString(Text).Y + Padding / 2);

            //Set the Bounding Rectangle from Text height, Width is set seperately
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), length, Textbox_height);
        }

        public override void Select()
        {
			cursorIndex = Text.Length;
            Colour = Color_Selected;
            IsSelected = true;
            HasFocus = true;
        }


        public override void Update(vxEngine vxEngine)
        {
            MouseState mouseState = vxEngine.InputManager.MouseState;

            if (mouseState.X > BoundingRectangle.Left && mouseState.X < BoundingRectangle.Right && 
                mouseState.Y < BoundingRectangle.Bottom && mouseState.Y > BoundingRectangle.Top)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released)
                        Select();
                    else if(IsSelected == false)
                        Hover();
                }            
            else if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released ||
                IsSelected == false)
                NotHover();
                 

            //Set State for next Loop
            PreviousMouseState = mouseState;

            if (IsSelected == true)
				ProcessKeyInputs(0.0167f);


        }

        public override void Draw(vxEngine vxEngine)
		{
            base.Draw(vxEngine);
            vxEngine.SpriteBatch.Begin();
            DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }


        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);
            this.Font = vxEngine.vxGUITheme.Font;

            DisplayText = Text;

            //
            //Update Rectangle
            //
            int length = Textbox_Length; // Math.Max(100, (int)(vxEngine.TinyFont.MeasureString(Text).X + Padding * 2));

            //Get Text Height
            Textbox_height = Math.Max(Textbox_height, (int)this.Font.MeasureString(DisplayText).Y + Padding / 2);

            //Set the Bounding Rectangle from Text height, Width is set seperately
            BoundingRectangle = new Rectangle((int)(Position.X - Padding), (int)(Position.Y - Padding / 2), length, Textbox_height);

            Rectangle BackRectangle = new Rectangle((int)(Position.X - Padding) - 1, (int)(Position.Y - Padding / 2) - 1, length + 2, (int)this.Font.MeasureString(DisplayText).Y + Padding / 2 + 2);

            //Make sure the cursor index doesn't go past the text length
            cursorIndex = Math.Min(cursorIndex, Text.Length);

            string leftPart = Text.Substring(0, cursorIndex);
            Vector2 cursorPos = Position + this.Font.MeasureString(leftPart);
            cursorPos.Y = Position.Y;

            //Sets the Caret Alpha

            if (IsSelected)
            {
                if (Caret_Blink < 30)
                {
                    Caret_Blink++;
                    alpha = 1;
                }
                else if (Caret_Blink < 60)
                {
                    Caret_Blink++;
                    alpha = 0;
                }
                else
                    Caret_Blink = 0;

            }
            else
                alpha = 0;

            Alpha_Actual = vxMathHelper.Smooth(Alpha_Actual, alpha, 4);

            //Draw the Text Box
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BackRectangle, Color.Black * 0.75f);
            Colour = HasFocus ? vxEngine.vxGUITheme.vxTextboxes.BackgroundHoverColour : vxEngine.vxGUITheme.vxTextboxes.BackgroundColour;
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour);

            //Draw Text
            Colour_Text = HasFocus ? vxEngine.vxGUITheme.vxTextboxes.TextHover : vxEngine.vxGUITheme.vxTextboxes.TextColour;
            vxEngine.SpriteBatch.DrawString(this.Font, DisplayText, new Vector2(Position.X, Position.Y + 2), Colour_Text);

            //Draw Caret Seperately
            vxEngine.SpriteBatch.DrawString(this.Font, Cursor, cursorPos, Colour_Text * Alpha_Actual);

        }

        /// <summary>
        /// Hand keyboard input.
        /// </summary>
        /// <param name="dt"></param>
        public void ProcessKeyInputs(float dt)
		{
			KeyboardState keyState = vxEngine.InputManager.KeyboardState;
			Keys[] keys = keyState.GetPressedKeys();

			bool shift = keyState.IsKeyDown(Keys.LeftShift) ||
				keyState.IsKeyDown(Keys.RightShift);

			foreach (Keys key in keys)
			{
				if (!IsKeyPressed(key, dt)) continue;

				char ch;
				if (KeyboardUtils.KeyToString(key, shift, out ch))
				{
					// Handle typical character input.
					Text = Text.Insert(cursorIndex, new string(ch, 1));
					cursorIndex++;


                    // Raise the 'TextChanged' event.
                    if (TextChanged != null)
                        TextChanged(this, new EventArgs());
                }
				else
				{
					switch (key)
					{
					case Keys.Back:
						if (cursorIndex > 0)
							Text = Text.Remove(--cursorIndex, 1);
						break;
					case Keys.Delete:
						if (cursorIndex < Text.Length)
							Text = Text.Remove(cursorIndex, 1);
						break;
					case Keys.Left:
						if (cursorIndex > 0)
							cursorIndex--;
						break;
					case Keys.Right:
						if (cursorIndex < Text.Length)
							cursorIndex++;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Pressing check with key repeating.
		/// </summary>
		/// <returns><c>true</c> if this instance is key pressed the specified key dt; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		/// <param name="dt">Dt.</param>
		bool IsKeyPressed(Keys key, float dt)
		{
			// Treat it as pressed if given key has not pressed in previous frame.
			if (vxEngine.InputManager.PreviousKeyboardState.IsKeyUp(key))
			{
				keyRepeatTimer = keyRepeatStartDuration;
				pressedKey = key;
				return true;
			}

			// Handling key repeating if given key has pressed in previous frame.
			if (key == pressedKey)
			{
				keyRepeatTimer -= dt;
				if (keyRepeatTimer <= 0.0f)
				{
					keyRepeatTimer += keyRepeatDuration;
					return true;
				}
			}

			return false;
		}
    }
}
