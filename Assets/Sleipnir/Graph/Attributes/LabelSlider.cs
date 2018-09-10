using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LabelSlider : Attribute
    {
        public bool IsShown;

        public LabelSlider(bool isShown = false)
        {
            IsShown = isShown;
        }
    }
}