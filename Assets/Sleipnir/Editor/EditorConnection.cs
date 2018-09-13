using System;
using UnityEngine;

namespace Sleipnir.Editor
{
    public class EditorConnection
    {
        public const float ConnectionWidth = 5f;
        public Connection Content;

        public EditorConnection(Connection content)
        {
            Content = content;
        }

        public static Tuple<Vector2, Vector2, Vector2, Vector2> BezierCurveData
            (Vector2 startGridPosition, Vector2 endGridPosition)
        {
            return new Tuple<Vector2, Vector2, Vector2, Vector2>(
                startGridPosition,
                endGridPosition,
                startGridPosition + Vector2.right * 100,
                endGridPosition + Vector2.left * 100);
        }
    }
}