using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{        
    public partial class GraphEditor : OdinEditorWindow
    {
        private IGraph _graph;
        public List<Connection> Connections;
        public List<Node> Nodes;

        public void LoadGraph(IGraph graph)
        {
            _graph = graph;
            Remap();
        }

        public void Remap()
        { 
            Nodes = new List<Node>();
            Connections = new List<Connection>();

            if (_graph == null)
                return;

            foreach (var graphNode in _graph.NodesInDrawingOrder())
                Nodes.Add(new Node(this, graphNode));

            foreach (var connection in _graph.ConnectionsInDrawingOrder())
                Connections.Add(new Connection(connection, this,
                    GetKnob(connection.OutputKnob), GetKnob(connection.InputKnob)));
        }

        private Knob GetKnob(IKnob obj)
        {
            return Nodes
                .SelectMany(n => n.Knobs)
                .First(knob => ReferenceEquals(knob.Content, obj));
        }

        protected override void DrawEditor(int index)
        {
            if (_graph == null)
                return;

            var matrix = GUI.matrix;
            ProcessInput();
            DrawGrid();
            BeginZoomed();

            foreach (var connection in Connections)
                 connection.DrawLine();

            if (_selectedKnob.HasValue)
            {
                var knobPosition = GridToGuiPositionNoClip(_selectedKnob.Value.Rect.center);
                var mousePosition = Event.current.mousePosition;

                if(_selectedKnob.Value.Content.Type == KnobType.Output)
                    Connection.DrawLine(knobPosition, mousePosition, _selectedKnob.Value.Color);
                else
                    Connection.DrawLine(mousePosition, knobPosition, _selectedKnob.Value.Color);
            }

            base.DrawEditor(index);
            EndZoomed();
            GUI.matrix = matrix;
        }
    }
}