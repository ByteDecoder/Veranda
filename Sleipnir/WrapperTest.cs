using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sleipnir.Wrapper;

namespace Sleipnir
{
    public class WrapperTest : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public GraphWrapper<Tested> Test = new GraphWrapper<Tested>(new Tested());
    }

    [Serializable, 
        Wrapper.Attributes.Knob(KnobType.Input, Blue = 1, Red = 0, Green = 1, Alpha = 0.9f, Description = "In"),
        Wrapper.Attributes.Knob(KnobType.Output, Description = "Out")]
    public class NodeTest
    {
        public float R = 2;
    }

    [Serializable]
    public class Tested : IWrappedGraph
    {
        public List<NodeTest> Nodes = new List<NodeTest>();
        public List<Tuple<NodeTest, NodeTest>> Connections = new List<Tuple<NodeTest, NodeTest>>();

        public IEnumerable<object> GetNodes()
        {
            return Nodes;
        }

        public IEnumerable<string> AvailableNodes()
        {
            return new [] {"Node"};
        }

        public object AddNode(string nodeId)
        {
            var n = new NodeTest();
            Nodes.Add(n);
            return n;
        }

        public void RemoveNode(object node)
        {
            Nodes.Remove((NodeTest)node);
        }

        public IEnumerable<Tuple<object, object, object>> GetConnections()
        {
            return Connections.Select(o => new Tuple<object, object, object>(o.Item1, o.Item2, null));
        }

        public bool AddConnection(Tuple<object, object> connection, out object content)
        {
            var n = new Tuple<NodeTest, NodeTest>((NodeTest)connection.Item1, (NodeTest)connection.Item2);
            Connections.Add(n);
            content = null;
            return true;
        }

        public void RemoveConnection(object connection)
        {
            var casted = (Tuple<object, object>) connection;
            var toRemove = Connections.First(o => o.Item1 == casted.Item1 && o.Item2 == casted.Item2);
            Connections.Remove(toRemove);
        }
    }
}