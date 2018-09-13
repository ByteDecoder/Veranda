namespace Sleipnir
{
    public class Knob
    {
        public ValueWrappedNode Node;
        public string PropertyPath;

        public Knob(ValueWrappedNode node, string propertyPath)
        {
            Node = node;
            PropertyPath = propertyPath;
        }
    }
}