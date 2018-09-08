using System;

namespace Sleipnir.Graph.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class Title : Attribute
    {
        public string Text;

        public Title(string text)
        {
            Text = text;
        }
    }
}