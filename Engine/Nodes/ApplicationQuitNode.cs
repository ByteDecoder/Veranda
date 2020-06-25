using System;
using RedOwl.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    [Serializable]
    [HideReferenceObjectPicker, InlineProperty]
    public class ApplicationQuitNode: Node, IFlowInNode
    {
        [SerializeField, HideInInspector]
        protected FlowIn flowIn;

        public FlowIn FlowIn => flowIn;
        
        protected override void Setup()
        {
            flowIn.SetCallback(RedOwlTools.Quit);
        }
    }
}