using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [HideMonoScript]
    public class GraphController : MonoBehaviour
    {
        [HideLabel]
        public GraphReference Graph;

        [Button]
        private void Awake()
        {
            Graph.Initialize();
        }

        private void Start()
        {
            Graph.Enter();
        }

        private void Update()
        {
            Graph.Update();
        }
    }
}