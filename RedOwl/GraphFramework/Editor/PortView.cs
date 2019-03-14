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
	public class PortView : RedOwlVisualElement, IOnMouse
	{
        private PropertyFieldX field;
		private Port port;
        private PortDirections direction;
        private bool isGraphPort;

		public PortView(PropertyFieldX field, Port port, PortDirections direction, bool isGraphPort) : base()
		{
            this.field = field;
            this.port = port;
            this.direction = direction;
            this.isGraphPort = isGraphPort;

            AddToClassList("unconnected");
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
			GraphWindow.ClickedPort(direction, port);
		}
		
		public void OnRightMouseUp(MouseUpEvent evt)
		{
			GraphWindow.Disconnect(port, direction.IsInput());
		}
		public void Connect()
		{
			RemoveFromClassList("unconnected");
			AddToClassList("connected");
		}

		public void Disconnect()
		{
			RemoveFromClassList("connected");
			AddToClassList("unconnected");
			field.UpdateField();
		}
		
		public Vector2 GetAnchor()
		{
			return this.LocalToWorld(transform.position + (Vector3)(layout.size * 0.5f));
		}
    }
}
#endif
