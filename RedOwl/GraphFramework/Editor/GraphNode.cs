#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
using RedOwl.Editor;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USSClass("float", "background")]
	public class GraphNode : RedOwlVisualElement, IOnMouse
	{
		[UXMLReference]
		private ToolbarButton collapse;
		
		[UXMLReference]
		private FontAwesome collapseIcon;
		
		[UXMLReference]
		private Label title;

		[UXMLReference]
		private ToolbarButton duplicate;
		
		[UXMLReference]
		private ToolbarButton delete;

		[UXMLReference]
		private VisualElement body;
		
		private InspectorElement properties;
		private VisualElement ports;
		private Dictionary<Guid, GraphSlot> portTable = new Dictionary<Guid, GraphSlot>(); 
		
		private GraphView view;		
		private Node node;
		private SerializedObject target;
		    	
		public GraphNode(GraphView view, Node node) : base()
		{
			this.view = view;
			this.node = node;
			this.name = node.GetType().Name;

			//style.width = node.view.layout.width;
			//style.backgroundColor = node.view.color;

			target = new SerializedObject(node);
			properties = new InspectorElement();
			properties.Bind(target);

			ports = new VisualElement();
			ports.name = "ports";
			foreach (Port item in node)
			{
				var port = new GraphSlot(view, node, item);
				ports.Add(port);
				portTable.Add(item.id, port);
			}
		}

		[UICallback(1, true)]
		private void CreateUI()
		{
			title.text = node.view.title;
			
			collapse.clickable.clicked += ToggleCollapse;
			duplicate.clickable.clicked += Duplicate;
			delete.clickable.clicked += Delete;
			
			if (!node.view.collapsed)
			{
				ShowBody();
			} else {
				HideBody();
			}
			Load();
		}

		private void ShowBody()
		{
			body.Add(properties);
			body.Add(ports);
			collapseIcon.icon = "fa-chevron-down";
			body.Show();
		}

		private void HideBody()
		{
			collapseIcon.icon = "fa-chevron-right";
			body.Clear();
			body.Show(false);
		}

		public void ConnectInput(Guid id)
		{
			portTable[id].ConnectInput();
		}
		
		public void ConnectOutput(Guid id)
		{
			portTable[id].ConnectOutput();
		}

		public void DisconnectInput(Guid id)
		{
			portTable[id].DisconnectInput();
		}
		
		public void DisconnectOutput(Guid id)
		{
			portTable[id].DisconnectOutput();
		}
	    
		public bool IsContentDragger { get { return false; } }
        
	    public IEnumerable<MouseFilter> MouseFilters {
		    get {
			    yield return new MouseFilter { 
				    button = MouseButton.LeftMouse,
					OnDown = OnDown,
				    OnMove = OnDrag
                };
		    }
	    }

		public void OnDown(MouseDownEvent evt)
		{
			BringToFront();
		}
        
		public void OnDrag(MouseMoveEvent evt, Vector3 delta)
		{
			transform.position += delta;
			BringToFront();
			Save();
		}
		
		private void Save()
		{
			node.view.layout.position = transform.position;
			view.graph.MarkDirty();
		}
		
		private void Load()
		{
			transform.position = node.view.layout.position;
		}
	    
		private void ToggleCollapse()
		{
			node.view.collapsed = !node.view.collapsed;
			if (node.view.collapsed)
			{
				HideBody();
			} else {
				ShowBody();
			}
			Save();
		}

		private void Duplicate()
		{
			view.graph.Duplicate(node);
		}
		
		private void Delete()
		{
			view.graph.Remove(node.id);
		}
		
		public Vector2 GetInputAnchor(Guid key)
		{
			if (node.view.collapsed)
			{
				return this.LocalToWorld(layout.position + new Vector2(1, layout.height * 0.5f));
			} else {
				return portTable[key].GetInputAnchor();
			}
		}
		
		public Vector2 GetOutputAnchor(Guid key)
		{
			if (node.view.collapsed)
			{
				return this.LocalToWorld(layout.position + new Vector2(layout.width - 1, layout.height * 0.5f));
			} else {
				return portTable[key].GetOutputAnchor();
			}
		}
    }
}
#endif
