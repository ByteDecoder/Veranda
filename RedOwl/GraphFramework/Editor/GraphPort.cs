#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML]
	public class GraphPort : RedOwlVisualElement, IOnMouse
	{
		[UXMLReference]
		private VisualElement body;
		private LabelX label;
		private PropertyFieldX field;
		
		[UXMLReference]
		private VisualElement input;
		
		[UXMLReference]
		private VisualElement output;
		
		private GraphView view;
		private Node node;
		private IPort port;
    	
		public GraphPort(GraphView view, Node node, IPort port) : base()
		{
			this.view = view;
			this.node = node;
			this.port = port;

			label = new LabelX(port.name);
			label.style.width = node.view.labelWidth;

			field = port.GetField();
			field.label.style.width = node.view.labelWidth;

			body.Add(field);

			input.Show(port.direction.IsInput());
			output.Show(port.direction.IsOutput());
		}
		
		public bool IsContentDragger { get { return false; } }
	    
		public IEnumerable<MouseFilter> MouseFilters {
			get {
				yield return new MouseFilter { 
					button = MouseButton.LeftMouse,
					OnUp = OnLeftMouseUp
                };
				yield return new MouseFilter { 
					button = MouseButton.RightMouse,
					OnUp = OnRightMouseUp
                };
			}
		}
        
		public void OnLeftMouseUp(MouseUpEvent evt)
		{
			if (input.ContainsPoint(this.ChangeCoordinatesTo(input, evt.localMousePosition)))
			{
				view.ClickInputPort(port);
			}
			
			if (output.ContainsPoint(this.ChangeCoordinatesTo(output, evt.localMousePosition)))
			{
				view.ClickOutputPort(node.id, port);
			}
		}
		
		public void OnRightMouseUp(MouseUpEvent evt)
		{
			if (input.ContainsPoint(this.ChangeCoordinatesTo(input, evt.localMousePosition)))
			{
				view.graph.Disconnect(port);
			}
			
			if (output.ContainsPoint(this.ChangeCoordinatesTo(output, evt.localMousePosition)))
			{
				view.graph.Disconnect(port);
			}
		}
		
		public void ConnectInput()
		{
			input.AddToClassList("portConnected");
			body.Clear();
			body.Add(label);
		}
		
		public void ConnectOutput()
		{
			output.AddToClassList("portConnected");
		}

		public void DisconnectInput()
		{
			input.RemoveFromClassList("portConnected");
			body.Clear();
			body.Add(field);
		}
		
		public void DisconnectOutput()
		{
			output.RemoveFromClassList("portConnected");
		}
		
		public Vector2 GetInputAnchor()
		{
			return input.LocalToWorld(input.transform.position + (Vector3)(input.layout.size * 0.5f));
		}
		
		public Vector2 GetOutputAnchor()
		{
			return output.LocalToWorld(output.transform.position + (Vector3)(input.layout.size * 0.5f));
		}
    }
}
#endif
