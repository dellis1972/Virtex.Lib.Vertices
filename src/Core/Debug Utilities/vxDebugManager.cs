﻿#region File Description
//-----------------------------------------------------------------------------
// DebugManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Virtex.Lib.Vrtc.Core.Debug
{
    /// <summary>
    /// DebugManager class that holds graphics resources for debug
    /// </summary>
    public class vxDebugManager : DrawableGameComponent
    {
        // the name of the font to load
        private string debugFont;

        #region Properties

		public vxEngine vxEngine;

        /// <summary>
        /// Gets a sprite batch for debug.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Gets white texture.
        /// </summary>
        public Texture2D WhiteTexture { get; private set; }

        /// <summary>
        /// Gets SpriteFont for debug.
        /// </summary>
        public SpriteFont DebugFont { get; private set; }

        #endregion

        #region Initialize

		public vxDebugManager(vxEngine vxEngine, Game game, string debugFont)
            : base(game)
        {
            // Added as a Service.
			Game.Services.AddService(typeof(vxDebugManager), this);
            this.debugFont = debugFont;

            // This component doesn't need be call neither update nor draw.
            this.Enabled = false;
            this.Visible = false;

			this.vxEngine = vxEngine;
        }

        protected override void LoadContent()
        {
            // Load debug content.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

			DebugFont = vxEngine.EngineContentManager.Load<SpriteFont>(debugFont);

            // Create white texture.
            WhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] whitePixels = new Color[] { Color.White };
            WhiteTexture.SetData<Color>(whitePixels);

            base.LoadContent();
        }

        #endregion
    }
}