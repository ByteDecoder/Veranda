using System;
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
        [DataOut]
        protected DataPort<T> data;

        protected GraphInputNode()
        {
            data = new DataPort<T>();
        }

        public DataPort Data => data;
    }
    
    // System
    public class GraphInputBoolean : GraphInputNode<bool> {}
    public class GraphInputSByte : GraphInputNode<sbyte> {}
    public class GraphInputByte : GraphInputNode<byte> {}
    public class GraphInputChar : GraphInputNode<char> {}
    public class GraphInputDouble : GraphInputNode<double> {}
    public class GraphInputDecimal : GraphInputNode<decimal> {}
    public class GraphInputInt : GraphInputNode<int> {}
    public class GraphInputUInt : GraphInputNode<uint> {}
    public class GraphInputLong : GraphInputNode<long> {}
    public class GraphInputULong : GraphInputNode<ulong> {}
    public class GraphInputShort : GraphInputNode<short> {}
    public class GraphInputUShort : GraphInputNode<ushort> {}
    public class GraphInputFloat : GraphInputNode<float> {}
    public class GraphInputString : GraphInputNode<string> {}
    public class GraphInputDataTime : GraphInputNode<DateTime> {}
    public class GraphInputDateTimeOffset : GraphInputNode<DateTimeOffset> {}
    public class GraphInputTimeSpan : GraphInputNode<TimeSpan> {}

    // Unity
    public class GraphInputUri : GraphInputNode<Uri> {}
    public class GraphInputGuid : GraphInputNode<Guid> {}
    public class GraphInputBounds : GraphInputNode<Bounds> {}
    public class GraphInputBoundsInt : GraphInputNode<BoundsInt> {}
    public class GraphInputGradient : GraphInputNode<Gradient> {}
    public class GraphInputColor : GraphInputNode<Color> {}
    public class GraphInputColor32 : GraphInputNode<Color32> {}
    public class GraphInputCurve : GraphInputNode<AnimationCurve> {}
    public class GraphInputLayerMask : GraphInputNode<LayerMask> {}
    public class GraphInputRect : GraphInputNode<Rect> {}
    public class GraphInputRectInt : GraphInputNode<RectInt> {}
    public class GraphInputTexture : GraphInputNode<Texture> {}
    public class GraphInputTexture2D : GraphInputNode<Texture2D> {}
    public class GraphInputRenderTexture : GraphInputNode<RenderTexture> {}
    public class GraphInputVector2 : GraphInputNode<Vector2> {}
    public class GraphInputVector2Int : GraphInputNode<Vector2Int> {}
    public class GraphInputVector3 : GraphInputNode<Vector3> {}
    public class GraphInputVector3Int : GraphInputNode<Vector3Int> {}
    public class GraphInputVector4 : GraphInputNode<Vector4> {}
    public class GraphInputQuaternion : GraphInputNode<Quaternion> {}
}