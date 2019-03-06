using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.GraphFramework
{
    public struct PortInfo
	{
        internal string name;
        internal PortStyles style;
        internal PortDirections direction;
        internal FieldInfo field;
        internal PropertyInfo property;

		public PortInfo(Type type, FieldInfo info) : this(type, info.Name)
		{
			var style = PortStyles.Single;
			var direction = PortDirections.None;
			info.FieldType.WithAttr<PortStyleAttribute>(a => { style = a.style; Debug.Log("Found Port Style Attr"); });
			info.FieldType.WithAttr<PortDirectionAttribute>(a => { direction = a.direction; Debug.Log("Found Port Direction Attr"); });
			this.style = style;
			this.direction = direction;
		}

		public PortInfo(Type type, PropertyInfo info) : this(type, info.Name)
		{
			var style = PortStyles.Single;
			var direction = PortDirections.None;
			info.PropertyType.WithAttr<PortStyleAttribute>(a => { style = a.style; Debug.Log("Found Port Style Attr"); });
			info.PropertyType.WithAttr<PortDirectionAttribute>(a => { direction = a.direction; Debug.Log("Found Port Direction Attr"); });
			this.style = style;
			this.direction = direction;
		}
        
		public PortInfo(Type type, string name)
		{
            this.name = name;
			this.style = PortStyles.Single;
			this.direction = PortDirections.None;
			field = type.GetField(name);
			property = type.GetProperty(name);
            //Debug.LogFormat("Creating PortInfo for {0} | {1}.{2}", Field == null ? "Property" : "Field", type.Name, name);
		}
        
        public Port Get(object instance)
        {
            return (Port)(field == null ? property.GetValue(instance, null) : field.GetValue(instance));
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
			foreach (PropertyInfo info in nodeType.GetProperties(PortFlags))
			{
                if (typeof(Port).IsAssignableFrom(info.PropertyType) && !(info.GetIndexParameters().Length > 0))
                    ports.Add(new PortInfo(nodeType, info));
			}
			_cache[nodeType] = ports;
		}
	}
}
