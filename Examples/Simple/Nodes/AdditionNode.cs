using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class AdditionNode : DemoNode
    {
        public float factor;

        public InOutPort<float> Data = new InOutPort<float>();

        public override void OnExecute()
        {
            Data.value += factor;
        }
    }
}
