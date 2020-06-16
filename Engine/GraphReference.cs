using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Graph", menuName = "Veranda/Graph")]
    public class GraphReference : ScriptableObject
    {
        [SerializeReference, HideReferenceObjectPicker, HideLabel, InlineProperty]
        //[HideInInspector]
        public IGraph graph;
    }
}