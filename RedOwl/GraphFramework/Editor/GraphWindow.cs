#if UNITY_EDITOR
#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USS, USS("RedOwl/GraphFramework/Editor/DefaultColors"), USS("RedOwl/GraphFramework/Editor/TypeColors")]
    public class GraphWindow : RedOwlAssetEditor<Graph, GraphWindow>
	{
		[UXMLReference]
		internal GraphView view;

        [UXMLReference]
        private ToolbarToggle autoexecute;

        public Graph graph {
            get { return view.graph; }
        }

        protected GraphBreadcrumbBar breadcrumbBar;

        public static void Open() => EnsureWindow();

        [OnOpenAssetAttribute(1)]
        public static bool HandleOpen(int instanceID, int line) => HandleAutoOpen(WindowDockStyles.Docked);

        public override string GetWindowTitle() => "Graph Editor";

        [Q("breadcrumbs")]
        protected void CreateBreadcrumbsBar(VisualElement element)
        {
            breadcrumbBar = new GraphBreadcrumbBar();
            element.Add(breadcrumbBar);
        }

        public override void Load(Graph graph)
        {
            GraphWindow.LoadGraph(graph);
            autoexecute.Bind(new SerializedObject(graph));
        }

        // API
        internal static void LoadGraph(Graph graph)
        {
            instance.breadcrumbBar.ClearBreadcrumbs();
            instance.breadcrumbBar.AddBreadcrumb(graph);
            instance.view.Load(graph);
        }

        internal static void LoadSubGraph(Graph graph)
        {
            instance.breadcrumbBar.AddBreadcrumb(graph);
            instance.view.Load(graph);
        }

        internal static void MarkDirty()
        {
            instance.graph.MarkDirty();
        }

		internal static void DuplicateNode(Node node)
		{
			instance.graph.DuplicateNode(node);
		}
		
		internal static void RemoveNode(Node node)
		{
			instance.graph.RemoveNode(node);
		}

        internal static void Disconnect(PortView view)
        {
            instance.graph.Disconnect(view.port, view.direction.IsInput());
        }

        internal static void ClickedPort(PortView view)
        {
            instance.view.ClickedPort(view);
        }

        internal static void Execute()
        {
            instance.view.graph.Execute();
        }
    }
}
#endif
