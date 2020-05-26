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

        [Button]
        private void Awake()
        {
            _data = Instantiate(data);
            _data.graph.Initialize();
            //Debug.Log($"GraphReference '{name}' Initialized!");
        }

        [Button]
        private void Start()
        {
            _data.graph.Start(this);
        }

        [Button]
        private void Update()
        {

        }
    }
}