using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vrtc.Core
{

	/// <summary>
	/// Is the game a Local game, or is it networked.
	/// </summary>
    public enum vxEnumGameType
    {
		/// <summary>
		/// The game is a local game.
		/// </summary>
        Local,

		/// <summary>
		/// The game is a network game.
		/// </summary>
        Networked
    }

    /*****************************************************************************/
    /*							Cascade Shadow Mapping  						 */
    /*****************************************************************************/

    /// <summary>
    /// Shadow map overlay mode.
    /// </summary>
    public enum vxEnumShadowMapOverlayMode
    {
        None,
        ShadowFrustums,
        ShadowMap,
        ShadowMapAndViewFrustum
    };

    /// <summary>
    /// Virtual camera mode.
    /// </summary>
    public enum vxEnumVirtualCameraMode
    {
        None,
        ViewFrustum,
        ShadowSplits
    };

    /// <summary>
    /// Scene shadow mode.
    /// </summary>
    public enum vxEnumSceneShadowMode
    {
        Default,
        SplitColors,
        BlockPattern,
    };
}
