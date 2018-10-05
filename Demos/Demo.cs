using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sleipnir;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public FloatAdjacencyList FloatGraph = new FloatAdjacencyList();
}

[Serializable]
public class FloatAdjacencyList : AdjacencyList<float> { }

[Serializable]
public class IntList
{
    public List<int> List = new List<int>();
}

[Serializable]
public class AdjacencyList<T> : IGraph
{
    [ReadOnly]
    public List<T> Nodes = new List<T>();
    [ReadOnly]
    public List<IntList> Connections = new List<IntList>();

#if UNITY_EDITOR
    #region Sleipnir data
    [SerializeField]
    [ReadOnly]
    private List<SerializedNodeData> _serializedNodeData = new List<SerializedNodeData>();

    private List<Node> _graphNodes;
    IList<Node> IGraph.Nodes
    {
        get
        {
            if (_graphNodes == null)
            {
                RemapNodes();
                RemapConnections();
            }
            return _graphNodes;
        }
    }

    private void RemapNodes()
    {
        _graphNodes = new List<Node>(_serializedNodeData.Count);
        for (var i = 0; i < _graphNodes.Count; i++)
        {
            var index = i;
            _graphNodes[i] = new Node(
                () => Nodes[index],
                value => Nodes[index] = (T)value,
                _serializedNodeData[index]
                );

            _graphNodes[i].Slots.Add(new Slot(SlotDirection.Output, _graphNodes[i], 25));
            _graphNodes[i].Slots.Add(new Slot(SlotDirection.Input, _graphNodes[i], 25));
        }
    }

    private List<Connection> _graphConnections;
    IEnumerable<Connection> IGraph.Connections()
    {
        if (_graphConnections == null)
        {
            RemapNodes();
            RemapConnections();
        }
        return _graphConnections;
    }
    
    private void RemapConnections()
    {
        _graphConnections = new List<Connection>();
        for (var i = 0; i < Connections.Count; i++)
        {
            var nestedList = Connections[i].List;
            foreach (var nestedIndex in nestedList)
                _graphConnections.Add(new Connection(_graphNodes[i].Slots[1],
                    _graphNodes[nestedIndex].Slots[0]));
        }
    }

    [SerializeField]
    private float _zoom;

    public float Zoom
    {
        get { return _zoom; }
        set { _zoom = value; }
    }

    [SerializeField]
    private Vector2 _pan;

    public Vector2 Pan
    {
        get { return _pan; }
        set { _pan = value; }
    }

    public IEnumerable<string> AvailableNodes()
    {
        return new[] { "Node" };
    }

    public void AddNode(string nodeId)
    {
        Nodes.Add(default(T));
        Connections.Add(new IntList());
        _serializedNodeData.Add(new SerializedNodeData());
        var index = _graphNodes.Count;
        var newNode = new Node(
            () => Nodes[index],
            value => Nodes[index] = (T)value,
            _serializedNodeData[index]
        );
        newNode.Slots.Add(new Slot(SlotDirection.Output, newNode, 25));
        newNode.Slots.Add(new Slot(SlotDirection.Input, newNode, 25));
        _graphNodes.Add(newNode);
    }

    public void RemoveNode(Node node)
    {
        var nodeIndex = _graphNodes.IndexOf(node);

        foreach (var connection in Connections)
        {
            connection.List.RemoveAll(e => e == nodeIndex);
            for (var i = connection.List.Count - 1; i >= 0; i--)
                if (connection.List[i] > nodeIndex)
                    connection.List[i]--;
        }

        Nodes.RemoveAt(nodeIndex);
        Connections.RemoveAt(nodeIndex);
        _serializedNodeData.RemoveAt(nodeIndex);
        _graphNodes.RemoveAt(nodeIndex);

        for (var i = 0; i < _graphNodes.Count; i++)
        {
            var index = i;
            _graphNodes[i].ValueGetter = () => Nodes[index];
            _graphNodes[i].ValueSetter = value => Nodes[index] = (T) value;
        }
    }
    
    public void AddConnection(Connection connection)
    {
        var outputIndex = _graphNodes.FindIndex(o => o.Slots.Contains(connection.OutputSlot));
        var inputIndex = _graphNodes.FindIndex(o => o.Slots.Contains(connection.InputSlot));
        Connections[outputIndex].List.Add(inputIndex);
        _graphConnections.Add(connection);
    }

    public void RemoveConnection(Connection connection)
    {
        var outputIndex = _graphNodes.FindIndex(o => o.Slots.Contains(connection.OutputSlot));
        var inputIndex = _graphNodes.FindIndex(o => o.Slots.Contains(connection.InputSlot));
        Connections[outputIndex].List.Remove(inputIndex);
        _graphConnections.Remove(connection);
    }
    #endregion
#endif
}