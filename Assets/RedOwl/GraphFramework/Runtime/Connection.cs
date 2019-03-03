using System;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    public struct Slot
    {
        public Guid node;
        public Guid port;

        public Slot(Node node, IPort port)
        {
            this.node = node.id;
            this.port = port.id;
        }
    }

    public struct Connection
    {
        public Slot input;
        public Slot output;

        public Connection(Node outputNode, IPort outputPort, Node inputNode, IPort inputPort)
        {
            this.input = new Slot(inputNode, inputPort);
            this.output = new Slot(outputNode, outputPort);
        }
    }
}