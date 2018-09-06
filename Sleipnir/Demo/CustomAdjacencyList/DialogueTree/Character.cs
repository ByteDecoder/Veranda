using UnityEngine;

namespace Sleipnir.Demo.CustomAdjacencyList.DialogueTree
{
    [CreateAssetMenu(menuName = "SleipnirDemo/Character")]
    public class Character : ScriptableObject
    {
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}