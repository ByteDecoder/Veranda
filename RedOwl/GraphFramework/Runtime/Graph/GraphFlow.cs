using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.GraphFramework
{
	public abstract partial class Graph
	{
		private static List<Guid> evaluatedNodes;
		
		protected override void InternalExecute()
		{
			evaluatedNodes = new List<Guid>(this.Count);
			var maxIterations = this.Count + 1;
			for (var i = 1; i <= maxIterations; i++)
				if (ExecuteLoop(i, this.ToArray())) break;
		}

		private bool ExecuteLoop(int iteration, Node[] nodes)
		{
			var count = this.Count;
			foreach (Node node in nodes)
			{
				if (evaluatedNodes.Contains(node.id)) continue;
				if (CanEvaluate(node, iteration))
					ExecuteNode(node);
			}
			//Debug.LogFormat("On loop {0} Nodes Evaluated: {1}/{2}", iteration, evaluatedNodes.Count, count);
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
				{
					if (connection.input.node == node.id) return false;
				}
			} else {
				canEvaluate = false;
				// When this is not the first iteration
				// Only allow evaluation of a node if 
				// all of the upstream output connection nodes have already evaluated
				foreach (var connection in this.connections)
				{
					if (connection.input.node == node.id && evaluatedNodes.Contains(connection.output.node)) return true;
				}
			}
			return canEvaluate;
		}

		private void ExecuteNode(Node node)
		{
			node.OnExecute();
			foreach (var connection in this.connections)
			{
				if (connection.output.node == node.id)
				{
					//Debug.LogFormat("     Shelping: {0} | {1} => {2}", this[connection.output.node], this[connection.output.node][connection.output.port].id, this[connection.input.node][connection.input.port].id);
					this[connection.input.node].GetPort(connection.input.port).data = this[connection.output.node].GetPort(connection.output.port).data;
				}
			}
			evaluatedNodes.Add(node.id);
		}
	}
}
