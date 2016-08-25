using System;
using Virtex.Lib.Vrtc.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Debug;
using System.Collections.Generic;

namespace Virtex.Lib.Vrtc.Graphics
{
	/// <summary>
	/// The Engine Graphical Settings Manager which holds all of settings for engine graphics such as
	/// resolution, fullscreen, vsync as well as other Depth of Field toggle or Cascade Shadow qaulity.
	/// </summary>
	public class vxGraphicsSettingsManager
	{
		/// <summary>
		/// Reference to the Engine.
		/// </summary>
		vxEngine Engine {get; set;}

		/// <summary>
		/// Gets the graphics device manager.
		/// </summary>
		/// <value>The graphics device manager.</value>
		GraphicsDeviceManager GraphicsDeviceManager {
			get
			{
				return Engine.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
			}
		}

		/// <summary>
		/// Gets or sets the resolution. NOTE: Apply needs to be called to apply these settings.
		/// </summary>
		/// <value>The resolution.</value>
		public Vector2 Resolution {
			get {
				return new Vector2(
					vxEnviroment.GetVar(vxEnumEnvVarType.RES_X).GetAsInt(),
					vxEnviroment.GetVar(vxEnumEnvVarType.RES_Y).GetAsInt()
				);
			}
			set {
				vxEnviroment.GetVar(vxEnumEnvVarType.RES_X).Value = (int)value.X;
				vxEnviroment.GetVar(vxEnumEnvVarType.RES_Y).Value = (int)value.Y;
			}
		}

		public bool LoadResolution {
			get {
				return _loadResolution;
			}
			set {
				_loadResolution = value;
			}
		}
		private bool _loadResolution = true;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is full screen. NOTE: Apply needs to be called to apply these settings.
		/// </summary>
		/// <value><c>true</c> if this instance is full screen; otherwise, <c>false</c>.</value>
		public bool IsFullScreen
		{
			get {
				return vxEnviroment.GetVar(vxEnumEnvVarType.FLSCRN).GetAsBool();
			}
			set {
				vxEnviroment.GetVar(vxEnumEnvVarType.FLSCRN).Value = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Graphics.vxGraphicsSettingsManager"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		public vxGraphicsSettingsManager (vxEngine Engine)
		{
			this.Engine = Engine;

			Engine.DebugSystem.DebugCommandUI.RegisterCommand (
				"grf",              // Name of command
				"Refresh the Grapahics Settings",     // Description of command
				delegate (IDebugCommandHost host, string command, IList<string> args) {
					this.Apply();
				});
		}

		/// <summary>
		/// Saves the Graphics Settings to the Isolated Storage Folder.
		/// </summary>
		public virtual void Save()
		{

		}

		/// <summary>
		/// Loads the Graphics Settings from the Isolated Storage Folder.
		/// </summary>
		public virtual void Load()
		{

		}

		public void Apply()
		{
			#if !VRTC_PLTFRM_DROID 
			if (_loadResolution == true)
			{

				#region Set Resolution

				try
				{
					this.GraphicsDeviceManager.PreferredBackBufferWidth = (int)Resolution.X;
					this.GraphicsDeviceManager.PreferredBackBufferHeight = (int)Resolution.Y;
				}
				catch (Exception exception)
				{
					vxConsole.WriteError(exception);
				}
				#endregion

				#region Set FullScreen or Not

				this.GraphicsDeviceManager.IsFullScreen = this.IsFullScreen;

				vxConsole.WriteLine(string.Format("Refreshing Graphics: - Resolution: {0} x {1} - Fullscreen: {2}",
					this.GraphicsDeviceManager.PreferredBackBufferWidth,
					this.GraphicsDeviceManager.PreferredBackBufferHeight,
					this.GraphicsDeviceManager.IsFullScreen));

				//Set Graphics
				this.GraphicsDeviceManager.ApplyChanges();
			#if VIRTICES_3D
			//Reset All Render Targets
			if (this.Engine.Renderer !=null)
					this.Engine.Renderer.InitialiseRenderTargetsAll();
			#endif
				for (int i = 0; i < 8; i++)
				{
					this.Engine.GraphicsDevice.SamplerStates[i] = SamplerState.PointClamp;
				}

				#endregion
			}
			#endif
		}
	}
}

