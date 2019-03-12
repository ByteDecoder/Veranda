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

        [UXMLReference]
        public VisualElement breadcrumbs;
        private GraphBreadcrumbBar breadcrumbBar;

        public static void Open() => EnsureWindow();

        [OnOpenAssetAttribute(1)]
        public static bool HandleOpen(int instanceID, int line) => HandleAutoOpen(WindowDockStyles.Docked);

        public override string GetWindowTitle() => "Graph Editor";

        protected override void BuildUI()
        {
            breadcrumbBar = new GraphBreadcrumbBar();
            breadcrumbs.Add(breadcrumbBar);
        }

        public override void Load(Graph graph)
        {
            breadcrumbBar.ClearBreadcrumbs();
            breadcrumbBar.AddBreadcrumb(graph);
            view.Load(graph);
        }

        public void LoadSubGraph(Graph graph)
        {
            breadcrumbBar.AddBreadcrumb(graph);
            view.Load(graph);
        }

        public void Execute()
        {
            // TODO - this won't execute the full graph tree if the view's graph is a nested graph - need to walk back upwards to the parent then execute
            if (view.graph.AutoExecute) view.graph.Execute();
        }
    }
}