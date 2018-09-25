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
        //[OnValueChanged("Remap")]
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
        public Dictionary<OdinSlot, Slot> Slots;
        [NonSerialized]
        public bool HasEvaluated = false;
        [NonSerialized]
        public bool CanEvaluate = false;
        [NonSerialized]
        public Nest Nest;

        [SerializeField, HideInInspector]
        private readonly SerializedNodeData _serializedData = new SerializedNodeData();

        private Node _nodeDrawingData;

        public Node NodeDrawingData(List<OdinNode<T>> graph) => 
            _nodeDrawingData ?? (_nodeDrawingData = GetDrawingData(graph));

        public Node GetDrawingData(List<OdinNode<T>> graph)
        {
            if(Slots == null)
                Slots = new Dictionary<OdinSlot, Slot>();
            var node = new Node(
                () => this,
                value => Value = ((OdinNode<T>)value).Value,
                _serializedData
                );
            Remap(graph, node);
            return node;
        }

        public void StartDrawing()
        {
        }

        public void EndDrawing()
        {
        }

        public void Remap(List<OdinNode<T>> graph, Node node)
        {
            Nest = new Nest(Value, "");
            var odinSlots = Nest.GetSlots<T>(graph.IndexOf(this), "");
            foreach (var slot in Slots)
                if (!odinSlots.Contains(slot.Key))
                    Slots.Remove(slot.Key);

            foreach (var odinSlot in odinSlots)
                if (!Slots.ContainsKey(odinSlot))
                    Slots.Add(odinSlot, new Slot(SlotDirection.Input, node, 0));

            node.Slots = Slots.Values.ToList();
        }
        #endregion
#endif
    }
}