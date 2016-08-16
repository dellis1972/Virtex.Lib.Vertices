#if VIRTICES_2D
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Physics.Farseer.Dynamics;
using Virtex.Lib.Vrtc.Physics.Farseer;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Mathematics;

namespace Virtex.Lib.Vrtc.Core.Entities
{
	/// <summary>
	/// A Two Dimensional Entity which uses a Farseerer Body too set it's position, or vice versa.
	/// </summary>
	public class vxEntity2D : vxEntity
	{
		/// <summary>
		/// Gets or sets the 2D Position of the entity.
		/// </summary>
		/// <value>The position.</value>
		public Vector2 Position
		{
			get
			{
				if (body != null)
					return ConvertUnits.ToDisplayUnits (body.Position);
				else
					return mPosition; 
			}
			set
			{
				if (body != null)
					body.Position = ConvertUnits.ToSimUnits(value);

				mPosition = value; 
			}
		}
		Vector2 mPosition = Vector2.Zero;

		/// <summary>
		/// Gets or sets the float Rotation of the entity.
		/// </summary>
		/// <value>The position.</value>
		public float Rotation
		{
			get
			{
				if (body != null)
					return body.Rotation;
				else
					return mRotation; 
			}
			set
			{
				if (body != null)
					body.Rotation = value;

				mRotation = value;

//				vxConsole.WriteToInGameDebug(mRotation%MathHelper.PiOver4);
//				if(mRotation%MathHelper.PiOver4 < 0.1f){
//					vxConsole.WriteToInGameDebug ("Set Too: " + mRotation / MathHelper.PiOver4);
//				}
			}
		}
		float mRotation = 0;

		/// <summary>
		/// The Farseer Physics Body (Note, it's not used in all instances of this class, so do a !=null check)
		/// </summary>
		public Body body{get;set;}

		/// <summary>
		/// The world.
		/// </summary>
		public  World World;

		/// <summary>
		/// Gets or sets the texture of the Entity.
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture 
		{
			get{ return texture; }
			set 
			{ 
				texture = value; 
				if(texture != null)
					BoundingRectangle = texture.Bounds;
			}
		}
		private Texture2D texture;


		/// <summary>
		/// The items sprite effect which dictates which direction it faces (left or right)
		/// </summary>
		public SpriteEffects SpriteEffect;

		/// <summary>
		/// Texture Origina.
		/// </summary>
		public Vector2 Origin = new Vector2(0);

		/// <summary>
		/// Entity Alpha value.
		/// </summary>
		public float Alpha = 1;

		/// <summary>
		/// Requested Entity Alpha value for smooth change.
		/// </summary>
		public float Alpha_Req = 1;

		/// <summary>
		/// The alpha chnage steps.
		/// </summary>
		public int AlphaChnageSteps = 4;

		/// <summary>
		/// Sets the Texture Layer Depth.
		/// </summary>
		public float LayerDepth = 0;

		/// <summary>
		/// The display color of the Entity.
		/// </summary>
		public Color DisplayColor = Color.White;

		/// <summary>
		/// The bounding rectangle.
		/// </summary>
		public Rectangle BoundingRectangle = new Rectangle();

		/// <summary>
		/// The highlite value.
		/// </summary>
		public float Highlite = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Core.Entities.vxEntity2D"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="sprite">Sprite.</param>
		public vxEntity2D(vxEngine vxEngine, Texture2D sprite): base (vxEngine)
		{
			Texture = sprite;
			if(sprite != null)
				Origin = new Vector2(sprite.Width / 2f, sprite.Height / 2f);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Core.Entities.vxEntity2D"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="world">World.</param>
		/// <param name="position">Position.</param>
		public vxEntity2D (vxEngine vxEngine, Texture2D texture, World world, Vector2 position)  
			: this (vxEngine, texture, world, position, new Vector2(0))
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Core.Entities.vxEntity2D"/> class.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="world">World.</param>
		/// <param name="position">Position.</param>
		/// <param name="origin">Origin.</param>
		public vxEntity2D (vxEngine vxEngine, Texture2D texture, World world, Vector2 position, Vector2 origin)  : base (vxEngine)
		{
			Texture = texture;
			Origin = origin;
			Position = position;
			this.World = world;
			vxEngine.Current2DSceneBase.List_vxEntity2D.Add (this);
		}

		//TODO: This method should be removed.
		/// <summary>
		/// OBSELETE - THIS METHOD WILL BE REMOVED IN A COMING RELEASE.
		/// </summary>
		/// <param name="vxEngine">Vx engine.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="origin">Origin.</param>
		public vxEntity2D (vxEngine vxEngine, Texture2D texture, Vector2 origin) : base (vxEngine)
		{
			Texture = texture;
			Origin = origin;
		}

		/// <summary>
		/// Disposes the entity.
		/// </summary>
		public override void DisposeEntity ()
		{
			vxEngine.Current2DSceneBase.List_vxEntity2D.Remove (this);

			base.DisposeEntity ();
		}

		/// <summary>
		/// Draw this instance.
		/// </summary>
		public virtual void Draw ()
		{
			Alpha = vxSmooth.SmoothFloat (Alpha, Alpha_Req, AlphaChnageSteps);

			if (body != null)
				mPosition = ConvertUnits.ToDisplayUnits (body.Position);
			
			if(Texture != null && body != null)
			vxEngine.SpriteBatch.Draw(Texture, 
				Position,
				null,
				DisplayColor * Alpha, 
				Rotation, 
				Origin, 
				1f,
				SpriteEffect, 
				LayerDepth);
		}
	}
}
#endif