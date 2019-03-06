using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.GraphFramework
{
	public abstract partial class Graph
	{
		/// <summary>
		/// Returns the number of nodes in the graph
		/// </summary>
		/// <value></value>
		public int Count {
			get { return nodes.Count; }
		}

		/// <summary>
		/// Removes all nodes and connections from the graph
		/// </summary>
		public void Clear()
		{
			AutoExecute = false;
			nodes.Clear();
			connections.Clear();
			ClearSubAssets();
			MarkDirty();
			FireCleared();
		}

		/// <summary>
		/// Creates and returns a node of type T and adds it to the graph
		/// </summary>
		/// <typeparam name="T">Node Type to instantiate</typeparam>
		/// <returns></returns>
		public T Add<T>() where T : Node
		{
			return (T)Add(typeof(T), Vector2.zero);
		}

		/// <summary>
		/// Creates and returns a node of type T and adds it to the graph at the specified x,y position
		/// </summary>
		/// <param name="x">The x position of the node</param>
		/// <param name="y">The y position of the node</param>
		/// <typeparam name="T">Node Type to instantiate</typeparam>
		/// <returns></returns>
		public T Add<T>(float x, float y) where T : Node
		{
			return (T)Add(typeof(T), new Vector2(x, y));
		}

		/// <summary>
		/// Creates and returns a node of type T and adds it to the graph at the specified position
		/// </summary>
		/// <param name="position">The position in graph space to place the node</param>
		/// <typeparam name="T">Node Type to instantiate</typeparam>
		/// <returns></returns>
		public T Add<T>(Vector2 position) where T : Node
		{
			return (T)Add(typeof(T), position);
		}

		/// <summary>
		/// Creates and returns a node of type and adds it to the graph at the specified position
		/// </summary>
		/// <param name="nodeType">Node Type to instantiate</param>
		/// <param name="position">The position in graph space to place the node</param>
		/// <returns></returns>
		internal Node Add(Type nodeType, Vector2 position)
		{
			Node node = (Node)CreateInstance(nodeType);
			node.id = Guid.NewGuid();
			node.graph = this;
			node.view.collapsed = false;
			node.view.layout = new Rect(position.x, position.y, 150, 0);
			node.Initialize();
			nodes.Add(node.id, node);
			AddSubAsset(node);
			FireNodeAdded(node);
			return node;
		}

		/// <summary>
		/// Removes the node from the graph
		/// </summary>
		/// <param name="node">Node to remove</param>
		public void Remove<T>(T node) where T : Node
		{
			Remove(node.id);
		}

		/// <summary>
		/// Removes the node from the graph with the given id
		/// </summary>
		/// <param name="id">The idea of the node to remove</param>
		public void Remove(Guid id)
		{
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if (connections[i].input.node == id || connections[i].output.node == id)
				{
					FireConnectionRemoved(connections[i]);
					connections.RemoveAt(i);
				}
			}
			FireNodeRemoved(this[id]);
			nodes.Remove(id);
			RemoveSubAsset(id);
		}

		/// <summary>
		/// Create a connection from output port to intput port
		/// </summary>
		/// <param name="output">The output port to connect</param>
		/// <param name="input">The input port to connect the output port to</param>
		/// <returns>Returns true of the connection was made</returns>
		public bool Connect(Port output, Port input)
		{
			if (output.style == PortStyles.Single || input.style == PortStyles.Single)
			{
				foreach (var item in connections)
				{
					if (item.input.port == input.id || item.output.port == output.id)
					{
						Debug.LogWarningFormat("There can only be one connection for ports: {0} || {1}", output.id, input.id);
						return false;
					}
				}
			}
			if (!output.CanConnectPort(input)) return false;
			Node nodeOutput = FindNodeWithPort(output);
			Node nodeInput = FindNodeWithPort(input);
			if (nodeOutput == null || nodeInput == null)
			{
				Debug.LogWarningFormat("Unable to find nodes for ports: {0} || {1}", output.id, input.id);
				return false;
			}
			Connection connection = new Connection(nodeOutput, output, nodeInput, input);
			connections.Add(connection);
			MarkDirty();
			FireConnectionAdded(connection);
			return true;
		}

		/// <summary>
		/// Disconnects all connections to the given port
		/// </summary>
		/// <param name="port">The port to disconnect all connections to/from</param>
		public void Disconnect(Port port)
		{
			bool changed = false;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if (connections[i].input.port == port.id || connections[i].output.port == port.id)
				{
					changed = true;
					FireConnectionRemoved(connections[i]);
					connections.RemoveAt(i);
				}
			}
			if (changed)
			{
				MarkDirty();
			}
		}

		/// <summary>
		/// Disconnects any connections involving both of these ports
		/// </summary>
		/// <param name="portA">The input/output port to disconnect</param>
		/// <param name="portB">The input/output port to disconnect</param>
		public void Disconnect(Port portA, Port portB)
		{
			bool changed = false;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if ((connections[i].input.port == portA.id && connections[i].output.port == portB.id) || (connections[i].input.port == portB.id && connections[i].output.port == portA.id))
				{
					changed = true;
					FireConnectionRemoved(connections[i]);
					connections.RemoveAt(i);
				}
			}
			if (changed)
			{
				MarkDirty();
			}
		}

		/// <summary>
		/// Given a port find and return the node which has ownership of the port
		/// </summary>
		/// <param name="port">The port to search and find the node for</param>
		/// <returns>Returns the node which has ownership of the given port</returns>
		public Node FindNodeWithPort(Port port)
		{
			foreach (var node in nodes.Values)
			{
				foreach (var item in node)
				{
					if (item.id == port.id) return node;
				}
			}
			return null;
		}
	}
}
