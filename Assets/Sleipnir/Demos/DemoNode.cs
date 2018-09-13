using System;
using System.Collections.Generic;

namespace Sleipnir.Demos
{
    [Serializable]
    public class DemoNode
    {
        [Knob(KnobDirection.Output)]
        public string Name;
        public List<SubNode> Sub = new List<SubNode>();
    }

    [Serializable]
    public struct SubNode
    {
        [Knob(KnobDirection.Output)]
        public string Output;
        
        [Knob(KnobDirection.Input)]
        public string Input;
    }
}