using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine.DataStructures
{
    [CreateAssetMenu(menuName = "Red Owl/TestSer")]
    public class TestSer : ScriptableObject
    {
        [SerializeReference]
        public GraphNode<string> node;
    }
}