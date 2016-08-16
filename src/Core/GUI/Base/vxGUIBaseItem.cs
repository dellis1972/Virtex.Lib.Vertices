using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Events;
using Virtex.Lib.Vrtc.Mathematics;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Microsoft.Xna.Framework.Input.Touch;

namespace Virtex.Lib.Vrtc.GUI
{
    /// <summary>
    /// GUI Base Class.
    /// </summary>
    public class vxGUIBaseItem
    {
        /// <summary>
        /// Gets or sets the vxEngine for this GUI item.
        /// </summary>
        /// <value>The vx engine.</value>
        public vxEngine vxEngine { get; set; }

        /// <summary>
        /// Object variable which allows arbitary data too be passed between methods.
        /// </summary>
        /// <value>The user data.</value>
        public object UserData { get; set; }

        /// <summary>
        /// The owning GUI Manger.
        /// </summary>
        public vxGuiManager GUIManager { get; set; }

        /// <summary>
        /// A string that is set in the base class of many items.
        /// </summary>
        public virtual Type GetBaseGuiType()
        {
            return typeof(vxGUIBaseItem);
        }

        /// <summary>
        /// Name Of GUI Item to help Identify it, not to be confused with Text
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string name = "<name>";

        /// <summary>
        /// Text Of GUI Item
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        string text = "<text>";

        /// <summary>
        /// Text Of GUI Item
        /// </summary>
        public SpriteFont Font
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }
        SpriteFont spriteFont;

