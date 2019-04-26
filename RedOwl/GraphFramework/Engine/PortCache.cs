using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    public struct PortInfo
	{
        internal string name;
        internal PortDirections direction;
        internal FieldInfo field;

		public PortInfo(Type type, FieldInfo field)
		{
			this.name = field.Name;
			var direction = PortDirections.None;
			field.FieldType.WithAttr<PortDirectionAttribute>(a => { direction = a.direction; });
			this.direction = direction;
			this.field = field;
		}
        
        public Port Get(object instance)
        {
            return (Port)field.GetValue(instance);
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
                if (typeof(Port).IsAssignableFrom(info.FieldType))
                    ports.Add(new PortInfo(nodeType, info));
            }
			_cache[nodeType] = ports;
		}
	}
}
