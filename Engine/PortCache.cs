using System;
using System.Collections.Generic;
using System.Reflection;
using RedOwl.Core;

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
        public FieldInfo Info;

        public DataPortPrototype(Type nodeType, PortIO io, FieldInfo info)
        {
            NodeNamespace = nodeType.Namespace;
            NodeName = nodeType.Name;
            Name = info.Name;
            Io = io;
            Info = info;
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
        public FieldInfo Info;

        public FlowPortPrototype(Type nodeType, PortIO io, FieldInfo info)
        {
            NodeNamespace = nodeType.Namespace;
            NodeName = nodeType.Name;
            Name = info.Name;
            Io = io;
            Info = info;
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
        
        public IEnumerable<DataPortPrototype> GetDataPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheDataPorts.TryGetValue(type, out var output) ? output : new List<DataPortPrototype>();
        }
        
        public IEnumerable<FlowPortPrototype> GetFlowPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheFlowPorts.TryGetValue(type, out var output) ? output : new List<FlowPortPrototype>();
        }

        public void ShouldBuildCache()
        {
            if (_cacheIsBuilt == false) BuildCache();
        }

        private void BuildCache()
        {
            _cacheDataPorts = new Dictionary<Type, List<DataPortPrototype>>();
            _cacheFlowPorts = new Dictionary<Type, List<FlowPortPrototype>>();

            var inputAttr = typeof(DataInAttribute);
            var outputAttr = typeof(DataOutAttribute);
            var inoutAttr = typeof(DataInOutAttribute);
            var flowInAttr = typeof(FlowInAttribute);
            var flowOutAttr = typeof(FlowOutAttribute);

            // TODO: Progress Bar?
            foreach (var type in TypeExtensions.GetAllTypes<INode>())
            {
                if (typeof(GraphReference).IsAssignableFrom(type)) continue;
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var dataPorts = new List<DataPortPrototype>(fields.Length);
                var flowPorts = new List<FlowPortPrototype>(fields.Length);

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
                    if (field.IsDefined(flowInAttr, false))
                    {
                        flowPorts.Add(new FlowPortPrototype(type, PortIO.In, field));
                    }

                    if (field.IsDefined(flowOutAttr, false))
                    {
                        flowPorts.Add(new FlowPortPrototype(type, PortIO.Out, field));
                    }
                }

                _cacheDataPorts.Add(type, dataPorts);
                _cacheFlowPorts.Add(type, flowPorts);
            }

            _cacheIsBuilt = true;
        }
    }
}