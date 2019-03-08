using System;
using UnityEngine;
using RedOwl.Serialization;

namespace RedOwl.GraphFramework
{
    public abstract partial class Graph
    {
        internal void AddNode(Node node, Vector2 position)
        {
            node.id = Guid.NewGuid();
			node.graph = this;
			node.view.collapsed = false;
			node.view.layout = new Rect(position.x, position.y, 150, 0);
			node.Initialize();
			nodes.Add(node.id, node);
			AddSubAsset(node);
			FireNodeAdded(node);
        }

        internal void AddConnection(Connection connection)
        {
			connections.Add(connection);
			FireConnectionAdded(connection);
            MarkDirty();
        }

        internal void RemoveConnection(int index)
        {
            Connection connection = connections[index];
            connections.RemoveAt(index);
            FireConnectionRemoved(connection);
			MarkDirty();
        }
    }
}
