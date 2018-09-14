using System;
using System.Collections.Generic;

namespace Sleipnir.Demos
{
    [Serializable]
    public class DemoNode
    {
        [SlotInOut]
        public string Name;
        public List<SubNode> Sub = new List<SubNode>();
    }

    [Serializable]
    public struct SubNode
    {
        [SlotInOut]
        public string Output;

        [SlotInOut]
        public string Input;
    }
}