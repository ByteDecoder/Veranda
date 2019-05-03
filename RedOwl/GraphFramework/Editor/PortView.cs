#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	public class PortView : RedOwlVisualElement, IOnMouse
	{
		public readonly Port port;
        public readonly PortDirections direction;
        private bool isGraphPort;

		public PortView(Port port, PortDirections direction, bool isGraphPort) : base()
		{
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
			GraphWindow.ClickedPort(this);
		}
		
		public void OnRightMouseUp(MouseUpEvent evt)
		{
			GraphWindow.Disconnect(this);
		}

		public void StartConnecting()
		{
			AddToClassList("connecting");
		}

		public void ClearConnecting()
		{
			RemoveFromClassList("connecting");
		}

		public void Connect()
		{
			RemoveFromClassList("unconnected");
			RemoveFromClassList("connecting");
			AddToClassList("connected");
		}

		public void Disconnect()
		{
			RemoveFromClassList("connected");
			AddToClassList("unconnected");
		}
		
		public Vector2 GetAnchor()
		{
			return this.LocalToWorld(transform.position + (Vector3)(layout.size * 0.5f));
		}
    }
}
