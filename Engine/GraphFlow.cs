using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedOwl.Veranda
{
    public interface IGraphFlow
    {
        IEnumerator Execute(IGraph graph, IFlowRootNode node);

        T Get<T>(DataPort<T> port);
        void Set(IDataPort port);
    }

    public class GraphFlow : IGraphFlow
    {
        private Dictionary<string, object> Data;
        private Dictionary<string, bool> NodeState;
        private int _stack;
        private int _cycles;

        public IEnumerator Execute(IGraph graph, IFlowRootNode node)
        {
            int count = graph.NodeCount;
            Data = new Dictionary<string, object>(count);
            NodeState = new Dictionary<string, bool>(count);
            foreach (var n in graph.Nodes)
            {
                NodeState[n.Id] = false;
            }
            
            _stack = 0;
            _cycles = 0;
            
            // Begin Execution of OutPorts on root node
            yield return Run(graph, node);
        }

        private IEnumerator Run(IGraph graph, IFlowRootNode node)
        {
            // TODO: there is something strange here with a connected flow of nodes > 800 that causes a crash
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            foreach (var port in node.FlowOutPorts)
            {
                yield return FollowConnections(graph, node, port);
            }
        
            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;
            //Debug.Log($"RunTime {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");
            //yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator FollowSuccession(IGraph graph, INode node, IFlowPort port)
        {
            PullData(graph, node);
            //Debug.Log($"Triggering: '{node}.{port}'");
            port.Trigger(this);
            if (!port.HasSuccession) yield break;
            if (port.IsAsync)
            {
                var succession = port.AsyncSuccession(this);
                while (succession.MoveNext())
                {
                    if (succession.Current is IFlowPort successionPort)
                    {
                        //Debug.Log($"Following Succession from '{node}.{port}' to '{node}.{successionPort}'");
                        SetData(node);
                        yield return FollowConnections(graph, node, successionPort);
                    }
                    yield return succession.Current;
                }
            } 
            else
            {
                var successionPort = port.SerialSuccession(this);
                //Debug.Log($"Following Succession from '{node}.{port}' to '{node}.{successionPort}'");
                SetData(node);
                yield return FollowConnections(graph, node, successionPort);
            }
        }

        private IEnumerator FollowConnections(IGraph graph, INode node, IFlowPort port)
        {
            //Debug.Log($"Triggering: '{node}.{port}'");
            port.Trigger(this);
            foreach (var connection in node.GetFlowConnections(port.Id))
            {
                var nextNode = graph.GetNode(connection.TargetNode);
                var nextPort = nextNode.GetFlowPort(connection.TargetPort);
                //Debug.Log($"Following Connection from '{node}.{port}' to '{nextNode}.{nextPort}'");
                _cycles += 1;
                if (_cycles > 250)
                {
                    _cycles = 0;
                    yield return null;
                }
                _stack += 1;
                yield return FollowSuccession(graph, nextNode, nextPort);
            }
        }

        private void PullData(IGraph graph, INode node)
        {
            foreach (var port in node.DataInPorts)
            {
                foreach (var connection in node.GetDataConnections(port.Id))
                {
                    // TODO: Check if upstream dataOutPort has already set it data to the flow and 'continue'
                    var nextNode = graph.GetNode(connection.TargetNode);
                    var nextPort = nextNode.GetDataPort(connection.TargetPort);
                    //Debug.Log($"Following Connection from '{node}.{port}' to '{nextNode}.{nextPort}'");
                    if (!Data.TryGetValue(connection.TargetPort, out object value)) continue;
                    //Debug.Log($"Pulling Data '{value}' from '{nextNode}.{nextPort}' to '{node}.{port}'");
                    port.Data = value;
                }
            }
        }

        private void SetData(INode node)
        {
            foreach (var port in node.DataOutPorts)
            {
                //Debug.Log($"SetData '{node}.{port}' | {port.Id} = {port.Data}");
                Set(port);
            }
        }

        public T Get<T>(DataPort<T> port)
        {
            if (Data.TryGetValue(port.Id, out object value))
            {
                return (T) value;
            }

            return default;
        }

        public void Set(IDataPort port)
        {
            Data[port.Id] = port.Data;
        }
    }
}