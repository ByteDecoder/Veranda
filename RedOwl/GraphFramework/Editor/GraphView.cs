#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USS("RedOwl/GraphFramework/Editor/InternalStyles"), USS("RedOwl/GraphFramework/Editor/DefaultColors"), USSClass("workspace", "flexfill", "graph")]
	public class GraphView : RedOwlVisualElement, IOnMouse, IOnMouseMove, IOnZoom, IOnContextMenu, IHandlesBezier
    {
		public new class UxmlFactory : UxmlFactory<GraphView> {}

		[UXMLReference]
		VisualElement workspace;
	    
	    [UXMLReference]
	    VisualElement nodes;
		private Dictionary<Guid, GraphNode> nodeTable = new Dictionary<Guid, GraphNode>(); 

		[UXMLReference]
	    HandlesRenderer connections;
	    
	    private Vector2 lastMousePosition = Vector2.zero;
	    
	    public Graph graph { get; private set; }

	    public IEnumerable<Tuple<Vector2, Vector2, Color, float>> GetBezierPoints()
	    {
	    	foreach (var conn in graph.connections)
	    	{
		    	yield return new Tuple<Vector2, Vector2, Color, float>(
			    	nodeTable[conn.output.node].GetOutputAnchor(conn.output.port),
			    	nodeTable[conn.input.node].GetInputAnchor(conn.input.port),
					Color.gray,
					3f
			    );
	    	}
	    
	    	if (outputPortReady)
	    	{
		    	yield return new Tuple<Vector2, Vector2, Color, float> (
			    	nodeTable[outputPort.Item1].GetOutputAnchor(outputPort.Item2.id),
			    	lastMousePosition,
					Color.yellow,
					3f
		    	);
	    	}
	    }
	    
	    public GraphView() : base() {}
	    
	    public void Load(Graph graph)
	    {
			if (this.graph != null)
			{
				this.graph.OnCleared -= OnClear;
				this.graph.OnNodeAdded -= AddNode;
				this.graph.OnNodeRemoved -= RemoveNode;
				this.graph.OnConnectionAdded -= OnConnectionsAdded;
				this.graph.OnConnectionRemoved -= OnConnectionsRemoved;
			}
		    this.graph = graph;
			this.AddStyleSheetPath(string.Format("{0}/GraphStyles", graph.GetType().Namespace));
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
			graph.OnNodeAdded += AddNode;
			graph.OnNodeRemoved += RemoveNode;
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
			nodeTable[connection.output.node].DisconnectOutput(connection.output.port);
			connections.MarkDirtyRepaint();
		}
	    
	    public void OnMouseMove(MouseMoveEvent evt)
	    {
	    	lastMousePosition = evt.mousePosition;
	    	if (outputPortReady) connections.MarkDirtyRepaint();
	    }
	    
	    public bool IsContentDragger { get { return true; } }
	    
	    public IEnumerable<MouseFilter> MouseFilters {
		    get {
			    yield return new MouseFilter { 
				    button = MouseButton.MiddleMouse,
				    OnMove = OnPan
                };
		    }
	    }
        
	    public void OnPan(MouseMoveEvent evt, Vector3 delta)
	    {
		    workspace.transform.position += delta;
	    }
	    
	    public float zoomMinScale { get { return 0.2f; } }
	    public float zoomMaxScale { get { return 5f; } }
	    public float zoomScaleStep { get { return 0.15f; } }
	    public EventModifiers zoomActivationModifiers { get { return EventModifiers.None; } }		
	    public void OnZoom(WheelEvent evt, Vector3 scale)
	    {
	    	workspace.transform.scale = scale;
	    }
	    
	    public void OnContextMenu(ContextualMenuPopulateEvent evt)
	    {
	    	if (outputPortReady) 
	    	{
	    		outputPortReady = false;
	    		connections.MarkDirtyRepaint();
	    	}
			IGraph g = graph as IGraph;
	    	if (g != null)
	    	{
	    		foreach (Tuple<string, Type> item in g.GetNodesTypes())
	    		{
	    			evt.menu.AppendAction(item.Item1, (a) => graph.Add(item.Item2, lastMousePosition), DropdownMenu.MenuAction.AlwaysEnabled);
	    		}
	    	}
	    }

		public void AddNode(Node node)
		{
			var view = new GraphNode(this, node);
		    nodes.Add(view);
			nodeTable.Add(node.id, view);
		}

		public void RemoveNode(Node node)
		{
			nodes.Remove(nodeTable[node.id]);
		}
	    
	    private bool outputPortReady = false;
		private Tuple<Guid, Port> outputPort;
	    public bool ClickOutputPort(Guid node, Port port)
	    {
		    if (outputPortReady) return false;
			outputPort = new Tuple<Guid, Port>(node, port);
		    outputPortReady = true;
		    return true;
	    }
	    
	    public bool ClickInputPort(Port port)
	    {
		    if (!outputPortReady) return false;
			graph.Connect(outputPort.Item2, port);
		    outputPortReady = false;
		    return true;
	    }
    }
}
#endif
