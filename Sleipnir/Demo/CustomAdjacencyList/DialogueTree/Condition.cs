using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    [CreateAssetMenu(menuName = "SleipnirDemo/Condition")]
    public class Condition : ScriptableObject
    {
        public bool Value;

        public override string ToString()
        {
            return name;
        }
    }
}