using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sleipnir.Graph.Attributes;

namespace Sleipnir.Graph
{
    public abstract class BaseNode
    {
        public abstract Knob[] GetKnobs(string fieldName, int index);
        public abstract Tuple<string, int> GetKnobInfo(Knob knob);

        protected Dictionary<Knob, Tuple<string, int>> Knobs;
    }

    public abstract class Node<TContent> : BaseNode
    {
        [HideLabel, HideReferenceObjectPicker, ShowInInspector, 
            Sirenix.OdinInspector.OnValueChanged("OnValueUpdate", true)]
        public abstract TContent Content { get; set; }

#if UNITY_EDITOR
        #region Sleipnir data
        public abstract Node EditorNode { get; set; }

        private Action[] _onKnobUpdate;
        private Action[] _onValueUpdate;

        public void LoadStartingData()
        {
            var attributes = Content.GetType().GetCustomAttributes().ToArray();

            if (attributes.Any(a => a.GetType() == typeof(NodeWidth)))
            {
                var width = (NodeWidth)attributes.First(a => a.GetType() == typeof(NodeWidth));
                EditorNode.NodeWidth = width.Width;
            }

            if (attributes.Any(a => a.GetType() == typeof(Title)))
            {
                var title = (Title)attributes.First(a => a.GetType() == typeof(Title));
                EditorNode.HeaderTitle = title.Text;
            }

            if (attributes.Any(a => a.GetType() == typeof(TitleColor)))
            {
                var titleColor = (TitleColor)attributes.First(a => a.GetType() == typeof(TitleColor));
                EditorNode.TitleColor = titleColor.Color;
            }

            if (attributes.Any(a => a.GetType() == typeof(HeaderColor)))
            {
                var headerColor = (HeaderColor)attributes.First(a => a.GetType() == typeof(HeaderColor));
                EditorNode.HeaderColor = headerColor.Color;
            }

            if (attributes.Any(a => a.GetType() == typeof(LabelSlider)))
            {
                var labelSlider = (LabelSlider)attributes.First(a => a.GetType() == typeof(LabelSlider));
                EditorNode.HasLabelSlider = true;
                EditorNode.IsLabelSliderShown = labelSlider.IsShown;
            }
            LoadKnobs();
        }

        public void OnValueUpdate()
        {
            foreach (var action in _onValueUpdate)
                action?.Invoke();
            UpdateKnobs();
        }

        public void UpdateKnobs()
        {
            LoadKnobs();
            foreach (var action in _onKnobUpdate)
                action?.Invoke();
        }

        public void LoadKnobs()
        {
            EditorNode.Knobs = new List<Knob>();
            Knobs = new Dictionary<Knob, Tuple<string, int>>();

            var objectKnob = (Attributes.Knob)Content.GetType()
                .GetCustomAttribute(typeof(Attributes.Knob));

            if (objectKnob != null)
            {
                if (objectKnob.Type == KnobType.Input || objectKnob.Type == KnobType.Both)
                {
                    var editorKnob = new Knob(18f, Sleipnir.KnobType.Input)
                    {
                        Color = objectKnob.Color,
                        Description = objectKnob.Description
                    };
                    EditorNode.Knobs.Add(editorKnob);
                    Knobs.Add(editorKnob, new Tuple<string, int>(null, -1));
                }
                if (objectKnob.Type == KnobType.Output || objectKnob.Type == KnobType.Both)
                {
                    var editorKnob = new Knob(18f, Sleipnir.KnobType.Output)
                    {
                        Color = objectKnob.Color,
                        Description = objectKnob.Description
                    };
                    EditorNode.Knobs.Add(editorKnob);
                    Knobs.Add(editorKnob, new Tuple<string, int>(null, -1));
                }
            }

            var fieldsKnobs = Content.GetType()
                .GetFields()
                .Where(m => m.GetCustomAttributes(typeof(Attributes.Knob)).Any())
                .Select(k => new Tuple<FieldInfo, Attributes.Knob>(k,
                    (Attributes.Knob)k.GetCustomAttribute(typeof(Attributes.Knob), true)));
            
            foreach (var knob in fieldsKnobs)
            {
                if (knob.Item2.Type == KnobType.Input || knob.Item2.Type == KnobType.Both)
                {
                    var editorKnob = new Knob(0, Sleipnir.KnobType.Input)
                    {
                        Color = knob.Item2.Color,
                        Description = knob.Item2.Description
                    };
                    EditorNode.Knobs.Add(editorKnob);
                    Knobs.Add(editorKnob, new Tuple<string, int>(knob.Item1.Name, -1));
                }

                if (knob.Item2.Type == KnobType.Output || knob.Item2.Type == KnobType.Both)
                {
                    var editorKnob = new Knob(0, Sleipnir.KnobType.Output)
                    {
                        Color = knob.Item2.Color,
                        Description = knob.Item2.Description
                    };
                    EditorNode.Knobs.Add(editorKnob);
                    Knobs.Add(editorKnob, new Tuple<string, int>(knob.Item1.Name, -1));
                }
            }

            var multiFields = Content.GetType()
                .GetFields()
                .Where(m => m.GetCustomAttributes(typeof(MultiKnob)).Any())
                .Select(k => new Tuple<FieldInfo, MultiKnob>(k,
                    (MultiKnob)k.GetCustomAttribute(typeof(MultiKnob), true)));
            
            
            foreach (var knob in multiFields)
            { 
                var index = 0;
                foreach (var unused in (IEnumerable)knob.Item1.GetValue(Content))
                {
                    if (knob.Item2.Type == KnobType.Input || knob.Item2.Type == KnobType.Both)
                    {
                        var editorKnob = new Knob(0, Sleipnir.KnobType.Input)
                        {
                            Color = knob.Item2.Color,
                            Description = knob.Item2.Description
                        };
                        EditorNode.Knobs.Add(editorKnob);
                        Knobs.Add(editorKnob, new Tuple<string, int>(knob.Item1.Name, index));
                    }
                    if (knob.Item2.Type == KnobType.Output || knob.Item2.Type == KnobType.Both)
                    {
                        var editorKnob = new Knob(0, Sleipnir.KnobType.Output)
                        {
                            Color = knob.Item2.Color,
                            Description = knob.Item2.Description
                        };
                        EditorNode.Knobs.Add(editorKnob);
                        Knobs.Add(editorKnob, new Tuple<string, int>(knob.Item1.Name, index));
                    }
                    index++;
                }
            }
        }

        public void AddNodeDelegates()
        {
            _onValueUpdate = GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OnValueChanged), false).Length > 0)
                .Select<MethodInfo, Action>(m => () => m.Invoke(this, new object[] { }))
                .ToArray();

            _onKnobUpdate = GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(OnKnobUpdate), false).Length > 0)
                .Select<MethodInfo, Action>(m => () => m.Invoke(this, new object[] { }))
                .ToArray();

            var nodeContextMethods = Content.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ContextFunction), false).Length > 0)
                .Select(m => new Tuple<string, Action>
                    (m.Name, () => m.Invoke(Content, new object[] { })));

            foreach (var nodeContextMethod in nodeContextMethods)
                EditorNode.ContextMenuFunctions.Add(nodeContextMethod);

            LoadKnobs();
        }

        public override Knob[] GetKnobs(string fieldName, int index)
        {
            return Knobs.Where(o => o.Value.Item1 == fieldName && o.Value.Item2 == index)
                .Select(o => o.Key)
                .ToArray();
        }

        public override Tuple<string, int> GetKnobInfo(Knob knob)
        {
            return Knobs[knob];
        }
        #endregion
#endif
    }
}