using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sleipnir.Graph.Attributes;
using UnityEngine;

namespace Sleipnir.Graph
{
    public abstract class GraphNode
    {
        public abstract IEnumerable<Knob> GetKnobs(string fieldName, int index);
    }

    public abstract class GraphNode<TContent> : GraphNode
    {
        [HideLabel]
        [HideReferenceObjectPicker]
        [ShowInInspector]
        [OnValueChanged("OnValueChange", true)]
        public abstract TContent Content { get; set; }

#if UNITY_EDITOR
        #region Sleipnir data
        private const float HeaderKnobYPosition = 18f;
        private const string InvalidParametersMessage =
            "Invalid parameter. Functions with node attributes can only be" +
            " parametless or with a sinlge parameter of type Node.";
        
        public abstract Node EditorNode { get; set; }

        private Dictionary<Knob, Tuple<string, int>> _knobs;
        private List<Action> _onDraw = new List<Action>();
        private List<Action> _onConnectionsUpdate = new List<Action>();
        private List<Action> _onValueChange = new List<Action>();

        public void LoadVisuals()
        {
            var attributes = Content.GetType().GetCustomAttributes().ToArray();

            ProcessOptionalAttribute<NodeWidth>(attributes, 
                attribute => EditorNode.NodeWidth = attribute.Width);

            ProcessOptionalAttribute<Title>(attributes,
                attribute => EditorNode.HeaderTitle = attribute.Text);

            ProcessOptionalAttribute<TitleColor>(attributes,
                attribute => EditorNode.TitleColor = attribute.Color);

            ProcessOptionalAttribute<HeaderColor>(attributes,
                attribute => EditorNode.HeaderColor = attribute.Color);

            ProcessOptionalAttribute<LabelWidth>(attributes,
                attribute => EditorNode.LabelWidth = attribute.Width);

            ProcessOptionalAttribute<LabelSlider>(attributes, attribute =>
                {
                    EditorNode.HasLabelSlider = true;
                    EditorNode.IsLabelSliderShown = attribute.IsShown;
                });
        }

        private static void ProcessOptionalAttribute<T>
            (IEnumerable<Attribute> attributes, Action<T> onFound) where T : Attribute
        {
            var attribute = (T)attributes.FirstOrDefault(a => a.GetType() == typeof(T));
            if (attribute != null)
                onFound(attribute);
        }

        public void LoadDelegates()
        {
            var contentMethods = Content.GetType().GetMethods();

            _onDraw = GetMethodsWithAttribute<OnDraw>(contentMethods);
            _onConnectionsUpdate = GetMethodsWithAttribute<OnConnectionsUpdate>(contentMethods);
            _onValueChange = GetMethodsWithAttribute<OnChanged>(contentMethods);
        }

        private List<Action> GetMethodsWithAttribute<T>
            (IEnumerable<MethodInfo> methods) where T : Attribute
        {
            var match = methods.Where(m => m.GetCustomAttribute(typeof(T)) != null);
            var result = new List<Action>();
            foreach (var methodInfo in match)
            {
                var parameters = methodInfo.GetParameters();
                if(parameters.Length == 0)
                    result.Add(() => methodInfo.Invoke(Content, new object[] { }));
                else if(parameters.Length == 1 && parameters[0].ParameterType == typeof(Node))
                    result.Add(() => methodInfo.Invoke(Content, new object[] { EditorNode }));
                else
                    Debug.LogError(InvalidParametersMessage);
            }
            return result;
        }

