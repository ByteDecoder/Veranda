using System;
using System.Collections.Generic;
using System.Linq;

namespace Sleipnir.Mapper
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
        private Connection? _connectionDrawingData;

        public OdinConnection(Connection connection, IEnumerable<OdinNode<T>> graph)
        {
            _connectionDrawingData = connection;
            var slots = graph.SelectMany(n => n.Slots).ToList();
            Output = slots.First(k => ReferenceEquals(k.Value, connection.OutputSlot)).Key;
            Input = slots.First(k => ReferenceEquals(k.Value, connection.InputSlot)).Key;
        }

        public Connection ConnectionDrawingData(IReadOnlyList<OdinNode<T>> graph)
        {
            if(!_connectionDrawingData.HasValue)
                _connectionDrawingData = GetDrawingData(graph);
            return _connectionDrawingData.Value;
        }

        private Connection GetDrawingData(IReadOnlyList<OdinNode<T>> graph)
        {
            var outputNode = graph[Output.NodeIndex];
            var inputNode = graph[Input.NodeIndex];

            var outputDrawingSlot = outputNode.Slots[Output];
            var inputDrawingSlot = inputNode.Slots[Input];

            return new Connection(outputDrawingSlot, inputDrawingSlot);
        }
        #endregion
#endif
    }
}