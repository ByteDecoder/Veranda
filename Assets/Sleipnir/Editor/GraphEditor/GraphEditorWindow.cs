using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sleipnir;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private static GraphEditor _Instance;
        public static GraphEditor Instance {
            get {
                if (_Instance == null)
                {
                    Init();
                }
                return _Instance;
            }
        }
        
        [MenuItem("Tools/Odin Inspector/Sleipnir")]
        private static void Init()
        {
            Debug.Log("Init");
            _Instance = GetWindow<GraphEditor>();
            _Instance.position = GUIHelper.GetEditorWindowRect().AlignCenter(1024, 768);
            _Instance.titleContent = new GUIContent("Sleipnir");
            _Instance.Repaint();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            InitStyles();
            InitInput();
            
            // Manual handlers have to be used over magic methods because
            // magic methods don't get triggered when the window is out of focus
            Selection.selectionChanged += SelectionChanged;
        }
        
        public void SelectionChanged()
        {
            System.Object obj = Selection.activeObject;
            if (obj != null && typeof(IGraph).IsAssignableFrom(obj.GetType()))
            {
                LoadGraph((IGraph)obj);
            }
        }
        
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool AutoOpenCanvas(int instanceID, int line) 
        {
            if (Selection.activeObject != null && typeof(IGraph).IsAssignableFrom(Selection.activeObject.GetType()))
            {
                Instance.SelectionChanged();
                return true;
            }
            return false;
        }
    }
}
