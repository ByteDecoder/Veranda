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
	public class NodeView : RedOwlVisualElement, IOnMouse
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

		[UXMLReference]
		private VisualElement slots;

		[UXMLReference]
		private InspectorElement properties;


		private Dictionary<string, SlotView> slotTable = new Dictionary<string, SlotView>();
		private Dictionary<Guid, Tuple<PortView, PortView>> portTable = new Dictionary<Guid, Tuple<PortView, PortView>>(); 
		
		private Node node;
		    	
		public NodeView(Node node) : base()
		{
			this.node = node;
			this.name = node.GetType().Name;

			properties.Bind(new SerializedObject(node));

			foreach (Port item in node.ports)
			{
				CreatePortViews(item);
			}
		}

		private SlotView GetOrCreateSlotView(Port port)
		{
			SlotView slot = null;
			if (!slotTable.TryGetValue(port.name, out slot))
			{
				slot = new SlotView();
				slots.Add(slot);
				slotTable[port.name] = slot;
			}
			return slot;
		}

		private void CreatePortViews(Port port)
		{
			SlotView slot = GetOrCreateSlotView(port);
			portTable[port.id] = slot.RegisterPortView(port, typeof(IGraphPort).IsAssignableFrom(this.node.GetType()));
		}

		[UICallback(1, true)]
		private void CreateUI()
		{
			title.text = node.view.title;
			
			collapse.clickable.clicked += ToggleCollapse;
			duplicate.clickable.clicked += () => { GraphWindow.DuplicateNode(node); };
			delete.clickable.clicked += () => { GraphWindow.RemoveNode(node); };
			
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
			body.Add(slots);
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
			portTable[id].Item1.Connect();
		}
		
		public void ConnectOutput(Guid id)
		{
			portTable[id].Item2.Connect();
		}

		public void DisconnectInput(Guid id)
		{
			portTable[id].Item1.Disconnect();
		}
		
		public void DisconnectOutput(Guid id)
		{
			portTable[id].Item2.Disconnect();
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
			GraphWindow.MarkDirty();
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
		
		public Vector2 GetInputAnchor(Guid key)
		{
			if (node.view.collapsed)
			{
				return this.LocalToWorld(layout.position + new Vector2(1, layout.height * 0.5f));
			} else {
				return portTable[key].Item1.GetAnchor();
			}
		}
		
		public Vector2 GetOutputAnchor(Guid key)
		{
			if (node.view.collapsed)
			{
				return this.LocalToWorld(layout.position + new Vector2(layout.width - 1, layout.height * 0.5f));
			} else {
				return portTable[key].Item2.GetAnchor();
			}
		}
    }
}
#endif
