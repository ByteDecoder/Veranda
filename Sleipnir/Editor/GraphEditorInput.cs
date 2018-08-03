using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private const float ZoomSpeed = 0.1f;

        private Node? _selectedNode;
        private Knob? _selectedKnob;
        private bool _isDragging;
        
        public Vector2 Position
        {
            get { return _graph.Position; }
            set
            {
                _graph.Position = value;
                GUIHelper.RequestRepaint(); 
            }
        }

        public float Zoom
        {
            get { return _graph.Zoom < 1 ? 1 : _graph.Zoom; }
            set
            {
                _graph.Zoom = Mathf.Clamp(value, 1f, 7f);
                GUIHelper.RequestRepaint(); 
            } 
        }

        private void ProcessInput()
        {
            if (_graph == null)
                return;

            if (Event.current.OnMouseDown(0, false))
            {
                OnLeftMouseDown();
                Repaint();
            }

            if (Event.current.OnMouseUp(0, false))
            {
                OnLeftMouseUp();
                Repaint();
            }

            if (Event.current.OnMouseDown(1, false))
            {
                OnRightMouseDown();
                Repaint();
            }

            if (Event.current.OnMouseMoveDrag(false))
            {
                OnDrag();
                Repaint();
            }

            if (Event.current.OnEventType(EventType.ScrollWheel))
            {
                OnScrollWheel();
                Repaint();
            }
        }
        

        public void OnDrag()
        {
            var delta = Event.current.delta * Zoom;

            if (_selectedNode.HasValue)
                _selectedNode.Value.Move(delta);
            else if (_isDragging)
                Position += delta;
        }

        public Vector2 GuiToGridPosition(Vector2 guiPosition)
        {
            return (guiPosition - position.size * 0.5f - Position / Zoom) * Zoom;
        }

        public Vector2 GridToGuiPosition(Vector2 gridPosition)
        {
            return position.size * 0.5f + Position / Zoom + gridPosition / Zoom;
        }

        public Rect GridToGuiDrawRect(Rect gridRect)
        {
            gridRect.position = GridToGuiPositionNoClip(gridRect.position);
            return gridRect;
        }

        public Vector2 GridToGuiPositionNoClip(Vector2 gridPosition)
        {
            var center = position.size * 0.5f;
            var xOffset = center.x * Zoom + (Position.x + gridPosition.x);
            var yOffset = center.y * Zoom + (Position.y + gridPosition.y);
            return new Vector2(xOffset, yOffset);
        }

        public void OnLeftMouseUp()
        {
            _isDragging = false;
        }

        public void OnLeftMouseDown()
        {
            _isDragging = true;
            var currentPosition = GuiToGridPosition(Event.current.mousePosition);
            if (Nodes.Any(node => node.Rect.Value.Contains(currentPosition))) 
                _isDragging = false;
        }

        public void SelectNode(Node node)
        {
            if (!_selectedNode.HasValue)
            {
                MoveNodeToFront(node);
                _selectedNode = node;
            }
            else
            {
                _selectedNode = null;
            }
        }

        private void OnRightMouseDown()
        {
            if (_selectedNode.HasValue)
            {
                _selectedNode = null;
                return;
            }

            if (_selectedKnob.HasValue)
            {
                _selectedKnob = null;
                return;
            }
            var currentPosition = GuiToGridPosition(Event.current.mousePosition);
            var menu = new GenericMenu();
            var onNode = false;

            foreach (var node in Nodes)
                if (node.Rect.Value.Contains(currentPosition))
                {
                    onNode = true;
                    menu.AddItem(new GUIContent("Delete Node"), false, () => { RemoveNode(node); });
                }

            if (!onNode)
                foreach (var node in _graph.AvaibleNodes())
                    menu.AddItem(new GUIContent("Create Node/" + node), false,
                        () => { CreateNode(node, currentPosition); });

            menu.ShowAsContext();
        }
        
        public void OnScrollWheel()
        {
            Zoom += Event.current.delta.y * ZoomSpeed;
        }
    }
}