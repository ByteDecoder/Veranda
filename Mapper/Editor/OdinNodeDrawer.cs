using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sleipnir.Editor;
using UnityEngine;

namespace Sleipnir.Mapper.Editor
{
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class OdinNodeDrawer<T, TNodeContent> : OdinValueDrawer<T> where T : OdinNode<TNodeContent>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }
            var node = ValueEntry.SmartValue;

            SlotMapper.CurrentNodeSlots = node.Slots;
            SlotMapper.NodeValue = node.Value;
            NestMapper.Nests = new Stack<Nest>();
            NestMapper.Nests.Push(node.Nest);
            NestMapper.CurrentPath = "";
            CallNextDrawer(label);
        }
    }
}