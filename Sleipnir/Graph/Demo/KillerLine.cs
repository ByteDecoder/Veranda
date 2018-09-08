using System;
using Sleipnir.Graph.Attributes;
using UnityEngine;

namespace Sleipnir.Graph.Demo
{
    [Serializable,
     NodeWidth(200),
     Title("This one is a killer"),
     TitleColor(1, 0, 0),
     HeaderColor(1, 1, 1)]
    public class KillerLine : DialogueLine
    {
        [ContextFunction]
        public void KillerHello()
        {
            Debug.Log("Killer Hello!");
        }
    }
}