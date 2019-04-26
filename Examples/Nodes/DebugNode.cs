using UnityEngine;
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class DebugNode : DemoNode
    {
        public InputPort<string> Data = new InputPort<string>();

        public override void OnExecute()
        {
            Debug.Log(Data.value);
        }
    }
}
