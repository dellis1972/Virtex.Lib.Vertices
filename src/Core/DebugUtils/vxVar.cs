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
		PATH_ENGINE_CONTENT,
		PATH_SETTINGS,
		PATH_SANDBOX,
	}
		

	/// <summary>
	/// A variable type which can be used too hold Enviroment Variables.
	/// </summary>
	public class vxVar
	{
		/// <summary>
		/// The type of the variable.
		/// </summary>
		public Type Type{
			get {
				return _type;
			}
		}
		private Type _type;

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public object Name {
			get {
				return _name;
			}
		}
		private object _name;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get {
				return _description;
			}
		}
		private string _description;

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get{ return _value; }
			set{
				try
				{
					//First check readonly status
					if(this._isReadOnly == true)
						throw new Exception(string.Format(
							"READ ONLY EXCEPTION!! VARIABLE '{0}' IS READ ONLY.",this.Name));

				if (Type == typeof(int))
					_value = int.Parse (value.ToString());
				else if (Type == typeof(bool))
					_value = bool.Parse (value.ToString());
				else if (Type == typeof(char))
					_value = char.Parse(value.ToString());
				else if (Type == typeof(string))
					_value = value;
				else
					throw new Exception(string.Format("Types Don't Match. EnvType = {0}, NewType = {1}", Type, value.GetType()));

				if(ChangeFunction != null)
					ChangeFunction (this);
				}
				catch(Exception ex) {
					vxConsole.WriteLine (ex.Message);
				}
			}
		}
		private object _value = 0;

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly {
			get {
				return _isReadOnly;
			}
		}
		private bool _isReadOnly = false;

		/// <summary>
		/// Returns the Var as a bool
		/// </summary>
		/// <returns><c>true</c>, if bool was gotten, <c>false</c> otherwise.</returns>
		public bool GetAsBool() { return (bool)_value; }

		/// <summary>
		/// Returns the Var as an int.
		/// </summary>
		/// <returns>The int.</returns>
		public int GetAsInt() { return (int)_value; }

		public float GetAsFloat() { return (float)_value; }

		/// <summary>
		/// Value change delegate.
		/// </summary>
		public delegate void ValueChangeDelegate(vxVar v);

		ValueChangeDelegate ChangeFunction;

		public vxVar (object value, string description)
		{
			this._type = value.GetType ();

			this._value = value;
			this._description = description;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.vxVar"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		/// <param name="description">Description.</param>
		public vxVar (object name, object value, string description) : this(name,value,description, null)
		{

		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.vxVar"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		/// <param name="description">Description.</param>
		/// <param name="isReadOnly">If set to <c>true</c> is read only.</param>
		public vxVar (object name, object value, string description, bool isReadOnly) : this(name,value,null)
		{
			this._isReadOnly = isReadOnly;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Virtex.Lib.Vrtc.vxVar"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		/// <param name="description">Description.</param>
		/// <param name="function">Function delegate to fire when the value is changed.</param>
		public vxVar (object name, object value, string description, ValueChangeDelegate function)
		{
			this._name = name;

			this._value = value;
			this._type = _value.GetType ();

			this._description = description;

			ChangeFunction = function;
		}
	}
}

