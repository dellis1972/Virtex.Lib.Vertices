using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vxVertices.Graphics
{
    /// <summary>
    /// Class holds all the settings used to tweak the bloom effect.
    /// </summary>
    public class vxBloomSettings
    {
        #region Fields


        /// <summary>
		/// Name of a preset bloom setting, for display to the user.
        /// </summary>
        public readonly string Name;

		/// <summary> 
		/// Controls how bright a pixel needs to be before it will bloom.
		/// Zero makes everything bloom equally, while higher values select
		/// only brighter colors. Somewhere between 0.25 and 0.5 is good.
		/// </summary>
        public readonly float BloomThreshold;


		/// <summary>
		/// Controls how much blurring is applied to the bloom image.
		/// The typical range is from 1 up to 10 or so.
		/// </summary>
        public readonly float BlurAmount;


		/// <summary>
		/// Controls the amount of the bloom and base images that
		/// will be mixed into the final scene. Range 0 to 1.
		/// </summary>
        public readonly float BloomIntensity;

		/// <summary>
		/// The base intensity.
		/// </summary>
        public readonly float BaseIntensity;


		/// <summary>
		/// Independently control the color saturation of the bloom and
		/// base images. Zero is totally desaturated, 1.0 leaves saturation
		/// unchanged, while higher values increase the saturation level.
		/// </summary>
        public readonly float BloomSaturation;
        
		/// <summary>
		/// The base saturation.
		/// </summary>
		public readonly float BaseSaturation;


        #endregion


        /// <summary>
        /// Constructs a new bloom settings descriptor.
        /// </summary>
        public vxBloomSettings(string name, float bloomThreshold, float blurAmount,
                             float bloomIntensity, float baseIntensity,
                             float bloomSaturation, float baseSaturation)
        {
            Name = name;
            BloomThreshold = bloomThreshold;
            BlurAmount = blurAmount;
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
        }


        /// <summary>
        /// Table of preset bloom settings, used by the sample program.
        /// </summary>
        public static vxBloomSettings[] PresetSettings =
        {
            //                Name           Thresh  Blur Bloom  Base  BloomSat BaseSat
            new vxBloomSettings("Default",     0.25f,  4,   1.25f, 1,    1,       1),
            new vxBloomSettings("Soft",        0,      3,   1,     1,    1,       1),
            new vxBloomSettings("Desaturated", 0.5f,   8,   2,     1,    0,       1),
            new vxBloomSettings("Saturated",   0.25f,  4,   2,     1,    2,       0),
            new vxBloomSettings("Blurry",      0,      2,   1,     0.1f, 1,       1),
            new vxBloomSettings("Subtle",      0.5f,   2,   1,     1,    1,       1),
        };
    }
}
