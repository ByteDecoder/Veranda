using System;
using System.Collections.Generic;

namespace Sleipnir.Demos
{
    [Serializable]
    public class DemoNode
    {
        [Slot(SlotDirection.Output)]
        public string Name;
        public List<SubNode> Sub = new List<SubNode>();
    }

    [Serializable]
    public struct SubNode
    {
        [Slot(SlotDirection.Output)]
        public string Output;
        
        [Slot(SlotDirection.Input)]
        public string Input;
    }
}