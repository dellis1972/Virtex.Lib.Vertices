using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using vxVertices.Core;

namespace vxVertices.GUI.Controls
{
    public class vxListViewScrollBar : vxGUIBaseItem
    {
        public vxListView ParentPanel;
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

        public vxListViewScrollBar(vxListView Parent, Vector2 Position, int TotalHeight, int ViewHeight)
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

        public override void Update(vxEngine vxEngine)
        {
            BoundingRectangle = new Rectangle((int)Position.X,
    (int)Position.Y + TravelPosition,
    BarWidth, ScrollBarHeight);

            if (HasFocus)
            {
                if (vxEngine.InputManager.MouseState.LeftButton == ButtonState.Pressed &&
                    PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    StartMousePosition = vxEngine.InputManager.MouseState.Y;
                    IsScrolling = true;
                }
            }

            if (vxEngine.InputManager.MouseState.LeftButton == ButtonState.Released)
                IsScrolling = false;


            if (IsScrolling)
                TravelPosition = vxEngine.InputManager.MouseState.Y - StartMousePosition;

            if (HasFocus || ParentPanel.HasFocus)
            {
                TravelPosition += (vxEngine.InputManager.MouseState.ScrollWheelValue - ScrollWheel_Previous) / -10;

                TravelPosition = Math.Max(Math.Min(TravelPosition, MaxTravel), 0);
            }
            base.Update(vxEngine);

            ScrollWheel_Previous = vxEngine.InputManager.MouseState.ScrollWheelValue;
        }

        public override void Draw(vxEngine vxEngine)
        {
            base.Draw(vxEngine);

            //
            //Draw Button
            //
            vxEngine.SpriteBatch.Begin();

            vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour);

            vxEngine.SpriteBatch.End();
        }
    }
}
