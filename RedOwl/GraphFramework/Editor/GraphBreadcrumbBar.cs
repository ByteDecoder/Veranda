#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using UnityEngine;
using UnityEngine.UIElements;
using RedOwl.Editor;
using RedOwl.GraphFramework;

namespace RedOwl.GraphFramework.Editor
{
	public class GraphBreadcrumbBar : BreadcrumbBar<Graph>
	{
		protected override void OnBreadcrumbClicked(Graph item)
		{
			GraphWindow.instance.view.Load(item);
		}
	}
}
#endif
