using System;
using System.Collections.Generic;
using System.Reflection;
using RedOwl.Core;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public enum PortIO
    {
        In,
        Out
    }
    
    public struct DataPortPrototype
    {
        public string NodeNamespace;
        public string NodeName;
        public string Name;
        public PortIO Io;
        public FieldInfo Field;

        public DataPortPrototype(Type nodeType, PortIO io, FieldInfo field)
        {
            NodeNamespace = nodeType.Namespace;
            NodeName = nodeType.Name;
            Name = field.Name;
            Io = io;
            Field = field;
        }

        public string GetHashId(string nodeId)
        {
            return RedOwlHash.GetHashId($"{NodeNamespace}.{NodeName}.{nodeId}.{Name}.{Io}");
        }
    }
    
    public struct FlowPortPrototype
    {
        public string NodeNamespace;
        public string NodeName;
        public string Name;
        public PortIO Io;
        public FieldInfo Field;
        public MethodInfo Method;

        public FlowPortPrototype(Type nodeType, PortIO io, FieldInfo field, MethodInfo method)
        {
            NodeNamespace = nodeType.Namespace;
            NodeName = nodeType.Name;
            Name = field.Name;
            Io = io;
            Field = field;
            Method = method;
        }

        public string GetHashId(string nodeId)
        {
            return RedOwlHash.GetHashId($"{NodeNamespace}.{NodeName}.{nodeId}.{Name}.{Io}");
        }
    }
    
    public class PortCache
    {
        private bool _cacheIsBuilt;
        private Dictionary<Type, List<DataPortPrototype>> _cacheDataPorts;
        private Dictionary<Type, List<FlowPortPrototype>> _cacheFlowPorts;
        private Dictionary<Type, Dictionary<string, MethodInfo>> _cacheFlowCallbacks;
        
        public List<DataPortPrototype> GetDataPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheDataPorts.TryGetValue(type, out var output) ? output : new List<DataPortPrototype>();
        }
        
        public List<FlowPortPrototype> GetFlowPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheFlowPorts.TryGetValue(type, out var output) ? output : new List<FlowPortPrototype>();
        }
        
        public MethodInfo GetFlowPortCallback(Type type, string name)
        {
            ShouldBuildCache();
            return _cacheFlowCallbacks.TryGetValue(type, out var output) ? output[name] : null;
        }

        public void ShouldBuildCache()
        {
            if (_cacheIsBuilt == false) BuildCache();
        }

        private void BuildCache()
        {
            _cacheDataPorts = new Dictionary<Type, List<DataPortPrototype>>();
            _cacheFlowPorts = new Dictionary<Type, List<FlowPortPrototype>>();
            _cacheFlowCallbacks = new Dictionary<Type, Dictionary<string, MethodInfo>>();

            var inputAttr = typeof(DataInAttribute);
            var outputAttr = typeof(DataOutAttribute);
            var inoutAttr = typeof(DataInOutAttribute);

            // TODO: Progress Bar?
            foreach (var type in TypeExtensions.GetAllTypes<INode>())
            {
                if (typeof(GraphReference).IsAssignableFrom(type)) continue;
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                var methodTable = new Dictionary<string, MethodInfo>(methods.Length);
                var dataPorts = new List<DataPortPrototype>(fields.Length);
                var flowPorts = new List<FlowPortPrototype>(fields.Length);

                foreach (var method in methods)
                {
                    //Debug.Log($"Adding method '{method.Name}' to nodetype '{type.Name}' ");
                    methodTable.Add(method.Name, method);
                }
                _cacheFlowCallbacks.Add(type, methodTable);

                foreach (var field in fields)
                {
                    // Data
                    if (field.IsDefined(inputAttr, false))
                    {
                        dataPorts.Add(new DataPortPrototype(type, PortIO.In, field));
                    }

                    if (field.IsDefined(outputAttr, false))
                    {
                        dataPorts.Add(new DataPortPrototype(type, PortIO.Out, field));
                    }

                    if (field.IsDefined(inoutAttr, false))
                    {
                        dataPorts.Add(new DataPortPrototype(type, PortIO.In, field));
                        dataPorts.Add(new DataPortPrototype(type, PortIO.Out, field));
                    }
                    
                    // Flow
                    if (field.TryGetAttribute(out FlowInAttribute inAttr, false))
                    {
                        if (methodTable.TryGetValue(inAttr.Name, out MethodInfo method))
                        {
                            flowPorts.Add(new FlowPortPrototype(type, PortIO.In, field, method));
                        }
                        else
                        {
                            Debug.LogWarning($"Unable to find method '{inAttr.Name}' for port '{field.Name}' on node '{type.Namespace}.{type.Name}'");
                        }
                    }

                    if (field.TryGetAttribute(out FlowOutAttribute outAttr, false))
                    {
                        if (methodTable.TryGetValue(outAttr.Name, out MethodInfo method))
                        {
                            flowPorts.Add(new FlowPortPrototype(type, PortIO.Out, field, method));
                        }
                        else
                        {
                            Debug.LogWarning($"Unable to find method '{outAttr.Name}' for port '{field.Name}' on node '{type.Namespace}.{type.Name}'");
                        }
                    }
                }

                _cacheDataPorts.Add(type, dataPorts);
                _cacheFlowPorts.Add(type, flowPorts);
            }

            _cacheIsBuilt = true;
        }
    }
}