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
    public abstract class Port<T> : IPort
    {
        public string name { get; set; }
        [SerializeField]
        private Guid _id;
        public Guid id { get { return _id; } protected set { _id = value; }}
        [SerializeField]
        private PortStyles _style;
        public PortStyles style { get { return _style; } protected set { _style = value; }}
        [SerializeField]
        private PortDirections _direction;
        public PortDirections direction { get { return _direction; } protected set { _direction = value; }}
        public T value;
        public Type type { get { return typeof(T); } }

        private TypeConverter _converter;
        private TypeConverter converter
        {
            get
            {
                if (_converter == null) _converter = TypeDescriptor.GetConverter(type);
                return _converter;
            }
        }

        public Port(PortDirections direction, PortStyles style = PortStyles.Single)
        {
            this.id = Guid.NewGuid();
            this.value = default(T);
            this.style = style;
            this.direction = direction;
        }

        public Port(T value, PortDirections direction, PortStyles style = PortStyles.Single)
        {
            this.id = Guid.NewGuid();
            this.value = value;
            this.style = style;
            this.direction = direction;
        }

        public object Get() => (object)value;

        public void Set(object value) => this.value = (T)Convert.ChangeType(value, type);

        public bool CanConnectPort(IPort port)
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

        public override string ToString() => value.ToString();
#if UNITY_EDITOR
        public PropertyFieldX GetField() => new PropertyFieldX<T>(ObjectNames.NicifyVariableName(name), () => { return value; }, (data) => { value = data; });
#endif
    }
}
