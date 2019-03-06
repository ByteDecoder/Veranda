using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.GraphFramework
{
	public abstract partial class Graph
	{
		private static List<Guid> evaluatedNodes;
		
		public void Execute()
		{
			evaluatedNodes = new List<Guid>(this.Count);
			var maxIterations = this.Count + 1;
			for (var i = 1; i <= maxIterations; i++)
				if (ExecuteLoop(i)) break;
		}

		private bool ExecuteLoop(int iteration)
		{
			var count = this.Count;
			foreach (Node node in this.nodes.Values)
			{
				if (evaluatedNodes.Contains(node.id)) continue;
				if (CanEvaluate(node, iteration))
					ExecuteNode(node);
			}
			return evaluatedNodes.Count == count;
		}

		private bool CanEvaluate(Node node, int iteration)
		{
			var canEvaluate = true;
			if (iteration == 1)
			{
				// When this is the first iteration
				// Only allow evaluation of a node if the graph has no input connections for it
				foreach (var connection in this.connections)
					if (connection.input.node == node.id) canEvaluate = false;
			}
			else
			{
				// When this is not the first iteration
				// Only allow evaluation of a node if 
				// all of the upstream output connection nodes have already evaluated
				foreach (var connection in this.connections)
					if (connection.input.node == node.id && evaluatedNodes.Contains(node.id))
						canEvaluate = false;
			}

			return canEvaluate;
		}

		private void ExecuteNode(Node node)
		{
			foreach (var connection in this.connections)
				if (connection.input.node == node.id)
				{
					//Debug.LogFormat("Shelping: {0} | {1} => {2}", this[connection.output.node], this[connection.output.node][connection.output.port], this[connection.input.node][connection.input.port]);
					this[connection.input.node][connection.input.port].data = this[connection.output.node][connection.output.port].data;
				}
			node.OnExecute();
			evaluatedNodes.Add(node.id);
		}
	}
}
