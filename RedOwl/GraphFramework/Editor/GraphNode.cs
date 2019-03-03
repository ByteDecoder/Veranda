#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
#endif
using RedOwl.Editor;
using RedOwl.GraphFramework;

/*
namespace RedOwl.GraphFramework.Editor
{
	//TODO: Collapse and Layout stuff i cannot get working correctly
	[UXML, USSClass("float", "node")]
	public class GraphNode : RedOwlVisualElement, IOnMouse
	{
		[UXMLReference]
		private ToolbarButton collapse;
		
		[UXMLReference]
		private FontAwesome collapseIcon;
		
		[UXMLReference]
		private Label title;
		
		[UXMLReference]
		private ToolbarButton delete;
		
		[UXMLReference]
		private VisualElement body;
		
		private GraphView view;		
		private Node node;
		private List<GraphSlot> ports;
		    	
		public GraphNode(GraphView view, Node node) : base()
		{
			this.view = view;
			this.node = node;
			title.text = node.view.title;
			style.width = node.view.layout.width;
			style.backgroundColor = node.view.color;
			
			collapse.clickable.clicked += ToggleCollapse;
			delete.clickable.clicked += Delete;
			
			CreateSlots();
			ConnectSlots();
			Load();
		}
		
		private void CreateSlots()
		{
			ports = new List<GraphSlot>();
			var target = new SerializedObject(node);
			SerializedProperty property = target.GetIterator();
			property.NextVisible(true);
			do
			{
				if (property.name == "m_Script") continue;
				var slot = new GraphSlot(this, property);
				ports.Add(slot);
				body.Add(slot);
			}
			while (property.NextVisible(false));
			body.Bind(target);
		}
		
		private void ConnectSlots()
		{
			foreach (Connection conn in view.Graph.connections)
			{
				if (conn.input.node == node.id)
				{
					ports[conn.input.slot].ConnectInput();
				}
				if (conn.output.node == node.id)
				{
					ports[conn.output.slot].ConnectOutput();
				}
			}
		}
	    
		public bool IsContentDragger { get { return false; } }
        
	    public IEnumerable<MouseFilter> MouseFilters {
		    get {
			    yield return new MouseFilter { 
				    button = MouseButton.LeftMouse,
				    OnMove = OnDrag
                };
		    }
	    }
        
		public void OnDrag(MouseMoveEvent evt, Vector3 delta)
		{
			transform.position += delta;
			BringToFront();
			Save();
		}
		
		[UICallback(5, true)]
		private void SizeFix()
		{
			node.SetHeight(this.layout.height);
			if (node.collapsed)
			{
				this.SetSize(new Vector2(node.layout.size.x, 34));
				collapseIcon.icon = "fa-chevron-right";
				body.Show(false);
			}
			Load();
		}
		
		private void Save()
		{
			node.SetPosition(this.layout, transform.position);
			view.Graph.MarkDirty();
		}
		
		private void Load()
		{
			transform.position = node.layout.position;
		}
	    
		private void ToggleCollapse()
		{
			node.collapsed = !node.collapsed;
			if (node.collapsed)
			{
				this.SetSize(new Vector2(node.layout.size.x, 34));
				collapseIcon.icon = "fa-chevron-right";
				body.Show(false);
			} else {
				this.SetSize(node.layout.size);
				collapseIcon.icon = "fa-chevron-down";
				body.Show();
			}
			Save();
		}
		
		private void Delete()
		{
			view.Graph.RemoveNode(node.id);
		}
		
		public Vector2 GetInputAnchor(int index)
		{
			if (node.collapsed)
			{
				return this.LocalToWorld(layout.position + new Vector2(1, layout.height * 0.5f));
			} else {
				return ports[index].GetInputAnchor();
			}
		}
		
		public Vector2 GetOutputAnchor(int index)
		{
			if (node.collapsed)
			{
				return this.LocalToWorld(layout.position + new Vector2(layout.width - 1, layout.height * 0.5f));
			} else {
				return ports[index].GetOutputAnchor();
			}
		}
		
		public void ClickSlot(bool add, SlotDirection direction, int slotIndex)
		{
			if (add)
			{
				if (direction.IsInput()) view.ClickInputPort(node.id, slotIndex);
				if (direction.IsOutput()) view.ClickOutputPort(node.id, slotIndex);
			} else {
				if (direction.IsInput()) view.RemoveInputConnection(node.id, slotIndex);
				if (direction.IsOutput()) view.RemoveOutputConnection(node.id, slotIndex);
			}
		}
    }
}
*/
#endif
