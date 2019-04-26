using System;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    public struct Slot
    {
        public Guid node;
        public Guid port;

        public Slot(Node node, Port port)
        {
            this.node = node.id;
            this.port = port.id;
        }
    }

    public struct Connection
    {
        public Guid id;
        public Slot input;
        public Slot output;

        public Connection(Node outputNode, Port outputPort, Node inputNode, Port inputPort)
        {
            this.id = Guid.NewGuid();
            this.input = new Slot(inputNode, inputPort);
            this.output = new Slot(outputNode, outputPort);
        }
    }
}