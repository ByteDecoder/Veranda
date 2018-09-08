using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sleipnir.Graph.Attributes;

namespace Sleipnir.Graph
{
    public abstract class BaseNode
    {
        public abstract Knob[] GetKnobs(string fieldName);
    }

    public abstract class Node<TContent> : BaseNode
    {
        [HideLabel, HideReferenceObjectPicker, ShowInInspector]
        public abstract TContent Content { get; set; }

#if UNITY_EDITOR
        #region Sleipnir data
        public abstract Node EditorNode { get; set; }

        private Action[] _onKnobUpdate;

        public void LoadStartingData()
        {
            var attributes = Content.GetType().GetCustomAttributes().ToArray();

            if (attributes.Any(a => a.GetType() == typeof(NodeWidth)))
            {
                var width = (NodeWidth) attributes.First(a => a.GetType() == typeof(NodeWidth));
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
        }

        public void UpdateKnobs()
        {
            foreach (var action in _onKnobUpdate)
                action?.Invoke();;
        }

        public void LoadKnobs()
        {
            EditorNode.Knobs = new List<Knob>();

            var objectKnob = (Attributes.Knob)Content.GetType()
                .GetCustomAttribute(typeof(Attributes.Knob));

            if (objectKnob != null)
            {
                if (objectKnob.Type == KnobType.Input || objectKnob.Type == KnobType.Both)
                {
                    var editorKnob = new Knob(18f, Sleipnir.KnobType.Input);
                    EditorNode.Knobs.Add(editorKnob);
                }
                if (objectKnob.Type == KnobType.Output || objectKnob.Type == KnobType.Both)
                {
                    var editorKnob = new Knob(18f, Sleipnir.KnobType.Output);
                    EditorNode.Knobs.Add(editorKnob);
                }
            }

            var fieldsKnobs = Content.GetType()
                .GetFields()
                .Where(m => m.GetCustomAttributes(typeof(Attributes.Knob)).Any())
                .Select(k => new Tuple<FieldInfo, Attributes.Knob>(k,
                    (Attributes.Knob)k.GetCustomAttribute(typeof(Attributes.Knob), true)));

            var inHeight = 50;
            var outHeight = 50;
            foreach (var knob in fieldsKnobs)
            {
                if (knob.Item2.Type == KnobType.Input || knob.Item2.Type == KnobType.Both)
                {
                    EditorNode.Knobs.Add(new Knob(inHeight, Sleipnir.KnobType.Input));
                    inHeight += 24;
                }
                if (knob.Item2.Type == KnobType.Output || knob.Item2.Type == KnobType.Both)
                {
                    outHeight += 24;
                    EditorNode.Knobs.Add(new Knob(outHeight, Sleipnir.KnobType.Output));
                };
            }
        }

        public void AddNodeDelegates()
        {
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

        }

        public override Knob[] GetKnobs(string fieldName)
        {
            var objectKnob = (Attributes.Knob)Content.GetType()
                .GetCustomAttribute(typeof(Attributes.Knob));
            
            if (fieldName.IsNullOrWhitespace())
            {
                if(objectKnob == null)
                    return new Knob[] { };

                return objectKnob.Type == KnobType.Both 
                    ? new[] { EditorNode.Knobs[0], EditorNode.Knobs[1] }
                    : new[] { EditorNode.Knobs[0]};
            }

            var knobIndex = 0;
            if (objectKnob != null)
            {
                knobIndex = objectKnob.Type == KnobType.Both
                    ? 2
                    : 1;
            }

            var orderedFieldsKnobs = Content.GetType()
                .GetFields()
                .Where(m => m.GetCustomAttributes(typeof(Attributes.Knob)).Any())
                .OrderBy(field => field.MetadataToken);

            foreach (var knobField in orderedFieldsKnobs)
            {
                var attribute = (Attributes.Knob)knobField.GetCustomAttribute(typeof(Attributes.Knob));
                var knobType = attribute.Type;
                if (knobField.Name == fieldName)
                {
                    return knobType == KnobType.Both 
                        ? new[] { EditorNode.Knobs[knobIndex], EditorNode.Knobs[knobIndex + 1] } 
                        : new[] { EditorNode.Knobs[knobIndex] };
                }
                knobIndex = knobType == KnobType.Both
                    ? knobIndex + 2
                    : knobIndex + 1;
            }

            return new Knob[] { };
        }
        #endregion
#endif
    }
}