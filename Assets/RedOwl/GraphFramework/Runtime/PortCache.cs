using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    public struct PortInfo
	{
        public string Name;
        public FieldInfo Field { get; private set; }
        public PropertyInfo Property { get; private set; }
        
		public PortInfo(Type type, string name)
		{
            Name = name;
			Field = type.GetField(name);
			Property = type.GetProperty(name);
            //Debug.LogFormat("Creating PortInfo for {0} | {1}.{2}", Field == null ? "Property" : "Field", type.Name, name);
		}
        
        public IPort Get(object instance)
        {
            return (IPort)(Field == null ? Property.GetValue(instance, null) : Field.GetValue(instance));
        }
    }

	public static class PortCache
	{
		private static Dictionary<System.Type, List<PortInfo>> _cache;
        
		private static BindingFlags PortFlags = BindingFlags.Public | BindingFlags.Instance;
        
		internal static List<PortInfo> Get(Type type)
		{
			ShouldBuildCache();
			List<PortInfo> output;
			if (_cache.TryGetValue(type, out output))
			{
				return output;
			}
			return new List<PortInfo>();
		}

/*
I've improved the performance of BuildCache so these callbacks/progressbars shouldn't be needed in the editor as it can be JIT

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

#if UNITY_EDITOR
		[DidReloadScripts]
		[InitializeOnLoadMethod]
#endif
*/
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ShouldBuildCache()
		{
			if (_cache == null) BuildCache();
		}

		internal static void BuildCache()
		{
			_cache = new Dictionary<System.Type, List<PortInfo>>();
			HashSet<Type> nodeTypes = new HashSet<Type>(Extensions.ForAllTypes<Node>());
			float count = nodeTypes.Count;
			float i = 0;
			foreach (var type in nodeTypes)
			{
/*
#if UNITY_EDITOR
				EditorUtility.DisplayProgressBar("Building Graph Editor Cache", type.Name, i / count);
#endif
*/
				try {
					CachePorts(type);
				} catch (System.Exception e)
				{
					UnityEngine.Debug.Log(e);
				}
				i += 1;
			}
/*
#if UNITY_EDITOR
			EditorUtility.ClearProgressBar();
#endif
*/
		}

		private static void CachePorts(Type nodeType)
		{
			var ports = new List<PortInfo>();
			foreach (FieldInfo info in nodeType.GetFields(PortFlags))
			{
                if (typeof(IPort).IsAssignableFrom(info.FieldType))
                    ports.Add(new PortInfo(nodeType, info.Name));
            }
			foreach (PropertyInfo info in nodeType.GetProperties(PortFlags))
			{
                if (typeof(IPort).IsAssignableFrom(info.PropertyType) && !(info.GetIndexParameters().Length > 0))
                    ports.Add(new PortInfo(nodeType, info.Name));
			}
			_cache[nodeType] = ports;
		}
	}
}
