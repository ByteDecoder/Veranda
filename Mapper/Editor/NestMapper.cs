using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sleipnir.Editor;
using UnityEngine;

namespace Sleipnir.Mapper.Editor
{
    [DrawerPriority(120, 0, 0)]
    public class NestMapper : OdinAttributeDrawer<NestedAttribute>
    {
        public static Nest CurrentNest;
        public static string CurrentPath;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }
            var oldNest = CurrentNest;
            var oldPath = CurrentPath;

            CurrentNest = CurrentNest.Nests.First(n => n.FieldName == Property.Name);
            CurrentPath = oldPath.IsNullOrWhitespace()
                ? Property.Name
                : oldPath + "." + Property.Name;

            CallNextDrawer(label);
            CurrentNest = oldNest;
            CurrentPath = oldPath;
        }
    }
}