using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace Virtex.Lib.Vrtc.Mathematics
{
	/// <summary>
	/// Math Utility
	/// </summary>
	public static class vxMathUtil
	{

		/// <summary>
		/// Rounds to nearest half.
		/// </summary>
		/// <returns>The to nearest half.</returns>
		/// <param name="value">Value.</param>
		public static float RoundToNearestHalf(float value)
		{
			return (float)Math.Round(value * 2) / 2;
		}

		/// <summary>
		/// Rounds to nearest ten.
		/// </summary>
		/// <returns>The to nearest ten.</returns>
		/// <param name="value">The index.</param>
		public static int RoundToNearestTen (float value)
		{
			return ((int)Math.Round(value / 10.0)) * 10;
		}

		/// <summary>
		/// Rounds to nearest specified number.
		/// </summary>
		/// <returns>The to nearest specified number.</returns>
		/// <param name="value">Value.</param>
		/// <param name="valueToRoundTo">Value to round to.</param>
		public static float RoundToNearestSpecifiedNumber (float value, float valueToRoundTo)
		{
			return ((float)Math.Round(value / valueToRoundTo)) * valueToRoundTo;
		}

		/// <summary>
		/// Rounds to nearest specified number.
		/// </summary>
		/// <returns>The to nearest number.</returns>
		/// <param name="value">Value.</param>
		/// <param name="valueToRoundTo">Value to round to.</param>
		public static Vector2 RoundVector2(Vector2 value, float valueToRoundTo)
		{
			return new Vector2 (
				RoundToNearestSpecifiedNumber (value.X, valueToRoundTo),
				RoundToNearestSpecifiedNumber (value.Y, valueToRoundTo)
			);

		}
	}
}
