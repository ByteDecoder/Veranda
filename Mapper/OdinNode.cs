using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Mapper
{
    [Serializable]
    public class OdinNode<T>
    {
        [OnValueChanged("Remap", true)]
        [HideReferenceObjectPicker]
        [HideLabel]
        public T Value;

        public OdinNode(T value)
        {
            Value = value;
        }

#if UNITY_EDITOR
        #region Sleipnir data
        [NonSerialized]
        public Dictionary<OdinSlot, Slot[]> Slots;
        [NonSerialized]
        public bool HasEvaluated = false;
        [NonSerialized]
        public bool CanEvaluate = false;
        [NonSerialized]
        public Nest Nest;

        [NonSerialized] public List<OdinNode<T>> Graph;
        [NonSerialized] public Node Node;

        [SerializeField, HideInInspector]
        private readonly SerializedNodeData _serializedData = new SerializedNodeData();

        private Node _nodeDrawingData;

        public Node NodeDrawingData(List<OdinNode<T>> graph) => 
            _nodeDrawingData ?? (_nodeDrawingData = GetDrawingData(graph));

        public Node GetDrawingData(List<OdinNode<T>> graph)
        {
            if(Slots == null)
                Slots = new Dictionary<OdinSlot, Slot[]>();
            var node = new Node(
                () => this,
                value => Value = ((OdinNode<T>)value).Value,
                _serializedData
                );
            Node = node;
            Graph = graph;
            Remap();
            return node;
        }
        
        public void Remap()
        {
            Nest = new Nest(Value, "");
            var mapped = Nest.GetSlots<T>(Graph.IndexOf(this), "");

            Slots = Slots
                .Where(s => mapped.Any(m => m.Item1.DeepReflectionPath == s.Key.DeepReflectionPath))
                .ToDictionary(s => s.Key, s => s.Value);

            foreach (var odinSlot in mapped)
                if (Slots.All(k => k.Key.DeepReflectionPath != odinSlot.Item1.DeepReflectionPath))
                {
                    if((odinSlot.Item2.Direction & Direction.InOut) != Direction.InOut)
                        Slots.Add(odinSlot.Item1, new []
                        {
                            new Slot(SlotDirection.Input, Node, 0),
                            new Slot(SlotDirection.Output, Node, 0)
                        });
                    else
                    {
                        var direction = odinSlot.Item2.Direction.IsOutput()
                            ? SlotDirection.Output
                            : SlotDirection.Input;
                        Slots.Add(odinSlot.Item1, new[]
                        {
                            new Slot(direction, Node, 0)
                        });
                    }
                }
            Node.Slots = Slots.SelectMany(s => s.Value).ToList();
        }
        #endregion
#endif
    }
}