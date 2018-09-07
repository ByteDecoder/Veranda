using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private Node _selectedNode;
        private Knob _selectedKnob;
        private Node _resizedNode;
        private bool _isDragging;
        private NodeResizeSide _resizedZone;

        private void OnEditorOpen()
        {
            _selectedKnob = null;
            _selectedNode = null;
            _resizedNode = null;
            _isDragging = false;
        }

        private void ProcessInput()
        {
            if (mouseOverWindow != this)
            {
                _isDragging = false;
                _selectedNode = null;
                _resizedNode = null;
                return;
            }

            AddHoverCursorZones();

            if (Event.current.OnMouseDown(0, false))
                OnLeftMouseDown();

            if (Event.current.OnMouseUp(0, false))
                OnLeftMouseUp();

            if (Event.current.OnMouseDown(1, false))
                OnRightMouseDown();

            if (Event.current.OnMouseMoveDrag(false))
                OnDrag();

            if (Event.current.OnEventType(EventType.ScrollWheel))
                OnScrollWheel();
        }

        private void AddHoverCursorZones()
        {
            if (_resizedNode != null)
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height),
                    MouseCursor.ResizeHorizontal);

            if (Nodes == null)
                return;

            foreach (var node in Nodes)
            {
                if (node.Content == null)
                    continue;
                EditorGUIUtility.AddCursorRect(GridToGuiDrawRect(node.Content.RightResizeZone), 
                    MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(GridToGuiDrawRect(node.Content.LeftResizeZone),
                    MouseCursor.ResizeHorizontal);
            }
        }

        private void OnLeftMouseDown()
        {
            var mouseGridPosition = GuiToGridPosition(Event.current.mousePosition);

            if (Nodes == null)
            {
                _isDragging = true;
                return;
            }

            foreach (var node in Nodes)
            {
                if (node.Content == null)
                    continue;

                if (node.Content.RightResizeZone.Contains(mouseGridPosition))
                {
                    _resizedNode = node.Content;
                    _resizedZone = NodeResizeSide.Right;
                    return;
                }

                if (node.Content.LeftResizeZone.Contains(mouseGridPosition))
                {
                    _resizedNode = node.Content;
                    _resizedZone = NodeResizeSide.Left;
                    return;
                }

                if (node.Content.HeaderRect.Contains(mouseGridPosition))
                {
                    _selectedNode = node.Content;
                    return;
                }

                if (node.Content.ContentRect.Contains(mouseGridPosition)
                    || node.Content.SliderRect.Contains(mouseGridPosition)
                    || node.Content.Knobs.Any(k => k.Rect.Contains(mouseGridPosition)))
                    return;
            }

            _isDragging = true;
        }

        private void OnLeftMouseUp()
        {
            _isDragging = false;
            _resizedNode = null;
            _selectedNode = null;
        }

        private void OnRightMouseDown()
        {
            if (_selectedNode != null || _selectedKnob != null || Nodes == null)
            {
                _selectedNode = null;
                _selectedKnob = null;
                return;
            }

            var mouseGridPosition = GuiToGridPosition(Event.current.mousePosition);
            
            foreach (var node in Nodes.Where(node => node.Content != null))
            {
                if (node.Content.HeaderRect.Contains(mouseGridPosition))
                {
                    ShowNodeContextMenu(node.Content);
                    return;
                }
                
                if (node.Content.ContentRect.Contains(mouseGridPosition))
                    return;
            }
            
            if (_graph.Connections() != null)
                foreach (var connection in _graph.Connections())
                    if (IsPointInBezierRange(mouseGridPosition, connection))
                    {
                        ShowConnectionContextMenu(connection);
                        return;
                    }

            ShowGridContextMenu(mouseGridPosition);
        }

        private void ShowNodeContextMenu(Node node)
        {
            var menu = new GenericMenu();
            if (node.ContextMenuFunctions != null)
                foreach (var function in node.ContextMenuFunctions)
                    menu.AddItem(new GUIContent(function.Item1), false, () => function.Item2());

            if (node.HasLabelSlider)
            {
                var content = node.IsLabelSliderShown
                    ? new GUIContent("Hide Label Width Slider")
                    : new GUIContent("Show Label Width Slider");
                menu.AddItem(content, false, () => node.IsLabelSliderShown = !node.IsLabelSliderShown);
            }
            menu.AddItem(new GUIContent("Delete Node"), false, () => RemoveNode(node));

            menu.ShowAsContext();
        }

        private void RemoveNode(Node node)
        {
            var index = _graph.Nodes.IndexOf(node);
            if (!_graph.RemoveNode(node))
                return;

            if (index < _graph.Nodes.Count)
                _graph.Nodes[index].NumberOfPrecedingDummies++;
        }

        private void ShowConnectionContextMenu(Connection connection)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Connection"), false,
                () => _graph.RemoveConnection(connection));
            menu.ShowAsContext();
        }

        private void ShowGridContextMenu(Vector2 mouseGridPosition)
        {
            var menu = new GenericMenu();
            var availableNodes = _graph.AvailableNodes();
            if (availableNodes == null)
                return;

            foreach (var nodeName in availableNodes)
                menu.AddItem(new GUIContent("Create Node/" + nodeName), false,
                    () => _graph.AddNode(nodeName, mouseGridPosition));

            menu.ShowAsContext();
        }
        
        private void OnDrag()
        {
            var delta = Event.current.delta * Scale;

            if (_selectedNode != null)
                _selectedNode.Move(delta);
            else if (_isDragging)
                Position += delta;
            else
                _resizedNode?.Resize(_resizedZone, delta.x);
        }

        private void OnScrollWheel()
        {
            Zoom(Event.current.delta.y * ZoomSpeed);
        }
        
        private void OnKnobClick(Knob knob)
        {
            if (_selectedKnob == null ||
                _selectedKnob.Type == knob.Type)
                _selectedKnob = knob;
            else
            {
                if (knob.Type == KnobType.Input)
                    _graph.AddConnection(_selectedKnob, knob);
                else
                    _graph.AddConnection(knob, _selectedKnob);
                _selectedKnob = null;
            }
        }
    }
}