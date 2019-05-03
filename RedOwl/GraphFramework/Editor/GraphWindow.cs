#pragma warning disable 0649 // UXMLReference variable declared but not assigned to.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using RedOwl.Editor;

namespace RedOwl.GraphFramework.Editor
{
	[UXML, USS, USS("RedOwl/GraphFramework/Editor/DefaultColors"), USS("RedOwl/GraphFramework/Editor/TypeColors")]
    public class GraphWindow : RedOwlAssetEditor<Graph, GraphWindow>
	{
		[UXMLReference]
		internal GraphView view;

        [UXMLReference]
        private Toolbar toolbar;

        private ToolbarToggle autoexecute;

        private List<NodeView> currentSelection;
        private IEnumerable<Node> selectedNodes {
            get {
                foreach (var item in currentSelection)
                {
                    yield return item.node;
                }
            }
        }

        public Graph graph {
            get { return view.graph; }
        }

        protected GraphBreadcrumbBar breadcrumbBar;

        protected override void OnEnable()
        {
            base.OnEnable();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        protected override void BuildUI()
        {
            currentSelection = new List<NodeView>();
            toolbar.Add(new ToolbarSpacer());
            toolbar.Add(new ToolbarSpacer { flex = true });
            autoexecute = new ToolbarToggle { text = "Auto Execute", bindingPath = "AutoExecute" };
            toolbar.Add(autoexecute);
            toolbar.Add(new ToolbarSpacer());
            toolbar.Add(new ToolbarButton(Execute) { text = "Execute"});
        }

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

        internal static void ToggleSelectNode(NodeView view)
        {
            if (instance.currentSelection.Contains(view))
            {
                view.RemoveFromClassList("Selected");
                instance.currentSelection.Remove(view);
            } else {
                view.AddToClassList("Selected");
                instance.currentSelection.Add(view);
            }
            Selection.objects = instance.selectedNodes.ToArray();
        }

        internal static void SelectNode(NodeView view)
        {
            if (instance.currentSelection.Contains(view)) return;
            UnselectNodes();
            view.AddToClassList("Selected");
            instance.currentSelection.Add(view);
            Selection.objects = new UnityEngine.Object[] {view.node};
        }

        internal static void UnselectNodes()
        {
            foreach (var item in instance.currentSelection)
            {
                item.RemoveFromClassList("Selected");
            }
            instance.currentSelection.Clear();
            Selection.objects = null;
        }

        internal static void DragNodes(Vector3 delta)
        {
            foreach (var item in instance.currentSelection)
            {
                item.DragNode(delta);
            }
            MarkDirty();
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
