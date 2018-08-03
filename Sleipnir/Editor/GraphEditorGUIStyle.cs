using System;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{        
    public partial class GraphEditor
    {
        [HideInInspector]
        [NonSerialized]
        public Lazy<GUIStyle> NodeStyle = new Lazy<GUIStyle>(() => new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D
                },
                border = new RectOffset(2, 2, 2, 2)
            });

        [HideInInspector]
        [NonSerialized]
        public Lazy<GUIStyle> SelectedNodeStyle = new Lazy<GUIStyle>(() => new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D
            },
            border = new RectOffset(2, 2, 2, 2)
        });

        [HideInInspector]
        [NonSerialized]
        public Lazy<GUIStyle> InPointStyle = new Lazy<GUIStyle>(() => new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D
            },
            active =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D
            },
            border = new RectOffset(1, 1, 2, 2)
        });

        [HideInInspector]
        [NonSerialized]
        public Lazy<GUIStyle> OutPointStyle = new Lazy<GUIStyle>(() => new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D
            },
            active =
            {
                background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D
            },
            border = new RectOffset(1, 1, 2, 2)
        });
    }
}