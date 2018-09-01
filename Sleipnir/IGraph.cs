using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir
{
    public interface IGraph
    {
        float Scale { get; set; }
        Vector2 Position { get; set; }

        IList<Node> Nodes { get; }
        IEnumerable<string> AvailableNodes();
        void AddNode(string nodeId, Vector2 position);
        void RemoveNode(Node node);

        IEnumerable<Connection> Connections();
        void AddConnection(Knob outputKnob, Knob inputKnob);
        void RemoveConnection(Connection connection);
    }
}