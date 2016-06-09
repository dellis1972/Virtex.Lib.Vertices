using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using vxVertices.Core;

namespace vxVertices.GUI.Controls
{
    public class vxScrollBar : vxGUIBaseItem
    {
        public vxScrollPanel ParentPanel;
        public int BarWidth = 12;

        int TotalHeight = 0;
        private int scrollLength = 0;
        public int ScrollLength
        {
            get { return scrollLength; }
            set { scrollLength = value; SetBounds(); }
        }
        int MaxTravel = 0;

        private int scrollBarHeight = 0;
        public int ScrollBarHeight
        {
            get { return scrollBarHeight; }
            set { scrollBarHeight = value; }
        }

        int travelPosition = 0;
        public int TravelPosition
        {
            get { return travelPosition; }
            set { travelPosition = value; SetBounds(); }
        }

        public float Percentage
        {
            get 
            {
                if (MaxTravel < 1)
                    return 0;
                else
                    return (float)(TravelPosition) / ((float)(MaxTravel)); 
            }
        }

        //public Vector2 BasePosition;
        int StartMousePosition;

        bool IsScrolling = false;
        int ScrollWheel_Previous;


        public vxScrollBar(vxScrollPanel Parent, Vector2 Position, int TotalHeight, int ViewHeight)
        {
            this.ParentPanel = Parent;
            this.Position = Position;
            //BasePosition = Position;

            this.TotalHeight = TotalHeight;

            scrollLength = ViewHeight;

            SetBounds();
        }
        
        void SetBounds()
        {
            ScrollBarHeight = (this.TotalHeight - Padding * 2) * (this.ParentPanel.Height 
                - this.ParentPanel.Padding * 2) / this.ScrollLength;

            MaxTravel = this.TotalHeight - ScrollBarHeight - Padding * 4;

            BoundingRectangle = new Rectangle((int)Position.X,
                (int)Position.Y + TravelPosition,
                BarWidth, ScrollBarHeight);
        }
        bool isFirstTouchDown = true;
        public override void Update(vxEngine vxEngine)
        {
            MouseState mouseState = vxEngine.InputManager.MouseState;

            BoundingRectangle = new Rectangle((int)Position.X,
    (int)Position.Y + TravelPosition,
    BarWidth, ScrollBarHeight);

#if VRTC_PLTFRM_DROID
            if (HasFocus)
            {
                if (vxEngine.InputManager.IsTouchPressed() && isFirstTouchDown == true)
                {
                    isFirstTouchDown = false;
                    StartMousePosition = (int)vxEngine.InputManager.Cursor.Y;
                    IsScrolling = true;
                }
            }

            if (vxEngine.InputManager.touchCollection.Count == 0)
            {
                isFirstTouchDown = true;
                IsScrolling = false;
            }
#else
            if (HasFocus)
            {
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    StartMousePosition = mouseState.Y;
                    IsScrolling = true;
                }
            }

            if (mouseState.LeftButton == ButtonState.Released)
                IsScrolling = false;
#endif

            if (IsScrolling)
                TravelPosition = (int)vxEngine.InputManager.Cursor.Y - StartMousePosition;

            if (HasFocus || ParentPanel.HasFocus)
            {
                TravelPosition += (mouseState.ScrollWheelValue - ScrollWheel_Previous) / -10;

                TravelPosition = Math.Max(Math.Min(TravelPosition, MaxTravel), 0);
            }
            base.Update(vxEngine);

            ScrollWheel_Previous = mouseState.ScrollWheelValue;
        }

        public override void Draw(vxEngine vxEngine)
        {
            vxEngine.SpriteBatch.Begin();
            this.DrawByOwner(vxEngine);
            vxEngine.SpriteBatch.End();
        }

        public override void DrawByOwner(vxEngine vxEngine)
        {
            base.DrawByOwner(vxEngine);
            Rectangle rect = new Rectangle(BoundingRectangle.X, 0, BoundingRectangle.Width, 10000);
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, rect, Color.Black*0.5f);
            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour);
        }
    }
}
