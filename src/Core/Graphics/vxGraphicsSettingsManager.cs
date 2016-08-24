using System;
using Virtex.Lib.Vrtc.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;

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
					(float)Engine.EnviromentVariables [vxEnumEnvVarType.RES_X.ToString ()].Var,
					(float)Engine.EnviromentVariables [vxEnumEnvVarType.RES_Y.ToString ()].Var
				);
			}
			set {
				Engine.EnviromentVariables [vxEnumEnvVarType.RES_X.ToString ()].Var = (int)value.X;
				Engine.EnviromentVariables [vxEnumEnvVarType.RES_Y.ToString ()].Var = (int)value.Y;
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
		private bool _loadResolution = false;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is full screen. NOTE: Apply needs to be called to apply these settings.
		/// </summary>
		/// <value><c>true</c> if this instance is full screen; otherwise, <c>false</c>.</value>
		public bool IsFullScreen
		{
			get {
				return (bool)Engine.EnviromentVariables [vxEnumEnvVarType.FLSCRN.ToString ()].Var;
			}
			set {
				Engine.EnviromentVariables [vxEnumEnvVarType.FLSCRN.ToString ()].Var = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.Graphics.vxGraphicsSettingsManager"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		public vxGraphicsSettingsManager (vxEngine Engine)
		{
			this.Engine = Engine;
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
					this.GraphicsDeviceManager.PreferredBackBufferHeight = (int)Resolution.X;
					this.GraphicsDeviceManager.PreferredBackBufferWidth = (int)Resolution.Y;


					vxConsole.WriteLine("Setting Resolution to: '" +
						this.GraphicsDeviceManager.PreferredBackBufferWidth.ToString() + " x " + 
						this.GraphicsDeviceManager.PreferredBackBufferHeight.ToString() + "'");

					this.GraphicsDeviceManager.ApplyChanges();
				}
				catch (Exception exception)
				{
					vxConsole.WriteError(exception);
				}
				#endregion

				#region Set FullScreen or Not

				this.GraphicsDeviceManager.IsFullScreen = this.IsFullScreen;

				vxConsole.WriteLine("Fullscreen: " + this.GraphicsDeviceManager.IsFullScreen);

				//Set Graphics
				this.GraphicsDeviceManager.ApplyChanges();
			#if VIRTICES_3D
			//Reset All Render Targets
			if (this.Renderer !=null)
			this.Renderer.InitialiseRenderTargetsAll();
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

