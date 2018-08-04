#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Sleipnir.Wrapper
{
    public interface IWrappedGraph
    {
        IEnumerable<object> GetNodes();
        IEnumerable<string> AvailableNodes();
        object AddNode(string nodeId);
        void RemoveNode(object node);

        // start, end, optional content
        IEnumerable<Tuple<object, object, object>> GetConnections();
        bool AddConnection(Tuple<object, object> connection, out object content);
        void RemoveConnection(object connection);
    }
}
#endif