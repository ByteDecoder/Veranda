using System.Linq;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private void CreateNode(string id, Vector2 gridPosition)
        {
            var newNode = _graph.CreateNode(id, gridPosition);
            if(newNode != null)
                _graph.Nodes.Add(newNode);
        }

        private void CreateConnection(Knob selectedOutput, Knob selectedInput)
        {
            _graph.AddConnection(selectedOutput, selectedInput);
        }

        private void RemoveNode(Node node)
        {
            var graphNodes = _graph.Nodes;
            var toRemoveIndex = graphNodes.IndexOf(node);

            if (!_graph.RemoveNode(node))
                return;
            
            var connectionsToRemove = _graph.Connections()
                .Where(o => node.Knobs.Contains(o.InputKnob)
                         || node.Knobs.Contains(o.OutputKnob))
                .ToArray();

            foreach (var connection in connectionsToRemove)
                RemoveConnection(connection);

            if (toRemoveIndex + 1 < graphNodes.Count)
                graphNodes[toRemoveIndex + 1].NumberOfPrecedingDummies++;

            graphNodes.Remove(node);
        }

        private void RemoveConnection(Connection connection)
        {
            _graph.RemoveConnection(connection);
        }
    }
}