using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vxVertices.Entities.Sandbox3D
{
    public enum vxSandboxEntityPropertyType
    {
        String,
        Integer,
        Float,
        Collection,
    }
    public class vxSandboxEntityProperty
    {
        /// <summary>
        /// The Properties Name
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _name;


        /// <summary>
        /// The Properties Value
        /// </summary>
        public virtual object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        object _value;

        /// <summary>
        /// Whether or not the Propertie is Read Only or Not
        /// </summary>
        public virtual bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }
        bool _readOnly = false;

        /// <summary>
        /// The Property Type
        /// </summary>
        public vxSandboxEntityPropertyType PropertyType
        {
            get;
            private set;
        }

        public vxSandboxEntityProperty(string Name)
        {
            _name = Name;
        }

        public vxSandboxEntityProperty(string Name, string Value):this(Name)
        {
            _value = Value;
            PropertyType = vxSandboxEntityPropertyType.String;
        }

        public vxSandboxEntityProperty(string Name, int Value) : this(Name)
        {
            _value = Value;
            PropertyType = vxSandboxEntityPropertyType.Integer;
        }

        public vxSandboxEntityProperty(string Name, float Value) : this(Name)
        {
            _value = Value;
            PropertyType = vxSandboxEntityPropertyType.Float;
        }

        public virtual void SetCollection<T>(List<T> a)
        {
            _value = a;
            PropertyType = vxSandboxEntityPropertyType.Collection;
        }
    }
}
