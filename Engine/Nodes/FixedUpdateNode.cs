using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class FixedUpdateNode : FlowRootNode, IFlowOutNode {}
}