#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USSClass("workspace")]
	public class GraphView : RedOwlVisualElement, IOnKeyboard, IOnMouse, IOnMouseMove, IOnZoom, IOnContextMenu, IHandlesBezier
    {
		public new class UxmlFactory : UxmlFactory<GraphView> {}

		[UXMLReference]
		GridSheet grid;

		[UXMLReference]
		VisualElement workspace;

		[UXMLReference]
		DragSelector selector;
		[UXMLReference]
		DragSelector toggleSelector;
	    
	    [UXMLReference]
	    VisualElement nodes;
		private Dictionary<Guid, NodeView> nodeTable = new Dictionary<Guid, NodeView>();
		private Dictionary<Guid, PortView> portTable = new Dictionary<Guid, PortView>(); 

		[UXMLReference]
	    HandlesRenderer connections;
	    
	    private Vector2 lastMousePosition = Vector2.zero;
	    
	    public Graph graph { get; private set; }

	    public IEnumerable<Tuple<Vector2, Vector2, Color, float>> GetBezierPoints()
	    {
			float curveThickness = workspace.transform.scale.x < 0.3f ? 8f : 3f;
	    	foreach (var conn in graph.connections)
	    	{
		    	yield return new Tuple<Vector2, Vector2, Color, float>(
			    	nodeTable[conn.output.node].GetOutputAnchor(conn.output.port),
			    	nodeTable[conn.input.node].GetInputAnchor(conn.input.port),
					Color.gray,
					curveThickness
			    );
	    	}
	    
	    	if (clickedOnce)
	    	{
				if (clickedDirection.IsInput())
				{
					yield return new Tuple<Vector2, Vector2, Color, float> (
						lastMousePosition,
						nodeTable[clickedPort.Item1].GetInputAnchor(clickedPort.Item2.id),
						Color.yellow,
						curveThickness
					);
				} else {
					yield return new Tuple<Vector2, Vector2, Color, float> (
						nodeTable[clickedPort.Item1].GetOutputAnchor(clickedPort.Item2.id),
						lastMousePosition,
						Color.yellow,
						curveThickness
					);
				}
	    	}
	    }
	    
	    public GraphView() : base()
		{
			selector.OnComplete += DoSelection;
			toggleSelector.OnComplete += DoSelectionToggle;
			toggleSelector.color = new Color(1.0f, 1.0f, 0f, 0.25f);
		}
	    
	    public void Load(Graph graph)
	    {
			if (this.graph != null)
			{
				this.graph.OnCleared -= OnClear;
				this.graph.OnChildAdded -= AddNode;
				this.graph.OnChildRemoved -= RemoveNode;
				this.graph.OnConnectionAdded -= OnConnectionsAdded;
				this.graph.OnConnectionRemoved -= OnConnectionsRemoved;
			}
		    this.graph = graph;
			var sheet = Resources.Load<StyleSheet>(string.Format("{0}/GraphStyles", graph.GetType().Namespace));
			if (sheet != null) this.styleSheets.Add(sheet);
			nodes.Clear();
			nodeTable.Clear();
		    foreach (Node node in graph)
		    {
				AddNode(node);
		    }
			foreach (Connection connection in graph.connections)
			{
				nodeTable[connection.input.node].ConnectInput(connection.input.port);
				nodeTable[connection.output.node].ConnectOutput(connection.output.port);
			}
			connections.Load(this);
			graph.OnCleared += OnClear;
			graph.OnChildAdded += AddNode;
			graph.OnChildRemoved += RemoveNode;
			graph.OnConnectionAdded += OnConnectionsAdded;
			graph.OnConnectionRemoved += OnConnectionsRemoved;
	    }

		private void OnClear()
		{
			nodes.Clear();
			nodeTable.Clear();
			connections.MarkDirtyRepaint();
		}

		private void OnConnectionsAdded(Connection connection)
		{
			nodeTable[connection.input.node].ConnectInput(connection.input.port);
			nodeTable[connection.output.node].ConnectOutput(connection.output.port);
			connections.MarkDirtyRepaint();
		}

		private void OnConnectionsRemoved(Connection connection)
		{
			nodeTable[connection.input.node].DisconnectInput(connection.input.port);
			if (!graph.IsConnected(connection.output.port, false)) nodeTable[connection.output.node].DisconnectOutput(connection.output.port);
			connections.MarkDirtyRepaint();
		}
	    
	    public void OnMouseMove(MouseMoveEvent evt)
	    {
	    	lastMousePosition = evt.mousePosition;
	    	if (clickedOnce) connections.MarkDirtyRepaint();
	    }

		public IEnumerable<KeyboardFilter> KeyboardFilters {
			get {
				yield return new KeyboardFilter {
					key = KeyCode.F,
					OnDown = OnFDown
				};
			}
		}

		public void OnFDown(KeyDownEvent evt)
		{
			Debug.Log("here");
			workspace.transform.scale = Vector3.one;
			workspace.transform.position = Vector3.zero;
		}
	    
	    public bool IsContentDragger { get { return true; } }
	    
	    public IEnumerable<MouseFilter> MouseFilters {
		    get {
				yield return new MouseFilter { 
				    button = MouseButton.LeftMouse,
				    OnDown = OnLeftDown,
					OnUp = OnLeftUp,
                };
			    yield return new MouseFilter {
				    button = MouseButton.MiddleMouse,
				    OnMove = OnPan
                };
				yield return new MouseFilter {
				    button = MouseButton.LeftMouse,
				    OnDown = selector.OnDown,
					OnUp = selector.OnUp,
					OnMove = selector.OnMove
				};
				yield return new MouseFilter {
				    button = MouseButton.LeftMouse,
					modifiers = EventModifiers.Control,
				    OnDown = toggleSelector.OnDown,
					OnUp = toggleSelector.OnUp,
					OnMove = toggleSelector.OnMove
				};
		    }
	    }

		public void OnLeftDown(MouseDownEvent evt)
		{
			if (clickedOnce)
			{
				clickedPort.Item3.ClearConnecting();
				clickedOnce = false;
			}
		}

		public void OnLeftUp(MouseUpEvent evt)
		{
			GraphWindow.UnselectNodes();
		}

		internal IEnumerable<NodeView> CaclulateSelectedNodes(Rect rect)
		{
			Rect localRect;
			foreach (var view in nodeTable.Values)
			{
				localRect = workspace.ChangeCoordinatesTo(view, rect);
				if (view.Overlaps(localRect)) yield return view;
			}
		}

		internal void DoSelection(Rect rect)
		{
			GraphWindow.UnselectNodes();
			foreach (var view in CaclulateSelectedNodes(rect))
			{
				GraphWindow.ToggleSelectNode(view);
			}
		}

		internal void DoSelectionToggle(Rect rect)
		{
			foreach (var view in CaclulateSelectedNodes(rect))
			{
				GraphWindow.ToggleSelectNode(view);
			}
		}
        
	    public void OnPan(MouseMoveEvent evt, Vector3 delta)
	    {
		    workspace.transform.position += delta;
			grid.pan += (Vector2)delta;
	    }
	    
	    public float zoomMinScale { get { return 0.1f; } }
	    public float zoomMaxScale { get { return 5f; } }
	    public float zoomScaleStep { get { return 0.15f; } }
	    public EventModifiers zoomActivationModifiers { get { return EventModifiers.None; } }		
	    public void OnZoom(WheelEvent evt, Vector3 scale)
	    {
	    	workspace.transform.scale = scale;
			grid.scale = (Vector2)scale;

			if (scale.x < 0.3f)
			{
				Debug.Log("TempCollapse");
				foreach (var view in nodeTable.Values)
				{
					view.MacroView(true);
				}
			} else {
				foreach (var view in nodeTable.Values)
				{
					view.MacroView(false);
				}
			}
	    }
	    
	    public void OnContextMenu(ContextualMenuPopulateEvent evt)
	    {
			IGraph g = graph as IGraph;
	    	if (g != null)
	    	{
	    		foreach (Tuple<string, Type> item in g.GetNodesTypes())
	    		{
	    			evt.menu.AppendAction(item.Item1, (a) => graph.AddNode(item.Item2, workspace.WorldToLocal(lastMousePosition)), DropdownMenuAction.AlwaysEnabled);
	    		}
	    	}
	    }

		public void AddNode(Node node)
		{
			var view = new NodeView(node);
		    nodes.Add(view);
			nodeTable.Add(node.id, view);
		}

		public void RemoveNode(Node node)
		{
			nodes.Remove(nodeTable[node.id]);
		}
	    
	    private bool clickedOnce = false;
		private PortDirections clickedDirection;
		private Tuple<Guid, Port, PortView> clickedPort;
	    internal void ClickedPort(PortView view)
	    {
			var port = view.port;
			var direction = view.direction;
		    if (clickedOnce)
			{
				if (clickedDirection == direction) return;
				if (direction.IsInput())
				{
					graph.Connect(clickedPort.Item2, port);
				} else {
					graph.Connect(port, clickedPort.Item2);
				}
				clickedPort.Item3.ClearConnecting();
				clickedOnce = false;
			} else {
				clickedDirection = direction;
				clickedPort = new Tuple<Guid, Port, PortView>(graph.FindNodeWithPort(port).id, port, view);
				clickedOnce = true;
				view.StartConnecting();
			}
	    }
    }
}
