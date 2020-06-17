using System.Collections.Generic;
using RedOwl.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [HideMonoScript]
    public class GraphController : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Red Owl/Graph Controller", false, 13)]
        private static void Create(UnityEditor.MenuCommand menuCommand)
        {
            RedOwlTools.Create<GraphController>(menuCommand.context as GameObject);
        }
#endif
        public bool Singleton;
        
        [HideLabel]
        public GraphReference data;

        private GraphReference _data;

        private List<StartNode> _startNodes;
        private List<UpdateNode> _updateNodes;
        private List<LateUpdateNode> _lateUpdateNodes;
        private List<FixedUpdateNode> _fixedUpdateNodes;

        private void Awake()
        {
            if (Singleton) DontDestroyOnLoad(this);
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
                if (!node.IsConnected) continue;
                Debug.Log($"Start: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        private void Update()
        {
            foreach (var node in _updateNodes)
            {
                if (!node.IsConnected) continue;
                Debug.Log($"Update: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        private void LateUpdate()
        {
            foreach (var node in _lateUpdateNodes)
            {
                if (!node.IsConnected) continue;
                Debug.Log($"LateUpdate: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }

        private void FixedUpdate()
        {
            foreach (var node in _fixedUpdateNodes)
            {
                if (!node.IsConnected) continue;
                Debug.Log($"FixedUpdate: Executing {node.name}");
                StartCoroutine(_data.graph.Execute(node));
            }
        }
    }
}