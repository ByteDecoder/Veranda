using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedOwl.Sleipnir.Engine
{
    [HideMonoScript]
    public class GraphController : MonoBehaviour
    {
        [HideLabel]
        public GraphReference data;

        private GraphReference _data;

        private List<StartNode> _startNodes;
        private List<UpdateNode> _updateNodes;
        private List<LateUpdateNode> _lateUpdateNodes;
        private List<FixedUpdateNode> _fixedUpdateNodes;

        [Button]
        private void Awake()
        {
            _data = Instantiate(data);
            _data.graph.Initialize();

            _startNodes = new List<StartNode>(_data.graph.GetNodes<StartNode>());
            _updateNodes = new List<UpdateNode>(_data.graph.GetNodes<UpdateNode>());
            _lateUpdateNodes = new List<LateUpdateNode>(_data.graph.GetNodes<LateUpdateNode>());
            _fixedUpdateNodes = new List<FixedUpdateNode>(_data.graph.GetNodes<FixedUpdateNode>());
            //Debug.Log($"GraphReference '{name}' Initialized!");
        }

        [Button]
        private void Start()
        {
            foreach (var node in _startNodes)
            {
                _data.graph.Execute(this, node);
            }
        }

        [Button]
        private void Update()
        {
            // TODO: Don't run Execute if node isn't connected
            // foreach (var node in _updateNodes)
            // {
            //     StartCoroutine(_data.graph.Execute(node));
            // }
        }

        [Button]
        private void LateUpdate()
        {
            // foreach (var node in _lateUpdateNodes)
            // {
            //     StartCoroutine(_data.graph.Execute(node));
            // }
        }

        [Button]
        private void FixedUpdate()
        {
            // foreach (var node in _fixedUpdateNodes)
            // {
            //     StartCoroutine(_data.graph.Execute(node));
            // }
        }
    }
}