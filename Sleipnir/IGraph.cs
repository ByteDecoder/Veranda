#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir
{
    public interface IGraph
    {
        float Zoom { get; set; }
        Vector2 Position { get; set; }

        IEnumerable<INode> NodesInDrawingOrder();
        IEnumerable<string> AvailableNodes();
        INode AddNode(string nodeId);
        bool RemoveNode(INode node);
        void MoveNodeToFront(INode node);

        IEnumerable<IConnection> ConnectionsInDrawingOrder();
        void MoveConnectionToFront(IConnection connection);
        IConnection AddConnection(IKnob outputKnob, IKnob inputKnob);
        bool RemoveConnection(IConnection connection);
    }
}
#endif