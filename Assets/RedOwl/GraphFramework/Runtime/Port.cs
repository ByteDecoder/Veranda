using System;
using System.ComponentModel;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    [Serializable]
    public abstract class Port<T> : IPort
    {
        public string name { get; set; }
        public Guid id { get ; protected set; }
        public PortStyles style { get; protected set; }
        public PortDirections direction { get; protected set; }
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

        public Port(T value, PortDirections direction, PortStyles style = PortStyles.Single)
        {
            this.id = Guid.NewGuid();
            this.value = value;
            this.style = style;
            this.direction = direction;
        }

        public object Get()
        {
            return (object)value;
        }

        public void Set(object value)
        {
            this.value = (T)Convert.ChangeType(value, type);
        }

        public bool CanConnectPort(IPort port)
        {
            if (type == port.type) return true;
            if (converter.CanConvertFrom(port.type))
            {
                if ((direction.IsInput() && port.direction.IsOutput()) || direction.IsOutput() && port.direction.IsInput())
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
