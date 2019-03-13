#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USSClass("container", "row")]
	public class SlotView : RedOwlVisualElement
	{
		[UXMLReference]
		private VisualElement body;
		
		[UXMLReference]
		private VisualElement inputs;
		
		[UXMLReference]
		private VisualElement outputs;
    	
		public SlotView() : base() {}

		public Tuple<PortView, PortView> RegisterPortView(Port port, bool isGraphPort)
		{
			var field = port.GetField();
			body.Add(field);
			if (isGraphPort) field.SetEnabled(false);

			PortView input = null;
			PortView output = null;
			if (port.direction.IsInput())
			{
				input = new PortView(field, port, PortDirections.Input, isGraphPort);
				inputs.Add(input);
			}
			if (port.direction.IsOutput())
			{
				output = new PortView(field, port, PortDirections.Output, isGraphPort);
				outputs.Add(output);
			}
			return new Tuple<PortView, PortView>(input, output);
		}
	}
}
#endif
