#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif
using RedOwl.Editor;
using RedOwl.GraphFramework;

/*
namespace RedOwl.GraphFramework.Editor
{
	[UXML, USS("RedOwl/GraphSystem/Styles"), USSClass("workspace", "flexfill")]
	public class GraphView : RedOwlVisualElement, IOnMouse, IOnMouseMove, IOnZoom, IOnContextMenu, IHandlesBezier
    {
		[UXMLReference]
		VisualElement workspace;
	    
	    [UXMLReference]
	    VisualElement nodesGroup;

		[UXMLReference]
	    HandlesRenderer connections;
	    
	    private GraphNode[] nodes;
	    private Vector2 lastMousePosition = Vector2.zero;
	    
	    public Graph Graph { get; private set; }
	    
	    public float LineWidth { get; } = 4f;
	    public Color LineColor { get; } = new Color32(241, 96, 62, 255);
	    public IEnumerable<Tuple<Vector2, Vector2>> GetBezierPoints()
	    {
	    	foreach (var conn in Graph.Connections)
	    	{
		    	yield return new Tuple<Vector2, Vector2>(
			    	nodes[Graph.IndexOf(conn.output.node)].GetOutputAnchor(conn.output.slot),
			    	nodes[Graph.IndexOf(conn.input.node)].GetInputAnchor(conn.input.slot)
			    );
	    	}
	    
	    	if (outputPortReady)
	    	{
		    	yield return new Tuple<Vector2, Vector2> (
			    	nodes[Graph.IndexOf(_outputPort.node)].GetOutputAnchor(_outputPort.slot),
			    	lastMousePosition
		    	);
	    	}
	    }
	    
	    public GraphView() : base() {}
	    
	    public void Load(Graph graph)
	    {
	    	if (Graph != null) Graph.OnGraphChanged -= BuildUI;
		    Graph = graph;
		    Graph.OnGraphChanged += BuildUI;
		    BuildUI();
	    }
	    
	    private void BuildUI()
	    {
		    nodesGroup.Clear();
		    foreach (Node node in Graph.Nodes)
		    {
		    	nodesGroup.Add(new GraphNode(this, node));
		    }
		    nodes = nodesGroup.Children<GraphNode>().ToArray();
		    connections.Load(this);
	    }
	    
	    public void OnMouseMove(MouseMoveEvent evt)
	    {
	    	lastMousePosition = evt.mousePosition;
	    	if (outputPortReady) connections.MarkDirtyRepaint();
	    }
		
	    private static bool outputPortSet = false;
	    private static Port outputPort;
        
	    public void SetOutputPort(Port port)
	    {
		    outputPort = port;
		    outputPortSet = true;
	    }

	    public string SetInputPort(Port port)
	    {
		    if (!outputPortSet) return string.Empty;
		    string output = Graph.AddConnection(port, outputPort);
		    outputPortSet = false;
		    return output;
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
	    	IGraph g = Graph as IGraph;
	    	if (g != null)
	    	{
	    		foreach (Tuple<string, Type> item in g.GetNodesTypes())
	    		{
	    			evt.menu.AppendAction(item.Item1, (a) => Graph.AddNode(item.Item2, lastMousePosition), DropdownMenu.MenuAction.AlwaysEnabled);
	    		}
	    	}
	    }
	    
	    private bool outputPortReady = false;
	    private Port _outputPort;
	    public bool ClickOutputPort(string nodeId, int slotIndex)
	    {
		    if (outputPortReady) return false;
		    _outputPort = new Port { node = Graph[nodeId].id, slot = slotIndex };
		    outputPortReady = true;
		    return true;
	    }
	    
	    public bool ClickInputPort(string nodeId, int slotIndex)
	    {
		    if (!outputPortReady) return false;
		    Graph.AddConnection(new Port { node = Graph[nodeId].id, slot = slotIndex }, _outputPort);
		    outputPortReady = false;
		    return true;
	    }
	    
	    public void RemoveInputConnection(string id, int slotIndex)
	    {
		    Graph.RemoveInputConnection(id, slotIndex);
	    }
        
	    public void RemoveOutputConnection(string id, int slotIndex)
	    {
		    Graph.RemoveOutputConnection(id, slotIndex);
	    }


    }
}
*/
#endif
