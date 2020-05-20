using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Graph", menuName = "Sleipnir/Graph")]
    public class GraphReference : ScriptableObject
    {
        [SerializeReference, HideReferenceObjectPicker, HideLabel, InlineProperty]
        //[HideInInspector]
        public IGraph graph;

        public void Initialize()
        {
            graph.Initialize();
            Debug.Log($"Graph '{name}' Initialized!");
        }

        public void Enter()
        {
            
        }
        
        public void Update()
        {
            
        }
    }
}