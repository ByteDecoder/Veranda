using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using RedOwl.Editor;
#endif

namespace RedOwl.GraphFramework
{
    public class GraphPort : Port
    {
        private Port proxy;

        new internal object data {
            get { return proxy.data; }
            set { proxy.data = value; }
        }

        public GraphPort(string name, Port proxy)
        {
            this.proxy = proxy;
            this.id = proxy.id;
            this.name = name;
            if (proxy.direction.IsInput())
            {
                this.direction = PortDirections.Output;
            } else {
                this.direction = PortDirections.Input;
            }
        }

        public override bool CanConnectPort(Port port)
        {
            if (type == port.type) return true;
            if (converter.CanConvertFrom(port.type))
            {
                if ((proxy.direction.IsOutput() && port.direction.IsOutput()) || proxy.direction.IsInput() && port.direction.IsInput())
                {
                    return true;
                }
            }
            return false;
        }
        public override Type type { get { return proxy.type; } }
#if UNITY_EDITOR
        public override PropertyFieldX GetField()
        {
            PropertyFieldX field = proxy.GetField();
            field.label.text = this.name;
            return field;
        }
#endif
    }
}