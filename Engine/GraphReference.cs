using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Graph", menuName = "Sleipnir/Graph")]
    public class GraphReference : ScriptableObject
    {
        [SerializeReference, HideReferenceObjectPicker, HideLabel, InlineProperty]
        //[HideInInspector]
        public IGraph graph;
    }
}