        /// <summary>
        /// Gets or sets the opacity of the current GUI Item.
        /// </summary>
        /// <value>The opacity.</value>
        public float Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }
        float opacity = 1;


        /// <summary>
        /// Gets or sets the opacity of the current GUI Item.
        /// </summary>
        /// <value>The opacity.</value>
        public bool DoBorder
        {
            get { return doBorder; }
            set { doBorder = value; }
        }
        bool doBorder = false;


        #region Item Status (Clicked, Hoverd Etc...)

        /// <summary>
        /// Gets or sets a value indicating whether this instance has focus.
        /// </summary>
        /// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
        public bool HasFocus
        {
            get { return hasFocus; }
            set
            {
                hasFocus = value;
                hoverAlphaReq = hasFocus ? hoverAlphaMax : hoverAlphaMin;
            }
        }
        bool hasFocus = false;


        /// <summary>
        /// Gets or sets a value indicating whether this instance has focus.
        /// </summary>
        /// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
        public bool CaptureInput
        {
            get { return _captureInput; }
            set
            {
                _captureInput = value;
                if (_captureInput == true)
                    this.GUIManager.FocusedItem = this;
                else
                    this.GUIManager.FocusedItem = null;
            }
        }
        bool _captureInput = false;

        /// <summary>
        /// Is the Item a Toggleable?
        /// </summary>
        public bool IsTogglable
        {
            get { return _isTogglable; }
            set
            {
                _isTogglable = value;
                if (value == true)
                    ToggleState = ToggleState.Off;

            }
        }
        bool _isTogglable = false;

        /// <summary>
        /// Toggle State of the GUI Item. Note: IsTogglable must be set too true.
        /// </summary>
        public ToggleState ToggleState { get; set; }


		/// <summary>
		/// Gets or sets the alpha of the GUI Item.
		/// </summary>
		/// <value>The alpha.</value>
		public float Alpha
		{
			get { return _alpha; }
			set { _alpha = value; }
		}
		float _alpha = 1;

        /// <summary>
        /// Gets or sets the hover alpha.
        /// </summary>
        /// <value>The hover alpha.</value>
        public float HoverAlpha
        {
            get { return hoverAlpha; }
            set { hoverAlpha = value; }
        }
        float hoverAlpha = 0;

        /// <summary>
        /// Gets or sets the requested hover alpha for smoothing.
        /// </summary>
        /// <value>The hover alpha req.</value>
        public float HoverAlphaReq
        {
            get { return hoverAlphaReq; }
            set { hoverAlphaReq = value; }
        }
        float hoverAlphaReq = 0;

        /// <summary>
        /// Gets or sets the hover alpha max.
        /// </summary>
        /// <value>The hover alpha max.</value>
        public float HoverAlphaMax
        {
            get { return hoverAlphaMax; }
            set { hoverAlphaMax = value; }
        }
        float hoverAlphaMax = 1;

        /// <summary>
        /// Gets or sets the hover alpha minimum.
        /// </summary>
        /// <value>The hover alpha minimum.</value>
        public float HoverAlphaMin
        {
            get { return hoverAlphaMin; }
            set { hoverAlphaMin = value; }
        }
        float hoverAlphaMin = 0;

        /// <summary>
        /// Gets or sets the hover alpha delta speed of smoothing.
        /// </summary>
        /// <value>The hover alpha delta speed.</value>
        public float HoverAlphaDeltaSpeed
        {
            get { return hoverAlphaDeltaSpeed; }
            set { hoverAlphaDeltaSpeed = value; }
        }
        float hoverAlphaDeltaSpeed = 4;

        /// <summary>
        /// Returns Whether or not the item is Selected
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                hoverAlphaReq = 0;
            }
        }
        bool isSelected = false;

        /// <summary>
        /// Event Raised when the item is clicked
        /// </summary>
        public event EventHandler<vxGuiItemClickEventArgs> Clicked;


        /// <summary>
        /// Event Raised when the Mouse First Begins too Hover over this item.
        /// </summary>
        public event EventHandler<EventArgs> OnInitialHover;

        /// <summary>
        /// Returns Whether or not the item is Enabled
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                // Raise the 'Changed' event.
                if (EnabledStateChanged != null)
                    EnabledStateChanged(this, new EventArgs());
            }
        }
        bool enabled = true;

        /// <summary>
        /// Occurs when enabled state changed.
        /// </summary>
        public event EventHandler<EventArgs> EnabledStateChanged;
        #endregion

        #region Colour Properties

        /// <summary>
        /// Normal Colour Of GUI Item TEXT
        /// </summary>
        public Color Colour_Text
        {
            get { return color_text; }
            set { color_text = value; }
        }
        Color color_text = Color.Black;

        /// <summary>
        /// Normal Colour Of GUI Item
        /// </summary>
        public Color Colour
        {
            get { return color; }
            set { color = value; }
        }
        Color color = Color.DarkOrange;

        /// <summary>
        /// Normal Colour Of GUI Item
        /// </summary>
        public Color Color_Normal
        {
            get { return color_normal; }
            set { color_normal = value; }
        }
        Color color_normal = Color.DarkOrange;

        /// <summary>
        /// Highlighted Colour Of GUI Item
        /// </summary>
        public Color Color_Highlight
        {
            get { return color_highlight; }
            set { color_highlight = value; }
        }
        Color color_highlight = Color.Orange;

        /// <summary>
        /// Selected Colour Of GUI Item
        /// </summary>
        public Color Color_Selected
        {
            get { return color_select; }
            set { color_select = value; }
        }
        Color color_select = Color.LightCoral;

        #endregion

        #region Item Layout Properties

        /// <summary>
        /// Position Of GUI Item
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (PositionChanged != null)
                    PositionChanged(this, new EventArgs());
            }
        }
        Vector2 position = Vector2.Zero;

        /// <summary>
        /// Event raised when Item Position is Changed
        /// </summary>
        public event EventHandler<EventArgs> PositionChanged;

        /// <summary>
        /// Position Of GUI Item
        /// </summary>
        public Vector2 OriginalPosition
        {
            get { return originalPosition; }
            set { originalPosition = value; }
        }
        Vector2 originalPosition = Vector2.Zero;


        /// <summary>
        /// Bounding Rectangle Of GUI Item
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get { return boundingRectangle; }
            set { boundingRectangle = value; }
        }
        Rectangle boundingRectangle = Rectangle.Empty;

        /// <summary>
        /// GUI Item Orientation
        /// </summary>
        public vxGUIItemOrientation ItemOreintation
        {
            get { return itemOreintation; }
            set
            {
                itemOreintation = value;

                // Raise the 'Changed' event.
                if (ItemOreintationChanged != null)
                    ItemOreintationChanged(this, new EventArgs());
            }
        }
        vxGUIItemOrientation itemOreintation = vxGUIItemOrientation.Top;


        /// <summary>
        /// Event Raised when Item Orientation is Changed
        /// </summary>
        public event EventHandler<EventArgs> ItemOreintationChanged;




        /// <summary>
        /// Padding Of GUI Item
        /// </summary>
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        int padding = 5;

        /// <summary>
        /// Padding Of GUI Item in the X Direction
        /// </summary>
        public int PaddingX
        {
            get { return paddingX; }
            set { paddingX = value; }
        }
        int paddingX = 5;

        /// <summary>
        /// Padding Of GUI Item in the Y Direction
        /// </summary>
        public int PaddingY
        {
            get { return paddingY; }
            set { paddingY = value; }
        }
        int paddingY = 5;

        /// <summary>
        /// Width Of GUI Item
        /// </summary>
        public int Width
        {
            get { return GetWidth(); }
            set { width = value; }
        }
        int width = 50;
        public virtual int GetWidth()
        {
            return width;
        }

        /// <summary>
        /// Width Of GUI Item
        /// </summary>
        public int Height
        {
            get { return GetHeight(); }
            set { height = value; }
        }
        int height = 50;
        public virtual int GetHeight()
        {
            return height;
        }

        #endregion

        /// <summary>
        /// Previous Mouse State
        /// </summary>
        public MouseState PreviousMouseState
        {
            get { return previousMouseState; }
            set { previousMouseState = value; }
        }
        MouseState previousMouseState;

        /// <summary>
        /// Element Index.
        /// </summary>
        public int Index = 0;





        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.vxGUIBaseItem"/> class.
        /// </summary>
        public vxGUIBaseItem() {  }


        /// <summary>
        /// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.GUI.vxGUIBaseItem"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        public vxGUIBaseItem(Vector2 position)
        {
            this.Position = position;
            this.OriginalPosition = position;
        }

        /// <summary>
        /// Initialise this instance.
        /// </summary>
        public virtual void Initialise()
        {
            //Set Initial State
            previousMouseState = Mouse.GetState();

            //Always Set Initial Colour
            Colour = Color_Normal;
        }

        bool InitialHover = true;

        /// <summary>
        /// When the Mouse is NOT over the GUIItem
        /// </summary>
        public virtual void NotHover()
        {
            Colour = Color_Normal;
            isSelected = false;
            HasFocus = false;

            //Reset Initial Hover Flag
            InitialHover = true;
        }

        /// <summary>
        /// When the Mouse is over the GUIItem
        /// </summary>
        public virtual void Hover()
        {
            Colour = Color_Highlight;
            isSelected = false;
            HasFocus = true;

            if (InitialHover)
            {
                InitialHover = false;

                if (OnInitialHover != null)
                    OnInitialHover(this, new EventArgs());
            }
        }

        /// <summary>
        /// When the GUIItem is Selected
        /// </summary>
        public virtual void Select()
        {
            if (Enabled)
            {
                //To Show some visible cure the click was registered.
                HoverAlpha = 0;

                // Raise the Clicked event.
                if (Clicked != null)
                    Clicked(this, new vxGuiItemClickEventArgs(this));

                Colour = Color_Selected;
                isSelected = true;
                HasFocus = true;

                if (IsTogglable)
                {
                    if (ToggleState == ToggleState.Off)
                        ToggleState = ToggleState.On;
                    else
                        ToggleState = ToggleState.Off;
                }
            }
            else
                Colour *= 0.5f;
        }

        
        /// <summary>
        /// Updates the GUI Item
        /// </summary>
        public virtual void Update(vxEngine vxEngine)
        {
            //Get Position of either Mouse or Touch Panel Click
            //Vector2 cursorPos = mouseState.Position.ToVector2();

            Vector2 cursorPos = new Vector2(vxEngine.InputManager.MouseState.X, vxEngine.InputManager.MouseState.Y);

            try
            {
#if VRTC_PLTFRM_DROID
                if (vxEngine.InputManager.touchCollection.Count > 0)
                {
                    cursorPos = vxEngine.InputManager.touchCollection[0].Position;
                }
#endif

                if (cursorPos.X > BoundingRectangle.Left && cursorPos.X < BoundingRectangle.Right)
                {
                    if (cursorPos.Y < BoundingRectangle.Bottom && cursorPos.Y > BoundingRectangle.Top)
                    {
#if VRTC_PLTFRM_DROID
                        if (vxEngine.InputManager.touchCollection.Count > 0)
                        {
                            //   this.vxEngine.InputManager.Cursor = touchCollection[0].Position;

                            //Only Fire Select Once it's been released
                            if (vxEngine.InputManager.touchCollection[0].State == TouchLocationState.Pressed)
                                Select();
                            //Hover if and only if Moved is selected
                            else if (vxEngine.InputManager.touchCollection[0].State == TouchLocationState.Moved)
                                Hover();
                        }
#else
                    if (vxEngine.InputManager.MouseState.LeftButton == ButtonState.Pressed && 
                        previousMouseState.LeftButton == ButtonState.Released)
                         Select();                    
                    else
                        Hover();
#endif
                    }
                    else
                        NotHover();
                }
                else
                    NotHover();
            }
			catch (Exception ex)
            {
				if (vxEngine == null)
					Console.WriteLine ("Engine");
				else
					vxConsole.WriteError (this.ToString (), ex.Message);
                
            }
            //Set State for next Loop
            previousMouseState = vxEngine.InputManager.MouseState;

            //If it's a Toggle Item, set Toggle State
            if (IsTogglable)
            {
                if (ToggleState == ToggleState.On)
                    hoverAlphaReq = hoverAlphaMax;
                else
                    hoverAlphaReq = hasFocus ? hoverAlphaMax : hoverAlphaMin;
            }
        }



        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public virtual void Draw(vxEngine vxEngine)
        {
            if (spriteFont == null)
            {
                spriteFont = vxEngine.vxGUITheme.Font;
            }
            hoverAlpha = vxSmooth.SmoothFloat(hoverAlpha, hoverAlphaReq, HoverAlphaDeltaSpeed);
        }

        /// <summary>
        /// This Draw Call is to be used whenn the SpriteBatch.Begin() call is done by the owning 
        /// eneity. This is handy for allowing for custom draw calls such as when 
        /// </summary>
        /// <param name="vxEngine"></param>
        public virtual void DrawByOwner(vxEngine vxEngine)
        {
            if (spriteFont == null)
            {
                spriteFont = vxEngine.vxGUITheme.Font;
            }
            hoverAlpha = vxSmooth.SmoothFloat(hoverAlpha, hoverAlphaReq, HoverAlphaDeltaSpeed);
        }

        public virtual void DrawBorder(vxEngine vxEngine)
        {

        }

        /// <summary>
        /// Tries to convert keyboard input to characters and prevents repeatedly returning the 
        /// same character if a key was pressed last frame, but not yet unpressed this frame.
        /// </summary>
        /// <param name="keyboard">The current KeyboardState</param>
        /// <param name="oldKeyboard">The KeyboardState of the previous frame</param>
        /// <param name="key">When this method returns, contains the correct character if conversion succeeded.
        /// Else contains the null, (000), character.</param>
        /// <returns>True if conversion was successful</returns>
        [Obsolete]
        public bool TryConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard, out char key)
        {
            Keys[] keys = keyboard.GetPressedKeys();
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = '('; } else { key = '9'; } return true;

                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;

                    //Special keys
                    case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } return true;
                    case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } return true;
                    case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } return true;
                    case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } return true;
                    case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } return true;
                    case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } return true;
                    case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } return true;
                    case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } return true;
                    case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } return true;
                    case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return true;
                    case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; } return true;
                    case Keys.Space: key = ' '; return true;
                }
            }

            key = (char)0;
            return false;
        }
    }
}
