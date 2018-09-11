using Sirenix.OdinInspector;
using Sleipnir.Graph.Attributes;
using UnityEngine;

namespace Sleipnir.Graph.Demo
{
    [CreateAssetMenu(menuName = "SleipnirDemo/Character")]
    [Attributes.Title("Hyyype!")]
    [LabelSlider(true)]
    public class Character : ScriptableObject
    {
        [Attributes.FieldKnob(KnobType.Input)]
        public string Name;
    }
}