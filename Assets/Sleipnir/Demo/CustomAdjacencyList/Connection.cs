using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList
{
    [Serializable]
    public class Connection<T>
    {
        [HideLabel]
        public T Content;
        [HideInInspector]
        public int TargetIndex = -1;
    }
}