#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.MessageBoxs;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Localization;


#endregion

namespace Virtex.Lib.Vrtc.GUI.MessageBoxs
{
	/// <summary>
	/// A popup message box screen, used to display "are you sure?"
	/// confirmation messages.
	/// </summary>
	public class vxMessageBoxSaveBeforeQuit : vxMessageBox
	{
		

		#region Initialization


		/// <summary>
		/// Constructor automatically includes the standard "A=ok, B=cancel"
		/// usage text prompt.
		/// </summary>
		public vxMessageBoxSaveBeforeQuit(string message, string title)
			: base(message, title, vxEnumButtonTypes.OkApplyCancel)
		{
			
		}

		public override void SetButtonText()
		{
			btn_Apply_text = "Save";
			btn_ok_text = "Don't";
			btn_ok_Cancel = LanguagePack.Get(vxLocalization.Misc_Cancel);
		}

		/// <summary>
		/// Loads graphics content for this screen. This uses the shared ContentManager
		/// provided by the Game class, so the content will remain loaded forever.
		/// Whenever a subsequent MessageBoxScreen tries to load this same content,
		/// it will just get back another reference to the already loaded data.
		/// </summary>
		public override void LoadContent()
		{
			base.LoadContent();


		}
		#endregion
	}
}
