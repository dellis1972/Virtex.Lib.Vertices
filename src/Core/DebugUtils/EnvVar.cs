using System;
using Virtex.Lib.Vrtc.Utilities;

namespace Virtex.Lib.Vrtc
{
	public enum vxEnumEnvVarType
	{
		RES_X,
		RES_Y,
		FLSCRN,
		VSYNC,
		DEBUG_SHW_FPS,
		DEBUG_SHW_TIMERULES,
		DEBUG_MESH,
		DEBUG_RNDRTRGT,
		DEBUG_INGMECNSL,
		PATH_SETTINGS,
		PATH_SANDBOX,
	}
	public class EnvVar
	{
		public Type VarType;

		public object Var
		{
			get{ return _var; }
			set{
				if (VarType == typeof(int))
					_var = int.Parse (value.ToString());
				else if (VarType == typeof(bool))
					_var = bool.Parse (value.ToString());
				else if (VarType == typeof(char))
					_var = char.Parse(value.ToString());
				else if (VarType == typeof(string))
					_var = value;
				else
					vxConsole.WriteLine (string.Format("Types Don't Match. EnvType = {0}, NewType = {1}", VarType, value.GetType()));
			}
		}
		private object _var = 0;

		public string Description = "";

		public EnvVar (object _var, string description)
		{
			VarType = _var.GetType ();

			this._var = _var;
			Description = description;
		}
	}
}

