using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir
{
    public interface IGraph
    {
        float Zoom { get; set; }
        Vector2 Pan { get; set; }

        IList<ValueWrappedNode> Nodes { get; }
        IEnumerable<Tuple<Type, string>> NodeTypes { get; }
        Node AddNode<T>(string key = null);
        void RemoveNode(Node node);

        IEnumerable<Connection> Connections();
        void AddConnection(Connection connection);
        void RemoveConnection(Connection connection);
        
        void SetDirty();
    }
}