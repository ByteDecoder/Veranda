using UnityEngine;

namespace Sleipnir.Editor
{
    public class EditorSlot
    {
        public readonly Slot Content;
        public readonly Rect Rect;
        public readonly SlotDirection Direction;

        public EditorSlot(Slot content, Rect rect, SlotDirection direction)
        {
            Content = content;
            Rect = rect;
            Direction = direction;
        }
    }
}