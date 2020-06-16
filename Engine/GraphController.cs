using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedOwl.Veranda
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

        private void Start()
        {
            foreach (var node in _startNodes)
            {
                Debug.Log($"Start: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        private void Update()
        {
            foreach (var node in _updateNodes)
            {
                Debug.Log($"Update: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        private void LateUpdate()
        {
            foreach (var node in _lateUpdateNodes)
            {
                Debug.Log($"LateUpdate: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        private void FixedUpdate()
        {
            foreach (var node in _fixedUpdateNodes)
            {
                Debug.Log($"FixedUpdate: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }
    }
}