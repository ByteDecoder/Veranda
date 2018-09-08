using System;
using System.Collections.Generic;
using Sleipnir.Graph.Attributes;
using Sleipnir.Graph.Unity;
using UnityEngine;

namespace Sleipnir.Graph.Demo
{
    [Serializable]
    public class Graph : UnityGraph<Node, DialogueLine>
    {
        public override IEnumerable<Connection> Connections()
        {
            return new Connection[] { };
        }

        public override void AddConnection(Connection connection)
        {
        }

        public override void RemoveConnection(Connection connection)
        {
        }

        [OnGraphLoad]
        public void Load()
        {
            Debug.Log("Loaded!");
        }

        [ContextFunction]
        public void SayGraphHello(Node node)
        {
            Debug.Log("Graph Hello!");
        }

        [OnNodeDelete]
        public void OnNodeDelete(Node node)
        {
            Debug.Log("Deleted!");
        }
    }
}