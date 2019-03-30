using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedOwl.GraphFramework
{
	public abstract partial class Graph
	{
		public override void Clear()
		{
			AutoExecute = false;
			_connections.Clear();
			base.Clear();
		}

		/// <summary>
		/// Creates and returns a node of type T and adds it to the graph
		/// </summary>
		/// <typeparam name="T">Node Type to instantiate</typeparam>
		/// <returns></returns>
		public T AddNode<T>() where T : Node
		{
			return (T)AddNode(typeof(T), Vector2.zero);
		}

		/// <summary>
		/// Creates and returns a node of type T and adds it to the graph at the specified x,y position
		/// </summary>
		/// <param name="x">The x position of the node</param>
		/// <param name="y">The y position of the node</param>
		/// <typeparam name="T">Node Type to instantiate</typeparam>
		/// <returns></returns>
		public T AddNode<T>(float x, float y) where T : Node
		{
			return (T)AddNode(typeof(T), new Vector2(x, y));
		}

		/// <summary>
		/// Creates and returns a node of type T and adds it to the graph at the specified position
		/// </summary>
		/// <param name="position">The position in graph space to place the node</param>
		/// <typeparam name="T">Node Type to instantiate</typeparam>
		/// <returns></returns>
		public T AddNode<T>(Vector2 position) where T : Node
		{
			return (T)AddNode(typeof(T), position);
		}

		internal Node AddNode(Type nodeType, Vector2 position)
		{
			Node node = (Node)CreateInstance(nodeType);
			node.view.collapsed = false;
			node.view.layout = new Rect(position.x, position.y, 150, 0);
			AddChild(node);
			return node;
		}

		/// <summary>
		/// Duplicate a node, add it to the graph and return it
		/// </summary>
		/// <param name="node">The node to duplicate</param>
		/// <returns></returns>
		public Node DuplicateNode(Node node)
		{
			Node dup = AddNode(node.GetType(), node.view.layout.position + new Vector2(30, 30));
			dup.Duplicate(node);
			return dup;
		}

		/// <summary>
		/// Removes the node from the graph with the given id
		/// </summary>
		/// <param name="id">The idea of the node to remove</param>
		public void RemoveNode(Guid id)
		{
			RemoveNode(this[id]);
		}

		/// <summary>
		/// Removes the node from the graph
		/// </summary>
		/// <param name="node">Node to remove</param>
		public void RemoveNode<T>(T node) where T : Node
		{
			foreach (var connection in connections)
			{
				if (connection.input.node == node.id || connection.output.node == node.id)
					RemoveConnection(connection);
			}
            if (parent != null && typeof(Graph).IsAssignableFrom(parent.GetType()))
            {
				Graph parentGraph = parent as Graph;
				foreach (var port in node.ports)
				{
					parentGraph.Disconnect(port);
				}
            }
			Graph subGraph = node as Graph;
			if (subGraph != null)
			{
				foreach (var subNode in subGraph.children.Values.ToArray())
				{
					subGraph.RemoveNode(subNode);
				}
			}
			RemoveChild(node);
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
			foreach (var connection in connections)
			{
				if ((connection.input.port == input.id))
					RemoveConnection(connection);
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
			foreach (var connection in connections)
			{
				if (connection.input.port == port.id || connection.output.port == port.id)
					RemoveConnection(connection);
			}
		}

		/// <summary>
		/// Disconnects the connections to a given port with respect to only one side of the connection
		/// </summary>
		/// <param name="port">the port to disconnect to/from</param>
		/// <param name="isInput">if true treats the port as an input port to limit disconnecting only one side of an InOut port</param>
		public void Disconnect(Port port, bool isInput)
		{
			foreach (var connection in connections)
			{
				if ((isInput && connection.input.port == port.id) || (!isInput && connection.output.port == port.id))
					RemoveConnection(connection);
			}
		}

		/// <summary>
		/// Disconnects any connections involving both of these ports
		/// </summary>
		/// <param name="portA">The input/output port to disconnect</param>
		/// <param name="portB">The input/output port to disconnect</param>
		public void Disconnect(Port portA, Port portB)
		{
			Disconnect(portA.id, portB.id);
		}

		/// <summary>
		/// Disconnects any connections involving both of these port ids
		/// </summary>
		/// <param name="portA">The input/output port id to disconnect</param>
		/// <param name="portB">The input/output port id to disconnect</param>
		public void Disconnect(Guid portA, Guid portB)
		{
			foreach (var connection in connections)
			{
				if ((connection.input.port == portA && connection.output.port == portB) || (connection.input.port == portB && connection.output.port == portA))
					RemoveConnection(connection);
			}
		}

		/// <summary>
		/// Given a port find and return the node which has ownership of the port
		/// </summary>
		/// <param name="port">The port to search and find the node for</param>
		/// <returns>Returns the node which has ownership of the given port</returns>
		public Node FindNodeWithPort(Port port)
		{
			foreach (var node in this)
			{
				foreach (var item in node.ports)
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
				if ((isInput && connection.input.port == port) || (!isInput && connection.output.port == port)) return true;
			}
			return false;
		}
	}
}
