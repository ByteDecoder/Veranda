using System;
using System.Collections.Generic;

namespace Sleipnir.Demos
{
    [Serializable]
    public class DemoNode
    {
        [SlotOutput]
        public string Name;
        public List<SubNode> Sub = new List<SubNode>();
    }

    [Serializable]
    public struct SubNode
    {
        public string Output;

        [SlotInput]
        public string Input;
    }
}