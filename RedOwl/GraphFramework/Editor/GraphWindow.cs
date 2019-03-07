#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.Experimental.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USS, USS("RedOwl/GraphFramework/Editor/DefaultColors"), USS("RedOwl/GraphFramework/Editor/TypeColors")]
    public class GraphWindow : RedOwlAssetEditor<Graph, GraphWindow>
	{
		[UXMLReference]
		public GraphView view;

        public static void Open() => EnsureWindow();

        [OnOpenAssetAttribute(1)]
        public static bool HandleOpen(int instanceID, int line) => HandleAutoOpen(WindowDockStyles.Docked);

        public override string GetWindowTitle() => "Graph Editor";

        public override void Load(Graph graph) => view.Load(graph);
    }
}