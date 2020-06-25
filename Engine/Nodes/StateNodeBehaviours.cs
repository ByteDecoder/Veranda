using System.Collections;
using RedOwl.Core;
using Sirenix.OdinInspector;

namespace RedOwl.Veranda
{
    public class GameEventStateBehaviours : StateBehaviour
    {
        [LabelText("Event")] public GameEvent evt;

        public override IEnumerator OnUpdate()
        {
            yield return evt.OnNext();
            yield return new ExitState();
        }
    }
}