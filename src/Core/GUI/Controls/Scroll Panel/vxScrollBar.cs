using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
	/// <summary>
	/// Scroll bar gui item which controls the scroll position of a <see cref="T:Virtex.Lib.Vrtc.GUI.Controls.vxScrollPanel"/>.
	/// </summary>
	public class vxScrollBar : vxGUIBaseItem
	{
		/// <summary>
		/// The parent panel.
		/// </summary>
		public vxScrollPanel ParentPanel;

		/// <summary>
		/// The width of the bar.
		/// </summary>
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

		/// <summary>
		/// The start mouse position when the mouse is first clicked.
		/// </summary>
		int StartMousePosition;

		/// <summary>
		/// Is the panel scrolling.
		/// </summary>
		bool IsScrolling = false;

		/// <summary>
		/// Previous scroll wheel value.
		/// </summary>
		int ScrollWheel_Previous;


#if VRTC_PLTFRM_DROID
		bool isFirstTouchDown = true;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.GUI.Controls.vxScrollBar"/> class.
		/// </summary>
		/// <param name="Parent">Parent.</param>
		/// <param name="Position">Position.</param>
		/// <param name="TotalHeight">Total height.</param>
		/// <param name="ViewHeight">View height.</param>
		public vxScrollBar(vxScrollPanel Parent, Vector2 Position, int TotalHeight, int ViewHeight)
		{
			//Get Parent Panel
			ParentPanel = Parent;

			//Set Position
			this.Position = Position;

			//Set Total Height
			this.TotalHeight = TotalHeight;

			// Set the View Length
			scrollLength = ViewHeight;

			SetBounds();
		}

		void SetBounds()
		{
			ScrollBarHeight = (TotalHeight - Padding * 2) * (ParentPanel.Height
				- ParentPanel.Padding * 2) / ScrollLength;

			MaxTravel = this.TotalHeight - ScrollBarHeight - Padding * 4;

			BoundingRectangle = new Rectangle((int)Position.X,
				(int)Position.Y + TravelPosition,
				BarWidth, ScrollBarHeight);
		}


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
				TravelPosition += (int)vxEngine.InputManager.Cursor.Y - (int)vxEngine.InputManager.PreviousCursor.Y;

			if (HasFocus || ParentPanel.HasFocus)
			{
				TravelPosition += (mouseState.ScrollWheelValue - ScrollWheel_Previous) / -10;

				//TravelPosition = Math.Max(Math.Min(TravelPosition, MaxTravel), 0);
			}

			TravelPosition = (int)MathHelper.Clamp(TravelPosition, 0, MaxTravel);

			base.Update(vxEngine);

			ScrollWheel_Previous = mouseState.ScrollWheelValue;
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
			var rect = new Rectangle(BoundingRectangle.X, 0, BoundingRectangle.Width, 10000);
			vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, rect, Color.Black * 0.5f);
			vxEngine.SpriteBatch.Draw(vxEngine.Assets.Textures.Blank, BoundingRectangle, Colour);
		}
	}
}
