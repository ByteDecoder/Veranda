using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class ValueNode : DemoNode
    {
        public OutputPort<float> Value = new OutputPort<float>(1f);
    }
}
