using System.Linq;
using UnityEngine;

namespace Sleipnir.Editor
{        
    public partial class GraphEditor
    {
        private void CreateNode(string id, Vector2 gridPosition)
        {
            var newNode = _graph.AddNode(id);
            if (newNode == null)
                return;

            newNode.Position = gridPosition;
            var graphNode = new Node(this, newNode);
            Nodes.Add(graphNode);
        }
        
        private void CreateConnection(IKnob selectedOutput, IKnob selectedInput)
        {
            var newConnection = _graph.AddConnection(selectedOutput, selectedInput);
            if (newConnection == null)
                return;

            var graphConnection = new Connection(newConnection, this,
                GetKnob(selectedOutput), GetKnob(selectedInput));
            Connections.Add(graphConnection);
        }
        
        public void OnKnobClick(Knob knob)
        {
            if (!_selectedKnob.HasValue ||
                _selectedKnob.Value.Content.Type == knob.Content.Type)
                _selectedKnob = knob;
            else
            {
                if(knob.Content.Type == KnobType.Input)
                    CreateConnection(_selectedKnob.Value.Content, knob.Content);
                else
                    CreateConnection(knob.Content, _selectedKnob.Value.Content);
                _selectedKnob = null;
            }
        }

        private void RemoveNode(Node node)
        {
            var connectionsToRemove = Connections
                .Where(o => node.Knobs.Contains(o.InputKnob) || node.Knobs.Contains(o.OutputKnob)).ToArray();
            foreach (var connection in connectionsToRemove)
                RemoveConnection(connection);
            if (_graph.RemoveNode(node.Content))
                Nodes.Remove(node);
        }

        public void RemoveConnection(Connection connection)
        {
            if (_graph.RemoveConnection(connection.Content))
                Connections.Remove(connection);
        }

        private void MoveNodeToFront(Node node)
        {
            _graph.MoveNodeToFront(node.Content);
            Nodes.Remove(node);
            Nodes.Add(node);
        }

        public void MoveConnectionToFront(Connection connection)
        {
            _graph.MoveConnectionToFront(connection.Content);
        }
    }
}