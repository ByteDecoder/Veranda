using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class MultiplyNode : DemoNode
    {
        public float factor;

        public InOutPort<float> Data = new InOutPort<float>();

        public override void OnExecute()
        {
            Data.value *= factor;
        }
    }
}
