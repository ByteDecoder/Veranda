using System;

namespace Sleipnir
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HeaderTitleAttribute : Attribute
    {
        public string Text;

        public HeaderTitleAttribute(string text)
        {
            Text = text;
        }
    }
}