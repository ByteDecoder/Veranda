using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Editor
{
    public struct EditorNode
    {
        // Node drawer needs editor to recalculate positions
        [HideInInspector] public readonly GraphEditor Editor;
        [HideInInspector] public readonly Node Content;

        public EditorNode(GraphEditor editor, Node node)
        {
            Editor = editor;
            Content = node;
        }
        
        [ShowInInspector, HideLabel, HideReferenceObjectPicker]
        private object Value
        {
            get
            {
                if (Content.ValueGetter != null)
                    return Content.ValueGetter();
                Debug.LogError("Value getter is not set.");
                return null;
            }
            set
            {
                if (Content.ValueSetter != null)
                {
                    Content.ValueSetter(value);
                    return;
                }
                Debug.LogError("Value setter is not set.");
            }
        }
    }
}