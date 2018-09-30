using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Sleipnir.Mapper
{
    [Serializable]
    public class OdinNode<T>
    {
        [OnValueChanged("EvaluateGraph", true)]
        [HideReferenceObjectPicker]
        [HideLabel]
        [OdinSerialize]
        public T Value;

        public OdinNode(T value, List<OdinNode<T>> graph, OdinGraph<T> _graph)
        {
            Value = value;
#if UNITY_EDITOR
            #region Sleipnir data
            LoadDrawingData(graph, _graph);

            var attributes = value.GetType().GetCustomAttributes(true);

            if (attributes.Any(a => a.GetType() == typeof(NodeWidthAttribute)))
            {
                var width = (NodeWidthAttribute)attributes.First(a => a.GetType() == typeof(NodeWidthAttribute));
                var gridRect = Node.SerializedNodeData.GridRect;
                gridRect.width = width.Width;
                Node.SerializedNodeData.GridRect = gridRect;
            }
            else
            {
                var gridRect = Node.SerializedNodeData.GridRect;
                gridRect.width = 200;
                Node.SerializedNodeData.GridRect = gridRect;
            }

            if (attributes.Any(a => a.GetType() == typeof(LabelAttribute)))
            {
                var labelSlider = (LabelAttribute)attributes
                    .First(a => a.GetType() == typeof(LabelAttribute));
                Node.SerializedNodeData.LabelWidth = labelSlider.Width;
                Node.HasLabelSlider = labelSlider.IsShown.HasValue;
                if (labelSlider.IsShown != null)
                    Node.SerializedNodeData.IsLabelSliderShown = labelSlider.IsShown.Value;
            }
            else
                Node.SerializedNodeData.LabelWidth = Node.SerializedNodeData.GridRect.width/3;
            #endregion

#endif
        }

#if UNITY_EDITOR
        #region Sleipnir data
        [SerializeField, HideInInspector]
        private readonly SerializedNodeData _serializedData = new SerializedNodeData();

        [NonSerialized]
        public Dictionary<OdinSlot, Slot[]> Slots;
        [NonSerialized]
        public bool HasEvaluated = false;
        [NonSerialized]
        public bool CanEvaluate = false;
        [NonSerialized]
        public Nest Nest;
        [NonSerialized]
        public List<OdinNode<T>> Graph;
        [NonSerialized]
        public Node Node;
        [NonSerialized]
        private OdinGraph<T> _graph;
        private List<Action> _onDraw;
        private List<Action> _onChanged;
        
        public void LoadDrawingData(List<OdinNode<T>> graph, OdinGraph<T> __graph)
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
            _graph = __graph;
            _onDraw = new List<Action>();
            _onChanged = new List<Action>();

            var attributes = Value.GetType().GetCustomAttributes(true);
            
            if (attributes.Any(a => a.GetType() == typeof(HeaderColorAttribute)))
            {
                var headerColor = (HeaderColorAttribute)attributes
                    .First(a => a.GetType() == typeof(HeaderColorAttribute));
                Node.HeaderColor = headerColor.Color;
            }

            if (attributes.Any(a => a.GetType() == typeof(LabelAttribute)))
            {
                var labelSlider = (LabelAttribute)attributes
                    .First(a => a.GetType() == typeof(LabelAttribute));
                Node.HasLabelSlider = labelSlider.IsShown.HasValue;
            }

            if (attributes.Any(a => a.GetType() == typeof(NodeTitleAttribute)))
            {
                var title = (NodeTitleAttribute) attributes
                    .First(a => a.GetType() == typeof(NodeTitleAttribute));
                Node.HeaderTitle = title.Text;
            }
            else
                Node.HeaderTitle = Value.GetType().Name;
            
            var methods = Value.GetType().GetMethods().ToArray();
            var contextFunctions = methods
                .Where(m => m.GetCustomAttributes(typeof(ContextFunctionAttribute), false).Length > 0);

            foreach (var contextFunction in contextFunctions)
            {
                var attribute = (ContextFunctionAttribute)contextFunction
                    .GetCustomAttribute(typeof(ContextFunctionAttribute), true);
                var name = attribute.Name.IsNullOrWhitespace()
                    ? contextFunction.Name
                    : attribute.Name;

                var parameters = contextFunction.GetParameters();
                if (parameters.Length == 0)
                    Node.ContextMenuFunctions.Add(
                        new Tuple<string, Action>(name, () => contextFunction.Invoke(Value, null))
                    );
                else if (parameters.Length == 1 && typeof(Node).IsAssignableFrom(parameters[0].ParameterType))
                    Node.ContextMenuFunctions.Add(
                        new Tuple<string, Action>(name, () => contextFunction.Invoke(Value, new object[] {Node}))
                    );
            }

            var onLoadFunctions = methods
                .Where(m => m.GetCustomAttributes(typeof(OnLoadAttribute), false).Length > 0);

            foreach (var onLoadFunction in onLoadFunctions)
            {
                var parameters = onLoadFunction.GetParameters();
                if (parameters.Length == 0)
                    onLoadFunction.Invoke(Value, null);
                else if (parameters.Length == 1 && typeof(Node).IsAssignableFrom(parameters[0].ParameterType)) 
                    onLoadFunction.Invoke(Value, new object[] { Node });
            }

            var onDraw = methods
                .Where(m => m.GetCustomAttributes(typeof(OnDrawAttribute), false).Length > 0);

            foreach (var onDrawFunction in onDraw)
            {
                var parameters = onDrawFunction.GetParameters();
                if (parameters.Length == 0)
                    _onDraw.Add(() => onDrawFunction.Invoke(Value, null));
                else if (parameters.Length == 1 && typeof(Node).IsAssignableFrom(parameters[0].ParameterType))
                    _onDraw.Add(() => onDrawFunction.Invoke(Value, new object[] { Node }));
            }

            var onChanged = methods
                .Where(m => m.GetCustomAttributes(typeof(OnChangedAttribute), false).Length > 0);

            foreach (var onChangedFunction in onChanged)
            {
                var parameters = onChangedFunction.GetParameters();
                if (parameters.Length == 0)
                    _onChanged.Add(() => onChangedFunction.Invoke(Value, null));
                else if (parameters.Length == 1 && typeof(Node).IsAssignableFrom(parameters[0].ParameterType))
                    _onChanged.Add(() => onChangedFunction.Invoke(Value, new object[] { Node }));
            }

            Changed();
        }

        // Sorry for the naming, it is temporary.
        public void EvaluateGraph()
        {
            _graph.Evaluate();
        }

        public void Draw()
        {
            foreach (var action in _onDraw)
                action.Invoke();
        }

        public void Changed()
        {
            Remap();
            foreach (var action in _onChanged)
                action.Invoke();
        }
        
        public void Remap()
        {
            Nest = new Nest(Value, "");
            // -1 in case of node that wasn't yet added to graph (OdinGraph.CreateNode calls it later)
            var index = Graph.IndexOf(this) == -1
                ? Graph.Count
                : Graph.IndexOf(this);
            var mapped = Nest.GetSlots<T>(index, "");
            Slots = Slots
                .Where(s => mapped.Any(m => m.Item1.DeepReflectionPath == s.Key.DeepReflectionPath))
                .ToDictionary(s => s.Key, s => s.Value);
            
            foreach (var odinSlot in mapped)
                if (Slots.All(k => k.Key.DeepReflectionPath != odinSlot.Item1.DeepReflectionPath))
                {
                    if(odinSlot.Item2.Direction == Direction.InOut)
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