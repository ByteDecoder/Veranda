using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir
{
    public class Node
    {
#if UNITY_EDITOR
        [HideInInspector]
        public Func<object> ValueGetter;
        [HideInInspector]
        public Action<object> ValueSetter;
        [HideInInspector]
        public List<Slot> Slots;
        
        [HideInInspector]
        public readonly SerializedNodeData SerializedNodeData;

        [HideInInspector]
        public Color HeaderColor;
        [HideInInspector]
        public Color TitleColor;
        [HideInInspector]
        public string HeaderTitle;
        [HideInInspector]
        public bool HasLabelSlider;
        [HideInInspector]
        public List<Tuple<string, Action>> ContextMenuFunctions;

        public Node(Func<object> valueGetter, Action<object> valueSetter, 
            SerializedNodeData serializedNodeData)
        {
            ValueGetter = valueGetter;
            ValueSetter = valueSetter;
            Slots = new List<Slot>();

            SerializedNodeData = serializedNodeData;
            
            HeaderColor = new Color(0.95f, 0.38f, 0.24f);
            TitleColor = Color.black;
            HasLabelSlider = true;
            HeaderTitle = Value.GetType().FullName;
        }

        [ShowInInspector]
        [HideLabel]
        [HideReferenceObjectPicker]
        private object Value
        {
            get
            {
                if (ValueGetter != null)
                    return ValueGetter();
                Debug.LogError("Value getter is not set.");
                return null;
            }
            set
            {
                if (ValueSetter != null)
                {
                    ValueSetter(value);
                    return;
                }
                Debug.LogError("Value setter is not set.");
            }
        }
#endif
    }
}