using UnityEngine;

namespace Sleipnir.Graph.Demo
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