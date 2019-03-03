#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
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
	[UXML]
	public class GraphSlot : RedOwlVisualElement, IOnMouse
	{
		[UXMLReference]
		private VisualElement body;
		private Label label;
		private PropertyField field;
		
		[UXMLReference]
		private VisualElement input;
		
		[UXMLReference]
		private VisualElement output;
		
		private GraphNode node;
		private Slot slot;
    	
		public GraphSlot(GraphNode node, SerializedProperty property, Slot slot) : base()
		{
			this.node = node;
			this.slot = slot;
			
			field = new PropertyField(property);
			body.Add(field);
			field.Show(true);
			
			//input.Show(slot.Direction.IsInput());
			//output.Show(slot.Direction.IsOutput());
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
				node.ClickSlot(true, SlotDirection.Input, slotIndex);
			}
			
			if (output.ContainsPoint(this.ChangeCoordinatesTo(output, evt.localMousePosition)))
			{
				node.ClickSlot(true, SlotDirection.Output, slotIndex);
			}
		}
		
		public void OnRightMouseUp(MouseUpEvent evt)
		{
			if (input.ContainsPoint(this.ChangeCoordinatesTo(input, evt.localMousePosition)))
			{
				node.ClickSlot(false, SlotDirection.Input, slotIndex);
			}
			
			if (output.ContainsPoint(this.ChangeCoordinatesTo(output, evt.localMousePosition)))
			{
				node.ClickSlot(false, SlotDirection.Output, slotIndex);
			}
		}
		
		public void ConnectInput()
		{
			input.AddToClassList("portConnected");
		}
		
		public void ConnectOutput()
		{
			output.AddToClassList("portConnected");
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
*/
#endif
