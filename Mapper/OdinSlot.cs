using System;
using Sirenix.Utilities;
using UnityEngine;

namespace Sleipnir.Mapper
{
    [Serializable]
    public class OdinSlot
    {
        public string DeepReflectionPath;
        public int NodeIndex;
        
        public Func<object, object> Getter<T>(OdinGraph<T> graph)
        {
            return DeepReflection.CreateWeakInstanceValueGetter(
                graph[NodeIndex].GetType(), 
                typeof(object), 
                DeepReflectionPath
                );
        }

        public Action<object, object> Setter<T>(OdinGraph<T> graph)
        {
            return DeepReflection.CreateWeakInstanceValueSetter(
                graph[NodeIndex].GetType(),
                typeof(object), 
                DeepReflectionPath
                );
        }
    }
}