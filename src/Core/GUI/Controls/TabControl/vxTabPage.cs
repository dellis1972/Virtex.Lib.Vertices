using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using vxVertices.Utilities;
using vxVertices.Core;
using vxVertices.Mathematics;

namespace vxVertices.GUI.Controls
{
	/// <summary>
	/// Tab Page
	/// </summary>
	public class vxTabPage : vxGUIBaseItem
	{
		/// <summary>
		/// The Tab Control which Manages this page.
		/// </summary>
		public vxTabControl xTabCntrl;

		/// <summary>
		/// Tab Height
		/// </summary>
		public int TabHeight {
			get { return tabHeight; }
			set {
				tabHeight = value;
				SetLengths ();
			}
		}

		int tabHeight = 100;

		/// <summary>
		/// Tab Width
		/// </summary>
		public int TabWidth {
			get { return tabWidth; }
			set {
				tabWidth = value;
				SetLengths ();
			}
		}

		int tabWidth = 50;


		Rectangle Rec_Tab = new Rectangle ();
		Rectangle Rec_Back = new Rectangle ();

		public Vector2 Position_Original = new Vector2 ();
		public Vector2 Position_Requested = new Vector2 ();
        
		//Viewport of displayed items
		Viewport viewport;

		//List of Items owned by this Tab Page
		List<vxGUIBaseItem> Items = new List<vxGUIBaseItem> ();


		//Rotation of Tab, Dependant on ItemOrientation
		float TabRotation = 0;

		Texture2D TabTexture;
		Texture2D BackTexture;

		Vector2 TabPositionOffset = Vector2.Zero;
		Vector2 TabTextPositionOffset = Vector2.Zero;
		Vector2 ChildElementOffset = Vector2.Zero;

		public Vector2 TabOffset = Vector2.Zero;

		Vector2 TabPosition = Vector2.Zero;
		Vector2 TabTextPosition = Vector2.Zero;

