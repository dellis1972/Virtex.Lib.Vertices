using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace Virtex.Lib.Vertices.Mathematics
{
	/// <summary>
	/// Math Utility
	/// </summary>
	public static class vxMathUtil
	{
		public static int RoundToNearestTen (float i)
		{
			return ((int)Math.Round(i / 10.0)) * 10;
		}
	}
}
