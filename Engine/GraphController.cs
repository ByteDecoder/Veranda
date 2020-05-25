using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [HideMonoScript]
    public class GraphController : MonoBehaviour
    {
        [HideLabel]
        public GraphReference data;

        [Button]
        private void Awake()
        {
            data.graph.Initialize();
            //Debug.Log($"GraphReference '{name}' Initialized!");
        }

        [Button]
        private void Start()
        {
            // TODO: Start a "Flow" context
            foreach (var node in data.graph.Nodes)
            {
                if (!(node is IEnterNode enterNode)) continue;
                if (enterNode.ActivateOnStart)
                {
                    if (!(node is IFlowNode flowNode)) continue;
                    //Debug.Log($"Execute Graph Starting @ Node: '{node}'");
                    flowNode.OnEnter(); // TODO This is ok?
                }
            }
        }

        [Button]
        private void Update()
        {
            foreach (var node in data.graph.Nodes)
            {
                if (!(node is IFlowNode flowNode)) continue;
                if (!flowNode.Active) continue;
                //Debug.Log($"Update Graph Node: '{node}'");
                flowNode.OnUpdate();
            }
        }
    }
}