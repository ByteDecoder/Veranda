using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphInput
    {
        DataPort Data { get; }
    }
    
    [Serializable]
    public abstract class GraphInputNode<T> : Node, IGraphInput
    {
        [SerializeField, HideLabel, HideReferenceObjectPicker, InlineProperty]
        [DataOut]
        protected DataPort<T> data;

        public DataPort Data => data;
    }
    
    // System
    [Serializable]
    public class GraphInputBoolean : GraphInputNode<bool> {}
    [Serializable]
    public class GraphInputSByte : GraphInputNode<sbyte> {}
    [Serializable]
    public class GraphInputByte : GraphInputNode<byte> {}
    [Serializable]
    public class GraphInputChar : GraphInputNode<char> {}
    [Serializable]
    public class GraphInputDouble : GraphInputNode<double> {}
    [Serializable]
    public class GraphInputDecimal : GraphInputNode<decimal> {}
    [Serializable]
    public class GraphInputInt : GraphInputNode<int> {}
    [Serializable]
    public class GraphInputUInt : GraphInputNode<uint> {}
    [Serializable]
    public class GraphInputLong : GraphInputNode<long> {}
    [Serializable]
    public class GraphInputULong : GraphInputNode<ulong> {}
    [Serializable]
    public class GraphInputShort : GraphInputNode<short> {}
    [Serializable]
    public class GraphInputUShort : GraphInputNode<ushort> {}
    [Serializable]
    public class GraphInputFloat : GraphInputNode<float> {}
    [Serializable]
    public class GraphInputString : GraphInputNode<string> {}
    [Serializable]
    public class GraphInputDataTime : GraphInputNode<DateTime> {}
    [Serializable]
    public class GraphInputDateTimeOffset : GraphInputNode<DateTimeOffset> {}
    [Serializable]
    public class GraphInputTimeSpan : GraphInputNode<TimeSpan> {}

    // Unity
    [Serializable]
    public class GraphInputUri : GraphInputNode<Uri> {}
    [Serializable]
    public class GraphInputGuid : GraphInputNode<Guid> {}
    [Serializable]
    public class GraphInputBounds : GraphInputNode<Bounds> {}
    [Serializable]
    public class GraphInputBoundsInt : GraphInputNode<BoundsInt> {}
    [Serializable]
    public class GraphInputGradient : GraphInputNode<Gradient> {}
    [Serializable]
    public class GraphInputColor : GraphInputNode<Color> {}
    [Serializable]
    public class GraphInputColor32 : GraphInputNode<Color32> {}
    [Serializable]
    public class GraphInputCurve : GraphInputNode<AnimationCurve> {}
    [Serializable]
    public class GraphInputLayerMask : GraphInputNode<LayerMask> {}
    [Serializable]
    public class GraphInputRect : GraphInputNode<Rect> {}
    [Serializable]
    public class GraphInputRectInt : GraphInputNode<RectInt> {}
    [Serializable]
    public class GraphInputTexture : GraphInputNode<Texture> {}
    [Serializable]
    public class GraphInputTexture2D : GraphInputNode<Texture2D> {}
    [Serializable]
    public class GraphInputRenderTexture : GraphInputNode<RenderTexture> {}
    [Serializable]
    public class GraphInputVector2 : GraphInputNode<Vector2> {}
    [Serializable]
    public class GraphInputVector2Int : GraphInputNode<Vector2Int> {}
    [Serializable]
    public class GraphInputVector3 : GraphInputNode<Vector3> {}
    [Serializable]
    public class GraphInputVector3Int : GraphInputNode<Vector3Int> {}
    [Serializable]
    public class GraphInputVector4 : GraphInputNode<Vector4> {}
    [Serializable]
    public class GraphInputQuaternion : GraphInputNode<Quaternion> {}
}