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
        private const float MaxScale = 7f;

        private IGraph _graph;
        
        [HideReferenceObjectPicker, HideLabel, ShowInInspector]
        public List<EditorNode> Nodes
        {
            get
            {
                return _graph.Nodes
                    .SelectMany(n => Enumerable.Repeat<Node>(null, n.NumberOfPrecedingDummies)
                                               .Concat(new[] {n}))
                    .Select(n => new EditorNode(this, n))
                    .ToList();
            }
            set
            {
                // without setter property is marked as readonly
            }
        }
        
        public void LoadGraph(IGraph graph)
        {
            _graph = graph;
            OnEditorOpen();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        public Vector2 Position
        {
            get
            {
                return _graph?.Position ?? Vector2.zero;
            }
            set
            {
                _graph.Position = value;
                GUIHelper.RequestRepaint();
            }
        }

        public float Scale
        {
            get
            {
                return _graph == null || _graph.Scale < 1
                    ? 1 
                    : _graph.Scale;
            }
            set
            {
                _graph.Scale = Mathf.Clamp(value, 1f, MaxScale);
                GUIHelper.RequestRepaint();
            }
        }

        public void Drag(Vector2 delta)
        {
            Position += delta;
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
            return (guiPosition - position.size * 0.5f - Position / Scale) * Scale;
        }

        public Vector2 GridToGuiPosition(Vector2 gridPosition)
        {
            return position.size * 0.5f + Position / Scale + gridPosition / Scale;
        }

        public Rect GridToGuiDrawRect(Rect gridRect)
        {
            gridRect.position = GridToGuiPositionNoClip(gridRect.position);
            return gridRect;
        }

        public Vector2 GridToGuiPositionNoClip(Vector2 gridPosition)
        {
            var center = position.size * 0.5f;
            var xOffset = center.x * Scale + (Position.x + gridPosition.x);
            var yOffset = center.y * Scale + (Position.y + gridPosition.y);
            return new Vector2(xOffset, yOffset);
        }
    }
}