using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vrtc.Graphics
{
	/// <summary>
	/// Holds all of the Render Passes
	/// </summary>
	public enum RenderPass { 
		/// <summary>
		/// The shadow pass.
		/// </summary>
		ShadowPass, 

		/// <summary>
		/// The colour pass.
		/// </summary>
		ColourPass, 

		/// <summary>
		/// The prep pass.
		/// </summary>
		PrepPass, 

		/// <summary>
		/// The water reflection.
		/// </summary>
		WaterReflection };


	/// <summary>
	/// Distortion techniques.
	/// </summary>
	public enum DistortionTechniques
	{
		/// <summary>
		/// Distortion casused by a distortion map.
		/// </summary>
		DisplacementMapped,

		/// <summary>
		/// The heat haze distortion.
		/// </summary>
		HeatHaze,

		/// <summary>
		/// Pull In Distortion.
		/// </summary>
		PullIn,

		/// <summary>
		/// No Displacement or Distortion.
		/// </summary>
		ZeroDisplacement,
	}

}
