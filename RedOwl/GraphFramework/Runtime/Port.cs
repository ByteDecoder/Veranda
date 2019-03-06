using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using RedOwl.Editor;
#endif

namespace RedOwl.GraphFramework
{
    [Serializable]
    public abstract class Port
    {
        public delegate void ValueChanged(object data);
		public event ValueChanged OnValueChanged;
		internal void FireValueChanged()
		{
			OnValueChanged?.Invoke(_data);
		}

        [SerializeField]
        protected object _data;
        internal object data {
            get { return _data; }
            set { _data = Convert.ChangeType(value, type); }
        }

        public readonly Guid id;

        internal string name;
        internal PortStyles style;
        internal PortDirections direction;

        private TypeConverter _converter;
        protected TypeConverter converter
        {
            get
            {
                if (_converter == null) _converter = TypeDescriptor.GetConverter(type);
                return _converter;
            }
        }

        public Port(object value, PortDirections direction, PortStyles style = PortStyles.Single)
        {
            this._data = value;
            this.id = Guid.NewGuid();
            this.style = style;
            this.direction = direction;
        }

        public override string ToString() => _data.ToString();

        // Contract
        public abstract bool CanConnectPort(Port port);
        public abstract Type type { get; }
#if UNITY_EDITOR
        public abstract PropertyFieldX GetField();
#endif
    }

    [Serializable]
    public abstract class Port<T> : Port
    {
        public T value {
            get { return (T)data; }
            set { data = value; }
        }

        public Port(PortDirections direction) : this(default(T), direction) {}
        public Port(PortDirections direction, PortStyles style) : this(default(T), direction, style) {}
        public Port(T value, PortDirections direction) : base(value, direction) {}
        public Port(T value, PortDirections direction, PortStyles style) : base(value, direction, style) {}

        // Contract
        public override bool CanConnectPort(Port port)
        {
            if (type == port.type) return true;
            if (converter.CanConvertFrom(port.type))
            {
                if ((direction.IsInput() && port.direction.IsOutput()) || direction.IsOutput() && port.direction.IsInput())
                {
                    return true;
                } else {
                    Debug.LogWarningFormat("Port directions do not matchup safely ports: {0} && {1} || {2} && {3}", direction.IsInput(), port.direction.IsOutput(), direction.IsOutput(), port.direction.IsInput());
                }
            } else {
                Debug.LogWarningFormat("Unable to safely connect ports: {0} || {1} - {2}", this.id, port.id, converter.CanConvertFrom(port.type));
            }
            return false;
        }

        public override Type type { get { return typeof(T); } }
        
#if UNITY_EDITOR
        public override PropertyFieldX GetField() => new PropertyFieldX<T>(ObjectNames.NicifyVariableName(name), () => { return value; }, (data) => { value = data; FireValueChanged(); });
#endif
    }
}
