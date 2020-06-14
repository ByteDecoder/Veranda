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
    
    public class PortInfo
    {
        public PortIO Io;
        public FieldInfo Field;
    }

    public class PortCache
    {
        private bool _cacheIsBuilt;
        private Dictionary<Type, List<PortInfo>> _cacheDataPorts;
        private Dictionary<Type, List<PortInfo>> _cacheFlowPorts;
        
        public List<PortInfo> GetDataPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheDataPorts.TryGetValue(type, out var output) ? output : new List<PortInfo>();
        }
        
        public List<PortInfo> GetFlowPorts(Type type)
        {
            ShouldBuildCache();
            return _cacheFlowPorts.TryGetValue(type, out var output) ? output : new List<PortInfo>();
        }

        public void ShouldBuildCache()
        {
            if (_cacheIsBuilt == false) BuildCache();
        }

        private void BuildCache()
        {
            _cacheDataPorts = new Dictionary<Type, List<PortInfo>>();
            _cacheFlowPorts = new Dictionary<Type, List<PortInfo>>();

            var dataIn = typeof(IDataInPort);
            var dataOut = typeof(IDataOutPort);
            var flowIn = typeof(IFlowInPort);
            var flowOut = typeof(IFlowOutPort);

            // TODO: Progress Bar?
            foreach (var type in TypeExtensions.GetAllTypes<INode>())
            {
                if (typeof(GraphReference).IsAssignableFrom(type)) continue;
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var dataPorts = new List<PortInfo>(fields.Length);
                var flowPorts = new List<PortInfo>(fields.Length);

                foreach (var field in fields)
                {
                    PortInfo dataPortInfo = null;
                    // Data
                    if (dataIn.IsAssignableFrom(field.FieldType))
                    {
                        dataPortInfo = new PortInfo {Io = PortIO.In, Field = field};
                    }

                    if (dataOut.IsAssignableFrom(field.FieldType))
                    {
                        if (dataPortInfo == null)
                        {
                            dataPortInfo = new PortInfo {Io = PortIO.Out, Field = field};
                        }
                        else
                        {
                            dataPortInfo.Io = PortIO.InOut;
                        }
                    }

                    if (dataPortInfo != null)
                    {
                        dataPorts.Add(dataPortInfo);
                        continue;
                    }

                    // Flow
                    if (flowIn.IsAssignableFrom(field.FieldType))
                    {
                        flowPorts.Add(new PortInfo {Io = PortIO.In, Field = field});
                    }
                    
                    if (flowOut.IsAssignableFrom(field.FieldType))
                    {
                        flowPorts.Add(new PortInfo {Io = PortIO.Out, Field = field});
                    }
                }

                _cacheDataPorts.Add(type, dataPorts);
                _cacheFlowPorts.Add(type, flowPorts);
            }

            _cacheIsBuilt = true;
        }
    }
}