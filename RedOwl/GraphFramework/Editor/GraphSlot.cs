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
	public class GraphSlot : RedOwlVisualElement, IOnMouse
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
		private Port port;

		private bool isInput;
		private bool isOutput;
    	
		public GraphSlot(GraphView view, Node node, Port port) : base()
		{
			this.view = view;
			this.node = node;
			this.port = port;
			port.OnValueChanged += (data) => { if (node.graph.AutoExecute) node.graph.Execute(); };

			label = new LabelX(port.name);
			label.AddToClassList("centered");
			label.style.width = node.view.labelWidth;

			field = port.GetField();
			field.label.style.width = node.view.labelWidth;

			body.Add(field);

			isInput = port.direction.IsInput();
			isOutput = port.direction.IsOutput();

			input.Show(isInput);
			input.name = port.type.Name;
			output.Show(isOutput);
			output.name = port.type.Name;
		}

		protected override void BuildUI()
		{
			body.style.paddingLeft = 13;
			body.style.paddingRight = 3;
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
			if (isInput && input.ContainsPoint(this.ChangeCoordinatesTo(input, evt.localMousePosition)))
			{
				view.ClickedPort(PortDirections.Input, node, port);
			}
			
			if (isOutput && output.ContainsPoint(this.ChangeCoordinatesTo(output, evt.localMousePosition)))
			{
				view.ClickedPort(PortDirections.Output, node, port);
			}
		}
		
		public void OnRightMouseUp(MouseUpEvent evt)
		{
			if (isInput && input.ContainsPoint(this.ChangeCoordinatesTo(input, evt.localMousePosition)))
			{
				view.graph.Disconnect(port, true);
			}
			
			if (isOutput && output.ContainsPoint(this.ChangeCoordinatesTo(output, evt.localMousePosition)))
			{
				view.graph.Disconnect(port, false);
			}
		}
		
		public void ConnectInput()
		{
			input.RemoveFromClassList("unconnected");
			input.AddToClassList("connected");
			body.Clear();
			body.AddToClassList("centered");
			body.Add(label);
		}
		
		public void ConnectOutput()
		{
			output.RemoveFromClassList("unconnected");
			output.AddToClassList("connected");
		}

		public void DisconnectInput()
		{
			input.RemoveFromClassList("connected");
			input.AddToClassList("unconnected");
			body.Clear();
			body.RemoveFromClassList("centered");
			body.Add(field);
			field.UpdateField();
		}
		
		public void DisconnectOutput()
		{
			output.RemoveFromClassList("connected");
			output.AddToClassList("unconnected");
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