		Vector2 OffsetVector {
			get {
				Vector2 vec = new Vector2 (-xTabCntrl.SelectionExtention, 0);
				switch (this.ItemOreintation) {
				case vxGUIItemOrientation.Left:
					vec = new Vector2 (xTabCntrl.SelectionExtention, 0);
					break;

				case vxGUIItemOrientation.Right:
					vec = new Vector2 (-xTabCntrl.SelectionExtention, 0);
					break;
				}

				return vec;

			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="vxVertices.GUI.Controls.vxTabPage"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="xtabCntrl">Xtab cntrl.</param>
		/// <param name="tabName">Tab name.</param>
		public vxTabPage (vxEngine vxEngine, vxTabControl xtabCntrl, string tabName)
		{
			this.vxEngine = vxEngine;

			this.Font = vxEngine.vxGUITheme.Font;

			this.xTabCntrl = xtabCntrl;

			//Set Tab Name
			Text = tabName;

			//Position is Set By Tab Control
			Position = this.xTabCntrl.Position;
			Position_Original = this.xTabCntrl.Position;

			Height = 46 + 2;

			SetLengths ();

			TabTexture = vxEngine.Assets.Textures.Blank;
			BackTexture = vxEngine.Assets.Textures.Blank;
			Padding = 5;


			Color_Normal = Color.Black;
			Color_Highlight = Color.DarkOrange;
			Color_Selected = Color.Black;

			HoverAlphaMax = 1.0f;
			HoverAlphaMin = 0.0f;
			HoverAlphaDeltaSpeed = 10;

			this.Width = xtabCntrl.Width;
			this.Height = xtabCntrl.Height;

			//Set up Events
			this.ItemOreintationChanged += new EventHandler<EventArgs> (vxTabPage_ItemOreintationChanged);
		}

		void vxTabPage_ItemOreintationChanged (object sender, EventArgs e)
		{
			switch (this.ItemOreintation) {
			case vxGUIItemOrientation.Left:
                    
                    //Set Rotatioj
				TabRotation = MathHelper.PiOver2;

                    //Set Tab Offsets
				TabPositionOffset = new Vector2 (this.Width, 0) + TabOffset;
				TabTextPositionOffset = new Vector2 (this.Width + tabHeight - Padding, Padding) + TabOffset;

				ChildElementOffset = new Vector2 (this.xTabCntrl.TabHeight + Padding, Padding);

                    //Position is Set By Tab Control
				Position = this.xTabCntrl.Position - new Vector2 (this.xTabCntrl.TabHeight, 0);
				Position_Original = Position;

				break;

			case vxGUIItemOrientation.Right:

                    //Set Rotatioj
				TabRotation = MathHelper.PiOver2;

				TabHeight *= -1;

                    //Set Tab Offsets
				TabPositionOffset = new Vector2 (this.xTabCntrl.TabHeight + Padding / 2, 0) + TabOffset;
				TabTextPositionOffset = new Vector2 (this.xTabCntrl.TabHeight - Padding, 0) + new Vector2 (TabHeight - Padding, Padding) + TabOffset;

				ChildElementOffset = new Vector2 (Padding, Padding);

                    //Position is Set By Tab Control
				Position = this.xTabCntrl.Position - new Vector2 (this.xTabCntrl.TabHeight - Padding, 0);
				Position_Original = Position;

				break;
			}
		}

		/// <summary>
		/// Sets the lengths.
		/// </summary>
		public void SetLengths ()
		{
			tabHeight = (int)(this.Font.MeasureString (Text).Y) + Padding * 2;
			tabWidth = (int)(this.Font.MeasureString (Text).X) + Padding * 2;
		}

		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <param name="guiItem">GUI item.</param>
		public void AddItem (vxGUIBaseItem guiItem)
		{
			Items.Add (guiItem);
		}


		//Clears out the GUI Items
		public void ClearItems ()
		{
			Items.Clear ();
		}

		/// <summary>
		/// The force select.
		/// </summary>
		public bool ForceSelect = false;

		/// <summary>
		/// Open this tab page. It won't close untill either Close(); is called, or if the tabpage recieves focus, and then loses focus.
		/// </summary>
		public void Open ()
		{
			ForceSelect = true;
		}

		public void Close ()
		{
			//Clear Focus
			HasFocus = false;

			//Clear Force Select just in case it was set.
			ForceSelect = false;

			//Reset Initial Position
			Position_Requested = Position_Original;
		}

		/// <summary>
		/// Updates the GUI Item
		/// </summary>
		/// <param name="mouseState">Mouse state.</param>
		public override void Update (vxEngine vxEngine)
		{
			SetLengths ();
			//Position_Requested = Position_Original;
			Position_Original = this.xTabCntrl.Position - new Vector2 (this.xTabCntrl.TabHeight - Padding, 0);

			if (HasFocus == true) {
				ForceSelect = false;
			}
			//First Set Position based off of Selection Status
			if (IsSelected || ForceSelect) {
				Position_Requested = Position_Original + OffsetVector;
			} else if (HasFocus == false)
				Position_Requested = Position_Original;

			Position = vxSmooth.Smooth2DVector (Position, Position_Requested, 6);

			//Then Set Tab Position
			TabPosition = this.Position + TabPositionOffset;
			TabTextPosition = this.Position + TabTextPositionOffset;

			Rec_Tab = new Rectangle ((int)(TabPosition.X),
				(int)(TabPosition.Y),
				tabHeight,
				tabWidth);
			//vxConsole.WriteToInGameDebug(TabPosition);

			BoundingRectangle = Rec_Tab;

			base.Update (vxEngine);

			if (HasFocus == false) {
				Rec_Back = new Rectangle ((int)(Position.X), (int)(Position.Y), this.Width, this.Height);
				BoundingRectangle = Rec_Back;
				base.Update (vxEngine);
			}

			//First Set Position
			for (int i = 0; i < Items.Count; i++) {  
				vxGUIBaseItem bsGuiItm = Items [i];

				bsGuiItm.Position = this.Position + ChildElementOffset
				+ bsGuiItm.OriginalPosition;
                
				bsGuiItm.Update (vxEngine);

				if (bsGuiItm.HasFocus == true)
					HasFocus = true;

				//Re-eable all child items
				if (Math.Abs (Vector2.Subtract (Position, Position_Requested).Length ()) < 10) {
					bsGuiItm.Enabled = true;
					bsGuiItm.HoverAlphaReq = 1;
				} else {
					bsGuiItm.Enabled = false;
					bsGuiItm.HoverAlphaReq = 0;
				}
			}
		}

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		public override void Draw (vxEngine vxEngine)
		{
			base.Draw (vxEngine);

			viewport = vxEngine.GraphicsDevice.Viewport;

			//
			//Draw Button
			//
			vxEngine.SpriteBatch.Begin ();
			Color ColorReq = Colour;

			if (HasFocus)
				ColorReq = Color.Black;

			vxEngine.SpriteBatch.Draw (BackTexture, Rec_Back, ColorReq * 0.5f);
			vxEngine.SpriteBatch.End ();
            
			foreach (vxGUIBaseItem bsGuiItm in Items) {
				bsGuiItm.Draw (vxEngine);
			}
             
		}

        
		/// <summary>
		/// Tabs are drawn by the tab control after everything else so that the are shown over tab of the open pages.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		public void DrawTab (vxEngine vxEngine)
		{
			vxEngine.SpriteBatch.Begin ();

			//Draw Hover Rectangle
			vxEngine.SpriteBatch.Draw (TabTexture, Rec_Tab, Colour * HoverAlpha);

			//Draw Edges
			vxEngine.SpriteBatch.Draw (TabTexture, new Rectangle (Rec_Tab.X, Rec_Tab.Y - 1, Rec_Tab.Width, 1), Color.White * 0.25f);
			vxEngine.SpriteBatch.Draw (TabTexture, new Rectangle (Rec_Tab.X, Rec_Tab.Y + Rec_Tab.Height, Rec_Tab.Width, 1), Color.White * 0.25f);

			//Draw String
			vxEngine.SpriteBatch.DrawString (this.Font, Text, TabTextPosition, Color.White, TabRotation,
				Vector2.Zero, 1, SpriteEffects.None, 0);


			vxEngine.SpriteBatch.End ();
		}

		/// <summary>
		/// When the GUIItem is Selected
		/// </summary>
		public override void Select ()
		{
			foreach (vxTabPage tabPage in xTabCntrl.Pages)
				tabPage.HasFocus = false;

			foreach (vxGUIBaseItem bsGuiItm in Items)
				bsGuiItm.Enabled = false;

			HasFocus = true;
			base.Select ();
		}
	}
}
