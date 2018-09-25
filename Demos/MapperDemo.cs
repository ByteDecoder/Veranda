using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sleipnir.Mapper;

namespace Sleipnir.Demos
{
    public class MapperDemo : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public OdinGraph<Node> Graph = new OdinGraph<Node>();
    }

    [Serializable]
    public class Node
    {
        [Nested]
        [Slot(Direction.Input)]
        public Nested A = new Nested();

        [Nested]
        [Slot(Direction.Output)]
        public Nested B = new Nested();
        
        //[Nested]
        //[Slot(Direction.Output)]
        //public List<Nested> List = new List<Nested>();
    }

    [Serializable]
    public class Nested
    {
        [Slot(Direction.Input)]
        public string C;

        [Slot(Direction.Input)]
        public string D;
    }
}