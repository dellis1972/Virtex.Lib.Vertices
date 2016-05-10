using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace vxVertices.Mathematics
{
	/// <summary>
	/// Smooths out Values based off of what they should be, what they are, and how many steps it should take to get there.
	/// </summary>
    public class vxSmooth
    {
		/// <summary>
		/// Smooths the color.
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
        static public Color SmoothColor(Color whatItIs, Color whatItShouldBe, float stepSize)
        {
            return new Color(
                SmoothFloat((float)whatItIs.R, (float)whatItShouldBe.R, stepSize),
                SmoothFloat((float)whatItIs.B, (float)whatItShouldBe.B, stepSize),
                SmoothFloat((float)whatItIs.G, (float)whatItShouldBe.G, stepSize)
                );
        }

		/// <summary>
		/// Smooth2s the D vector.
		/// </summary>
		/// <returns>The D vector.</returns>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
        static public Vector2 Smooth2DVector(Vector2 whatItIs, Vector2 whatItShouldBe, float stepSize)
        {
            whatItIs.X = SmoothFloat(whatItIs.X, whatItShouldBe.X, stepSize);
            whatItIs.Y = SmoothFloat(whatItIs.Y, whatItShouldBe.Y, stepSize);

            return whatItIs;
        }

		/// <summary>
		/// Smooths the vector.
		/// </summary>
		/// <returns>The vector.</returns>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
        static public Vector3 SmoothVector(Vector3 whatItIs, Vector3 whatItShouldBe, float stepSize)
        {
            whatItIs.X = SmoothFloat(whatItIs.X, whatItShouldBe.X, stepSize);
            whatItIs.Y = SmoothFloat(whatItIs.Y, whatItShouldBe.Y, stepSize);
            whatItIs.Z = SmoothFloat(whatItIs.Z, whatItShouldBe.Z, stepSize);

            return whatItIs;
        }

		/// <summary>
		/// Smooths the float.
		/// </summary>
		/// <returns>The float.</returns>
		/// <param name="whatItIs_f">What it is f.</param>
		/// <param name="whatItShouldBe_f">What it should be f.</param>
		/// <param name="stepSize">Step size.</param>
        static public float SmoothFloat(float whatItIs_f, float whatItShouldBe_f, float stepSize)
        {
            whatItIs_f = whatItIs_f + (whatItShouldBe_f - whatItIs_f) / stepSize;
            return whatItIs_f;
        }

		/// <summary>
		/// Smooths the int.
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="whatItIs">What it is.</param>
		/// <param name="whatItShouldBe">What it should be.</param>
		/// <param name="stepSize">Step size.</param>
        static public int SmoothInt(int whatItIs, int whatItShouldBe, int stepSize)
        {
            whatItIs = whatItIs + (whatItShouldBe - whatItIs) / stepSize;
            return whatItIs;
        }
    }
}
