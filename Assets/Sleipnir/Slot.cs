namespace Sleipnir
{
    public class Slot
    {
        public ValueWrappedNode Node;
        public string PropertyPath;

        public Slot(ValueWrappedNode node, string propertyPath)
        {
            Node = node;
            PropertyPath = propertyPath;
        }
    }
}