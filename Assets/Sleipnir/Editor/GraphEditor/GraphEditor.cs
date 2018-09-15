using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{        
    public partial class GraphEditor : OdinEditorWindow
    {
        private const float ZoomSpeed = 0.1f;
        private const float MaxZoom = 7f;

        private IGraph _graph;

        [HideInInspector]
        public List<EditorSlot> Slots;

        [HideReferenceObjectPicker, HideLabel, ShowInInspector]
        public List<EditorNode> Nodes
        {
            get
            {
                Slots = new List<EditorSlot>();
                return _graph.Nodes
                        .SelectMany(n => Enumerable.Repeat<ValueWrappedNode>(null, n.Node.NumberOfPrecedingDummies)
                            .Concat(new[] { n }))
                        .Select(n => new EditorNode(n))
                        .ToList();
            }
            set { } // without setter property is marked as readonly
        }

        [HideReferenceObjectPicker, HideLabel, ShowInInspector]
        public List<EditorConnection> Connections
        {
            get { return _graph.Connections()?.Select(o => new EditorConnection(o)).ToList(); }
        }
        
        public Rect GetSlotRect(Slot slot, SlotDirection direction)
        {
            return Slots.First(k => k.Content.Node.Node == slot.Node.Node
                                 && k.Content.PropertyPath == slot.PropertyPath
                                 && k.Direction == direction).Rect;
        }

        public void LoadGraph(IGraph graph)
        {
            _graph = graph;
            InitInput();
        }
        
        public Vector2 Pan
        {
            get
            {
                return _graph?.Pan ?? Vector2.zero;
            }
            set
            {
                _graph.Pan = value;
                GUIHelper.RequestRepaint();
            }
        }

        public float Scale
        {
            get
            {
                return _graph == null || _graph.Zoom < 1
                    ? 1 
                    : _graph.Zoom;
            }
            set
            {
                _graph.Zoom = Mathf.Clamp(value, 1f, MaxZoom);
                GUIHelper.RequestRepaint();
            }
        }

        public void Drag(Vector2 delta)
        {
            Pan += delta;
        }

        private void Zoom(float delta)
        {
            Scale += delta;
        }

        private void BeginZoomed()
        {
            GUI.EndClip();
            GUI.EndClip();
            
            GUIUtility.ScaleAroundPivot(Vector2.one / Scale, position.size * 0.5f);
            GUI.BeginClip(new Rect(-(position.width * Scale - position.width) * 0.5f,
                -((position.height * Scale - position.height) * 0.5f) + 22 * Scale,
                position.width * Scale,
                position.height * Scale));
        }

        private void EndZoomed()
        {
            GUIUtility.ScaleAroundPivot(Vector2.one * Scale, position.size * 0.5f);
            GUI.BeginClip(GUIHelper.CurrentWindow.position);
        }
        
        public Vector2 GuiToGridPosition(Vector2 guiPosition)
        {
            return (guiPosition - position.size * 0.5f - Pan / Scale) * Scale;
        }

        public Vector2 GridToGuiPosition(Vector2 gridPosition)
        {
            return position.size * 0.5f + Pan / Scale + gridPosition / Scale;
        }

        public Rect GridToGuiDrawRect(Rect gridRect)
        {
            gridRect.position = GridToGuiPositionNoClip(gridRect.position);
            return gridRect;
        }

        public Vector2 GridToGuiPositionNoClip(Vector2 gridPosition)
        {
            var center = position.size * 0.5f;
            var xOffset = center.x * Scale + (Pan.x + gridPosition.x);
            var yOffset = center.y * Scale + (Pan.y + gridPosition.y);
            return new Vector2(xOffset, yOffset);
        }
    }
}