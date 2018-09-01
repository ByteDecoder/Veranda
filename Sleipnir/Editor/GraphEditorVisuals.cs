using System;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private const float ConnectionLineWidth = 5f;
        private const float ConnectionTangentMultiplier = 300f;
        public static readonly Vector2 KnobLabelOffset = new Vector2(2, 2);
        public static readonly Vector2 KnobSize = new Vector2(12, 12);
        public static readonly float KnobHorizontalOffset = 4f;

        private static Lazy<GUIStyle> KnobLabelGUIStyle => 
            new Lazy<GUIStyle>(() => new GUIStyle { normal = { textColor = Color.white } });

        public static Lazy<GUIStyle> NodeHeaderTitleGUIStyle =>
            new Lazy<GUIStyle>(() => 
            new GUIStyle(GUI.skin.GetStyle("Label")) { alignment = TextAnchor.MiddleCenter });
    }
}