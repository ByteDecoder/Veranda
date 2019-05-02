using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
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

        public virtual object GetData()
        {
            return _data;
        }

        public virtual void SetData(object value)
        {
            try
            {
                _data = Convert.ChangeType(value, type);
            }
            catch (System.Exception)
            {
                _data = value;
            }
            FireValueChanged();
        }

        [SerializeField]
        public Guid id { get; protected set; }

        public string name;
        public PortDirections direction;

        private TypeConverter _converter;
        protected TypeConverter converter
        {
            get
            {
                if (_converter == null) _converter = TypeDescriptor.GetConverter(type);
                return _converter;
            }
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
            get { return (T)GetData(); }
            set { SetData(value); }
        }

        public Port(PortDirections direction) : this(default(T), direction) {}
        public Port(T value, PortDirections direction)
        {
            this.value = value;
            this.id = Guid.NewGuid();
            this.direction = direction;
        }

        // Contract
        public override bool CanConnectPort(Port port)
        {
            if (type == port.type) return true;
            Type unity = typeof(UnityEngine.Object);
            if (unity.IsAssignableFrom(type) && unity.IsAssignableFrom(port.type))
            {
                if (port.type.IsAssignableFrom(type)) return true;
            }
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
        public override PropertyFieldX GetField()
        {
            var field = new PropertyFieldX<T>(ObjectNames.NicifyVariableName(name), () => { return value; }, (data) => { value = data; });
            OnValueChanged += (value) => { field.UpdateField(); };
            return field;
        }
#endif
    }
}
