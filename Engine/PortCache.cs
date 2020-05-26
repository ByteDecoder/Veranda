using System;
using System.Collections.Generic;
using System.Reflection;
using RedOwl.Core;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Flags]
    public enum PortIO
    {
        In = 1,
        Out = 2,
        InOut = In | Out,
    }

    public static class PortIOExtensions
    {
        public static bool IsInput(this PortIO self)
        {
            return self.HasFlag(PortIO.In);
        }

        public static bool IsOutput(this PortIO self)
        {
            return self.HasFlag(PortIO.Out);
        }
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

    public class PortCache
    {
        private bool _cacheIsBuilt;
        private Dictionary<Type, List<DataAttribute>> _cacheDataPorts;
        private Dictionary<Type, List<FlowAttribute>> _cacheFlowPorts;
        
        public List<DataAttribute> GetDataPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheDataPorts.TryGetValue(type, out var output) ? output : new List<DataAttribute>();
        }
        
        public List<FlowAttribute> GetFlowPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheFlowPorts.TryGetValue(type, out var output) ? output : new List<FlowAttribute>();
        }

        public void ShouldBuildCache()
        {
            if (_cacheIsBuilt == false) BuildCache();
        }

        private void BuildCache()
        {
            _cacheDataPorts = new Dictionary<Type, List<DataAttribute>>();
            _cacheFlowPorts = new Dictionary<Type, List<FlowAttribute>>();

            var inputAttr = typeof(DataInAttribute);
            var outputAttr = typeof(DataOutAttribute);
            var inoutAttr = typeof(DataInOutAttribute);

            // TODO: Progress Bar?
            foreach (var type in TypeExtensions.GetAllTypes<INode>())
            {
                if (typeof(GraphReference).IsAssignableFrom(type)) continue;
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var dataPorts = new List<DataAttribute>(fields.Length);
                var flowPorts = new List<FlowAttribute>(fields.Length);

                foreach (var field in fields)
                {
                    // Data
                    if (field.TryGetAttribute(out DataInAttribute dataInAttr, false))
                    {
                        dataInAttr.Io = PortIO.In;
                        dataInAttr.Field = field;
                        dataPorts.Add(dataInAttr);
                    }

                    if (field.TryGetAttribute(out DataOutAttribute dataOutAttr, false))
                    {
                        dataOutAttr.Io = PortIO.Out;
                        dataOutAttr.Field = field;
                        dataPorts.Add(dataOutAttr);
                    }

                    if (field.TryGetAttribute(out DataInOutAttribute dataInOutAttr, false))
                    {
                        dataInOutAttr.Io = PortIO.InOut;
                        dataInOutAttr.Field = field;
                        dataPorts.Add(dataInOutAttr);
                    }
                    
                    // Flow
                    if (field.TryGetAttribute(out FlowInAttribute flowInAttr, false))
                    {
                        flowInAttr.Io = PortIO.In;
                        flowInAttr.Field = field;
                        flowPorts.Add(flowInAttr);
                    }

                    if (field.TryGetAttribute(out FlowOutAttribute flowOutAttr, false))
                    {
                        flowOutAttr.Io = PortIO.Out;
                        flowOutAttr.Field = field;
                        flowPorts.Add(flowOutAttr);
                    }
                }

                _cacheDataPorts.Add(type, dataPorts);
                _cacheFlowPorts.Add(type, flowPorts);
            }

            _cacheIsBuilt = true;
        }
    }
}