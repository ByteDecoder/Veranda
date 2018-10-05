using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sleipnir.Mapper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sleipnir.Demos
{
    public class MapperDemo : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public OdinGraph<Node> Graph = new OdinGraph<Node>();
    }

    [Serializable]
    [NodeTitle("Example")]
    public class Node
    {
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

        [ContextFunction(Name = "Stupify Name")]
        public void Foo()
        {
            C = "A";
        }

        //[OnChanged]
        public void RandomizeColor(Sleipnir.Node node)
        {
            node.HeaderColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

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