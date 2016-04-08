using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace vxVertices.Graphics
{
	/// <summary>
	/// Custom vertex structure for drawing particles.
	/// </summary>
	struct vxMeshVertex
	{

		// Describe the layout of this vertex structure.
		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3,
					VertexElementUsage.Position, 1),

				new VertexElement(12, VertexElementFormat.Vector3,
					VertexElementUsage.Normal, 0),

				new VertexElement(24, VertexElementFormat.Color,
					VertexElementUsage.Color, 0),

				new VertexElement(28, VertexElementFormat.Single,
					VertexElementUsage.TextureCoordinate, 0)
			);


		// Describe the size of this vertex structure.
		public const int SizeInBytes = 32;
	}

	/// <summary>
	/// Imports an *.obj file, parsing out the Vertices, Texture UV Coordinates as well as Vertice Normals
	/// </summary>
	public class vxModel
	{
		/// <summary>
		/// The Model Texture
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture
		{
			get{ return _texture; }
			set { _texture = value; }
		}
		Texture2D _texture;

		/// <summary>
		/// Gets or sets the collection of Vertices.
		/// </summary>
		/// <value>The vertices.</value>
		public List<Vector3> Vertices { get; set; }


		/// <summary>
		/// Gets or sets the collection of Normals.
		/// </summary>
		/// <value>The vertices.</value>
		public List<Vector3> Normals { get; set; }

		/// <summary>
		/// Gets or sets the collection of texture UV coordinates.
		/// </summary>
		/// <value>The texture UV coordinate.</value>
		public List<Vector2> TextureUVCoordinate { get; set; }


		public vxModel (string path)
		{

		}
	}
}

