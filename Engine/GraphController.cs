using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
            // TODO: Validate This works
            foreach (var node in _startNodes)
            {
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        [Button]
        private void Update()
        {
            // TODO: Validate This works
            foreach (var node in _updateNodes)
            {
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        [Button]
        private void LateUpdate()
        {
            foreach (var node in _lateUpdateNodes)
            {
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        [Button]
        private void FixedUpdate()
        {
            foreach (var node in _fixedUpdateNodes)
            {
                StartCoroutine(_data.graph.Execute(node));
            }
        }
    }
}