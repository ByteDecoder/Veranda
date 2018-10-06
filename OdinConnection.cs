using System;
using System.Collections.Generic;
using System.Linq;

namespace Sleipnir
{
    [Serializable]
    public class OdinConnection<T>
    {
        public readonly OdinSlot Input;
        public readonly OdinSlot Output;

        public void Shlep(OdinGraph<T> graph)
        {
            var getter = Output.Getter(graph);
            var setter = Input.Setter(graph);

            var inputNode = graph[Input.NodeIndex];
            var outputNode = graph[Output.NodeIndex];
            setter(inputNode, getter(outputNode));
        }

#if UNITY_EDITOR
        #region Sleipnir data
        public Connection ConnectionDrawingData;

        public OdinConnection(Connection connection, IEnumerable<OdinNode<T>> graph)
        {
            ConnectionDrawingData = connection;
            var slots = graph.SelectMany(n => n.Slots).ToList();
            Output = slots.First(k => ReferenceEquals(k.Value.Last(), connection.OutputSlot)).Key;
            Input = slots.First(k => ReferenceEquals(k.Value.First(), connection.InputSlot)).Key;
        }

        public void LoadDrawingData(IReadOnlyList<OdinNode<T>> graph)
        {
            var outputNode = graph[Output.NodeIndex];
            var inputNode = graph[Input.NodeIndex];

            var outputDrawingSlot = outputNode.Slots
                .First(s => s.Key.DeepReflectionPath == Output.DeepReflectionPath).Value.Last();
            var inputDrawingSlot = inputNode.Slots
                .First(s => s.Key.DeepReflectionPath == Input.DeepReflectionPath).Value.First();

            ConnectionDrawingData = new Connection(outputDrawingSlot, inputDrawingSlot);
        }
        #endregion
#endif
    }
}