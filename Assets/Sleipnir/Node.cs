using System;
using UnityEngine;

namespace Sleipnir
{
    [Serializable]
    public class Node
    {
        // Node height is automatically updated.
        public Rect NodeRect;
        public float LabelWidth;
        public bool IsLabelSliderShown;
        
        // Dummies are null objects inserted to in-graph node list.
        // They allow nodes to keep their content properly extended 
        // after a node is deleted.
        public int NumberOfPrecedingDummies = 0;
    }
}