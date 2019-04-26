using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

namespace RedOwl.GraphFramework
{
    public class GraphPort : Port
    {
        private object instance;
        private PortInfo info;

        internal Port port {
            get { return info.Get(instance); }
        }

        public override object GetData()
        {
            return port.GetData();
        }

        public override void SetData(object value)
        {
            port.SetData(value);
        }

        public GraphPort(string name, object instance, PortInfo info)
        {
            this.instance = instance;
            this.info = info;
            this.id = port.id;
            this.name = name;
            if (info.direction.IsInput())
            {
                this.direction = PortDirections.Output;
            } else {
                this.direction = PortDirections.Input;
            }
        }

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
                if ((info.direction.IsOutput() && port.direction.IsOutput()) || info.direction.IsInput() && port.direction.IsInput())
                {
                    return true;
                }
            }
            return false;
        }
        public override Type type { get { return port.type; } }
#if UNITY_EDITOR
        public override PropertyField GetField()
        {
            return new PropertyField();
            //PropertyFieldX field = port.GetField();
            //field.label.text = this.name;
            //return field;
        }
#endif
    }
}