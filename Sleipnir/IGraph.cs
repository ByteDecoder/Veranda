using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir
{
    public interface IGraph
    {
        float Zoom { get; set; }
        Vector2 Position { get; set; }

        List<Node> Nodes { get; }
        IEnumerable<string> AvailableNodes();
        Node CreateNode(string nodeId, Vector2 position);
        bool RemoveNode(Node node);

        IEnumerable<Connection> Connections();
        void AddConnection(Knob outputKnob, Knob inputKnob);
        void RemoveConnection(Connection connection);
    }
}