        public void AddNodeContextFunctions()
        {
            var matchingMethods = Content.GetType().GetMethods()
                .Where(m => m.GetCustomAttribute<ContextFunction>() != null)
                .Select(m => 
                new Tuple<MethodInfo, ContextFunction>(m, m.GetCustomAttribute<ContextFunction>()))
                    .Where(t => t.Item2 != null);

            foreach (var tuple in matchingMethods)
            {
                var methodInfo = tuple.Item1;
                var parameters = methodInfo.GetParameters();
                var name = tuple.Item2.Name;
                name = name.IsNullOrWhitespace()
                    ? methodInfo.Name
                    : name;
                
                if (parameters.Length == 0)
                    EditorNode.ContextMenuFunctions.Add(new Tuple<string, Action>
                        (name, () => methodInfo.Invoke(Content, new object[] { })));
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Node))
                    EditorNode.ContextMenuFunctions.Add(new Tuple<string, Action>
                        (name, () => methodInfo.Invoke(Content, new object[] { EditorNode })));
                else
                    Debug.LogError(InvalidParametersMessage);
            }
        }

        private void AddKnob(string fieldName, int index, Color color, float positionY,
            Sleipnir.KnobType type, string description)
        {
            var editorKnob = new Knob(positionY, type, color, description);
            EditorNode.Knobs.Add(editorKnob);
            _knobs.Add(editorKnob, new Tuple<string, int>(fieldName, index));
        }

        private void AddKnob(string fieldName, int index, Attributes.Knob attribute,
            float positionY = 0)
        {
            if (attribute.Type == KnobType.Input || attribute.Type == KnobType.Both)
                AddKnob(fieldName, index, attribute.Color, 
                    positionY, Sleipnir.KnobType.Input, attribute.Description);

            if (attribute.Type == KnobType.Output || attribute.Type == KnobType.Both)
                AddKnob(fieldName, index, attribute.Color,
                    positionY, Sleipnir.KnobType.Output, attribute.Description);
        }

        public void LoadKnobs()
        {
            EditorNode.Knobs = new List<Knob>();
            _knobs = new Dictionary<Knob, Tuple<string, int>>();

            LoadHeaderKnobs();
            LoadFieldKnobs();
            LoadCollectionKnobs();

            OnConnectionUpdate();
        }

        private void LoadHeaderKnobs()
        {
            var objectKnob = Content.GetType().GetCustomAttribute<HeaderKnob>();

            if (objectKnob != null)
                AddKnob(null, -1, objectKnob, HeaderKnobYPosition);
        }
        
        private void LoadFieldKnobs()
        {
            var fieldsKnobs = Content.GetType().GetFields()
                .Select(f => new Tuple<FieldInfo, FieldKnob>
                    (f, (FieldKnob)f.GetCustomAttribute(typeof(FieldKnob))))
                .Where(t => t.Item2 != null);

            foreach (var knob in fieldsKnobs)
            {
                var name = knob.Item1.Name;
                var attribute = knob.Item2;
                AddKnob(name, -1, attribute);
            }
        }

        private void LoadCollectionKnobs()
        {
            var multiFields = Content.GetType().GetFields()
                .Select(f => new Tuple<FieldInfo, CollectionKnob>
                    (f, (CollectionKnob)f.GetCustomAttribute(typeof(CollectionKnob))))
                .Where(t => t.Item2 != null);

            foreach (var knob in multiFields)
            {
                var index = 0;
                var name = knob.Item1.Name;
                var attribute = knob.Item2;
                foreach (var unused in (IEnumerable)knob.Item1.GetValue(Content))
                {
                    AddKnob(name, index, attribute);
                    index++;
                }
            }
        }

        public void OnValueChange()
        {
            LoadKnobs();
            foreach (var action in _onValueChange)
                action?.Invoke();
        }

        public void OnConnectionUpdate()
        {
            foreach (var action in _onConnectionsUpdate)
                action?.Invoke();
        }
        
        public void OnDraw()
        {
            foreach (var action in _onDraw)
                action.Invoke();
        }
        
        public override IEnumerable<Knob> GetKnobs(string fieldName, int index)
        {
            return _knobs?.Where(o => o.Value.Item1 == fieldName && o.Value.Item2 == index)
                .Select(o => o.Key)
                .ToArray();
        }

        public Tuple<string, int> GetKnobInfo(Knob knob)
        {
            return _knobs.ContainsKey(knob) 
                ? _knobs[knob] 
                : null;
        }
        #endregion
#endif
    }
}