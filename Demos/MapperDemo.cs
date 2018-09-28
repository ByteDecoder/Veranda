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
        [Slot(Direction.Input)]
        public string C;
        
        public Nested A = new Nested();

        [Nested]
        [Slot(Direction.InOut)]
        public Nested B = new Nested();
        
        [OdinSerialize]
        [ReadOnly]
        public List<Nested> BList = new List<Nested>();

        [Nested]
        [OdinSerialize]
        [Slot(Direction.InOut)]
        public List<Nested> AList = new List<Nested>();
    }

    [Serializable]
    public class Nested
    {
        [Slot(Direction.Output)]
        public string C;

        [Slot(Direction.Input)]
        public string D;
    }
}