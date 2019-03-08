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
			AddNode(node, position);
			return node;
		}

		/// <summary>
		/// Duplicate a node, add it to the graph and return it
		/// </summary>
		/// <param name="node">The node to duplicate</param>
		/// <returns></returns>
		public Node Duplicate(Node node)
		{
			Node dup = Add(node.GetType(), node.view.layout.position + new Vector2(30, 30));
			dup.Duplicate(node);
			return dup;
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
			Node node = this[id];
			Connection connection;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				connection = connections[i];
				if (connection.input.node == id || connection.output.node == id)
				{
					connections.RemoveAt(i);
					FireConnectionRemoved(connection);
				}
			}
			nodes.Remove(id);
			FireNodeRemoved(node);
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
			if (!output.CanConnectPort(input)) return false;
			Node nodeOutput = FindNodeWithPort(output);
			Node nodeInput = FindNodeWithPort(input);
			if (nodeOutput == null || nodeInput == null)
			{
				Debug.LogWarningFormat("Unable to find nodes for ports: {0} || {1}", output.id, input.id);
				return false;
			}
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if ((connections[i].input.port == input.id)) RemoveConnection(i);
			}
			AddConnection(new Connection(nodeOutput, output, nodeInput, input));
			return true;
		}

		/// <summary>
		/// Disconnects all connections to the given port
		/// </summary>
		/// <param name="port">The port to disconnect all connections to/from</param>
		public void Disconnect(Port port)
		{
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if (connections[i].input.port == port.id || connections[i].output.port == port.id)
				{
					RemoveConnection(i);
				}
			}
		}

		/// <summary>
		/// Disconnects any connections involving both of these ports
		/// </summary>
		/// <param name="portA">The input/output port to disconnect</param>
		/// <param name="portB">The input/output port to disconnect</param>
		public void Disconnect(Port portA, Port portB)
		{
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if ((connections[i].input.port == portA.id && connections[i].output.port == portB.id) || (connections[i].input.port == portB.id && connections[i].output.port == portA.id))
				{
					RemoveConnection(i);
				}
			}
		}

		/// <summary>
		/// Disconnects any connections involving both of these port ids
		/// </summary>
		/// <param name="portA">The input/output port id to disconnect</param>
		/// <param name="portB">The input/output port id to disconnect</param>
		public void Disconnect(Guid portA, Guid portB)
		{
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if ((connections[i].input.port == portA && connections[i].output.port == portB) || (connections[i].input.port == portB && connections[i].output.port == portA))
				{
					RemoveConnection(i);
				}
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

		/// <summary>
		/// Given a port find and return all the connections its involved in
		/// </summary>
		/// <param name="port">the port to search and find the connections for</param>
		/// <param name="isInput">if true the port will be treated as an input port</param>
		/// <returns>Returns an enumerable of connections this port was involved in</returns>
		public IEnumerable<Connection> FindConnectionsWithPort(Port port, bool isInput)
		{
			foreach (var connection in connections)
			{
				if ((isInput && connection.input.port == port.id) || (connection.output.port == port.id)) yield return connection;
			}
		}

		/// <summary>
		/// Returns true if the given port is still connected
		/// </summary>
		/// <param name="port">the port to search for in the connections</param>
		/// <param name="isInput">if true treat the port as an input port</param>
		/// <returns></returns>
		public bool IsConnected(Port port, bool isInput)
		{
			return IsConnected(port.id, isInput);
		}

		/// <summary>
		/// Returns true if the given port is still connected
		/// </summary>
		/// <param name="port">the port to search for in the connections</param>
		/// <param name="isInput">if true treat the port as an input port</param>
		/// <returns></returns>
		public bool IsConnected(Guid port, bool isInput)
		{
			foreach (var connection in connections)
			{
				if ((isInput && connection.input.port == port) || connection.output.port == port) return true;
			}
			return false;
		}
	}
}
