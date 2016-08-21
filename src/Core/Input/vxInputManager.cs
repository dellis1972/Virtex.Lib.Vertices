using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Virtex.Lib.Vrtc.Core.Input
{
    /// <summary>
    ///   an enum of all available mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public enum KeyboardTypes
    {
        QWERTY,
        AZERTY,
        CUSTOM
    }

    /// <summary>
    /// Class holding Key Press Type for common movements. This allows players to customise keyboard controls 
    /// for different keyboard types.
    /// </summary>
    public class KeyBindings
    {
        public Keys Forward = Keys.W;
        public Keys Left = Keys.A;
        public Keys Right = Keys.D;
        public Keys Back = Keys.S;
        public Keys Jump = Keys.Space;
        public Keys Croutch = Keys.LeftControl;

        public Keys Interact1 = Keys.E;
        public Keys Interact2 = Keys.Q;

        public KeyBindings()
        {

        }
    }

    public class vxInputManager
    {
        private readonly List<GestureSample> _gestures = new List<GestureSample>();

        private bool _handleVirtualStick;

        private bool _cursorIsVisible;

        public KeyBindings KeyBindings = new KeyBindings();

        /// <summary>
        /// Gets or sets the cursor sprite.
        /// </summary>
        /// <value>The cursor sprite.</value>
        public Texture2D CursorSprite
        {
            get { return _cursorSprite; }
            set { _cursorSprite = value; }
        }
        private Texture2D _cursorSprite;


        public Texture2D CursorSpriteClicked
        {
            get { return _cursorSpriteClicked; }
            set { _cursorSpriteClicked = value; }
        }
        private Texture2D _cursorSpriteClicked;

        /// <summary>
        /// Gets or sets the cursor rotation.
        /// </summary>
        /// <value>The cursor rotation.</value>
        public float CursorRotation { get; set; }

#if WINDOWS_PHONE
		private VirtualStick _phoneStick;
		private VirtualButton _phoneA;
		private VirtualButton _phoneB;
#endif

        private vxEngine _manager;
        private Viewport _viewport;

        /// <summary>
        ///   Constructs a new input state.
        /// </summary>
        public vxInputManager(vxEngine manager)
        {
            KeyboardState = new KeyboardState();
            GamePadState = new GamePadState();
            MouseState = new MouseState();
            VirtualState = new GamePadState();

            PreviousKeyboardState = new KeyboardState();
            PreviousGamePadState = new GamePadState();
            PreviousMouseState = new MouseState();
            PreviousVirtualState = new GamePadState();

            _manager = manager;


            _cursorIsVisible = false;
            IsCursorMoved = false;
#if WINDOWS_PHONE || VRTC_PLTFRM_DROID
            IsCursorValid = false;
#else
			IsCursorValid = true;
#endif
            Cursor = Vector2.Zero;

            _handleVirtualStick = false;
        }

        public GamePadState GamePadState { get; private set; }

        public KeyboardState KeyboardState { get; private set; }

        public TouchCollection touchCollection { get; private set; }

        public MouseState MouseState { get; set; }

        public GamePadState VirtualState { get; private set; }

        public GamePadState PreviousGamePadState { get; private set; }

        public KeyboardState PreviousKeyboardState { get; private set; }

        public MouseState PreviousMouseState { get; private set; }

        /// <summary>
        /// Gets the Change in Scroll wheel position since the last update
        /// </summary>
        public int ScrollWheelDelta { get; private set; }
        private int PreviousScrollWheel;

        public GamePadState PreviousVirtualState { get; private set; }

        public bool ShowCursor
        {
            get { return _cursorIsVisible; }
            set { _cursorIsVisible = value; }
        }

        public bool EnableVirtualStick
        {
            get { return _handleVirtualStick; }
            set { _handleVirtualStick = value; }
        }

        public Vector2 Cursor { get; set; }
        public Vector2 PreviousCursor { get; set; }

        public bool IsCursorMoved { get; private set; }

        public bool IsCursorValid { get; private set; }

        public void LoadContent()
        {
            _cursorSprite = _manager.EngineContentManager.Load<Texture2D>("Textures/Cursor");
            _cursorSpriteClicked = _manager.EngineContentManager.Load<Texture2D>("Textures/Cursor");

#if WINDOWS_PHONE
			// virtual stick content
			_phoneStick = new VirtualStick(_manager.Content.Load<Texture2D>("Common/socket"),
			_manager.Content.Load<Texture2D>("Common/stick"), new Vector2(80f, 400f));

			Texture2D temp = _manager.Content.Load<Texture2D>("Common/buttons");
			_phoneA = new VirtualButton(temp, new Vector2(695f, 380f), new Rectangle(0, 0, 40, 40), new Rectangle(0, 40, 40, 40));
			_phoneB = new VirtualButton(temp, new Vector2(745f, 360f), new Rectangle(40, 0, 40, 40), new Rectangle(40, 40, 40, 40));
#endif
            _viewport = _manager.GraphicsDevice.Viewport;
            TouchPanel.EnabledGestures = GestureType.Tap;

            PreviousScrollWheel = MouseState.ScrollWheelValue;
        }

        /// <summary>
        ///   Reads the latest state of the keyboard and gamepad and mouse/touchpad.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            PreviousKeyboardState = KeyboardState;
            PreviousGamePadState = GamePadState;
            PreviousMouseState = MouseState;
            PreviousCursor = Cursor;


            if (_handleVirtualStick)
                PreviousVirtualState = VirtualState;

            KeyboardState = Keyboard.GetState();
            GamePadState = GamePad.GetState(PlayerIndex.One);
            MouseState = Mouse.GetState();
            touchCollection = TouchPanel.GetState();

            if (_handleVirtualStick)
            {
#if XBOX
				VirtualState = GamePad.GetState(PlayerIndex.One);
#elif WINDOWS
				VirtualState = GamePad.GetState(PlayerIndex.One).IsConnected ? GamePad.GetState(PlayerIndex.One) : HandleVirtualStickWin();
#elif WINDOWS_PHONE
				VirtualState = HandleVirtualStickWP7();
#endif
            }

            _gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                _gestures.Add(TouchPanel.ReadGesture());
            }

            // Update cursor
            Vector2 oldCursor = Cursor;
            if (GamePadState.IsConnected && GamePadState.ThumbSticks.Left != Vector2.Zero)
            {
                Vector2 temp = GamePadState.ThumbSticks.Left;
                Cursor += temp * new Vector2(300f, -300f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Mouse.SetPosition((int)Cursor.X, (int)Cursor.Y);
            }
#if VRTC_PLTFRM_DROID
            else
                {
                if (touchCollection.Count > 0)
                {
                    //Only Fire Select Once it's been released
                    if (touchCollection[0].State == TouchLocationState.Moved || touchCollection[0].State == TouchLocationState.Pressed)
                    {
                        Cursor = touchCollection[0].Position;
                    }
                }
            }

#else
            else
            {
                Cursor = new Vector2(MouseState.X, MouseState.Y);// - _manager.Game.Window.Position.ToVector2();


				//TODO: There's a current bug in OSX DesktopGL in MonoGame 3.5 which doesn't update the mouse position
				//if the left or right button is pressed down. This is a workaround until the problem is solved in MG 3.6.
			#if VRTC_PLTFRM_GL

				//Get the Mouses Relative Position to the Window
				Point mouseRelative = new Point(
					System.Windows.Forms.Control.MousePosition.X - _manager.Game.Window.ClientBounds.X, 
					System.Windows.Forms.Control.MousePosition.Y - _manager.Game.Window.ClientBounds.Y);

				//Now set the Cursor to the relative position
				Cursor = mouseRelative.ToVector2();

				#endif
            }
#endif
            Cursor = new Vector2(MathHelper.Clamp(Cursor.X, 0f, _manager.GraphicsDevice.Viewport.Width), MathHelper.Clamp(Cursor.Y, 0f, _manager.GraphicsDevice.Viewport.Height));

            if (IsCursorValid && oldCursor != Cursor)
                IsCursorMoved = true;
            else
                IsCursorMoved = false;

            //#if WINDOWS
            IsCursorValid = _viewport.Bounds.Contains(MouseState.X, MouseState.Y);
#if WINDOWS_PHONE || VRTC_PLTFRM_DROID
            IsCursorValid = MouseState.LeftButton == ButtonState.Pressed;
#endif

            ScrollWheelDelta = MouseState.ScrollWheelValue - PreviousScrollWheel;
            PreviousScrollWheel = MouseState.ScrollWheelValue;
        }

        public void Draw()
        {
            //_cursorIsVisible = true;
            if (_cursorIsVisible)
            {

                _manager.SpriteBatch.Begin();
                /*
			_manager.SpriteBatch.DrawString (_manager.Assets.Fonts.DebugFont, 
				Cursor.ToString (),
				new Vector2 (30, 30),
				Color.Black);
				
			_manager.SpriteBatch.DrawString (_manager.Assets.Fonts.DebugFont, 
				(_manager.Game.Window.Position).ToString(),
				new Vector2 (30, 50),
				Color.Black);
				*/

                Texture2D textureToDraw = this.MouseState.LeftButton == ButtonState.Pressed ? _cursorSpriteClicked : _cursorSprite;

                _manager.SpriteBatch.Draw(
                        textureToDraw,
                    Cursor,
                    null,
                    Color.White,
                    CursorRotation,
                    new Vector2(_cursorSprite.Width / 2, _cursorSprite.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);

                _manager.SpriteBatch.End();

            }
#if WINDOWS_PHONE
			if (_handleVirtualStick)
			{
			_manager.SpriteBatch.Begin();
			_phoneA.Draw(_manager.SpriteBatch);
			_phoneB.Draw(_manager.SpriteBatch);
			_phoneStick.Draw(_manager.SpriteBatch);
			_manager.SpriteBatch.End();
			}
#endif
        }

        private GamePadState HandleVirtualStickWin()
        {
            Vector2 leftStick = Vector2.Zero;
            List<Buttons> buttons = new List<Buttons>();

            if (KeyboardState.IsKeyDown(Keys.A))
                leftStick.X -= 1f;
            if (KeyboardState.IsKeyDown(Keys.S))
                leftStick.Y -= 1f;
            if (KeyboardState.IsKeyDown(Keys.D))
                leftStick.X += 1f;
            if (KeyboardState.IsKeyDown(Keys.W))
                leftStick.Y += 1f;
            if (KeyboardState.IsKeyDown(Keys.Space))
                buttons.Add(Buttons.A);
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
                buttons.Add(Buttons.B);
            if (leftStick != Vector2.Zero)
                leftStick.Normalize();

            return new GamePadState(leftStick, Vector2.Zero, 0f, 0f, buttons.ToArray());
        }

        private GamePadState HandleVirtualStickWP7()
        {
            List<Buttons> buttons = new List<Buttons>();
            Vector2 stick = Vector2.Zero;
#if WINDOWS_PHONE
			_phoneA.Pressed = false;
			_phoneB.Pressed = false;
			TouchCollection touchLocations = TouchPanel.GetState();
			foreach (TouchLocation touchLocation in touchLocations)
			{
			_phoneA.Update(touchLocation);
			_phoneB.Update(touchLocation);
			_phoneStick.Update(touchLocation);
			}
			if (_phoneA.Pressed)
			{
			buttons.Add(Buttons.A);
			}
			if (_phoneB.Pressed)
			{
			buttons.Add(Buttons.B);
			}
			stick = _phoneStick.StickPosition;
#endif
            return new GamePadState(stick, Vector2.Zero, 0f, 0f, buttons.ToArray());
        }

        /// <summary>
        ///   Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (KeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key));
        }

        public bool IsNewKeyRelease(Keys key)
        {
            return (PreviousKeyboardState.IsKeyDown(key) && KeyboardState.IsKeyUp(key));
        }

        /// <summary>
        ///   Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (GamePadState.IsButtonDown(button) && PreviousGamePadState.IsButtonUp(button));
        }

        public bool IsNewButtonRelease(Buttons button)
        {
            return (PreviousGamePadState.IsButtonDown(button) && GamePadState.IsButtonUp(button));
        }

        /// <summary>
        ///   Helper for checking if a mouse button was newly pressed during this update.
        /// </summary>
        public bool IsNewMouseButtonPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (MouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (MouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (MouseState.MiddleButton == ButtonState.Pressed && PreviousMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (MouseState.XButton1 == ButtonState.Pressed && PreviousMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (MouseState.XButton2 == ButtonState.Pressed && PreviousMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }


        /// <summary>
        /// Checks if the requested mouse button is released.
        /// </summary>
        /// <param name="button">The button.</param>
        public bool IsNewMouseButtonRelease(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (PreviousMouseState.LeftButton == ButtonState.Pressed && MouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (PreviousMouseState.RightButton == ButtonState.Pressed && MouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (PreviousMouseState.MiddleButton == ButtonState.Pressed && MouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (PreviousMouseState.XButton1 == ButtonState.Pressed && MouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (PreviousMouseState.XButton2 == ButtonState.Pressed && MouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }

        /// <summary>
        ///   Checks for a "menu select" input action.
        /// </summary>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start) || IsNewMouseButtonPress(MouseButtons.LeftButton) || IsTouchReleased();
        }

        public bool IsMenuPressed()
        {
            return KeyboardState.IsKeyDown(Keys.Space) || KeyboardState.IsKeyDown(Keys.Enter) || GamePadState.IsButtonDown(Buttons.A) || GamePadState.IsButtonDown(Buttons.Start) || MouseState.LeftButton == ButtonState.Pressed || IsTouchReleased();
        }

        public bool IsMenuReleased()
        {
            return IsNewKeyRelease(Keys.Space) || IsNewKeyRelease(Keys.Enter) || IsNewButtonRelease(Buttons.A) || IsNewButtonRelease(Buttons.Start) || IsNewMouseButtonRelease(MouseButtons.LeftButton);
        }

        public bool IsTouchPressed()
        {
            if (touchCollection.Count > 0)
            {
                return (touchCollection[0].State != TouchLocationState.Released);
            }
            else
                return false;
        }

        public bool IsTouchReleased()
        {
            if (touchCollection.Count > 0)
            {
                return (touchCollection[0].State == TouchLocationState.Released);
            }
            else
                return false;
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp()
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up) ||
                   IsNewButtonPress(Buttons.DPadUp) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown()
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down) ||
                   IsNewButtonPress(Buttons.DPadDown) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame()
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.Back) ||
                   IsNewButtonPress(Buttons.Start);
        }

        /// <summary>
        ///   Checks for a "menu cancel" input action.
        /// </summary>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.Back);
        }
    }
}

