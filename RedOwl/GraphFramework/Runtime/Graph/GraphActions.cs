using System;
using UnityEngine;
using RedOwl.Serialization;

namespace RedOwl.GraphFramework
{
    public abstract partial class Graph
    {
        internal void AddConnection(Connection connection)
        {
			connections.Add(connection);
			FireConnectionAdded(connection);
            MarkDirty();
        }

        internal void RemoveConnection(int index)
        {
            FireConnectionRemoved(connections[index]);
            connections.RemoveAt(index);
			MarkDirty();
        }
    }
}
