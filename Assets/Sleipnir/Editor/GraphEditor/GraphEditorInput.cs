using System;
using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private EditorNode _selectedNode;
        private EditorNode _resizedNode;
        private Slot _selectedInputSlot;
        private Slot _selectedOutputSlot;
        private bool _isDragging;
        private NodeResizeSide _resizedZone;

        private void OnEditorOpen()
        {
            _selectedInputSlot = null;
            _selectedOutputSlot = null;
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
                EditorGUIUtility.AddCursorRect(GridToGuiDrawRect(node.RightResizeZone),
                    MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(GridToGuiDrawRect(node.LeftResizeZone),
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

                if (node.RightResizeZone.Contains(mouseGridPosition))
                {
                    _resizedNode = node;
                    _resizedZone = NodeResizeSide.Right;
                    return;
                }

                if (node.LeftResizeZone.Contains(mouseGridPosition))
                {
                    _resizedNode = node;
                    _resizedZone = NodeResizeSide.Left;
                    return;
                }

                if (node.HeaderRect.Contains(mouseGridPosition))
                {
                    _selectedNode = node;
                    return;
                }

                if (node.ContentRect.Contains(mouseGridPosition)
                    || node.SliderRect.Contains(mouseGridPosition))
                {
                    _selectedInputSlot = null;
                    _selectedOutputSlot = null;
                    return;
                }
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
            if (_selectedNode != null || _selectedInputSlot != null || _selectedOutputSlot != null || Nodes == null)
            {
                _selectedNode = null;
                _selectedInputSlot = null;
                _selectedOutputSlot = null;
                return;
            }

            var mouseGridPosition = GuiToGridPosition(Event.current.mousePosition);

            foreach (var node in Nodes.Where(node => node.Content != null))
            {
                if (node.HeaderRect.Contains(mouseGridPosition))
                {
                    ShowNodeContextMenu(node);
                    return;
                }

                if (node.ContentRect.Contains(mouseGridPosition))
                    return;
            }
            
            if(Connections != null)
                foreach (var connection in Connections)
                    if (IsMouseOverConnection(mouseGridPosition, connection))
                    {
                        ShowConnectionContextMenu(connection);
                        return;
                    }

            ShowGridContextMenu(mouseGridPosition);
        }

        private bool IsMouseOverConnection(Vector2 mouseGridPosition, EditorConnection connection)
        {
            var outputSlot = GetSlotRect(connection.Content.Output);
            var inputSlot = GetSlotRect(connection.Content.Input);
            var startGridPosition = outputSlot.center;
            var endGridPosition = inputSlot.center;

            var bezierCurveData = EditorConnection.BezierCurveData(startGridPosition, endGridPosition);

            return HandleUtility.DistancePointBezier(
                mouseGridPosition,
                bezierCurveData.Item1,
                bezierCurveData.Item2,
                bezierCurveData.Item3,
                bezierCurveData.Item4) < EditorConnection.ConnectionWidth;
        }

        private void ShowNodeContextMenu(EditorNode node)
        {
            var menu = new GenericMenu();
            if (node.HasLabelSlider)
            {
                var content = node.Content.Node.IsLabelSliderShown
                    ? new GUIContent("Hide Label Width Slider")
                    : new GUIContent("Show Label Width Slider");
                menu.AddItem(content, false, 
                    () => node.Content.Node.IsLabelSliderShown = !node.Content.Node.IsLabelSliderShown);
            }
            menu.AddItem(new GUIContent("Delete Node"), false, () => RemoveNode(node));

            menu.ShowAsContext();
        }

        private void RemoveNode(EditorNode node)
        {
            var index = _graph.Nodes.IndexOf(node.Content);
            _graph.RemoveNode(node.Content.Node);

            // Node hasn't been removed
            if (_graph.Nodes.Contains(node.Value))
                return;
            
            if (index < _graph.Nodes.Count)
                _graph.Nodes[index].Node.NumberOfPrecedingDummies++;
        }

        private void ShowConnectionContextMenu(EditorConnection connection)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Connection"), false,
                () => _graph.RemoveConnection(connection.Content));
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
                    () =>
                    {
                        var node = _graph.AddNode(nodeName);
                        if (node == null)
                            return;
                        node.NodeRect = new Rect(mouseGridPosition, new Vector2());
                    });

            menu.ShowAsContext();
        }

        private void OnDrag()
        {
            var delta = Event.current.delta * Scale;

            if (_selectedNode != null)
                _selectedNode.Move(delta);
            else if (_isDragging)
                Pan += delta;
            else
                _resizedNode?.Resize(_resizedZone, delta.x);
        }

        private void OnScrollWheel()
        {
            Zoom(Event.current.delta.y * ZoomSpeed);
        }

        public void OnSlotClick(Slot slot, SlotDirection type)
        {
            if (type.IsInput())
            {
                if (_selectedOutputSlot == null)
                {
                    _selectedInputSlot = slot;
                } else {
                    _graph.AddConnection(new Connection(_selectedOutputSlot, slot));
                    _selectedOutputSlot = null;
                }
            }
            
            if (type.IsOutput())
            {
                if (_selectedInputSlot == null)
                {
                    _selectedOutputSlot = slot;
                } else {
                    _graph.AddConnection(new Connection(slot, _selectedInputSlot));
                    _selectedInputSlot = null;
                }
            }
        }
    }
}