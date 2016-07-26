using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vertices.Scenes.Sandbox3D
{
	/// <summary>
	/// Selection state, useful for Sandbox items. 
	/// </summary>
    public enum vxEnumSelectionState
    {
		/// <summary>
		/// The Item is selected.
		/// </summary>
        Selected,
        
		/// <summary>
		/// Item is being hovered.
		/// </summary>
		Hover,
        
		/// <summary>
		/// Item is unseleced.
		/// </summary>
		Unseleced,
    }
		
	/// <summary>
	/// Sandbox game type. Is it a fresh start, should it open a file, or is it running the file as a game.
	/// </summary>
    public enum vxEnumSandboxGameType
    {
        /// <summary>
        /// Starts a New Sanbox game.
        /// </summary>
        NewSandbox,

        /// <summary>
        /// Opens a specified sandbox file.
        /// </summary>
        OpenSandbox,

        /// <summary>
        /// Runs the level as if it's the game, no editing is allowed in this setting.
        /// </summary>
        RunGame,
    }

	/// <summary>
	/// Sandbox game state.
	/// </summary>
    public enum vxEnumSandboxGameState
    {
        /// <summary>
        /// Sandbox is in Edit Mode
        /// </summary>
        EditMode,

        /// <summary>
        /// Sandbox is Running
        /// </summary>
        Running,
    }

    public enum vxEnumSanboxMouseClickState
    {
        /// <summary>
        /// Adds an Item on Click
        /// </summary>
        AddItem,

        /// <summary>
        /// Selects the Highlited Item on Click
        /// </summary>
        SelectItem,

    }
}
